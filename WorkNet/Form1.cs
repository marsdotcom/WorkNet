using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.IO;

namespace WorkNet
{    
    public partial class Form1 : Form
    {
        public static int month, year;
        public static Period period;
        public static Tabel tabel;
        public static Workers workers;
        public static string[] monthnames = {
                                            "Январь",
                                            "Февраль",
                                            "Март",
                                            "Апрель",
                                            "Май",
                                            "Июнь",
                                            "Июль",
                                            "Август",
                                            "Сентябрь",
                                            "Октябрь",
                                            "Ноябрь",
                                            "Декабрь"
                                            };
        public int min_salaryday,extraday;

        public static Dictionary<int, Image> images = new Dictionary<int, Image>();
        public static string pathforImages;

        bool changed = false, userchange = false;

        DateTime ToDay;

        FormPeriod formPeriod = new FormPeriod();
        FormAddWorker formAddWorker = new FormAddWorker();

        const int colcount = 5;

        public Form1()
        {            
            InitializeComponent();
           
            dataGridView1.AutoGenerateColumns = false;
            
        }


        private void Form1_Load(object sender, EventArgs e)
        {

            if (DB.Connect(Application.StartupPath + "\\dbTabel.mdb"))
            {
                Groups.Load();
                min_salaryday = Groups.min_salaryday;
                extraday = Groups.extraday;
                formAddWorker.groups = Groups.groups;                

                DataGridViewColumn Col;
                int j;
                for (j = 2; j < 32; j++)
                {
                    Col = (DataGridViewColumn)dataGridView1.Columns[colcount].Clone();
                    Col.HeaderText = j.ToString();
                    dataGridView1.Columns.Insert(colcount - 1 + j, Col);
                }

                if (extraday > 0)
                    for (j = min_salaryday; j < 32; j++)
                    {
                        Col = (DataGridViewColumn)dataGridView1.Columns[colcount].Clone();
                        Col.HeaderText = j.ToString();
                        Col.ReadOnly = true;
                        Col.Visible = false;
                        Col.DefaultCellStyle.BackColor = Color.DarkSeaGreen;
                        dataGridView1.Columns.Insert(colcount + j - min_salaryday, Col);
                    }

                ToDay = DateTime.Today;
                month = ToDay.Month;
                year = ToDay.Year;
                Text = "Табель  " + monthnames[month - 1] + "  " + year.ToString();

                workers = new Workers();
                formAddWorker.T = workers.Table;
                workers.AppendWoker += new delAppend(ReloadData);
                workers.UpdateWoker += new delAppend(UpdateWorker);

                period = new Period();
                tabel = new Tabel(DB.connection);
                tabel.workers = workers;

                workers.period = period;
                tabel.period = period;
                LoadCalendar();
                LoadWokers();
                LoadTabel();
            }
            else
            {
                //MessageBox.Show("База \"dbTabel.mdb\"  не найдена !");
                Application.Exit();
            }

            pathforImages = Application.StartupPath + "\\images\\";

            if (!Directory.Exists(pathforImages))
                Directory.CreateDirectory(pathforImages);

            string[] files = Directory.GetFiles(pathforImages, "*.jpg");

            foreach (string s in files)
            {
                int i;
                int.TryParse(Path.GetFileNameWithoutExtension(s),out i);
                if (i>0)
                    images.Add(i, Image.FromFile(s));
            }

            GC.Collect();

        }

        void ReloadData()
        {
            if (changed)
                if (MessageBox.Show(
                    "Сохранить изменения в табеле?",
                    "Предупреждение!",
                    MessageBoxButtons.YesNo) == DialogResult.Yes) SaveTabel();
                else
                {
                    changed = false;
                    tbPost.Enabled = false;
                    changes.Clear();
                }
            LoadWokers();
            LoadTabel();
        }

        void UpdateWorker()
        {
            userchange = false;
            int j = 0, col = 5;
            for (int i = 0; i < workers.Count; i++)
            {
                for (j = 0; j < col; j++)
                    dataGridView1.Rows[i].Cells[j].Value = workers.Table.Rows[i][j + 1];
            }
            userchange = true;
        }

        void LoadWokers()
        {
            dataGridView1.Rows.Clear();
            workers.Load();
            userchange = false;
            int j = 0, col = 5, i;
            for (i = 0; i < workers.Count; i++)
            {
                dataGridView1.Rows.Add();
                for (j = 0; j < col; j++)
                    dataGridView1.Rows[i].Cells[j].Value = workers.Table.Rows[i][j + 1];
            }
            userchange = true;         
        }

        void LoadTabel()
        {
            tabel.Load();
            userchange = false;
            int j,i;

            if (!tabel.newTabel)
            {
                for (i = 0; i < tabel.countworker; i++)
                    for (j = 0; j < period.Length; j++)
                        dataGridView1.Rows[i].Cells[j + colcount].Value = tabel.marks[i, j];
            }
            else
            {
                for (i = 0; i < tabel.countworker; i++)
                    for (j = 0; j < period.Length; j++)
                        dataGridView1.Rows[i].Cells[j + colcount].Value = "";
            }

            userchange = true;
        }

        void LoadCalendar()
        {
            period.Load(year, month, min_salaryday);

            int i = 0;           

            if (period.lastDay > 0)
            {
                for (i = 0; i < period.days.Length; i++)
                {
                    dataGridView1.Columns[i + colcount + extraday].Visible = true;
                    switch (period[i])
                    {
                        case 0: dataGridView1.Columns[i + colcount].HeaderCell.Style.BackColor = Color.Black; 
                            break;
                        case 1: dataGridView1.Columns[i + colcount].HeaderCell.Style.BackColor = Color.LightGray;
                            dataGridView1.Columns[i + colcount].DefaultCellStyle.ForeColor = Color.Black;
                            break;
                        case 2: dataGridView1.Columns[i + colcount].HeaderCell.Style.BackColor = Color.Red;
                            dataGridView1.Columns[i + colcount].DefaultCellStyle.ForeColor = Color.Red;
                            break;
                        case 3: dataGridView1.Columns[i + colcount].HeaderCell.Style.BackColor = Color.Blue;
                            dataGridView1.Columns[i + colcount].DefaultCellStyle.ForeColor = Color.Blue;
                            break;
                    }

                    dataGridView1.Columns[i + colcount].Visible = (period[i] > 0);
                }

                if ((year == ToDay.Year) && (month == ToDay.Month))
                    dataGridView1.Columns[colcount - 1 + ToDay.Day + extraday].DefaultCellStyle.BackColor = Color.Bisque;
            }

            previsible = true;
            VisiblePreDayCol();
            visible = true;
            VisibleDayCol();            
        }

        private void tsCalendar_Click(object sender, EventArgs e)
        {
            if (month == 0)  tbPeriod_Click(null, null);
            if ((new FormCalendar()).ShowDialog() == DialogResult.OK) LoadCalendar();
        }

        private void tbPeriod_Click(object sender, EventArgs e)
        {
            int im,iy;
            if (formPeriod.ShowDialog(out im, out iy) == DialogResult.OK)
            {
                month = im;
                year = iy;
                Text = "Табель  " + monthnames[month - 1] + "  " + year.ToString();
                LoadCalendar();
                ReloadData();
            }                
        }

        private void tbPost_Click(object sender, EventArgs e)
        {
            SaveTabel();
        }

        public void SaveTabel()
        {
            if (!changed) return;
            if (period.lastDay < 1) return;

            foreach (Point point in changes)
            {
                if (dataGridView1.Rows[point.X].Cells[point.Y].Value != null)
                    tabel.Save(point.X, point.Y - colcount, dataGridView1.Rows[point.X].Cells[point.Y].Value.ToString());
                else
                    tabel.Save(point.X, point.Y - colcount, "");
            }

            changes.Clear();
            changed = false;
            tbPost.Enabled = false;
        }       

        private void tsReference_Click(object sender, EventArgs e)
        {
            (new FormWokers()).Show();
        }

        string str;
        const string validmarks = "А Б П В Р М _ Х ПЕ УВ ОТ 1 2 3 4 5 6 7 8 9 0 10 11 12 13 14 15 16 17 18 19 20 21 22 23 24";
        Queue<Point> changes = new Queue<Point>();

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (userchange)
            {
                if (!changed)
                {
                    changed = true;
                    tbPost.Enabled = true;
                }
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null) str = "";
                else
                {
                    str = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = str.ToUpper().Trim();
                }
                if (!validmarks.Contains(str))
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "";
                changes.Enqueue(new Point(e.RowIndex, e.ColumnIndex));
            }
        }

        bool visible = true;

        void VisibleDayCol()
        {
            int i = 0;
            
            for (i = extraday; i < period.lastDay + extraday; i++)
                dataGridView1.Columns[i + colcount].Visible = visible;
            for (i = 36; i < 45; i++)
                dataGridView1.Columns[i+extraday].Visible = !visible;
            if (visible = !visible) Calculate();
        }


        public void Calculate()
        {
            userchange = false;

            for (int i = 0; i < workers.Count; i++)
            {
                tabel.Calculate(i);

                dataGridView1.Rows[i].Cells[0 + 31 + colcount + extraday].Value = tabel.D;
                dataGridView1.Rows[i].Cells[1 + 31 + colcount + extraday].Value = tabel.H;
                dataGridView1.Rows[i].Cells[2 + 31 + colcount + extraday].Value = tabel.V;
                dataGridView1.Rows[i].Cells[3 + 31 + colcount + extraday].Value = tabel.P;
                dataGridView1.Rows[i].Cells[4 + 31 + colcount + extraday].Value = tabel.B;
                dataGridView1.Rows[i].Cells[5 + 31 + colcount + extraday].Value = tabel.A;
                dataGridView1.Rows[i].Cells[6 + 31 + colcount + extraday].Value = tabel.AD;
                dataGridView1.Rows[i].Cells[7 + 31 + colcount + extraday].Value = tabel.SD;
                dataGridView1.Rows[i].Cells[8 + 31 + colcount + extraday].Value = tabel.SH;
            }

            userchange = true;
        }

        bool previsible = true;

        void VisiblePreDayCol()
        {
            if (extraday > 0)
            {
                previsible = !previsible;
                for (int i = 0; i <= period.pre_lastDay - min_salaryday; i++)
                    dataGridView1.Columns[i + colcount].Visible = previsible;
            }
        }

        int x, y;

        private void tbAutoFillRow_Click(object sender, EventArgs e)
        {
            x = dataGridView1.SelectedCells[0].ColumnIndex;
            y = dataGridView1.SelectedCells[0].RowIndex;
            for (int i = x; i < period.lastDay + colcount +extraday; i++)
            {
                if ((dataGridView1.Rows[y].Cells[i].Value == null) ||
                    (dataGridView1.Rows[y].Cells[i].Value.ToString() == ""))
                {
                    if (period[i - 5] == 1)
                    {
                        if ((bool)workers.Table.Rows[y][10]) W = "7";
                        else W = "8";                        
                    }
                    else if (period[i - colcount] == 3) W = "7";
                    else if (period[i - colcount] == 2) W = "В";
                    dataGridView1.Rows[y].Cells[i].Value = W;
                }
            }
        }

        string W;

        private void tbAutoFillCol_Click(object sender, EventArgs e)
        {
            x = dataGridView1.SelectedCells[0].ColumnIndex;
            y = dataGridView1.SelectedCells[0].RowIndex;
            for (int i = y; i < workers.Count; i++)
            {
                if ((dataGridView1.Rows[i].Cells[x].Value == null) ||
                    (dataGridView1.Rows[i].Cells[x].Value.ToString() == ""))
                {
                    if (period[x - colcount] == 1)
                    {
                        if ((bool)workers.Table.Rows[i][10]) W = "7";
                        else W = "8";
                    }
                    else if (period[x - colcount] == 3) W = "7";
                    else if (period[x - colcount] == 2) W = "В";
                    dataGridView1.Rows[i].Cells[x].Value = W;
                }
            }
        }

        private void tbClearRow_Click(object sender, EventArgs e)
        {
            x = dataGridView1.SelectedCells[0].ColumnIndex;
            y = dataGridView1.SelectedCells[0].RowIndex;
            for (int i = x; i < period.lastDay + colcount + extraday; i++)
            {
                dataGridView1.Rows[y].Cells[i].Value = "";
            }
        }

        private void tbClearCol_Click(object sender, EventArgs e)
        {
            x = dataGridView1.SelectedCells[0].ColumnIndex;
            y = dataGridView1.SelectedCells[0].RowIndex;
            for (int i = y; i < workers.Count; i++)
            {
                dataGridView1.Rows[i].Cells[x].Value = "";
            }
        }        

        private void tbResume_Click(object sender, EventArgs e)
        {
            VisibleDayCol();
            if (previsible) VisiblePreDayCol();
        }

        private void tbPreDayVisible_Click(object sender, EventArgs e)
        {
            VisiblePreDayCol();
        }

        private void tbExcel_Click(object sender, EventArgs e)
        {
            if (changed)
                if (MessageBox.Show(
                    "Сохранить изменения в табеле?",
                    "Предупреждение!",
                    MessageBoxButtons.YesNo) == DialogResult.Yes) SaveTabel();
            FormReport Form = new FormReport();
            Form.ShowDialog();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (changed)
                if (MessageBox.Show(
                    "Сохранить изменения в табеле?",
                    "Предупреждение!",
                    MessageBoxButtons.YesNo) == DialogResult.Yes) SaveTabel();
                else
                {
                    changed = false;
                    tbPost.Enabled = false;
                }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                formAddWorker.ShowDialog((int)workers.Table.Rows[e.RowIndex][0]);
            }
        }

        public static void GetImagebyID(int ID,ref PictureBox pBox)
        {
            if (images.ContainsKey(ID))
            {
                pBox.Image = images[ID];
            }
            else
                pBox.Image = Properties.Resources.HsTZSDw4avx;
        }

        int indexPrior = 0;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (((e.ColumnIndex == 0) || (e.ColumnIndex > 5)) && (e.RowIndex >=0))
            {                
                dataGridView1[0, indexPrior].Style.ForeColor = dataGridView1.Columns[0].DefaultCellStyle.ForeColor;
                dataGridView1[0, indexPrior].Style.BackColor = dataGridView1.Columns[0].DefaultCellStyle.BackColor;
                indexPrior = e.RowIndex;
                dataGridView1[0, indexPrior].Style.ForeColor = dataGridView1.DefaultCellStyle.SelectionForeColor;
                dataGridView1[0, indexPrior].Style.BackColor = dataGridView1.DefaultCellStyle.SelectionBackColor;
               
                pictureBox1.Top = dataGridView1.GetCellDisplayRectangle(1, e.RowIndex, true).Bottom + 2;
                if (pictureBox1.Bottom > dataGridView1.Bottom)
                    pictureBox1.Top = dataGridView1.GetCellDisplayRectangle(1, e.RowIndex, true).Top - 2 - pictureBox1.Height;

                GetImagebyID((int)workers.Table.Rows[e.RowIndex][0],ref pictureBox1);
                pictureBox1.Show();               
            }
            else if (pictureBox1.Visible) { 
                pictureBox1.Hide();
                dataGridView1[0, indexPrior].Style.ForeColor = dataGridView1.Columns[0].DefaultCellStyle.ForeColor;
                dataGridView1[0, indexPrior].Style.BackColor = dataGridView1.Columns[0].DefaultCellStyle.BackColor;
            }

        }

        private void dataGridView1_Scroll(object sender, ScrollEventArgs e)
        {
            pictureBox1.Hide();
        }


    }     
}