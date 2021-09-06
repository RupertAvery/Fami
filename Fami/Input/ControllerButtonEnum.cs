namespace Fami.Input
{
    public enum ControllerButtonEnum
    {
        // Add offset for controller buttons to make things easier
        Up = 0x108,
        Down = 0x104,
        Left = 0x102,
        Right = 0x101,
        A = 0x180,
        B = 0x140,
        Select = 0x120,
        Start = 0x110,
        // Standard buttons go here
        Rewind = 1,
        SaveState = 2,
        LoadState = 3,
        NextState = 4,
        PreviousState = 5,
    }
}