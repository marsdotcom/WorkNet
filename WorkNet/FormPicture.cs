using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormPicture : Form
    {
        Rectangle rect = new Rectangle();
        Pen pen = new Pen(Color.Green,2);
        bool hand;
        float factor = 1;
        bool down;
        int xdown, ydown;
        float K;
        int X, Y;

        public Image Image;

        public FormPicture()
        {
            pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            InitializeComponent();
            openFileDialog1.Filter = "(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG";
            this.MouseWheel += new MouseEventHandler(FormPicture_MouseWheel);
        }



        void FormPicture_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                factor -= 0.05f; if (factor < 0.05) factor = 0.05f;
                ResizeRect();
            }
            else
            {
                factor += 0.05f; if (factor > 1) factor = 1;
                ResizeRect();
                if (!IfRectInside())
                {
                    factor -=0.05f;
                    ResizeRect();
                    return;
                }
            }                        

            pictureBox1.Invalidate(false);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == openFileDialog1.ShowDialog())
            {
                pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            }
        }

       

        private void pictureBox1_Resize(object sender, EventArgs e)
        {
            EvaluteFactors();
            ResizeRect();
            CenterRect();
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (hand = rect.Contains(new Point(e.X, e.Y)))
                pictureBox1.Cursor = Cursors.Hand;
            else
                pictureBox1.Cursor = Cursors.Default;

            if (down)
            {
                int x = rect.X;
                int y = rect.Y;
                rect.X = e.X - xdown;
                rect.Y = e.Y - ydown;                

                if (IfRectInside())
                {
                    pictureBox1.Invalidate(false);                    
                }
                else
                {
                    rect.X = x;
                    rect.Y = y;
                    xdown = e.X - rect.X;
                    ydown = e.Y - rect.Y;
                }
            }

        }

        void CenterRect()
        {
            rect.X = (pictureBox1.Width - rect.Width) / 2;
            rect.Y = (pictureBox1.Height - rect.Height) / 2;
        }

        void ResizeRect()
        {
            int cx = rect.X + rect.Width / 2;
            int cy = rect.Y + rect.Height / 2;
            if (X == 0)
            {
                rect.Height = (int)((pictureBox1.Height - Y * 2) * factor);
                rect.Width = (int)(rect.Height * 3 / 4);
            }
            else
            {
                rect.Width = (int)((pictureBox1.Width - X * 2) * factor);
                rect.Height = (int)(rect.Width * 4 / 3);
            }
            rect.X = cx - rect.Width / 2;
            rect.Y = cy - rect.Height / 2;
        }

        bool IfRectInside()
        {
            return (rect.X > X && (rect.X + rect.Width) < (pictureBox1.Width - X) &&
                    (rect.Y > Y && (rect.Y + rect.Height) < (pictureBox1.Height - Y)));
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(pen, rect);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (hand)
            {
                down = true;
                xdown = e.X - rect.X;
                ydown = e.Y - rect.Y;
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;            
        }

        private void FormPicture_Shown(object sender, EventArgs e)
        {
            EvaluteFactors();
            ResizeRect();
            CenterRect();
            pictureBox1.Invalidate(false);            
        }        

        void EvaluteFactors()
        {
            Image I = pictureBox1.Image;
            float bk = (float)I.Width / (float)I.Height;
            float pk = (float)pictureBox1.Width / (float)pictureBox1.Height;
            if (bk > pk)
            {
                K = (float)I.Width / (float)pictureBox1.Width; // ширина совпала!!!
                X = 0;
                Y = (pictureBox1.Height - (int)(pictureBox1.Width / bk)) / 2;
            }
            else
            {
                K = (float)I.Height / (float)pictureBox1.Height;
                Y = 0;
                X = (pictureBox1.Width - (int)(pictureBox1.Height * bk)) / 2;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Image = pictureBox1.Image;
        }

        Stack<Image> stack = new Stack<Image>();
        int depth = 0;

        private void button3_Click(object sender, EventArgs e)
        {
            EvaluteFactors();
            Bitmap B = (Bitmap)pictureBox1.Image;
            Rectangle clonRect =
                Rectangle.Round((new RectangleF(rect.X * K - X * K, rect.Y * K - Y * K, rect.Width * K, rect.Height * K)));
            B = B.Clone(clonRect, B.PixelFormat);
            stack.Push(pictureBox1.Image);
            pictureBox1.Image = (Image)B;
            factor = 1;
            this.FormPicture_Shown(null, null);
            depth++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (depth>0)
            {
                depth--;
                pictureBox1.Image = stack.Pop();
                factor = 1;
                this.FormPicture_Shown(null, null); 
            }
        }

        private void FormPicture_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

        }

        private void FormPicture_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] file = (string[])e.Data.GetData(DataFormats.FileDrop);
                pictureBox1.Image = Image.FromFile(file[0]);
            }
        }

    }
}
