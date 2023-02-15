using System.Runtime.InteropServices;

public enum MessageType
{
    GameStart = 0,
    Inputs = 1,
    GameResponse = 2,
    GameStats = 3,
    Other = 999
}

public enum GameResponse
{
    GameStarted = 0
}

public class InputCommands
{
    public bool FireWeapon = false;
    public bool MoveForward = false;
    public bool MoveRight = false;
    public bool MoveBackward = false;
    public bool MoveLeft = false;
    public bool RotateUp = false;
    public bool RotateRight = false;
    public bool RotateDown = false;
    public bool RotateLeft = false;
}

[StructLayout(LayoutKind.Explicit, Pack = 1)]
public struct ClientAnswer
{
    #region Header

    [FieldOffset(0)] public int MessageLength;
    [FieldOffset(4)] public MessageType MessageType;

    #endregion

    #region GameResponse

    [FieldOffset(8)] public GameResponse GameResponse;

    #endregion

    #region GameStats

    [FieldOffset(8)] public int ElapsedTime;
    [FieldOffset(12)] public int ShotsFired;
    [FieldOffset(16)] public int TargetsHit;

    #endregion

    //other fields should also have an offset of 8
}