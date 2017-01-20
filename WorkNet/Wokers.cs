using System.Data;
using System.Data.OleDb;
using System.IO;

namespace WorkNet
{
    public delegate void delAppend();

    public class Workers
    {
        OleDbDataAdapter Adapter = new OleDbDataAdapter();
        OleDbCommand DelComm, DisComm;

        public event delAppend AppendWoker;
        public event delAppend UpdateWoker;

        public Period period;
        public DataTable Table = new DataTable("Workers");

        public int[] ids;

        public int Count
        {
            get { return Table.Rows.Count; }
        }

        public Workers()
        {
            DB.GenCommand(Table, Adapter);

            Adapter.SelectCommand.CommandText = @"SELECT * FROM WorkersPlus 
                                WHERE (sDate <= ?) AND (eDate >= ?) ORDER BY Grade DESC";

            Adapter.SelectCommand.Parameters.Add("sDate", OleDbType.DBDate);
            Adapter.SelectCommand.Parameters.Add("eDate", OleDbType.DBDate);

            DelComm = new OleDbCommand("DELETE FROM Workers WHERE ID = ?", DB.connection);
            DelComm.Parameters.Add("ID", OleDbType.Integer);

            DisComm = new OleDbCommand("UPDATE Workers SET eDate = ? WHERE ID = ?");
            DisComm.Parameters.Add("eDate", OleDbType.DBDate);
            DisComm.Parameters.Add("ID", OleDbType.Integer);

        }

        void Createids()
        {
            ids = new int[Table.Rows.Count];
            for (int i = 0; i < ids.Length; i++)
                ids[i] = (int)Table.Rows[i][0];
        }

        public void Load()
        {
            if (period == null) return;
            Adapter.SelectCommand.Parameters[0].Value = period.eDate;
            Adapter.SelectCommand.Parameters[1].Value = period.sDate;
            Table.Clear();
            Adapter.Fill(Table);
            Createids();
        }

        public void Append(params object[] obj)
        {
            Table.Rows.Add(obj);
            Adapter.Update(Table);
            if (AppendWoker != null) AppendWoker();
            Createids();
        }

        public void Update(params object[] obj)
        {
            DataRow Row = Table.Rows.Find(obj[0]);
            int i;
            if (Row != null)
            {
                for (i = 1; i < obj.Length; i++)
                {
                    if (obj[i] != null) Row[i] = obj[i];
                }
                Adapter.Update(Table);
            }
            else
            {
                for (i = 1; i < obj.Length; i++)
                    Adapter.UpdateCommand.Parameters[i - 1].Value = obj[i];
                Adapter.UpdateCommand.Parameters[i - 1].Value = obj[0];
                Adapter.UpdateCommand.ExecuteNonQuery();
            }
            if (UpdateWoker != null) UpdateWoker();
        }

        public void DisUpdate(object d, int ID)
        {
            DisComm.Parameters[0].Value = d;
            DisComm.Parameters[1].Value = ID;
            DisComm.ExecuteNonQuery();
            if (UpdateWoker != null) UpdateWoker();
        }


        public void Delete(int ID)
        {
            DataRow Row = Table.Rows.Find(ID);
            if (Row != null)
            {
                Row.Delete();
                Adapter.Update(Table);
                if (AppendWoker != null) AppendWoker();
                Createids();
            }
            else
            {
                DelComm.Parameters[0].Value = ID;
                DelComm.ExecuteNonQuery();
            }
        }

    }
}

  
