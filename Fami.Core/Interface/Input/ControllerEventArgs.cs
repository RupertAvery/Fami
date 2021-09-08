namespace Fami.Core.Interface.Input
{
    public class ControllerEventArgs
    {
        public int Player { get; set; }
        public ControllerEventType EventType { get; set; }
        public ControllerButtonEnum Button { get; set; }
        public int ZapperX { get; set; }
        public int ZapperY { get; set; }
        public bool Trigger { get; set; }
    }
}