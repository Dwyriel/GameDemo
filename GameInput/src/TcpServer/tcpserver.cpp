#include "tcpserver.h"

TcpServer::TcpServer(QObject *parent, quint16 port) : QObject(parent), qTcpServer(new QTcpServer(this)), logger(Logger::Instance()) {
    connectSignals();
    logger->writeLog("Starting TCP Server on port " + QString::number(port));
    qTcpServer->listen(QHostAddress::AnyIPv4, port);
}

TcpServer::~TcpServer() {
    logger->writeLog("Cleaning sockets and closing server");
    while (!sockets.empty()) {
        sockets.last().socket->disconnect();
        sockets.last().socket->close();
        sockets.removeLast();
    }
    qTcpServer->close();
    delete qTcpServer;
}

void TcpServer::connectSignals() {
    connect(qTcpServer, &QTcpServer::acceptError, this, &TcpServer::ErrorOccurredOnNewConnection);
    connect(qTcpServer, &QTcpServer::pendingConnectionAvailable, this, &TcpServer::newPendingConnection);
}

void TcpServer::ErrorOccurredOnNewConnection(QAbstractSocket::SocketError socketError) {
    logger->writeLog("A client tried to connect but an error occurred. Error code: " + QString::number(socketError));
}

void TcpServer::newPendingConnection() {
    if (!qTcpServer->hasPendingConnections())
        return;
    auto socket = qTcpServer->nextPendingConnection();
    InternalTcpSocket internalTcpSocket = {idCounter, socket};
    sockets.push_back(internalTcpSocket);
    connect(socket, &QTcpSocket::disconnected, this, &TcpServer::socketDisconnected);
    connect(socket, &QTcpSocket::readyRead, this, &TcpServer::socketSentMessage);
    if (sockets.size() >= MAX_SOCKET_CONNECTIONS)
        qTcpServer->pauseAccepting();
    emit clientConnected();
    logger->writeLog("New client connected and was assigned the ID: " + QString::number(idCounter++));
}

void TcpServer::socketDisconnected() {
    for (qsizetype i = 0; i < sockets.size(); i++)
        if (sockets[i].socket->state() == QAbstractSocket::SocketState::ClosingState || sockets[i].socket->state() == QAbstractSocket::SocketState::UnconnectedState) {
            sockets[i].socket->disconnect();
            logger->writeLog("Client of id " + QString::number(sockets[i].id) + " disconnected");
            sockets.removeAt(i--);
        }
    if (sockets.size() < MAX_SOCKET_CONNECTIONS)
        qTcpServer->resumeAccepting();
    emit clientDisconnected();
}

void TcpServer::socketSentMessage() {
    for (auto &internalTcpSocket: sockets) {
        if (internalTcpSocket.socket->bytesAvailable() > 0) {
            logger->writeLog("New message received from client " + QString::number(internalTcpSocket.id));
            if (internalTcpSocket.socket->bytesAvailable() < 8) {
                internalTcpSocket.socket->readAll();
                continue;
            }
            char header[sizeof(int) * 2];
            memset(header, 0, sizeof(header));
            auto bytesRead = internalTcpSocket.socket->read(header, sizeof(header));
            if (bytesRead != sizeof(header)) {
                internalTcpSocket.socket->readAll();
                continue;
            }
            ClientAnswer clientAnswer;
            clientAnswer.messageLength = *(int *) header;
            clientAnswer.messageType = (MessageType) (*(int *) (header + sizeof(int)));
            auto expectedMessageLength = calculateExpectedMessageLength(clientAnswer.messageType);
            if (clientAnswer.messageLength != expectedMessageLength) {
                internalTcpSocket.socket->readAll();
                continue;
            }
            char message[clientAnswer.messageLength];
            memset(message, 0, clientAnswer.messageLength);
            bytesRead = internalTcpSocket.socket->read(message, clientAnswer.messageLength);
            if (bytesRead != clientAnswer.messageLength) {
                internalTcpSocket.socket->readAll();
                continue;
            }
            memcpy(&clientAnswer.bytes, message, clientAnswer.messageLength);
            emit clientSentResponse(clientAnswer);
        }
    }
}

size_t TcpServer::calculateExpectedMessageLength(MessageType messageType) {
    switch (messageType) {
        case MessageType::GameResponse:
            return sizeof(ClientAnswer::gameResponse);
        case MessageType::GameStats:
            return sizeof(ClientAnswer::gameStats);
        default:
            return 0;
    }
}

void TcpServer::SendStartGameCommand() {
    GameStartCommand gameStartCommand;
    QString representation = Logger::bytesToHexRepresentation((char *) &gameStartCommand, sizeof(GameStartCommand));
    logger->writeLog("Sending start game command to all connected clients, binary data: " + representation);
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &gameStartCommand, sizeof(GameStartCommand));
        internalTcpSocket.socket->flush();
    }
}

void TcpServer::SendInputCommands(InputCommands &msg) {
    QString representation = Logger::bytesToHexRepresentation((char *) &msg, sizeof(InputCommands));
    logger->writeLog("Sending input commands to all connected clients, binary data: " + representation);
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &msg, sizeof(InputCommands));
        internalTcpSocket.socket->flush();
    }
}

void TcpServer::SendMessage(const Message &msg) {
    logger->writeLog("Sending a custom message to all connected clients");
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &msg, sizeof(msg.messageLength) + sizeof(msg.messageType));
        internalTcpSocket.socket->write(msg.message);
        internalTcpSocket.socket->flush();
    }
}
