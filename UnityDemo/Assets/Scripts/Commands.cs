enum MessageType
{
    GameStart = 0,
    Inputs = 1,
    Other = 999
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