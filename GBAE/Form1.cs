using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBAE
{
    public partial class Form1 : Form
    {
        private Emulator e;
        public Form1()
        {
            InitializeComponent();
            e = new Emulator();
            StartButton.Click += new EventHandler(StartClick);
        }

        private void StartClick(Object sender, EventArgs ea)
        {
            e.LoadRom();
        }
    }
}
