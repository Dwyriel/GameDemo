#include "custompushbutton.h"

CustomPushButton::CustomPushButton(QWidget *parent, int buttonID) : QPushButton(parent), buttonID(buttonID) {}

void CustomPushButton::mousePressEvent(QMouseEvent *qMouseEvent) {
    if (qMouseEvent->button() == Qt::LeftButton)
        emit buttonPressed(buttonID);
    QAbstractButton::mousePressEvent(qMouseEvent);
}

void CustomPushButton::mouseReleaseEvent(QMouseEvent *qMouseEvent) {
    if (qMouseEvent->button() == Qt::LeftButton)
        emit buttonReleased(buttonID);
    QAbstractButton::mouseReleaseEvent(qMouseEvent);
}

void CustomPushButton::keyPressEvent(QKeyEvent *qKeyEvent) {
    if (qKeyEvent->key() == Qt::Key_Space) {
        qKeyEvent->ignore();
        return;
    }
    if(!qKeyEvent->isAutoRepeat() && (qKeyEvent->key() == Qt::Key_Return || qKeyEvent->key() == Qt::Key_Enter)){
        emit buttonPressed(buttonID);
        qKeyEvent->ignore();
        return;
    }
    QPushButton::keyPressEvent(qKeyEvent);
}

void CustomPushButton::keyReleaseEvent(QKeyEvent *qKeyEvent) {
    if(!qKeyEvent->isAutoRepeat() && (qKeyEvent->key() == Qt::Key_Return || qKeyEvent->key() == Qt::Key_Enter)){
        emit buttonReleased(buttonID);
        emit clicked();
        qKeyEvent->ignore();
        return;
    }
    QAbstractButton::keyReleaseEvent(qKeyEvent);
}
