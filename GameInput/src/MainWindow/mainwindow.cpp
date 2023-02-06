#include "mainwindow.h"
#include "./ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow), tcpServer(new TcpServer(this)) {
    ui->setupUi(this);
    connectSignals();
}

MainWindow::~MainWindow() {
    delete ui;
    delete tcpServer;
}

void MainWindow::connectSignals() {
    connect(ui->pushButton, &QPushButton::clicked, this, &MainWindow::buttonClicked);
}

void MainWindow::buttonClicked() {
    tcpServer->SendCommands(commands);
}

void MainWindow::keyPressEvent(QKeyEvent *qKeyEvent) {
    bool handled = false;
    if(!qKeyEvent->isAutoRepeat()) {
        handled = true;
        switch (qKeyEvent->key()) {
            case Qt::Key_W:
                commands.moveForward = true;
                break;
            case Qt::Key_D:
                commands.moveRight = true;
                break;
            case Qt::Key_S:
                commands.moveBackward = true;
                break;
            case Qt::Key_A:
                commands.moveLeft = true;
                break;
            case Qt::Key_Up:
                commands.rotateUp = true;
                break;
            case Qt::Key_Right:
                commands.rotateRight = true;
                break;
            case Qt::Key_Down:
                commands.rotateDown = true;
                break;
            case Qt::Key_Left:
                commands.rotateLeft = true;
                break;
            case Qt::Key_G:
                commands.fireWeapon = true;
                break;
            default:
                handled = false;
        }
    }
    if(handled) {
        tcpServer->SendCommands(commands);
        return;
    }
    QWidget::keyPressEvent(qKeyEvent);
}

void MainWindow::keyReleaseEvent(QKeyEvent *qKeyEvent) {
    bool handled = false;
    if(!qKeyEvent->isAutoRepeat()) {
        handled = true;
        switch (qKeyEvent->key()) {
            case Qt::Key_W:
                commands.moveForward = false;
                break;
            case Qt::Key_D:
                commands.moveRight = false;
                break;
            case Qt::Key_S:
                commands.moveBackward = false;
                break;
            case Qt::Key_A:
                commands.moveLeft = false;
                break;
            case Qt::Key_Up:
                commands.rotateUp = false;
                break;
            case Qt::Key_Right:
                commands.rotateRight = false;
                break;
            case Qt::Key_Down:
                commands.rotateDown = false;
                break;
            case Qt::Key_Left:
                commands.rotateLeft = false;
                break;
            case Qt::Key_G:
                commands.fireWeapon = false;
                break;
            default:
                handled = false;
        }
    }
    if(handled) {
        tcpServer->SendCommands(commands);
        return;
    }
    QWidget::keyReleaseEvent(qKeyEvent);
}
