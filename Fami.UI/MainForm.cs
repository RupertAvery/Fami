using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fami.Core.Interface;
using Fami.Core.Interface.Input;

namespace Fami.UI
{
    public partial class MainForm : Form, IMainInterface
    {
        private readonly Main _main;
        public Action OnHostResize { get; set; }
        public Func<string, bool> LoadRom { get; set; }
        public Action<int> ChangeSlot { get; set; }
        public Action LoadState { get; set; }
        public Action SaveState { get; set; }
        public Action<int,int> ResizeWindow { get; set; }
        public Action<ControllerButtonEnum> SetMapping { get; set; }
        public Configuration Configuration { get;set;}

        public MainForm(Main main)
        {
            _main = main;
            _main.StateChanged = StateChanged;
            SizeChanged += (sender, args) => OnHostResize?.Invoke();
            InitializeComponent();
            Configuration = new Configuration();
        }

        private void StateChanged(int slot)
        {
            UncheckSlots();
            switch (slot)
            {
                case 1: slot1ToolStripMenuItem.Checked = true; break;
                case 2: slot2ToolStripMenuItem.Checked = true; break;
                case 3: slot3ToolStripMenuItem.Checked = true; break;
                case 4: slot4ToolStripMenuItem.Checked = true; break;
                case 5: slot5ToolStripMenuItem.Checked = true; break;
                case 6: slot6ToolStripMenuItem.Checked = true; break;
                case 7: slot7ToolStripMenuItem.Checked = true; break;
                case 8: slot8ToolStripMenuItem.Checked = true; break;
                case 9: slot9ToolStripMenuItem.Checked = true; break;
                case 10: slot10ToolStripMenuItem.Checked = true; break;

            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            SetSize(4);

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

            x1ToolStripMenuItem.Click += (o, args) => { UncheckSizes(); SetSize(1); x1ToolStripMenuItem.Checked = true; };
            x2ToolStripMenuItem.Click += (o, args) => { UncheckSizes(); SetSize(2); x2ToolStripMenuItem.Checked = true; };
            x3ToolStripMenuItem.Click += (o, args) => { UncheckSizes(); SetSize(3); x3ToolStripMenuItem.Checked = true; };
            x4ToolStripMenuItem.Click += (o, args) => { UncheckSizes(); SetSize(4); x4ToolStripMenuItem.Checked = true; };
        }

        private void SetSize(int scale)
        {
            //ResizeWindow?.Invoke(256 * scale, 240 * scale + menuStrip1.Height);
            Size = new Size(256 * scale, 240 * scale + menuStrip1.Height);
            // This resizes the window *twice*
            //Width = 256 * scale;
            //Height = 240 * scale + menuStrip1.Height;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private const int MaxRecentItems = 10;

        private void AddRecentItem(string item)
        {
            if (!Configuration.RecentItems.Contains(item))
            {
                Configuration.RecentItems.Add(item);
                var recentItem = new ToolStripMenuItem(Path.GetFileName(item));
                recentItem.Click += (sender, args) =>
                {
                    LoadRom.Invoke(item);
                };
                recentToolStripMenuItem.DropDownItems.Insert(0, recentItem);
              
                if (recentToolStripMenuItem.DropDownItems.Count > MaxRecentItems + 1)
                {
                    recentToolStripMenuItem.DropDownItems.RemoveAt(MaxRecentItems);
                }
            }
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
                        AddRecentItem(openFileDialog1.FileName);
                        SetMenus(true);
                    }
                }
            }
        }

        private void UncheckSizes()
        {
            x1ToolStripMenuItem.Checked = false;
            x2ToolStripMenuItem.Checked = false;
            x3ToolStripMenuItem.Checked = false;
            x4ToolStripMenuItem.Checked = false;
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

        private void controllersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var mapper = new MappingForm(_main);
            mapper.Show(this);
        }

        private void clearItemsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Configuration.RecentItems.Clear();
            while (recentToolStripMenuItem.DropDownItems.Count > 1)
            {
                recentToolStripMenuItem.DropDownItems.RemoveAt(recentToolStripMenuItem.DropDownItems.Count - 2);
            }
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            _main.Redraw();
        }
    }
}
