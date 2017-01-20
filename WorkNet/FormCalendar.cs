using System;
using System.Drawing;
using System.Windows.Forms;

namespace WorkNet
{
    public partial class FormCalendar : Form
    {
        Calendar calendar = new Calendar(DB.connection);

        public FormCalendar()
        {
            InitializeComponent();
            for (int i = 0; i < 6; i++)
                dataGridView1.Rows.Add();
        }

        private void FormCalendar_Shown(object sender, EventArgs e)
        {
            Text = "Календарь  " + Form1.monthnames[Form1.month - 1];
            calendar.Load(Form1.year, Form1.month);
            DateTime T = new DateTime(Form1.year, Form1.month, 1);
            int d = (int)T.DayOfWeek;
            int DayCount = calendar.lastDay;
            d = (d == 0) ? 7 : d;
            d--;
            int k = 0,i,j;
            
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 7; j++)
                {
                    if (k >= DayCount) break;
                    if ((i > 0) || (j >= d))
                    {
                        dataGridView1.Rows[i].Cells[j].Value = ++k;
                    }
                    else
                        dataGridView1.Rows[i].Cells[j].Value = null;
                }
            }

            EvaluteDays();            
        }

        void EvaluteDays()
        {
            labelworkhour.Text = calendar.workhours.ToString();
            labelworkdays.Text = calendar.workdays.ToString();
            labelholiday.Text = calendar.holidays.ToString();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value != null)
                switch (calendar.days[(int)e.Value-1])
                {
                    case 1: e.CellStyle.ForeColor = Color.Black; break;
                    case 2: e.CellStyle.ForeColor = Color.Red; break;
                    case 3: e.CellStyle.ForeColor = Color.Blue; break;
                }
        }

        int x, y;
        
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {            
            dataGridView1.ContextMenuStrip.Show(this, new Point(x, y));
        }

        private void dataGridView1_MouseMove(object sender, MouseEventArgs e)
        {
            x = e.X;
            y = e.Y;
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells[0].Value == null) return;
            switch (((ToolStripMenuItem)sender).Text)
            {
                case "Выходной": calendar.days[(int)dataGridView1.SelectedCells[0].Value - 1] = 2; break;
                case "Сокращенный": calendar.days[(int)dataGridView1.SelectedCells[0].Value - 1] = 3; break;
                case "Рабочий": calendar.days[(int)dataGridView1.SelectedCells[0].Value - 1] = 1; break;
            }
            dataGridView1.Invalidate();
            calendar.EvaluteDays();
            EvaluteDays();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            calendar.Save();
        }
    }
}