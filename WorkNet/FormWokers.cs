using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace WorkNet
{
    public partial class FormWokers : Form
    {
        FormAddWorker formAddWorker = new FormAddWorker();

        int tabelnum = 0;

        public static DataSet dataset = new DataSet();
        DataTable T;
        OleDbDataAdapter[] adapters = new OleDbDataAdapter[3];
        string[] ntables = {"Workers","Groups","Grade"};
        string[] groups;

        public FormWokers()
        {
            InitializeComponent();
       
            for (int i = 0; i < ntables.Length; i++)
            {
                adapters[i] = new OleDbDataAdapter();
                adapters[i].SelectCommand = DB.GenSelectCom(ntables[i]);
                adapters[i].FillSchema(dataset, SchemaType.Source, ntables[i]);
                if (i > 0)
                {
                    adapters[i].InsertCommand = DB.GenInsertCom(dataset.Tables[i]);
                    adapters[i].UpdateCommand = DB.GenUpdateCom(dataset.Tables[i]);
                    adapters[i].DeleteCommand = DB.GenDeleteCom(dataset.Tables[i]);
                }
                adapters[i].Fill(dataset.Tables[i]);
            }

            groups = new string[dataset.Tables[1].Rows.Count];
            for (int i = 0; i < groups.Length; i++)
                groups[i] = dataset.Tables[1].Rows[i][0].ToString();

            formAddWorker.groups = groups;
            formAddWorker.T = dataset.Tables[0];

            formAddWorker.UpdateWorker +=new delAppend(UpdateWorker);
        }

        public void UpdateWorker()
        {
            dataset.Tables[0].Rows.Clear();
            adapters[0].Fill(dataset, ntables[0]);
        }

        private void tbWokers_Click(object sender, EventArgs e)
        {
            int i = Convert.ToInt32(((ToolStripButton)sender).Tag);
            if (i == tabelnum) return;
            tabelnum = i;
            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = dataset.Tables[tabelnum-1];
            switch (tabelnum)
            {
                case 1:
                    dataGridView1.Columns[0].Visible = false;
                    dataGridView1.Columns[1].HeaderText = "ФИО";
                    dataGridView1.Columns[2].HeaderText = "Таб N";                    
                    dataGridView1.Columns[3].HeaderText = "Должность";
                    dataGridView1.Columns[4].HeaderText = "Разряд";
                    dataGridView1.Columns[5].HeaderText = "Группа";
                    dataGridView1.Columns[6].HeaderText = "Первый день";
                    dataGridView1.Columns[7].HeaderText = "Последный день";
                    dataGridView1.Columns[8].HeaderText = "За вредность";
                    dataGridView1.Columns[9].HeaderText = "За совмещение";
                    dataGridView1.Columns[10].HeaderText = "Несо-летный";break;
                case 2:
                    dataGridView1.Columns[0].HeaderText = "Группа";
                    dataGridView1.Columns[1].HeaderText = "День выдачи зарплаты";
                    dataGridView1.Columns[2].HeaderText = "Кту";
                    dataGridView1.Columns[3].HeaderText = "Способ рас. зарплаты"; break; 
                case 3:
                    dataGridView1.Columns[0].HeaderText = "Разряд";
                    dataGridView1.Columns[1].HeaderText = "Тарифный план";
                    dataGridView1.Columns[2].HeaderText = "Часовые тарифные ставки";
                    dataGridView1.Columns[3].HeaderText = "Месячные тарифные ставки"; break;
            }
            for (i = 0; i < dataGridView1.Columns.Count; i++)
                dataGridView1.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;        
        }

        private void FormWokers_Load(object sender, EventArgs e)
        {            
            tbWokers_Click(tbWokers, null);
        }

        private void tbAppend_Click(object sender, EventArgs e)
        {
            switch (tabelnum)
            {
                case 1: formAddWorker.ShowDialog(0); break;
                case 2: if ((new FormAddGroup()).ShowDialog() == DialogResult.OK)
                        adapters[1].Update(dataset.Tables[1]); break;
                case 3: if ((new FormUpdateGrade()).ShowDialog() == DialogResult.OK)
                        adapters[2].Update(dataset.Tables[2]); break;
            }
        }

        private void tbModify_Click(object sender, EventArgs e)
        {
            switch (tabelnum)
            {
                case 1: formAddWorker.ShowDialog((int)dataGridView1.SelectedRows[0].Cells[0].Value); break;
                case 2: if ((new FormAddGroup(dataGridView1.SelectedRows[0].Cells[0].Value)).ShowDialog() == DialogResult.OK)
                        adapters[1].Update(dataset.Tables[1]); break;
                case 3: if ((new FormUpdateGrade(dataGridView1.SelectedRows[0].Cells[0].Value)).ShowDialog() == DialogResult.OK)
                        adapters[2].Update(dataset.Tables[2]); break;
            }
        }

        private void tbDelete_Click(object sender, EventArgs e)
        {
            if (
            MessageBox.Show(
                "Вы уверены что хотите удалить эту запись",
                "Предупреждение", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                try
                {
                    if (tabelnum > 1)
                    {
                        dataset.Tables[tabelnum - 1].Rows.RemoveAt(dataGridView1.SelectedRows[0].Index);
                        adapters[tabelnum - 1].Update(dataset.Tables[tabelnum]);
                    }
                    else
                    {
                        Form1.workers.Delete((int)dataGridView1.SelectedRows[0].Cells[0].Value);
                        UpdateWorker();
                    }
                }
                catch (Exception ee)
                {
                    MessageBox.Show(ee.Message);
                }
            }
        }

        private void FormWokers_FormClosing(object sender, FormClosingEventArgs e)
        {
            dataset.Clear();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            this.tbModify_Click(null, null);
        }
    }
}