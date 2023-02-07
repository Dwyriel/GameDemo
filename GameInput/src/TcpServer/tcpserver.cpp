#include "tcpserver.h"

TcpServer::TcpServer(QObject *parent, quint16 port) : QObject(parent), qTcpServer(new QTcpServer(this)) {
    connectSignals();
    qTcpServer->listen(QHostAddress::AnyIPv4, port);
}

TcpServer::~TcpServer() {
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
    qDebug() << "Error occurred when attempting a new connection";
}

void TcpServer::newPendingConnection() {
    if (!qTcpServer->hasPendingConnections())
        return;
    auto socket = qTcpServer->nextPendingConnection();
    InternalTcpSocket internalTcpSocket = {idCounter, socket};
    sockets.push_back(internalTcpSocket);
    connect(socket, &QTcpSocket::disconnected, this, &TcpServer::socketDisconnected);
    connect(socket, &QTcpSocket::readyRead, this, &TcpServer::socketSentMessage);
    qDebug() << "new socket connected. id:" << idCounter++;
    if (sockets.size() >= MAX_SOCKET_CONNECTIONS)
        qTcpServer->pauseAccepting();
    emit clientConnected();
    //todo log
}

void TcpServer::socketDisconnected() {
    for (qsizetype i = 0; i < sockets.size(); i++)
        if (sockets[i].socket->state() == QAbstractSocket::SocketState::ClosingState || sockets[i].socket->state() == QAbstractSocket::SocketState::UnconnectedState) {
            sockets[i].socket->disconnect();
            qDebug() << "disconnected socket of id" << sockets[i].id;
            sockets.removeAt(i--);
            //todo log
        }
    if (sockets.size() < MAX_SOCKET_CONNECTIONS)
        qTcpServer->resumeAccepting();
    emit clientDisconnected();
}

void TcpServer::socketSentMessage() {
    for (auto &internalTcpSocket: sockets)
        if (internalTcpSocket.socket->bytesAvailable() > 0) {
            qDebug() << "internalTcpSocket sent msg:" << internalTcpSocket.socket->readAll();
            //todo log
            //todo deal with message
        }
}

void TcpServer::SendStartGameCommand() {
    GameStartCommand gameStartCommand;
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &gameStartCommand, sizeof(GameStartCommand));
        internalTcpSocket.socket->flush();
    }
}

void TcpServer::SendInputCommands(InputCommands &msg) {
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &msg, sizeof(InputCommands));
        internalTcpSocket.socket->flush();
    }
    //todo log
}

void TcpServer::SendMessage(const Message &msg) {
    for (auto &internalTcpSocket: sockets) {
        internalTcpSocket.socket->write((char *) &msg, sizeof(msg.messageLength) + sizeof(msg.messageType));
        internalTcpSocket.socket->write(msg.message);
        internalTcpSocket.socket->flush();
    }
    //todo log
}
