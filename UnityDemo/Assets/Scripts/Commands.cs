enum MessageType
{
    Commands = 0,
    Other = 1
}

public class Commands
{
    public bool StartGame = false;
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