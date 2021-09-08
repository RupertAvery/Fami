using System.Windows.Forms;

namespace Fami.UI
{
    public static class FormExtensions
    {
        public static void InvokeIfRequired(this Form control, MethodInvoker action)
        {
            // See Update 2 for edits Mike de Klerk suggests to insert here.

            if (control.InvokeRequired)
            {
                control.Invoke(action);
            }
            else
            {
                action();
            }
        }
    }
}