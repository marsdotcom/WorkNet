using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;

namespace WorkNet
{
    public class Tabel
    {
        OleDbConnection C;
        OleDbCommand SelectCom;
        OleDbCommand PreSelectCom;
        OleDbCommand ModifyCom;
        OleDbCommand InsertCom;

        public Period period;
        public Workers workers;
        public int countworker;
        public string[,] marks;
        public int[] ids;
        public bool newTabel = false;

        public Tabel(OleDbConnection C)
        {
            this.C = C;
            GenCommand();
        }

        void GenCommand()
        {
            SelectCom = new OleDbCommand(
                "SELECT ID,D,mark FROM Tabel WHERE (Y = ?) AND (M = ?) AND (ID = ?) ORDER BY D", 
                C);
            SelectCom.Parameters.Add("Y", OleDbType.Integer);
            SelectCom.Parameters.Add("M", OleDbType.Integer);
            SelectCom.Parameters.Add("ID", OleDbType.Integer);

            PreSelectCom = new OleDbCommand(
                "SELECT ID,D,mark FROM Tabel WHERE (Y = ?) AND (M = ?) AND (ID = ?) AND (D > ?) ORDER BY D",
                C);
            PreSelectCom.Parameters.Add("Y", OleDbType.Integer);
            PreSelectCom.Parameters.Add("M", OleDbType.Integer);
            PreSelectCom.Parameters.Add("ID", OleDbType.Integer);
            PreSelectCom.Parameters.Add("D", OleDbType.Integer);

            ModifyCom = new OleDbCommand(
                "UPDATE Tabel SET mark = ? WHERE (Y = ?) AND (M = ?) AND (D = ?) AND (ID = ?)",
                C);
            ModifyCom.Parameters.Add("mark", OleDbType.VarWChar);
            ModifyCom.Parameters.Add("Y", OleDbType.Integer);
            ModifyCom.Parameters.Add("M", OleDbType.Integer);
            ModifyCom.Parameters.Add("D", OleDbType.Integer);
            ModifyCom.Parameters.Add("ID", OleDbType.Integer);

            InsertCom = new OleDbCommand(
                "INSERT INTO Tabel (mark,Y,M,D,ID) VALUES (?,?,?,?,?)",
                C);
            InsertCom.Parameters.Add("mark", OleDbType.VarWChar);
            InsertCom.Parameters.Add("Y", OleDbType.Integer);
            InsertCom.Parameters.Add("M", OleDbType.Integer);
            InsertCom.Parameters.Add("D", OleDbType.Integer);
            InsertCom.Parameters.Add("ID", OleDbType.Integer);
        }

        public int Load()
        {
            if (period == null) return 0;

            ids = workers.ids;
            countworker = ids.Length;

            if (countworker <1) return 0;

            marks = new string[countworker, 31 + period.extraday];

            int k = 0, i = 0;
            OleDbDataReader R;

            if (period.extraday > 0)
            {
                PreSelectCom.Parameters[0].Value = period.pre_year;
                PreSelectCom.Parameters[1].Value = period.pre_month;
                PreSelectCom.Parameters[3].Value = period.min_salaryday - 1;

                for (k = 0; k < countworker; k++)
                {
                    PreSelectCom.Parameters[2].Value = ids[k];
                    R = PreSelectCom.ExecuteReader();
                    i = 0;
                    while (R.Read())
                    {
                        marks[k, i++] = (string)R[2];
                    }
                    //if (i == 0) AddWoker(ids[k]);
                    R.Close();
                }

            }

            SelectCom.Parameters[0].Value = period.year;
            SelectCom.Parameters[1].Value = period.month;
                        
            for (k = 0; k < countworker; k++)
            {
                SelectCom.Parameters[2].Value = ids[k];
                R = SelectCom.ExecuteReader();
                i = period.extraday;
                while (R.Read())
                {
                    marks[k, i++] = (string)R[2];
                }
                if (i == period.extraday) AddWoker(ids[k]);
                R.Close();
            }
            return k;
        }

        public void AddWoker(int ID)
        {
            for (int j = 0; j < period.lastDay; j++)
            {
                InsertCom.Parameters[0].Value = "";
                InsertCom.Parameters[1].Value = period.year;
                InsertCom.Parameters[2].Value = period.month;
                InsertCom.Parameters[3].Value = j + 1;
                InsertCom.Parameters[4].Value = ID;
                InsertCom.ExecuteNonQuery();
            }
        }

        public void Save()
        {
            int j;
            for (int i = 0; i < countworker; i++)
            {
                for (j = 0; j < period.lastDay; j++)
                {
                    ModifyCom.Parameters[0].Value = marks[i, j + period.extraday];
                    ModifyCom.Parameters[1].Value = period.year;
                    ModifyCom.Parameters[2].Value = period.month;
                    ModifyCom.Parameters[3].Value = j + 1;
                    ModifyCom.Parameters[4].Value = ids[i];
                    ModifyCom.ExecuteNonQuery();
                }
            }
        }

        public void Save(int i, int j,string s)
        {
            marks[i, j] = s;            
            ModifyCom.Parameters[0].Value = s;
            ModifyCom.Parameters[1].Value = period.year;
            ModifyCom.Parameters[2].Value = period.month;
            ModifyCom.Parameters[3].Value = j + 1 - period.extraday;
            ModifyCom.Parameters[4].Value = ids[i];
            ModifyCom.ExecuteNonQuery();
        }

        public int AD, D, H, P, B, A, V, SD, SH; 

        public void Calculate(int index)
        {
            string str;
            int h, j, s;
            bool minor;

            SD = SH = AD = D = H = P = B = A = V = 0;

            s = Convert.ToInt32(workers.Table.Rows[index][12]);
            minor = (bool)workers.Table.Rows[index][10];

            if (s == 1)
            {
                j = period.extraday;
                s = period.lastDay + period.extraday;
            }
            else
            {
                j = 32 - s - period.extraday;
                s = 25 + period.extraday;
            }

            while (j < s)
            {
                if (period[j] > 0)
                {
                    AD++;
                    str = marks[index, j];
                    switch (str)
                    {
                        case "Â": V++; break;
                        case "Ï": P++; break;
                        case "À": A++; break;
                        case "Á": B++; break;
                        default:
                            if (int.TryParse(str, out h))
                            {
                                H += h;
                                D++;
                                if (minor)
                                    if (h > 7)
                                        SH += (h - 7);
                                if (period[j] == 2)
                                {
                                    SD++;
                                    SH += h;
                                }
                                if (period[j] == 3)
                                    if (h > 7)
                                        SH += (h - 7);
                            }
                            break;
                    }
                }
                j++;
            }
        }

    }  
    
}
