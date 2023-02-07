#ifndef GAMEINPUT_TCPSERVER_H
#define GAMEINPUT_TCPSERVER_H

#include <QObject>
#include <QTcpServer>
#include <QTcpSocket>
#include <QList>
#include <QString>
#include <QDebug>

#include "../messages.h"

class TcpServer : public QObject {
Q_OBJECT
public:
    explicit TcpServer(QObject *parent = nullptr, quint16 port = 7030);

    ~TcpServer() override;

    void SendMessage(const Message &msg);

    void SendCommands(Commands &msg);

private:
    struct InternalTcpSocket {
        quint64 id;
        QTcpSocket *socket;
    };
    quint64 idCounter = 0;
    QTcpServer *qTcpServer;
    QList<InternalTcpSocket> sockets;

    void connectSignals();

private slots:

    void ErrorOccurredOnNewConnection(QAbstractSocket::SocketError socketError);

    void newPendingConnection();

    void socketDisconnected();

    void socketSentMessage();
};

#endif //GAMEINPUT_TCPSERVER_H
