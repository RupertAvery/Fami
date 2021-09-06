namespace Fami.Input
{
    public class ControllerEventArgs
    {
        public ControllerButtonEvent Event { get; set; }
        public int Player { get; set; }
        public ControllerButtonEnum Button { get; set; }
    }
}