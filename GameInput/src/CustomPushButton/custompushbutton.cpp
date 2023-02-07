#include "custompushbutton.h"

CustomPushButton::CustomPushButton(QWidget *parent, int buttonID) : QPushButton(parent), buttonID(buttonID) {

}

void CustomPushButton::keyPressEvent(QKeyEvent *qKeyEvent) {
    if (qKeyEvent->key() == Qt::Key_Space) {
        qKeyEvent->ignore();
        return;
    }
    QPushButton::keyPressEvent(qKeyEvent);
}
