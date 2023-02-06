#ifndef GAMEINPUT_MESSAGES_H
#define GAMEINPUT_MESSAGES_H

struct Commands {
    const int messageLength = sizeof(Commands) - (sizeof(int) * 2);
    const int messageType = 0;
    bool startGame = false;
    bool fireWeapon = false;
    bool moveForward = false;
    bool moveRight = false;
    bool moveBackward = false;
    bool moveLeft = false;
    bool rotateUp = false;
    bool rotateRight = false;
    bool rotateDown = false;
    bool rotateLeft = false;
} __attribute__((packed));

/**
 * @attention messageLength should be set to the size of message (message.size())
 */
struct Message {
    int messageLength = 0;
    const int messageType = 1;
    QByteArray message;
} __attribute__((packed));

#endif //GAMEINPUT_MESSAGES_H
