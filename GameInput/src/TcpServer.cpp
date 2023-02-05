#include "TcpServer.h"

TcpServer::TcpServer(QObject *parent, quint16 port) : QObject(parent), qTcpServer(new QTcpServer(this)) {
    connectSignals();
    qTcpServer->listen(QHostAddress::AnyIPv4, port);
}

TcpServer::~TcpServer() {
    qTcpServer->close();
    delete qTcpServer;
    while(!sockets.empty()){
        sockets.first()->disconnect();
        sockets.first()->close();
        sockets.removeFirst();
    }
}

void TcpServer::connectSignals() {
    connect(qTcpServer, &QTcpServer::acceptError, this, &TcpServer::ErrorOccurredOnNewConnection);
    connect(qTcpServer, &QTcpServer::pendingConnectionAvailable, this, &TcpServer::newPendingConnection);
}

void TcpServer::ErrorOccurredOnNewConnection(QAbstractSocket::SocketError socketError) {

}

void TcpServer::newPendingConnection() {
    //todo log
    auto socket = qTcpServer->nextPendingConnection();
    sockets.push_back(socket);
    connect(socket, &QTcpSocket::disconnected, this, &TcpServer::socketDisconnected);
    connect(socket, &QTcpSocket::readyRead, this, &TcpServer::socketSentMessage);
    qDebug() << "new socket connected";
}

void TcpServer::socketDisconnected() {
    for (qsizetype i = 0; i < sockets.size(); i++)
        if (sockets[i]->state() == QAbstractSocket::SocketState::ClosingState || sockets[i]->state() == QAbstractSocket::SocketState::UnconnectedState) {
            sockets[i]->disconnect();
            sockets.removeAt(i);
            i--;
            //todo log
            qDebug() << "disconnected socket at index" << i;
        }
}

void TcpServer::socketSentMessage() {
    for (auto &socket: sockets)
        if (socket->bytesAvailable() > 0) {
            qDebug() << "socket sent msg:" << socket->readAll();
            //todo log
            //todo deal with message
        }
}

void TcpServer::SendMessage(QByteArray& msg) {
    for (auto &socket: sockets) {
        socket->write(msg);
        socket->flush();
    }
    //todo log
}
