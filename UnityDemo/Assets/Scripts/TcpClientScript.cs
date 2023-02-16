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
    private const int TimeBetweenReconnectAttemptsMilliseconds = 2000;
    private const int TimeBetweenReadAttemptsMilliseconds = 10;
    private const int MaxFailedReadAttemptsBeforeEndingConnection = 10;
    private readonly List<string> _messages = new();
    private TcpClient _tcpClient;
    private Thread _clientThread;

    #region Events

    public delegate void GameStartEventHandler();

    public event GameStartEventHandler GameStartEvent;

    #endregion

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
    public static bool IsConnected = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
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
                IsConnected = false;
                _tcpClient?.Close();
                _tcpClient = new TcpClient(ConstValuesAndUtility.IpAddress, ConstValuesAndUtility.Port);
                using var networkStream = _tcpClient.GetStream();
                IsConnected = true;
                var failedConnectionCounter = 0;
                while (true)
                {
                    if (failedConnectionCounter > MaxFailedReadAttemptsBeforeEndingConnection)
                        break;
                    var header = new byte[HeaderSize];
                    var length = networkStream.Read(header, 0, header.Length);
                    if (length < HeaderSize)
                    {
                        Thread.Sleep(TimeBetweenReadAttemptsMilliseconds);
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
                            if (length != messageLength)
                                continue;
                            GameStartEvent?.Invoke();
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
                IsConnected = false;
                break;
            }
            catch (ThreadAbortException)
            {
                IsConnected = false;
                break;
            }
            catch (Exception exception)
            {
                IsConnected = false;
                Debug.Log(exception);
                Thread.Sleep(TimeBetweenReconnectAttemptsMilliseconds);
            }
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

    public void SendAnswerToServer(ClientAnswer answer)
    {
        if (_tcpClient is null)
            return;
        try
        {
            var header = BitConverter.GetBytes(answer.MessageLength).Concat(BitConverter.GetBytes((int) answer.MessageType));
            var message = answer.MessageType switch
            {
                MessageType.GameResponse => header.Concat(BitConverter.GetBytes((int) answer.GameResponse)).ToArray(),
                MessageType.GameStats => header.Concat(BitConverter.GetBytes(answer.ElapsedTime)).Concat(BitConverter.GetBytes(answer.ShotsFired)).Concat(BitConverter.GetBytes(answer.TargetsHit)).ToArray(),
                _ => null
            };
            if (message is null)
                return;
            _tcpClient.GetStream().Write(message, 0, message.Length);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }

    public void SendMessageToServer(string message)
    {
        if (_tcpClient is null)
            return;
        try
        {
            var messageAsBytes = Encoding.ASCII.GetBytes(message);
            var header = BitConverter.GetBytes(messageAsBytes.Length).Concat(BitConverter.GetBytes((int) MessageType.Other));
            var fullMessage = header.Concat(messageAsBytes).ToArray();
            _tcpClient.GetStream().Write(fullMessage, 0, fullMessage.Length);
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
        }
    }
}