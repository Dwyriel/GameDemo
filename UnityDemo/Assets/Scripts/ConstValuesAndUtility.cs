using UnityEngine;

public static class ConstValuesAndUtility
{
    public const string IpAddress = "127.0.0.1";
    public const int Port = 7030;

    #region tags

    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";
    public const string BulletTag = "Bullet";
    public const string ScenarioTag = "Scenario";
    public const string MapCenterPointTag = "MapCenterPoint";

    #endregion

    #region Round

    public const float DelayBeforeLoadingGameScene = 2f;
    public const float DelayBeforeLoadingIdleScene = 5f;
    public const int NumberOfEnemiesToSpawn = 3;
    public const float MaxRoundTime = 120f;

    #endregion

    #region Enemies

    public const float MaxDistanceToMidAllowed = 300;
    public const float RandomMoveTargetRange = 50f;
    public const float MovingUnitsPerSecond = 15;
    public const float RotationAnglePerSecond = 20f;

    #endregion

    public static void AddTag(Transform transform, string tag)
    {
        transform.gameObject.tag = tag;
        foreach (Transform childTransform in transform)
        {
            AddTag(childTransform, tag);
        }
    }
}
