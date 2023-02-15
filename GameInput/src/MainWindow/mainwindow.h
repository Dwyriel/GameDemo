#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QPushButton>
#include <QKeyEvent>

#include "src/CustomPushButton/custompushbutton.h"
#include "src/TcpServer/tcpserver.h"
#include "src/Logger/Logger.h"

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow {
Q_OBJECT

public:
    explicit MainWindow(QWidget *parent = nullptr);

    ~MainWindow() override;

private:
    Ui::MainWindow *ui;
    TcpServer *tcpServer;
    Logger *logger;
    InputCommands commands;

    void assignButtonIDs();

    void connectSignals();

    bool updateCommands(int key, bool isClicked);

protected:

    void keyPressEvent(QKeyEvent *qKeyEvent) override;

    void keyReleaseEvent(QKeyEvent *qKeyEvent) override;

private slots:

    void clientConnected();

    void clientDisconnected();

    void clientSentResponse(ClientAnswer clientAnswer);

    void startGameBtnClicked();

    void buttonPressed(int id);

    void buttonReleased(int id);
};

#endif // MAINWINDOW_H
