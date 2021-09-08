using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fami.Core.Interface;
using Fami.Core.Interface.Input;

namespace Fami.UI
{
    public partial class MappingForm : Form
    {
        private readonly Main _main;

        public MappingForm(Main main)
        {
            _main = main;
            InitializeComponent();
        }
        
        private void ShowMapping(ControllerButtonEnum button)
        {
            _main.SetMapping(button);
            var mapWait = new MapWaitForm(_main.InputProvider, button);
            mapWait.Top = Top + (Height - mapWait.Height) / 2;
            mapWait.Left = Left + (Width - mapWait.Width) / 2;
            mapWait.ShowDialog(this);
        }

        private void GetMapping(ControllerButtonEnum button, TextBox textbox)
        {
            textbox.Text = _main.InputProvider.GetMapping(button);
        }

        private void MappingForm_Load(object sender, EventArgs e)
        {
            GetMapping(ControllerButtonEnum.Up, textBoxUp);
            GetMapping(ControllerButtonEnum.Down, textBoxDown);
            GetMapping(ControllerButtonEnum.Left, textBoxLeft);
            GetMapping(ControllerButtonEnum.Right, textBoxRight);
            GetMapping(ControllerButtonEnum.B, textBoxB);
            GetMapping(ControllerButtonEnum.A, textBoxA);
            GetMapping(ControllerButtonEnum.Select, textBoxSelect);
            GetMapping(ControllerButtonEnum.Start, textBoxStart);


            buttonUp.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Up); GetMapping(ControllerButtonEnum.Up, textBoxUp); };
            buttonDown.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Down); GetMapping(ControllerButtonEnum.Down, textBoxDown); };
            buttonLeft.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Left); GetMapping(ControllerButtonEnum.Left, textBoxLeft); };
            buttonRight.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Right); GetMapping(ControllerButtonEnum.Right, textBoxRight); };
            buttonB.Click += (o, e) => { ShowMapping(ControllerButtonEnum.B); GetMapping(ControllerButtonEnum.B, textBoxB); };
            buttonA.Click += (o, e) => { ShowMapping(ControllerButtonEnum.A); GetMapping(ControllerButtonEnum.A, textBoxA); };
            buttonSelect.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Select); GetMapping(ControllerButtonEnum.Select, textBoxSelect); };
            buttonStart.Click += (o, e) => { ShowMapping(ControllerButtonEnum.Start); GetMapping(ControllerButtonEnum.Start, textBoxStart); };
        }
    }
}
