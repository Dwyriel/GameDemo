using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class TcpClientScript : MonoBehaviour
{
    private const int HeaderSize = sizeof(int) * 2;
    private const int RetryToConnectAfterMilliseconds = 2000;
    private const int RetryToReconnectAfterMilliseconds = 10;
    private const int MaxFailedConnectionsBeforeEndingConnection = 500;
    private readonly List<string> _messages = new();
    private TcpClient _tcpClient;
    private Thread _clientThread;

    public InputCommands InputCommands { get; } = new();
    public bool MessageAvailable => _messages.Count > 0;

    public string GetOldestMessage
    {
        get
        {
            var message = _messages.First();
            _messages.RemoveAt(0);
            return message;
        }
    }

    public static TcpClientScript Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartClientThread();
    }

    private void OnApplicationQuit()
    {
        if (_clientThread is not null && _clientThread.IsAlive)
            _clientThread.Abort();
    }

    private void StartClientThread()
    {
        try
        {
            _clientThread = new Thread(ListenForMessages)
            {
                IsBackground = true
            };
            _clientThread.Start();
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    private void ListenForMessages()
    {
        while (true)
        {
            try
            {
                _tcpClient?.Close();
                _tcpClient = new TcpClient("localhost", 7030);
                using var networkStream = _tcpClient.GetStream();
                var failedConnectionCounter = 0;
                while (true)
                {
                    Debug.Log(failedConnectionCounter);
                    if (failedConnectionCounter > MaxFailedConnectionsBeforeEndingConnection)
                        break;
                    var header = new byte[HeaderSize];
                    var length = networkStream.Read(header, 0, header.Length);
                    Debug.Log(length);
                    if (length < HeaderSize)
                    {
                        Thread.Sleep(RetryToReconnectAfterMilliseconds);
                        failedConnectionCounter++;
                        continue;
                    }
                    var messageLength = BitConverter.ToInt32(header, 0);
                    var messageType = (MessageType) BitConverter.ToInt32(header, 0 + sizeof(int));
                    switch (messageType)
                    {
                        case MessageType.GameStart:
                            var gameStartData = new byte[messageLength];
                            length = networkStream.Read(gameStartData, 0, messageLength);
                            //todo
                            break;
                        case MessageType.Inputs:
                            var inputs = new byte[messageLength];
                            length = networkStream.Read(inputs, 0, messageLength);
                            if (length != messageLength)
                                continue;
                            failedConnectionCounter = 0;
                            UpdateInputCommands(inputs);
                            break;
                        case MessageType.Other:
                            var message = new byte[messageLength];
                            length = networkStream.Read(message, 0, messageLength);
                            failedConnectionCounter = 0;
                            _messages.Add(Encoding.ASCII.GetString(message, 0, length));
                            break;
                        default:
                            continue;
                    }
                }
            }
            catch (System.IO.IOException)
            {
                break;
            }
            catch (ThreadAbortException)
            {
                break;
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
                Thread.Sleep(RetryToConnectAfterMilliseconds);
            }
        }
    }

    private void SendMessageToServer(string message)
    {
        if (_tcpClient is null)
            return;
        try
        {
            using var stream = _tcpClient.GetStream();
            if (!stream.CanWrite)
                return;
            var messageAsBytes = Encoding.ASCII.GetBytes(message);
            stream.Write(messageAsBytes, 0, messageAsBytes.Length);
        }
        catch (SocketException socketException)
        {
            Debug.Log(socketException);
        }
    }

    private void UpdateInputCommands(byte[] commands)
    {
        var index = 0;
        InputCommands.FireWeapon = BitConverter.ToBoolean(commands, index++);
        InputCommands.MoveForward = BitConverter.ToBoolean(commands, index++);
        InputCommands.MoveRight = BitConverter.ToBoolean(commands, index++);
        InputCommands.MoveBackward = BitConverter.ToBoolean(commands, index++);
        InputCommands.MoveLeft = BitConverter.ToBoolean(commands, index++);
        InputCommands.RotateUp = BitConverter.ToBoolean(commands, index++);
        InputCommands.RotateRight = BitConverter.ToBoolean(commands, index++);
        InputCommands.RotateDown = BitConverter.ToBoolean(commands, index++);
        InputCommands.RotateLeft = BitConverter.ToBoolean(commands, index);
    }
}