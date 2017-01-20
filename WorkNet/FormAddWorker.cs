using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormAddWorker : Form
    {
        DateTime eDate = new DateTime(3000, 1, 1);
        bool append = true;
        bool dismiss = false;
        bool grade = true;
        bool imagechanged = false;
        int ID;
        public string[] groups;
        DataRow Row;
        public DataTable T;

        public event delAppend UpdateWorker;

        public FormAddWorker()
        {            
            InitializeComponent();
        }

        public DialogResult ShowDialog(int ID)
        {
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(groups);
            dismiss = false;
            this.ID = ID;
            imagechanged = false;
                       

            if (ID == 0)
            {
                button1.Hide();
                append = true;
                button1.Text = "Уволить";
                grade = true;
                pictureBox1.Hide();
                button6.Hide();
            }
            else
            {
                button6.Show();
                pictureBox1.Show();
                Form1.GetImagebyID(ID, ref pictureBox1);
                button1.Show();
                append = false;                
                Row = T.Rows.Find(ID);
                textBox1.Text = Row[2].ToString();
                textBox2.Text = Row[1].ToString();
                textBox3.Text = Row[3].ToString();
                textBox4.Text = Row[4].ToString();
                textBox5.Text = Row[8].ToString();
                textBox6.Text = Row[9].ToString();
                comboBox1.SelectedIndex = comboBox1.Items.IndexOf(Row[5]);
                checkBox1.Checked = (bool)Row[10];
                if (Convert.ToInt32(Row[4]) == 0)
                {
                    grade = false;
                    textBox4.Text = Row[11].ToString();
                }
                else
                {
                    grade = true;
                }

                dateTimePicker1.Value = (DateTime)Row[6];

                Text = textBox3.Text + " " + textBox2.Text;

                if (((DateTime)Row[7]).Year != 3000)
                {
                    button1.Text = "Вернуть";
                    dismiss = true;
                    Text += "  Уволен";
                }

                
            }

            if (grade) button3.Text = "Разряд";
            else button3.Text = "Оклад";
            return ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //ImageConverter I = new ImageConverter();
            try
            {
                if (append)
                    Form1.workers.Append(
                        null,
                        textBox2.Text,
                        textBox1.Text,
                        textBox3.Text,
                        (grade) ? textBox4.Text : "0",
                        comboBox1.Text,
                        dateTimePicker1.Value.ToShortDateString(),
                        eDate,
                        textBox5.Text,
                        textBox6.Text,
                        checkBox1.Checked,
                        (!grade) ? textBox4.Text : "0");
                else
                    Form1.workers.Update(
                        ID,
                        textBox2.Text,
                        textBox1.Text,
                        textBox3.Text,
                        (grade) ? textBox4.Text : "0",
                        comboBox1.Text,
                        dateTimePicker1.Value.ToShortDateString(),
                        eDate,
                        textBox5.Text,
                        textBox6.Text,
                        checkBox1.Checked,
                        (!grade) ? textBox4.Text : "0");
                        //(imagechanged)? I.ConvertTo(pictureBox1.Image,typeof(byte[])) : null);
                        

                if (UpdateWorker != null) UpdateWorker();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            panel1.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!dismiss) panel1.Show();
            else
            {
                Form1.workers.DisUpdate(eDate, ID);
                Text = textBox3.Text + " " + textBox2.Text;
                dismiss = false;
                button1.Text = "Уволить";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form1.workers.DisUpdate(dateTimePicker2.Value.ToShortDateString(), ID);
            Text +=  "  Уволень";
            dismiss = true;
            button1.Text = "Вернуть";
            panel1.Hide();    
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == 2) textBox4.Text = "0";
            else 
                if (!append) textBox4.Text = Row[4].ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            grade = !grade;
            if (grade) button3.Text = "Разряд";
            else button3.Text = "Оклад";
        }


     
        private void button6_Click(object sender, EventArgs e)
        {
            FormPicture F = new FormPicture();
            if (F.ShowDialog() == DialogResult.OK)
            {
                ImageConverter I = new ImageConverter();
                try
                {
                    imagechanged = true;
                    string fileforsave = Form1.pathforImages + ID.ToString() + ".jpg";
                    pictureBox1.Image = F.Image.GetThumbnailImage(pictureBox1.Width, pictureBox1.Height, null, IntPtr.Zero);
                    if (Form1.images.ContainsKey(ID))
                    {
                        Form1.images[ID].Dispose();
                        Form1.images.Remove(ID);
                    }
                    Form1.images.Add(ID, pictureBox1.Image);
                    pictureBox1.Image.Save(fileforsave, System.Drawing.Imaging.ImageFormat.Jpeg);
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }


        private void FormAddWorker_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);                
            }
        }



    }
}