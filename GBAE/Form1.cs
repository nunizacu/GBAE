using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GBAE
{
    public partial class Form1 : Form
    {
        private String _romFileName;

        public Form1()
        {
            
            InitializeComponent();
            Emulator.LoadRom(@"C:\Advanced Wars  # GBA.GBA");

        }


        private void mnuQuit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void mnuOpen_Click(object sender, EventArgs e)
        {
            openFD.Title = "Insert rom";
            openFD.FileName = "";
            openFD.Filter = "GameBoy Advance|*.GBA|All Files|*.*";

            openFD.ShowDialog();

            _romFileName = openFD.FileName;
            //if(_romFileName != "")
            //    em.LoadRom(_romFileName);
            //em.LoadRom(@"C:\Advanced Wars  # GBA.GBA");
        }
    }
}
