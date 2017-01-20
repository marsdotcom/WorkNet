using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormAddGroup : Form
    {
        bool append;
        DataRow row;
        string expstring;

        public FormAddGroup()
        {
            InitializeComponent();
            append = true;
        }

        public FormAddGroup(object ID)
        {
            InitializeComponent();
            append = false;
            row = FormWokers.dataset.Tables[1].Rows.Find(ID);
            textBox1.Text = row[0].ToString();
            textBox2.Text = row[1].ToString();
            checkBox1.Checked = (bool)row[2];
            expstring = row[3].ToString();

            DisplayText();
        }

        void DisplayText()
        {
            if (expstring != null)
            {
                textBox3.Text = Expressions.ExpStrToText(expstring);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (append)
                FormWokers.dataset.Tables[1].Rows.Add(
                    textBox1.Text,
                    textBox2.Text,
                    checkBox1.Checked,
                    expstring);
            else
            {
                row[0] = textBox1.Text;
                row[1] = textBox2.Text;
                row[2] = checkBox1.Checked;
                row[3] = expstring;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FormFormulas F = new FormFormulas();
            F.expstring = expstring;
            F.ShowDialog();
            expstring = F.expstring;
            DisplayText();
            Groups.expchanged = true;
        }
    }
}