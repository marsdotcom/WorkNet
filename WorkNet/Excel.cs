using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Ex = Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace WorkNet
{
    class Excel
    {
        static object Z = Missing.Value;
        int numbeeps = 2;
        public string nformat = "0" + Keyboard.NumberSeparator + "00";
        public string dformat = "ÄÄ" + Keyboard.DateSeparator + "ÌÌ" + Keyboard.DateSeparator + "ÃÃÃÃ";

        Ex.Application excel;     
        Ex.Workbook  book;
        Ex.Worksheet sheet;
        Ex.Range range;
        int countsheet;
        int currentsheet;
        int currentbook;
        int count;

        public int NumberEps
        {
            get { return numbeeps; }
            set
            {
                nformat = "0";
                numbeeps = value;
                if (value > 0)
                {
                    nformat += Keyboard.NumberSeparator;
                    for (int i = 0; i < value; i++)
                        nformat += "0";
                }
            }
        }

        public int CurrentSheet
        {
            get { return currentsheet; }
            set
            {
                if ((value <= book.Sheets.Count) || (value > 0))
                {
                    currentsheet = value;
                    sheet = (Ex.Worksheet)book.Sheets[currentsheet];                    
                }
            }
        }

        public int CurrentBook
        {
            get { return currentbook; }
            set
            {
                if ((value <= excel.Workbooks.Count) || (value > 0))
                {
                    currentbook = value;
                    book = excel.Workbooks[currentbook];
                }
            }
        }

        public void ImportArray(out object[,] obj, string cell,int colcount,int rowcount)
        {
            range = sheet.get_Range(cell, Z);
            range = range.get_Resize(rowcount,colcount);

            obj = new object[rowcount, colcount];

            obj = (object[,])range.Value2;           
        }

        public object ImportObj(string cell)
        {
            return sheet.get_Range(cell, Z).Value2;
        }

        public void ExportObj(string cell,object obj)
        {
            range = sheet.get_Range(cell, Z);
            range.Value2 = obj;
        }

        public void ImportArray(out object[] obj,string cell,int count)
        {
            range = sheet.get_Range(cell, Z);
            obj = new object[count];
            obj = (object[])range.Value2;
        }

        public void ExportTable(DataTable T, bool header, string cell, int start, int end, int colmer)
        {
            int shift = (header) ? 1 : 0;

            int rows = T.Rows.Count;
            if (end < 0) end = rows;
            if (start < 0) start = 0;
            if (end > (rows - shift)) end = rows;
            if (start > end) start = end;

            rows = end - start + shift;

            if ((rows - shift) == 0) return;

            int cols = T.Columns.Count;
            int j;
            object[,] obj = new Object[rows, cols];

            if (header)
                for (j = 0; j < cols; j++)
                    obj[0, j] = T.Columns[j].ColumnName;

            for (int i = start; i < end; i++)
            {
                for (j = 0; j < cols; j++)
                    obj[i + shift - start, j] = T.Rows[i][j];
            }

            range = sheet.get_Range(cell, Z);
            range = range.get_Resize(rows, cols);

            range.Borders[Ex.XlBordersIndex.xlEdgeBottom].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeLeft].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeRight].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeTop].LineStyle = Ex.XlLineStyle.xlContinuous;
            if ((rows > 1) && (cols > 1))
            {
                range.Borders[Ex.XlBordersIndex.xlInsideHorizontal].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlInsideVertical].LineStyle = Ex.XlLineStyle.xlContinuous;
            }
            range.NumberFormat = "@";
            range.Value2 = obj;          // assignation

            range = sheet.get_Range(cell, Z);
            range = range.get_Resize(rows, 1);            

            for (j = 0; j < cols; j++)   // formatting
            {
                if (T.Columns[j].DataType == typeof(DateTime))
                    range.get_Offset(shift, j).NumberFormat = dformat;
                else if (T.Columns[j].DataType == typeof(Single))
                    range.get_Offset(shift, j).NumberFormat = nformat;
            }

            if (colmer >= 0)  // merging the same ranges.
            {
                range = range.get_Offset(0, colmer);
                range.VerticalAlignment = Ex.XlVAlign.xlVAlignCenter;
                range.HorizontalAlignment = Ex.XlHAlign.xlHAlignCenter;
                int a = shift;
                for (j = 1+shift; j < rows; j++)
                    if (!obj[j, colmer].Equals(obj[a, colmer]))
                    {
                        if ((j - a) > 1)
                        {
                            range.get_Offset(a, 0).get_Resize(j - a, 1).Merge(false);
                        }
                        a = j;
                    }

                if ((j - a) > 1)
                {
                    range.get_Offset(a, 0).get_Resize(j - a, 1).Merge(false);
                }
            }

            if (header)
            {
                range = sheet.get_Range(cell, Z);
                range = range.get_Resize(1, cols);
                range.Font.Bold = true;
            }
        }

        public void MergeCell(string R1,string R2)
        {
            range = sheet.get_Range(R1, R2);
            range.Merge(false);
            //range.HorizontalAlignment = Ex.XlHAlign.xlHAlignCenter;
        }

        public void BoldRange(bool b)
        {
            if (range != null)
                range.Font.Bold = b;
        }

        public void NumberFormat(string cell,string f)
        {
            range = sheet.get_Range(cell, Z);
            range.NumberFormat = f;
        }

        public void BorderRange()
        {
            try
            {
                range.HorizontalAlignment = Ex.XlHAlign.xlHAlignCenter;
                range.Borders[Ex.XlBordersIndex.xlEdgeBottom].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeLeft].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeRight].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeTop].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlInsideHorizontal].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlInsideVertical].LineStyle = Ex.XlLineStyle.xlContinuous;
            }
            catch
            { }
        }

        public void BorderRange(string cells)
        {
            range = sheet.get_Range(cells, Z);
            try
            {
                range.HorizontalAlignment = Ex.XlHAlign.xlHAlignCenter;
                range.Borders[Ex.XlBordersIndex.xlEdgeBottom].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeLeft].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeRight].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlEdgeTop].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlInsideHorizontal].LineStyle = Ex.XlLineStyle.xlContinuous;
                range.Borders[Ex.XlBordersIndex.xlInsideVertical].LineStyle = Ex.XlLineStyle.xlContinuous;
            }
            catch
            { }
        }


        public void ExportArray(string cell,params object[] array)
        {
            range = sheet.get_Range(cell, Z);
            range = range.get_Resize(1, array.Length);
            range.Borders[Ex.XlBordersIndex.xlEdgeBottom].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeLeft].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeRight].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.Borders[Ex.XlBordersIndex.xlEdgeTop].LineStyle = Ex.XlLineStyle.xlContinuous;
            range.NumberFormat = "@";
            range.Value2 = array;          // assignation
            range.Font.Bold = true;
        }

        public void UndelineCharacters(string Cell, int start, int length)
        {
            range = sheet.get_Range(Cell, Z);
            range.get_Characters(start, length).Font.Underline = 
                Ex.XlUnderlineStyle.xlUnderlineStyleSingle;
        }

        public void ExportArray(string cell, object[,] array,int row,int col)
        {
            range = sheet.get_Range(cell, Z);
            range = range.get_Resize(row, col);           
            range.Value2 = array;
        }

        public void AutoFill(string sCell,string eCell)
        {
            range = sheet.get_Range(sCell, Z);
            range.AutoFill(sheet.get_Range(sCell, eCell), Ex.XlAutoFillType.xlFillDefault);
        }

        public void FormulaSum(string Cell,int h)
        {
            range = sheet.get_Range(Cell, Z);
            range.FormulaR1C1 = "=ÑÓÌÌ(R[-1]C:R[-" + h.ToString() + "]C)";
        }

        public void Formula(string Cell, string str)
        {
            sheet.get_Range(Cell, Z).Formula = str;
        }

        public void AutoFit(string cell)
        {
            range = sheet.get_Range("A1", cell);
            range.EntireColumn.AutoFit();
        }

        public Excel()                     // constructor
        {
            excel = new Ex.Application();
            excel.DisplayAlerts = false;
            excel.Visible = false;
        }

        public Excel(bool attach)                     // constructor
        {
            if (attach)
            {            
                excel = (Ex.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
                count = excel.Workbooks.Count;
                if (count > 0)
                {
                    CurrentBook = 1;
                    countsheet = book.Worksheets.Count;
                    if (countsheet > 0) CurrentSheet = 1;
                }
            }
            else
            {
                excel = new Ex.Application();
                excel.DisplayAlerts = false;
                excel.Visible = false;
            }
        }

        public void AddBook()
        {
            book = excel.Workbooks.Add(Z);
            count = excel.Workbooks.Count;
            currentbook = count;
            countsheet = book.Sheets.Count;
            DeleteSheet(3);
            DeleteSheet(2);
            CurrentSheet = 1;            
        }

        public void AddBook(string fName)
        {
            book = excel.Workbooks.Add(fName);
            count = excel.Workbooks.Count;
            currentbook = count;
            countsheet = book.Sheets.Count;
            //DeleteSheet(3);
            //DeleteSheet(2);
            CurrentSheet = 1;
        }

        public void OpenBook(string FileName)
        {
           book = excel.Workbooks.Open(FileName, Z, Z, Z, Z, Z, Z, Z, Z, Z, Z, Z, Z, Z, Z);
           count = excel.Workbooks.Count;
           currentbook = count;

           countsheet = book.Sheets.Count;
           CurrentSheet = 1;
        }

        public void CloseBook(int n)
        {
            if (n <= count && n > 0)
            {
                excel.Workbooks[n].Close(false,Z,Z);
                count = excel.Workbooks.Count;
                if (count > 0) CurrentBook = count;
            }
        }

        public void Close()
        {
            if (excel != null)
            {
                excel.DisplayAlerts = false;
                excel.Quit();
                book = null;
                sheet = null;
                range = null;
                excel = null;
                GC.Collect();
            }
        }

        public void Disconnect()
        {
            excel = null;
            book = null;
            sheet = null;
            range = null;
            GC.Collect();
        }

        public void DeleteSheet(int n)
        {
            countsheet = book.Sheets.Count;
            if (countsheet > 1)
            {
                if ((n <= book.Sheets.Count) || (n > 0))
                {
                    ((Ex.Worksheet)book.Sheets[n]).Delete();
                    countsheet--;
                }
                CurrentSheet = 1;
            }
        }

        public void AddSheet(string name)
        {
            book.Sheets.Add(Z, Z, Z, Z);
            countsheet++;
            if (name != null)
                ((Ex.Worksheet)book.Sheets[1]).Name = name;
            else
                ((Ex.Worksheet)book.Sheets[1]).Name = "Ëèñò"+countsheet.ToString();
            CurrentSheet = 1;
        }

        public void CopySheet(int sourceBook,int sourceSheet,int targetBook,int targetSheet)
        {
            ((Ex.Worksheet)excel.Workbooks[sourceBook].Sheets[sourceSheet]).Copy(Z,
                excel.Workbooks[targetBook].Sheets[targetSheet]);
        }

        public void Show()
        {
            excel.DisplayAlerts = true;
            excel.Visible = true;
        }

        public void Hide()
        {
            excel.DisplayAlerts = false;
            excel.Visible = false;
        }
    }
}
