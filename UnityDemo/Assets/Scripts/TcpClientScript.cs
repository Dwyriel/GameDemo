using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;

public class TcpClientScript : MonoBehaviour
{
    public static TcpClientScript Instance { get; private set; }

    private TcpClient _tcpClient;
    private Thread _clientThread;
    private readonly List<string> _messages = new();

    public bool IsConnected => _tcpClient?.Connected ?? false;

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

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        StartClientThread();
    }

    void Update()
    {
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
                while (IsConnected)
                {
                    if (failedConnectionCounter > 1000)
                        break;
                    var data = new byte[256];
                    var message = new Span<byte>();
                    var length = networkStream.Read(data, 0, data.Length);
                    if (length > 0)
                    {
                        failedConnectionCounter = 0;
                        Debug.Log(Encoding.ASCII.GetString(data, 0, length));
                        _messages.Add(message.ToString());
                    }

                    Thread.Sleep(10);
                    failedConnectionCounter++;
                }
            }
            catch (SocketException socketException)
            {
                Debug.Log(socketException);
                Thread.Sleep(2000);
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
}