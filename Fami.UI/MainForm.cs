using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Fami.UI
{
    public partial class MainForm : Form
    {
        public Func<string, bool> LoadRom { get; set; }
        public Action<int> ChangeSlot { get; set; }
        public Action LoadState { get; set; }
        public Action SaveState { get; set; }

        public MainForm()
        {
            InitializeComponent();
        }



        private void MainForm_Load(object sender, EventArgs e)
        {
            slot1ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(1); slot1ToolStripMenuItem.Checked = true; };
            slot2ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(2); slot2ToolStripMenuItem.Checked = true; };
            slot3ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(3); slot3ToolStripMenuItem.Checked = true; };
            slot4ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(4); slot4ToolStripMenuItem.Checked = true; };
            slot5ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(5); slot5ToolStripMenuItem.Checked = true; };
            slot6ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(6); slot6ToolStripMenuItem.Checked = true; };
            slot7ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(7); slot7ToolStripMenuItem.Checked = true; };
            slot8ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(8); slot8ToolStripMenuItem.Checked = true; };
            slot9ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(9); slot9ToolStripMenuItem.Checked = true; };
            slot10ToolStripMenuItem.Click += (o, args) => { UncheckSlots(); ChangeSlot?.Invoke(10); slot10ToolStripMenuItem.Checked = true; };

            loadStateToolStripMenuItem.Click += (o, args) => LoadState?.Invoke();
            saveStateToolStripMenuItem.Click += (o, args) => SaveState?.Invoke();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "All supported files|*.nes;*.zip|All files|*.*";
            var result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (LoadRom != null)
                {
                    if (LoadRom.Invoke(openFileDialog1.FileName))
                    {
                        SetMenus(true);
                    }
                }
            }
        }

        private void UncheckSlots()
        {
            slot1ToolStripMenuItem.Checked = false;
            slot2ToolStripMenuItem.Checked = false;
            slot3ToolStripMenuItem.Checked = false;
            slot4ToolStripMenuItem.Checked = false;
            slot5ToolStripMenuItem.Checked = false;
            slot6ToolStripMenuItem.Checked = false;
            slot7ToolStripMenuItem.Checked = false;
            slot8ToolStripMenuItem.Checked = false;
            slot9ToolStripMenuItem.Checked = false;
            slot10ToolStripMenuItem.Checked = false;
        }

        private void SetMenus(bool enabled)
        {
            changeSlotToolStripMenuItem.Enabled = enabled;
            loadStateToolStripMenuItem.Enabled = enabled;
            saveStateToolStripMenuItem.Enabled = enabled;
        }
    }
}
