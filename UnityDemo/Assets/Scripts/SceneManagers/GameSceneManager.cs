using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviour
{
    #region Events

    public delegate void GameOverEventHandler();

    public event GameOverEventHandler GameOverEvent;

    #endregion

    [SerializeField] private Text connectionLostUIText;
    [SerializeField] private Text elapsedTimeText;
    [SerializeField] private Text shotsFiredText;
    [SerializeField] private Text enemiesLeftText;
    [SerializeField] private Text gameOverText;
    [SerializeField] private GameObject mapCenter;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Min(0)] private float spawnZoneVerticalVariation = 5f;
    [SerializeField, Min(0)] private float spawnZoneHorizontalVariation = 50f;
    [SerializeField, Min(0)] private float spawnZoneVerticalOffset = 50f;
    [SerializeField, Min(0)] private float spawnZoneHorizontalOffset = 350f;

    private const string ShotsFiredString = "Shots fired: ";
    private const string EnemiesLeftString = "Enemies left: ";
    private bool _shouldRunUpdate = true;
    private float _elapsedTime;
    private int _remainingEnemies;
    private int _shotsFired;
    private bool _wasConnected = true;

    private void Start()
    {
        SpawnEnemies();
        GameObject.FindWithTag(ConstValuesAndUtility.PlayerTag).GetComponent<PlayerScript>().ShotFiredEvent += ShotFired;
        enemiesLeftText.text = EnemiesLeftString + _remainingEnemies;
        _wasConnected = TcpClientScript.IsConnected;
        if (_wasConnected)
            TellServerGameStarted();
    }

    // Update is called once per frame
    private void Update()
    {
        if (!_shouldRunUpdate)
            return;
        if (!_wasConnected && TcpClientScript.IsConnected)
            TellServerGameStarted();
        _wasConnected = TcpClientScript.IsConnected;
        _elapsedTime += Time.deltaTime;
        elapsedTimeText.text = _elapsedTime.ToString("0.00");
        connectionLostUIText.text = TcpClientScript.IsConnected ? "" : "Lost Connection";
        if (_elapsedTime > ConstValuesAndUtility.MaxRoundTime || _remainingEnemies == 0)
            GameOver();
    }

    private void SpawnEnemies()
    {
        var y = spawnZoneVerticalOffset - spawnZoneVerticalVariation;
        for (var i = 0; i < ConstValuesAndUtility.NumberOfEnemiesToSpawn; i++, _remainingEnemies++, y += spawnZoneVerticalVariation)
        {
            float x, z;
            var spawnOnXAxis = Random.value > .5;
            if (spawnOnXAxis)
            {
                x = Random.Range(-spawnZoneHorizontalOffset, spawnZoneHorizontalOffset);
                z = (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset) + Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation);
            }
            else //spawn on Z axis
            {
                x = (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset) + Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation);
                z = Random.Range(-spawnZoneHorizontalOffset, spawnZoneHorizontalOffset);
            }
            var spawnPoint = new Vector3(x, y, z);
            var instantiatedGameObject = Instantiate(enemyPrefab, spawnPoint, Quaternion.LookRotation(mapCenter.transform.position - spawnPoint));
            instantiatedGameObject.GetComponent<EnemyScript>().EnemyDestroyedEvent += EnemyDestroyed;
        }
    }

    private void ShotFired()
    {
        _shotsFired++;
        shotsFiredText.text = ShotsFiredString + _shotsFired;
    }

    private void EnemyDestroyed()
    {
        _remainingEnemies--;
        enemiesLeftText.text = EnemiesLeftString + _remainingEnemies;
    }

    private void TellServerGameStarted()
    {
        var clientAnswer = new ClientAnswer
        {
            MessageLength = sizeof(int),
            MessageType = MessageType.GameResponse,
            GameResponse = GameResponse.GameStarted
        };
        TcpClientScript.Instance.SendAnswerToServer(clientAnswer);
    }

    private void GameOver()
    {
        _shouldRunUpdate = false;
        GameOverEvent?.Invoke();
        gameOverText.text = "Game Over";
        var clientAnswer = new ClientAnswer
        {
            MessageLength = sizeof(int) * 3,
            MessageType = MessageType.GameStats,
            ElapsedTime = (int) (_elapsedTime * 1000f),
            ShotsFired = _shotsFired,
            TargetsHit = ConstValuesAndUtility.NumberOfEnemiesToSpawn - _remainingEnemies
        };
        TcpClientScript.Instance.SendAnswerToServer(clientAnswer);
        StartCoroutine(LoadIdleScene());
    }

    private IEnumerator LoadIdleScene()
    {
        yield return new WaitForSeconds(ConstValuesAndUtility.DelayBeforeLoadingIdleScene);
        var clientAnswer = new ClientAnswer
        {
            MessageLength = sizeof(int),
            MessageType = MessageType.GameResponse,
            GameResponse = GameResponse.GameEnded
        };
        TcpClientScript.Instance.SendAnswerToServer(clientAnswer);
        SceneManager.LoadSceneAsync("Scenes/IdleScene");
    }
}