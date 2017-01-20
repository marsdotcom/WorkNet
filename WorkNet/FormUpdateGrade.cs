using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormUpdateGrade : Form
    {
        bool append;
        DataRow row;

        public FormUpdateGrade()
        {
            InitializeComponent();
            append = true;
        }

        public FormUpdateGrade(object ID)
        {
            InitializeComponent();
            append = false;
            row = FormWokers.dataset.Tables[2].Rows.Find(ID);
            textBox1.Text = row[0].ToString();
            textBox2.Text = row[1].ToString();
            textBox3.Text = row[2].ToString();
            textBox4.Text = row[3].ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (append)
                FormWokers.dataset.Tables[2].Rows.Add(
                    textBox1.Text,
                    textBox2.Text,
                    textBox3.Text,
                    textBox4.Text);
            else
            {
                row[0] = textBox1.Text;
                row[1] = textBox2.Text;
                row[2] = textBox3.Text;
                row[3] = textBox4.Text;
            }
        }
    }
}