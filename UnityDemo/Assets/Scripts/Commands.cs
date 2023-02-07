using System.Runtime.InteropServices;

public enum MessageType
{
    GameStart = 0,
    Inputs = 1,
    GameResponse = 2,
    Other = 999
}

public enum GameResponse
{
    GameStarted  = 0,
    GameEnded = 1
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

[StructLayout(LayoutKind.Explicit, Pack=1)]
public struct ClientAnswer
{
    [FieldOffset(0)]
    public int MessageLength;
    [FieldOffset(4)]
    public MessageType MessageType;
    [FieldOffset(8)]
    public GameResponse GameResponse;
    //other fields should also have an offset of 8
}