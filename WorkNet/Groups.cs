using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Collections.Generic;

namespace WorkNet
{
    public static class Groups
    {
        static OleDbDataAdapter adapter = new OleDbDataAdapter();
        public static DataTable table = new DataTable("Groups");
        public static string[] groups;
        public static int min_salaryday, extraday;
        public static Dictionary<string, Expressions> ListExp = new Dictionary<string, Expressions>();
        public static bool expchanged = true;

        public static void LoadExp()
        {
            adapter.Fill(table);
            ListExp.Clear();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Expressions E = new Expressions();
                E.LoadFromString(table.Rows[i][3].ToString());
                ListExp.Add(table.Rows[i][0].ToString(), E);
            }
            table.Clear();            
        }
        
        public static void Load()
        {
            DB.GenCommand(table, adapter);
            adapter.Fill(table);
            groups = new string[table.Rows.Count];
            for (int i = 0; i < table.Rows.Count; i++)
                groups[i] = table.Rows[i][0].ToString();
            table.Clear();
            EvaluteMinSalaryDay();

           
        }

        public static void EvaluteMinSalaryDay()
        {
            object o = DB.GenSelectCom("MinSalaryDay").ExecuteScalar();
            int min_new;
            try
            {
                min_new = (int)o;
            }
            catch
            {
                min_new = 1;
            }
            if (min_new != min_salaryday)
            {
                min_salaryday = min_new;
                if (min_salaryday == 1) extraday = 0;
                else extraday = 32 - min_salaryday;
            }
        }
    }
}
