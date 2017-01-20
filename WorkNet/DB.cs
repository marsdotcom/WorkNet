using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

namespace WorkNet
{
  
    public static class DB
    {
        public static OleDbConnectionStringBuilder CSB = new OleDbConnectionStringBuilder();
        public static OleDbConnection connection = new OleDbConnection();
        static bool byzero = false;

        public static bool Connect(string DataSource)
        {
            CSB.Provider = "Microsoft.Jet.OLEDB.4.0";
            CSB.DataSource = DataSource;
            connection.Close();
            connection.ConnectionString = CSB.ConnectionString;
            try
            {
                connection.Open();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public static OleDbCommand GenSelectCom(string ntable)
        {
            return new OleDbCommand("SELECT * FROM " + ntable, connection);
        }

        public static string _GenInsertCom(DataTable T)
        {
            StringBuilder S = new StringBuilder();
            S.Append("INSERT INTO ");
            S.Append(T.TableName);
            S.Append(" (");
            int i = 1;
            if (byzero) i = 0;
            if ((T.Columns.Count > 1) || (byzero))
                for (; i < T.Columns.Count - 1; i++) S.Append(T.Columns[i].ColumnName).Append(",");
            S.Append(T.Columns[i].ColumnName);
            S.Append(") VALUES (?");
            if (byzero) i = 1; else i = 2;
            for (; i < T.Columns.Count; i++) S.Append(",?");
            S.Append(")");
            return S.ToString();
        }

        public static OleDbCommand GenInsertCom(DataTable T)
        {
            byzero = T.Columns[0].DataType == typeof(string);
            OleDbCommand Com = new OleDbCommand(_GenInsertCom(T), connection);
            if (byzero) Com.Parameters.Add(GenParam(T.Columns[0]));
            Com.Parameters.AddRange(GenParam(T));
            return Com;
        }

        public static string _GenUpdateCom(DataTable T)
        {
            StringBuilder S = new StringBuilder();
            S.Append("UPDATE ");
            S.Append(T.TableName);
            S.Append(" SET ");
            int i = 1;
            if (byzero) i = 0;
            if ((T.Columns.Count > 1) || (byzero))
                for (; i < T.Columns.Count - 1; i++) S.Append(T.Columns[i].ColumnName).Append(" = ?,");
            S.Append(T.Columns[i].ColumnName).Append(" = ? WHERE (").Append(T.Columns[0].ColumnName);
            S.Append(" = ?)");
            return S.ToString();
        }

        public static OleDbCommand GenUpdateCom(DataTable T)
        {
            byzero = T.Columns[0].DataType == typeof(string);
            OleDbCommand Com = new OleDbCommand(_GenUpdateCom(T), connection);
            if (byzero) Com.Parameters.Add(GenParam(T.Columns[0]));
            Com.Parameters.AddRange(GenParam(T));
            Com.Parameters.Add(GenParam_N(T));
            return Com;
        }

        public static string _GenDeleteCom(DataTable T)
        {
            return "DELETE FROM " + T.TableName + " WHERE (" + T.Columns[0].ColumnName + "= ?)";
        }

        public static OleDbCommand GenDeleteCom(DataTable T)
        {
            OleDbCommand Com = new OleDbCommand(_GenDeleteCom(T), connection);
            Com.Parameters.Add(GenParam_N(T));
            return Com;
        }

        public static OleDbParameter[] GenParam(DataTable T)
        {
            int count = T.Columns.Count - 1;
            OleDbParameter[] Ps = new OleDbParameter[count];
            for (int i = 0; i < count; i++)
            {
                Ps[i] = new OleDbParameter(T.Columns[i + 1].ColumnName,
                    TypeToOleType(T.Columns[i + 1].DataType), 0, T.Columns[i + 1].ColumnName);
            }
            return Ps;
        }

        public static OleDbParameter GenParam(DataColumn C)
        {
            return new OleDbParameter(C.ColumnName, TypeToOleType(C.DataType), 0, C.ColumnName);
        }

        public static OleDbParameter GenParam_N(DataTable T)
        {
            return new OleDbParameter(T.Columns[0].ColumnName, TypeToOleType(T.Columns[0].DataType),
                           0, ParameterDirection.Input, false, ((Byte)(0)), ((Byte)(0)),
                           T.Columns[0].ColumnName, DataRowVersion.Original, null);
        }

        public static OleDbParameter GenParam_N(DataColumn C)
        {
            return new OleDbParameter(C.ColumnName, TypeToOleType(C.DataType),
                           0, ParameterDirection.Input, false, ((Byte)(0)), ((Byte)(0)),
                           C.ColumnName, DataRowVersion.Original, null);
        }

        public static OleDbType TypeToOleType(Type T)
        {
            switch (T.Name)
            {
                case "Byte": return OleDbType.Integer;
                case "Int32": return OleDbType.Integer;
                case "Int64": return OleDbType.BigInt;
                case "String": return OleDbType.VarWChar;
                case "DateTime": return OleDbType.DBDate;
                case "Boolean": return OleDbType.Boolean;
                case "Single": return OleDbType.Single;
                case "Byte[]": return OleDbType.Binary;
            }
            return OleDbType.Variant;
        }

        public static void GenCommand(DataTable T, OleDbDataAdapter A)
        {
            if (connection.State == ConnectionState.Open)
            {
                A.SelectCommand = GenSelectCom(T.TableName);
                A.FillSchema(T,SchemaType.Source);
                A.InsertCommand = GenInsertCom(T);
                A.UpdateCommand = GenUpdateCom(T);
                A.DeleteCommand = GenDeleteCom(T);
            }
        }
   
        public static void Close()
        {
            connection.Close();
            CSB.Clear();
        }
    }
}
