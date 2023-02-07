#ifndef GAMEINPUT_MESSAGES_H
#define GAMEINPUT_MESSAGES_H

enum class MessageType : int {
    GameStart = 0,
    Commands = 1,
    Other = 999
};

struct GameStartCommand {
    const int messageLength = sizeof(GameStartCommand) - (sizeof(int) * 2);
    const MessageType messageType = MessageType::Commands;
    int discard = 0;
} __attribute__((packed));

struct InputCommands {
    const int messageLength = sizeof(InputCommands) - (sizeof(int) * 2);
    const MessageType messageType = MessageType::Commands;
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
    MessageType messageType = MessageType::Other;
    QByteArray message;
} __attribute__((packed));

#endif //GAMEINPUT_MESSAGES_H
