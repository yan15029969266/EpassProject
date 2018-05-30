using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ConceptFlower.Models;
using System.Text.RegularExpressions;
using System.IO;

namespace ConceptFlower.Static
{
    public class ExcelEditor
    {
        public string mFilename;
        public Microsoft.Office.Interop.Excel.Application _app;
        public Microsoft.Office.Interop.Excel.Workbook _wb;
        private Worksheet _sheet;
        public ExcelEditor()
        {

        }

        public ExcelEditor Open(string path)
        {
            _app = new Microsoft.Office.Interop.Excel.Application();
            _wb = _app.Workbooks.Open(path, Type.Missing, false, Type.Missing, Type.Missing, Type.Missing, true, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            return this;
        }



        public ExcelEditor GetSheet(string name)
        {
            _sheet = (Microsoft.Office.Interop.Excel.Worksheet)_wb.Worksheets[name];
            getMax();
            return this;
        }


        public ExcelEditor SetRowIndex(int index)
        {
            _maxLine = index;
            return this;
        }

        public ExcelEditor SetMaxIndex()
        {
            _maxLine = getMax();
            return this;
        }


        private int? _maxLine = 1;
        public int getMax()
        {
            //predict it used 
            int tempX = _sheet.UsedRange.Rows.Count;
            //Horizontal detection of MaxColumn columns
            int tempY = _sheet.UsedRange.Columns.Count;

            int maxLine = 1;

            for (int i = 1; i <= (tempY > 15 ? 15 : tempY); i++) //columns,but restrict it less than 15
            {
                for (int j = 1; j <= tempX; j++)//rows
                {
                    if (_sheet.Cells[j, i].Value2 != null)
                    {
                        if (_sheet.Cells[j, i].Value2.ToString().Trim().Length > 0)
                        {
                            if (j > maxLine)
                            {
                                maxLine = j;
                            }
                        }
                    }

                }
            }
            return maxLine;
        }


        private int? _column = 1;
        public ExcelEditor SetColumnIndex(int col)
        {
            _column = col;
            return this;
        }

        public ExcelEditor AppendRow(params string[] numStack)
        {
            for (int i = 0; i < numStack.Length; i++)
            {
                _sheet.Cells[_maxLine, i + _column].Value = numStack[i].ToString();
            }
            _maxLine++;
            return this;
        }


        public ExcelEditor Save()
        {
            _wb.Save();
            return this;
        }

        private bool CheckPath(string path)
        {
            string pattern = @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(path);
        }
        public  bool SaveDataTableToExcel(System.Data.DataTable excelTable, string filePath,string sheetName)
        {
          
            try
            {
                _app.Visible = false;
                Worksheet wSheet = GetSheet(sheetName)._sheet;
                if (excelTable.Rows.Count > 0)
                {
                    int row = 0;
                    row = excelTable.Rows.Count;
                    int col = excelTable.Columns.Count;
                    for (int i = 0; i < row; i++)
                    {
                        for (int j = 0; j < col; j++)
                        {
                            string str = excelTable.Rows[i][j].ToString();
                            if (CheckPath(str))
                            {
                                if(File.Exists(str))
                                {
                                    Range cell = wSheet.Cells[i + 2, j + 1];
                                    cell.Value2 = "View Picture";
                                    wSheet.Hyperlinks.Add(cell, str, Type.Missing, "", Type.Missing);
                                }
                            }
                            else
                            {
                                wSheet.Cells[i + 2, j + 1] = str;
                            }
                        }
                    }
                }
                int size = excelTable.Columns.Count;
                for (int i = 0; i < size; i++)
                {
                    wSheet.Cells[1, 1 + i] = excelTable.Columns[i].ColumnName.Replace("_"," ");
                }
                //设置禁止弹出保存和覆盖的询问提示框   
                _app.DisplayAlerts = false;
                _app.AlertBeforeOverwriting = false;
                Save();
                Close();
              
                return true;
            }
            catch (Exception err)
            {
               
                return false;
            }
            finally
            {
                Marshal.ReleaseComObject(_app);

            }
        }



        public bool SetSpecifiedToExcel(List<ExcelWhlist> list , string filePath, string sheetName)
        {

            try
            {
                _app.Visible = false;

                Worksheet wSheet = GetSheet(sheetName)._sheet;

                list.ForEach(x=>{
                    wSheet.Cells[int.Parse(x.RowIndex), 17] = x.Process_result;
                });

                //设置禁止弹出保存和覆盖的询问提示框   
                _app.DisplayAlerts = false;
                _app.AlertBeforeOverwriting = false;
                Save();
                Close();

                return true;
            }
            catch (Exception err)
            {

                return false;
            }
            finally
            {
                Marshal.ReleaseComObject(_app);

            }
        }




        public void Close()
        {
            try
            {
                _wb.Close(Type.Missing, Type.Missing, Type.Missing);
                _app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(_app);
                GC.Collect();
            }
            catch (Exception)
            {

            }
        }
    }
}
