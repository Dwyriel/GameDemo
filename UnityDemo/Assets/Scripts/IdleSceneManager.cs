using System;
using System.Collections;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class IdleSceneManager : MonoBehaviour
{
    [SerializeField] private Text connectionStatusUIText;

    private readonly ConcurrentQueue<Action> _gameThreadActions = new();
    private bool _isGameStarting;

    private void Start()
    {
        TcpClientScript.Instance.GameStartEvent += GameStartCommandReceived;
    }

    private void OnDestroy()
    {
        TcpClientScript.Instance.GameStartEvent -= GameStartCommandReceived;
    }

    private void FixedUpdate()
    {
        if (_isGameStarting)
            return;
        connectionStatusUIText.text = TcpClientScript.IsConnected ? "Connected\nand\nWaiting" : "Awaiting\nConnection";
        while (_gameThreadActions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }

    private void GameStartCommandReceived()
    {
        _gameThreadActions.Enqueue(() => StartCoroutine(LoadGameScene()));
    }

    private IEnumerator LoadGameScene()
    {
        var asyncOperation = SceneManager.LoadSceneAsync("Scenes/GameScene", LoadSceneMode.Single);
        asyncOperation.allowSceneActivation = false;
        _isGameStarting = true;
        connectionStatusUIText.text = "Starting Game";
        yield return new WaitForSeconds(ConstValuesAndUtility.DelayBeforeLoadingScene);
        while (asyncOperation.progress < .9f)
            yield return null;
        var clientAnswer = new ClientAnswer
        {
            MessageLength = sizeof(int),
            MessageType = MessageType.GameResponse,
            GameResponse = GameResponse.GameStarted
        };
        TcpClientScript.Instance.SendAnswerToServer(clientAnswer);
        asyncOperation.allowSceneActivation = true;
    }
}