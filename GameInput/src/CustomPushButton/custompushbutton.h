#ifndef CUSTOMPUSHBUTTON_H
#define CUSTOMPUSHBUTTON_H

#include <QPushButton>
#include <QMouseEvent>
#include <QKeyEvent>

class CustomPushButton : public QPushButton {
Q_OBJECT
public:
    explicit CustomPushButton(QWidget *parent = nullptr, int buttonID = 0);

    int buttonID;

protected:
    void mousePressEvent(QMouseEvent *qMouseEvent) override;

    void mouseReleaseEvent(QMouseEvent *qMouseEvent) override;

    void keyPressEvent(QKeyEvent *qKeyEvent) override;

    void keyReleaseEvent(QKeyEvent *qKeyEvent) override;

signals:

    void buttonPressed(int id);

    void buttonReleased(int id);
};

#endif // CUSTOMPUSHBUTTON_H
