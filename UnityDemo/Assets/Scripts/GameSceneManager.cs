using System.Collections;
using System.Globalization;
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

    private void Start()
    {
        SpawnEnemies();
        GameObject.FindWithTag(ConstValuesAndUtility.PlayerTag).GetComponent<PlayerScript>().ShotFiredEvent += ShotFired;
        enemiesLeftText.text = EnemiesLeftString + _remainingEnemies;
    }

    // Update is called once per frame
    private void Update()
    {
        if(!_shouldRunUpdate)
            return;
        _elapsedTime += Time.deltaTime;
        elapsedTimeText.text = _elapsedTime.ToString("0.00");
        connectionLostUIText.text = TcpClientScript.IsConnected ? "" : "Lost Connection";
        if (_elapsedTime <= ConstValuesAndUtility.MaxRoundTime && _remainingEnemies != 0) 
            return;
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

    private void SpawnEnemies()
    {
        var mapCenterPosition = mapCenter.transform.position;
        for (var i = 0; i < ConstValuesAndUtility.NumberOfEnemiesToSpawn; i++, _remainingEnemies++)
        {
            var x = Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation) + (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset);
            var y = Random.Range(-spawnZoneVerticalVariation, spawnZoneVerticalVariation) + spawnZoneVerticalOffset;
            var z = Random.Range(-spawnZoneHorizontalVariation, spawnZoneHorizontalVariation) + (Random.value > .5 ? spawnZoneHorizontalOffset : -spawnZoneHorizontalOffset);
            var spawnPoint = new Vector3(x, y, z);
            var instantiatedGameObject = Instantiate(enemyPrefab, spawnPoint, Quaternion.LookRotation(mapCenterPosition - spawnPoint));
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