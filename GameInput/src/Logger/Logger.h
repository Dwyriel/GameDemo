#ifndef GAMEINPUT_LOGGER_H
#define GAMEINPUT_LOGGER_H

#include <QCoreApplication>
#include <QObject>
#include <QString>
#include <QDir>
#include <QFile>
#include <QDateTime>

#include <src/defines.h>

class Logger : QObject {
Q_OBJECT
public:
    static Logger *Instance();

    void writeLog(const QString &log);

    static QString bytesToHexRepresentation(char* bytes, int size);

private:
    explicit Logger();

    QString logsDirPath;
    QFile file;

    static QString getFormattedCurrentTime();
};


#endif //GAMEINPUT_LOGGER_H
