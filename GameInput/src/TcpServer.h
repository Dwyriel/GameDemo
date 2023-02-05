#ifndef GAMEINPUT_TCPSERVER_H
#define GAMEINPUT_TCPSERVER_H

#include <QObject>
#include <QTcpServer>
#include <QTcpSocket>
#include <QList>
#include <QString>
#include <QDebug>

class TcpServer : public QObject {
Q_OBJECT
public:
    TcpServer(QObject *parent = nullptr, quint16 port = 7030);

    ~TcpServer();

    void SendMessage(QByteArray& msg);

private:
    QTcpServer *qTcpServer;
    QList<QTcpSocket *> sockets;

    void connectSignals();

private slots:

    void ErrorOccurredOnNewConnection(QAbstractSocket::SocketError socketError);

    void newPendingConnection();

    void socketDisconnected();

    void socketSentMessage();
};

#endif //GAMEINPUT_TCPSERVER_H
