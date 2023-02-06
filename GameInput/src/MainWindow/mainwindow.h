#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QPushButton>
#include <QKeyEvent>

#include "src/TcpServer/TcpServer.h"

QT_BEGIN_NAMESPACE
namespace Ui { class MainWindow; }
QT_END_NAMESPACE

class MainWindow : public QMainWindow {
Q_OBJECT

public:
    MainWindow(QWidget *parent = nullptr);

    ~MainWindow();

private:
    Ui::MainWindow *ui;
    TcpServer *tcpServer;
    Commands commands;

    void connectSignals();

protected:

    void keyPressEvent(QKeyEvent *qKeyEvent) override;

    void keyReleaseEvent(QKeyEvent *qKeyEvent) override;

private slots:

    void buttonClicked();
};

#endif // MAINWINDOW_H
