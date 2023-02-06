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
