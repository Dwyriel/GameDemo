using UnityEngine;

public static class ConstValuesAndUtility
{
    public const string IpAddress = "127.0.0.1";
    public const int Port = 7030;
    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";
    public const string BulletTag = "Bullet";
    public const string ScenarioTag = "Scenario";

    public static void AddTag(Transform transform, string tag)
    {
        transform.gameObject.tag = tag;
        foreach (Transform childTransform in transform)
        {
            AddTag(childTransform, tag);
        }
    }
}
