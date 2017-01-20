using System;
using System.Data.OleDb;

namespace WorkNet
{
    public class Calendar
    {
        OleDbConnection C;
        OleDbCommand Command;
        OleDbCommand SelectCom;
        OleDbCommand ModifyCom;
        OleDbCommand InsertCom;
        bool newMonth = false;

        public int[] days = new int[31];
        public int lastDay, month, year;
        public int workdays, workhours, holidays;

        public Calendar(OleDbConnection C)
        {
            this.C = C;
            GenCommand();
        }

        public int this[int i]
        {
            get { return days[i]; }
            set { days[i] = value; }
        }

        void GenCommand()
        {
            SelectCom = new OleDbCommand(
                "SELECT D,mark FROM Calendar WHERE (Y = ?) AND (M = ?) ORDER BY D",
                C);
            SelectCom.Parameters.Add("Y", OleDbType.Integer);
            SelectCom.Parameters.Add("M", OleDbType.Integer);

            ModifyCom = new OleDbCommand(
                "UPDATE Calendar SET mark = ? WHERE (Y = ?) AND (M = ?) AND (D = ?)",
                C);
            ModifyCom.Parameters.Add("mark", OleDbType.Integer);
            ModifyCom.Parameters.Add("Y", OleDbType.Integer);
            ModifyCom.Parameters.Add("M", OleDbType.Integer);
            ModifyCom.Parameters.Add("D", OleDbType.Integer);

            InsertCom = new OleDbCommand(
                "INSERT INTO Calendar (mark,Y,M,D) VALUES (?,?,?,?)",
                C);
            InsertCom.Parameters.Add("mark", OleDbType.Integer);
            InsertCom.Parameters.Add("Y", OleDbType.Integer);
            InsertCom.Parameters.Add("M", OleDbType.Integer);
            InsertCom.Parameters.Add("D", OleDbType.Integer);

            Command = new OleDbCommand(
                "SELECT M,Y FROM Calendar WHERE (Y = ?) AND (M = ?) AND (D = 1)",
                C);
            Command.Parameters.Add("Y", OleDbType.Integer);
            Command.Parameters.Add("M", OleDbType.Integer);
        }

        void ModifyInsert(OleDbCommand Com)
        {
            for (int i = 0; i < 31; i++)
            {
                if (days[i] < 1) break;
                Com.Parameters[0].Value = days[i];
                Com.Parameters[1].Value = year;
                Com.Parameters[2].Value = month;
                Com.Parameters[3].Value = i + 1;
                Com.ExecuteNonQuery();
            }
        }

        public void Save()
        {
            if (newMonth) ModifyInsert(InsertCom);
            else ModifyInsert(ModifyCom);
        }

        void CreateMonth()
        {
            DateTime T = new DateTime(year, month, 1);
            int d = (int)T.DayOfWeek;
            DateTime TN = T.AddMonths(1);
            TN = TN.AddDays(-1);
            lastDay = TN.Day;
            d = (d == 0) ? 7 : d;
            d--;
            int k = 0, i, j;
            for (i = 0; i < 6; i++)
            {
                for (j = 0; j < 7; j++)
                {
                    if (k >= lastDay) break;
                    if ((i > 0) || (j >= d))
                    {
                        if (j > 4) days[k] = 2;
                        else days[k] = 1;
                        k++;            
                    }
                }
            }
            ModifyInsert(InsertCom);
        }

        public bool Load(int year, int month)
        {
            days[30] = days[29] = days[28] = 0;
            this.year = year;
            this.month = month;
            SelectCom.Parameters[0].Value = year;
            SelectCom.Parameters[1].Value = month;
            OleDbDataReader R = SelectCom.ExecuteReader();
            int i = 0;
            while (R.Read())
            {
                days[i++] = Convert.ToInt32(R[1]);
            }
            R.Close();
            newMonth = (i < 1);
            lastDay = i;
            if (lastDay == 0) CreateMonth();
            EvaluteDays();
            return newMonth;
        }

        public void EvaluteDays()
        {
            holidays = workhours = 0;
            for (int i = 0; i < lastDay; i++)
                if (days[i] == 2) holidays++;
                else if (days[i] == 1) workhours += 8;
                else workhours += 7;
            workdays = lastDay - holidays;
        }
    }
}
