using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormPeriod : Form
    {
        public FormPeriod()
        {
            InitializeComponent();
        }

        DialogResult Res;

        public DialogResult ShowDialog(out int m,out int y)
        {
            Res = ShowDialog();
            m = comboBox1.SelectedIndex + 1;
            y = comboBox2.SelectedIndex + 2007;
            return Res;
        }

        private void FormPeriod_Shown(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = Form1.year - 2007;
            comboBox1.SelectedIndex = Form1.month - 1;            
        }

    }
}