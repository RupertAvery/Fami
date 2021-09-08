using System;
using Fami.Core.Interface.Input;

namespace Fami.Core.Interface
{
    public interface IMainInterface
    {

        IntPtr Handle { get;  }
        Action OnHostResize { get; set; }
        Func<string, bool> LoadRom { get; set; }
        Action<int> ChangeSlot { get; set; }
        Action LoadState { get; set; }
        Action SaveState { get; set; }
        Action<int, int> ResizeWindow { get; set; }
        Action<ControllerButtonEnum> SetMapping { get; set; }
    }
}