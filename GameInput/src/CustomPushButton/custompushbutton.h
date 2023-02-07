#ifndef CUSTOMPUSHBUTTON_H
#define CUSTOMPUSHBUTTON_H

#include <QPushButton>
#include <QKeyEvent>

class CustomPushButton : public QPushButton {
Q_OBJECT
public:
    explicit CustomPushButton(QWidget *parent = nullptr, int buttonID = 0);

    int buttonID;

protected:
    void keyPressEvent(QKeyEvent *qKeyEvent) override;
};

#endif // CUSTOMPUSHBUTTON_H
