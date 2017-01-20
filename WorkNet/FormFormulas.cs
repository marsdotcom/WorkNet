using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormFormulas : Form
    {
        Expressions Exps = new Expressions() ;
        public string expstring;

        public FormFormulas()
        {
            InitializeComponent();
            button17.Tag = Token_ID.number;
        }

        private bool nonNumberEntered = false;

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (nonNumberEntered == true)
                e.Handled = true;
        }

        //InputLanguage ILang = new InputLanguage();

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            nonNumberEntered = true;

           // Text = e.KeyValue.ToString();

            if (e.Shift) return;

            if ((e.KeyData >= Keys.D0 && e.KeyData <= Keys.D9)
                || (e.KeyData >= Keys.NumPad0 && e.KeyData <= Keys.NumPad9)
                || e.KeyData == Keys.Back)
                nonNumberEntered = false;
            if (e.KeyData == Keys.Oemcomma)
                if (!textBox2.Text.Contains(",") && (textBox2.Text.Length > 0))
                    nonNumberEntered = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Token_ID Ti = (Token_ID)((Button)sender).Tag;
            Token T = new Token(Ti, 0);

            if (Exps.Count > 25)
            {
                MessageBox.Show("Слишком длинная формула");
                return;
            }
            
            if (Ti == Token_ID.number)
            {
                if (textBox2.Text.Length > 10)
                {
                    MessageBox.Show("Слишком длинное число");
                    return;
                }
                float f;
                if (!float.TryParse(textBox2.Text, out f))
                {
                    MessageBox.Show("Неправильное число");
                    return;
                }
                T.value = f;
            }

            if (!Exps.Append(T))
            {
                MessageBox.Show("Ощибка");
            }
            else
                textBox1.Text = Exps.ToString();
        }


        private void button18_Click(object sender, EventArgs e)
        {
            Exps.Remove();
            textBox1.Text = Exps.ToString();
        }

       

        private void Formulas_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < 16; i++)
            {
                ((Button)this.flowLayoutPanel1.Controls[i]).Text = Token.apparences[i+1];
                ((Button)this.flowLayoutPanel1.Controls[i]).Tag = (Token_ID)(i+1);
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (!Exps.Test())
            {
                MessageBox.Show("Не правильная формула");                
                return;                
            }

            float f = 0;
            float f2 = 0;

            if (Exps.Evalute(ref f,ref f2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1))
                textBox1.Text += f.ToString();
            else
            {
                MessageBox.Show("Error");
                return;
            }

            expstring = Exps.SaveToString();

        }

        private void Formulas_Shown(object sender, EventArgs e)
        {
            if (expstring != null)
            {
                Exps.LoadFromString(expstring);
                textBox1.Text = Exps.ToString();
            }
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            if (!Exps.Test())
            {
                MessageBox.Show("Не правильная формула");
                radioButton1.Checked = !radioButton1.Checked;
                return;
            }
            Exps.Switch(radioButton1.Checked);
            textBox1.Text = Exps.ToString();
        }

    }
}
