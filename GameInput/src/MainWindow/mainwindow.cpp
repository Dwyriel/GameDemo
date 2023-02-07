#include "mainwindow.h"
#include "./ui_mainwindow.h"

MainWindow::MainWindow(QWidget *parent) : QMainWindow(parent), ui(new Ui::MainWindow), tcpServer(new TcpServer(this)) {
    ui->setupUi(this);
    assignButtonIDs();
    connectSignals();
}

MainWindow::~MainWindow() {
    delete ui;
    delete tcpServer;
}

void MainWindow::assignButtonIDs() {
    ui->btnStartGame->buttonID = Qt::Key_unknown;
    ui->btnMoveForward->buttonID = Qt::Key_W;
    ui->btnMoveRight->buttonID = Qt::Key_D;
    ui->btnMoveBackward->buttonID = Qt::Key_S;
    ui->btnMoveLeft->buttonID = Qt::Key_A;
    ui->btnRotateUp->buttonID = Qt::Key_I;
    ui->btnRotateRight->buttonID = Qt::Key_L;
    ui->btnRotateDown->buttonID = Qt::Key_K;
    ui->btnRotateLeft->buttonID = Qt::Key_J;
    ui->btnFireWeapon->buttonID = Qt::Key_Space;
}

void MainWindow::connectSignals() {
    ui->btnStartGame->setEnabled(true);
    //Start Game
    connect(ui->btnStartGame, &QPushButton::clicked, this, &MainWindow::buttonClicked);
    //Game Commands Pressed
    connect(ui->btnMoveForward, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnMoveRight, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnMoveBackward, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnMoveLeft, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnRotateUp, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnRotateRight, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnRotateDown, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnRotateLeft, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    connect(ui->btnFireWeapon, &CustomPushButton::buttonPressed, this, &MainWindow::buttonPressed);
    //Game Commands Released
    connect(ui->btnMoveForward, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnMoveRight, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnMoveBackward, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnMoveLeft, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnRotateUp, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnRotateRight, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnRotateDown, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnRotateLeft, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
    connect(ui->btnFireWeapon, &CustomPushButton::buttonReleased, this, &MainWindow::buttonReleased);
}

bool MainWindow::updateCommands(int key, bool isClicked) {
    switch (key) {
        case Qt::Key_W:
            commands.moveForward = isClicked;
            break;
        case Qt::Key_D:
            commands.moveRight = isClicked;
            break;
        case Qt::Key_S:
            commands.moveBackward = isClicked;
            break;
        case Qt::Key_A:
            commands.moveLeft = isClicked;
            break;
        case Qt::Key_I:
            commands.rotateUp = isClicked;
            break;
        case Qt::Key_L:
            commands.rotateRight = isClicked;
            break;
        case Qt::Key_K:
            commands.rotateDown = isClicked;
            break;
        case Qt::Key_J:
            commands.rotateLeft = isClicked;
            break;
        case Qt::Key_Space:
            commands.fireWeapon = isClicked;
            break;
        default:
            return false;
    }
    return true;
}

void MainWindow::keyPressEvent(QKeyEvent *qKeyEvent) {
    bool handled = false;
    if (!qKeyEvent->isAutoRepeat())
        handled = updateCommands(qKeyEvent->key(), true);
    if (handled) {
        tcpServer->SendCommands(commands);
        return;
    }
    QWidget::keyPressEvent(qKeyEvent);
}

void MainWindow::keyReleaseEvent(QKeyEvent *qKeyEvent) {
    bool handled = false;
    if (!qKeyEvent->isAutoRepeat())
        handled = updateCommands(qKeyEvent->key(), false);
    if (handled) {
        tcpServer->SendCommands(commands);
        return;
    }
    QWidget::keyReleaseEvent(qKeyEvent);
}

void MainWindow::buttonClicked() {
    //todo start game
}

void MainWindow::buttonPressed(int id) {
    updateCommands(id, true);
    tcpServer->SendCommands(commands);
}

void MainWindow::buttonReleased(int id) {
    updateCommands(id, false);
    tcpServer->SendCommands(commands);
}
