#ifndef GAMEINPUT_MESSAGES_H
#define GAMEINPUT_MESSAGES_H

enum class MessageType : int {
    GameStart = 0,
    Commands = 1,
    GameResponse = 2,
    GameStats = 3,
    Other = 999
};

enum class GameResponse : int {
    GameStarted = 0,
    GameEnded = 1
};

struct GameStartCommand {
    const int messageLength = sizeof(GameStartCommand) - (sizeof(int) * 2);
    const MessageType messageType = MessageType::GameStart;
    int discard = 0;
};

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

struct GameStats {
    int elapsedTime;
    int shotsFired;
    int targetsHit;
};

struct ClientAnswer {
    int messageLength = sizeof(ClientAnswer) - (sizeof(int) * 2);
    MessageType messageType = MessageType::GameResponse;
    union {
        char bytes = 0;
        GameResponse gameResponse;
        GameStats gameStats;
    };
};

/**
 * @attention messageLength should be set to the size of message (message.size())
 */
struct Message {
    int messageLength = 0;
    MessageType messageType = MessageType::Other;
    QByteArray message;
};

#endif //GAMEINPUT_MESSAGES_H
