#include "Logger.h"

Logger::Logger() {
    logsDirPath = QCoreApplication::applicationDirPath() + QDir::separator() + LOGS_DIR_NAME;
    QDir logDir(logsDirPath);
    if (!logDir.exists())
        logDir.mkdir(logDir.path());
    file.setFileName(logsDirPath + QDir::separator() + getFormattedCurrentTime() + LOG_FILE_EXTENSION);
    int fileExistCounter = 1;
    while (file.exists())
        file.setFileName(logsDirPath + QDir::separator() + getFormattedCurrentTime() + "_" + QString::number(++fileExistCounter) + LOG_FILE_EXTENSION);
}

Logger *Logger::Instance() {
    static auto logger = new Logger();
    return logger;
}

void Logger::writeLog(const QString &log) {
    file.open(QIODeviceBase::Append | QIODeviceBase::ReadWrite);
    QTextStream fileOutput(&file);
    fileOutput << '[' << getFormattedCurrentTime() << "] " << log << '\n';
    file.close();
}

QString Logger::bytesToHexRepresentation(char *bytes, int size) {
    QString str;
    str.reserve(size * 2);
    for(int i = 0; i < size; i ++)
        str.append(QString("%1").arg((unsigned char)bytes[i], 0 , 16));
    return str;
}

QString Logger::getFormattedCurrentTime() {
    return QDateTime::currentDateTime().toString(DATE_FORMAT);
}
