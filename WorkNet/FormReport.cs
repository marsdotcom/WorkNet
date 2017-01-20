using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WorkNet
{
    public partial class FormReport : Form
    {
        Excel excel;
        public object[,] obj_workers, obj;
        int n, j, i, templatebook;
        Single k;
        public int mindex;
        bool first = true;
        DataTable table, wTable;
        Tabel fTabel;
        OleDbDataAdapter adapter;

        public FormReport()
        {
            InitializeComponent();
        }

        void RunExcel()
        {
            if (first)
            {
                first = false;
                excel = new Excel();
                wTable = Form1.workers.Table;
                n = Form1.workers.Count;
                fTabel = Form1.tabel;
                obj_workers = new object[n, 6];
                for (i = 0; i < n; i++)
                {
                    obj_workers[i, 0] = i + 1;
                    obj_workers[i, 1] = wTable.Rows[i][1];
                    obj_workers[i, 2] = wTable.Rows[i][2];
                    obj_workers[i, 3] = wTable.Rows[i][3];
                    obj_workers[i, 4] = wTable.Rows[i][4];
                }

                if (Groups.expchanged)
                    Groups.LoadExp();
            }
            else
            {
                excel.Hide();
            }
            excel.AddBook(Application.StartupPath + "/" + textBox1.Text);
            templatebook = excel.CurrentBook;            
        }


        private void button3_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("Файл не найден");                
                return;
            }
            RunExcel();            
            string text = Convert.ToString(excel.ImportObj("B5"));
            excel.AddBook();
            excel.CopySheet(templatebook, 1, excel.CurrentBook, 1);
            excel.CloseBook(templatebook);
            excel.DeleteSheet(1);
            excel.ExportObj("L2", "За " + Form1.monthnames[Form1.month-1] + " месяц 2008 г.");
            excel.UndelineCharacters("L2", 0, Form1.monthnames[Form1.month-1].Length + 14);
           
            excel.ExportArray("A5", obj_workers, n, 5);
            
            obj = new object[n, 32];
            for (i = 0; i < n; i++)
                for (j = 0; j < 32; j++)
                {
                    if (j < 15)
                        obj[i, j] = fTabel.marks[i, j + Form1.period.extraday];
                    else if (j > 15)
                        obj[i, j] = fTabel.marks[i, j - 1 + Form1.period.extraday];
                }
            excel.ExportArray("F5", obj, n, 32);

            for (i = 0; i < n; i++)                
            {
                fTabel.Calculate(i);
                obj[i, 0] = fTabel.D;
                obj[i, 1] = obj[i, 2] =  obj[i, 3] = null;
                obj[i, 4] = fTabel.B;
                if (fTabel.B == 0) obj[i, 4] = null;
                obj[i, 5] = null;
                obj[i, 6] = fTabel.A;
                if (fTabel.A == 0) obj[i, 6] = null;
                obj[i, 7] = fTabel.P;
                if (fTabel.P == 0) obj[i, 7] = null;
                obj[i, 8] = fTabel.V;
                if (fTabel.V == 0) obj[i, 8] = null;
                obj[i, 9] = obj[i, 10] = null;
                obj[i, 11] = fTabel.AD;             
            }

            excel.ExportArray("AL5", obj, n, 12);

            i = 4 + n;
            excel.BorderRange("A5:AY" + i.ToString());
            i += 2;
            excel.MergeCell("B" + i.ToString(), "U" + i.ToString());
            excel.ExportObj("B"+i.ToString(), text);
            excel.BoldRange(true);

            excel.Show();
        }

        private void FormReport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (excel != null)
                excel.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!File.Exists(textBox1.Text))
            {
                MessageBox.Show("Файл шаблона не найден");
                return;
            }
            RunExcel();            
            excel.AddBook();
            excel.CopySheet(templatebook, 2, excel.CurrentBook, 1);
            excel.CloseBook(templatebook);
            excel.DeleteSheet(1);
            string text = Convert.ToString(excel.ImportObj("B10"));

            table = new DataTable();
            adapter = new OleDbDataAdapter();
            adapter.SelectCommand = DB.GenSelectCom("Grade");
            adapter.FillSchema(table,SchemaType.Source);
            adapter.Fill(table);

            excel.ExportArray("A10", obj_workers, n, 2);

            obj = new object[n, 5];
            object[,] obj2 = new object[n, 3];

            DataRow row;
            Calendar calendar = new Calendar(DB.connection);
            calendar.Load(Form1.year,Form1.month);

            float f1 = 0;
            float f2 = 0;

            for (i = 0; i < n; i++)
            {
                fTabel.Calculate(i);
                row = table.Rows.Find(obj_workers[i, 4]);
                obj[i, 0] = fTabel.D;       // отработанные дни
                obj[i, 1] = fTabel.H;       // отработанные часы      
                obj[i, 2] = row[3];         // разряд/оклад
                //obj[i, 3] = ?;            // зарплата по тарифу
                obj[i, 4] = null;           // кту

                //obj2[i, 0] = ?;           // за вредность
                //obj2[i, 1] = ?;           // за выходные дни 
                //obj2[i, 2] = ?;           // за совмещение 
                #region
                /*
                if (wTable.Rows[i][5].ToString() == "Рабочие")
                {
                    obj[i, 3] = fTabel.H * Convert.ToSingle(row[2]);   // часы * часовую ставку
                    k = fTabel.SD * Convert.ToSingle(row[2]) * 8;      // дни за вых. * часовую ставку * 8
                    if (k > 0) obj2[i, 1] = k;                        
                    obj[i, 4] = 1;                                    
                }
                else if (wTable.Rows[i][5].ToString() == "ИТР")
                {
                    obj[i, 3] = fTabel.H * Convert.ToSingle(row[3]) / calendar.workhours;   
                    k = fTabel.SD * Convert.ToSingle(row[3]) / calendar.workhours * 8;
                    if (k > 0) obj2[i, 1] = k;
                }
                else
                {
                    obj[i, 3] = wTable.Rows[i][11];                    
                }
                */
                #endregion

                /*
                     "часы по календарю",
                     "дни оп календарю",
                     "часы по табелю",
                      "дни по табелю",
                     "часы за выходние",
                     "дни за выходние",
                     "тариф",
                     "часовая ставка",
                     "месячная ставка",      
                     "оклад",           */


                Groups.ListExp[wTable.Rows[i][5].ToString()].Evalute(
                    ref f1, ref f2,
                    calendar.workhours,
                    calendar.workdays,
                    fTabel.H,
                    fTabel.D,
                    fTabel.SH,
                    fTabel.SD,
                    (float)row[1],
                    (float)row[2],
                    (float)row[3],
                    Convert.ToSingle(wTable.Rows[i][11]));

                if ((bool)wTable.Rows[i][13])
                    obj[i, 4] = 1;

                obj[i, 3] = f1;
                if (f2 > 0)
                    obj2[i, 1] = f2;

                if (Convert.ToInt32(wTable.Rows[i][8]) > 0)
                    obj2[i, 0] = Convert.ToInt32(wTable.Rows[i][8]) *
                                Convert.ToInt32(obj[i, 3]) / 100;
                if (Convert.ToInt32(wTable.Rows[i][9]) > 0)
                    obj2[i, 2] = Convert.ToInt32(wTable.Rows[i][9]) *
                                Convert.ToInt32(obj[i, 3]) / 100;
            }

            excel.ExportArray("C10", obj, n, 5);
            excel.ExportArray("J10", obj2, n, 3);
            //excel.Show();

            j = n + 9;            

            for (i = 0; i < n; i++)
                if ((bool)wTable.Rows[i][10])
                {
                    j++;
                    excel.ExportObj("B" + j.ToString(), obj_workers[i, 1]);
                    excel.ExportObj("C" + j.ToString(), "несов");
                    fTabel.Calculate(i);
                    row = table.Rows.Find(obj_workers[i, 4]);
                    excel.ExportObj("D" + j.ToString(), fTabel.D);
                    excel.ExportObj("E" + j.ToString(), row[3]);
                    excel.ExportObj("F" + j.ToString(), fTabel.D * Convert.ToInt32(row[2]));
                }

            excel.AutoFill("H10", "H" + j.ToString());
            excel.AutoFill("M10", "M" + j.ToString());
            j++;
            excel.FormulaSum("F" + j.ToString(), j-10);
            excel.FormulaSum("H" + j.ToString(), j-10);
            excel.FormulaSum("J" + j.ToString(), j-10);
            excel.FormulaSum("K" + j.ToString(), j-10);
            excel.FormulaSum("L" + j.ToString(), j-10);
            for (i = 10; i < n + 10; i++)
            {
                excel.Formula("I" + i.ToString(), string.Format("=H{0}*I{1}", i,j+1));
            }
            excel.Formula("I"+j.ToString(),string.Format("=M{0}-F{0}-J{0}-K{0}-L{0}",j));

            excel.ExportObj("I5", "ВСЕГО    " + textBox2.Text.PadRight(9, ' ') + "  руб.");
            excel.UndelineCharacters("I5", 6, 15);

            string text2;
            text2 = excel.ImportObj("A1") + "  ";
            excel.ExportObj("A1", text2 + Form1.monthnames[Form1.month - 1] + "  "+Form1.year.ToString()+" г.");
            excel.UndelineCharacters("A1", text2.Length, Form1.monthnames[Form1.month - 1].Length + 2);

            text2 = excel.ImportObj("A3") + "   ";
            int m = Convert.ToInt32(excel.ImportObj("F" + j.ToString()));
            excel.ExportObj("A3", text2 + m.ToString().PadRight(9, ' ') + "  руб.");
            excel.UndelineCharacters("A3", text2.Length, 15);

            excel.ExportObj("M" + j.ToString(), textBox2.Text);
            excel.ExportObj("B" + j.ToString(), "Итого");
            excel.BoldRange(true);
            excel.BorderRange("A5:M" + j.ToString());
            
            excel.NumberFormat("H10:M" + j.ToString(), "0");
            excel.NumberFormat("F10:F"+j.ToString(),"0");
            j++;
            excel.MergeCell("F" + j.ToString(), "H" + j.ToString());
            excel.ExportObj("F" + j.ToString(), "К Доп = ");
            excel.Formula("I"+j.ToString(),string.Format("=I{0}/H{0}",j-1));
            excel.NumberFormat("I" + j.ToString(), "0"+Keyboard.NumberSeparator+"000");

            j+=2;
            excel.ExportObj("B" + j.ToString(), text);
            excel.BoldRange(true);

            excel.Show();
        }
    }

}