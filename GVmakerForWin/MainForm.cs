using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GVmakerForWin
{
     partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            mainCanvas.BackColor = Color.White;
        }

        private void MainForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            Keys keys =e.KeyCode;
            if(keys == Keys.PageUp)
            {

            }
        }
    }
}
