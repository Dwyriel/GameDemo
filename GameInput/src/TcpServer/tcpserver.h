#ifndef GAMEINPUT_TCPSERVER_H
#define GAMEINPUT_TCPSERVER_H

#include <QObject>
#include <QTcpServer>
#include <QTcpSocket>
#include <QList>
#include <QString>

#include "../messagesStructure.h"
#include "../defines.h"
#include "src/Logger/Logger.h"

class TcpServer : public QObject {
Q_OBJECT
public:
    explicit TcpServer(QObject *parent = nullptr, quint16 port = SERVER_DEFAULT_PORT);

    ~TcpServer() override;

    void SendStartGameCommand();

    void SendInputCommands(InputCommands &msg);

    void SendMessage(const Message &msg);

private:
    struct InternalTcpSocket {
        quint64 id;
        QTcpSocket *socket;
    };
    quint64 idCounter = 0;
    QTcpServer *qTcpServer;
    QList<InternalTcpSocket> sockets;
    Logger *logger;

    void connectSignals();

    static size_t calculateExpectedMessageLength(MessageType messageType);

signals:

    void clientConnected();

    void clientDisconnected();

    void clientSentResponse(ClientAnswer gameResponse);

private slots:

    void ErrorOccurredOnNewConnection(QAbstractSocket::SocketError socketError);

    void newPendingConnection();

    void socketDisconnected();

    void socketSentMessage();
};

#endif //GAMEINPUT_TCPSERVER_H
