﻿using System;
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
        private Emulator em;
        string _chosenFile = "";
        public Form1()
        {
            InitializeComponent();
            em = new Emulator();
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

            _chosenFile = openFD.FileName;
            em.LoadRom(_chosenFile);

        }
    }
}
