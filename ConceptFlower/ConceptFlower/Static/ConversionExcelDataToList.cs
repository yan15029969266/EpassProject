using System.Data.OleDb;
using System.IO;
using System.Data;
using System;
using System.Collections.Generic;
using ConceptFlower.Models;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using excel = Microsoft.Office.Interop.Excel;

namespace ConceptFlower.Static
{
 public   class ConversionExcelDataToList
    {


        public string path { get; set; }


        public OleDbConnection openExcel()
        {

            OleDbConnection conn = new OleDbConnection();
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet dsEmployeeInfo = new DataSet();

            string Import_FileName = path;
            try
            {
                string fileExtension = Path.GetExtension(Import_FileName);
                if (fileExtension == ".xls")
                    conn.ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 8.0;HDR=YES;IMEX=1;'";
                if (fileExtension == ".xlsx")
                    conn.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + Import_FileName + ";" + "Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1;'";
                conn.Open();
            }
            catch (Exception ex)
            {
            }
            return conn;       
            
        }

        /// <summary>
        /// Extract 'Service Information' sheet
        /// </summary>
        /// <param name="oledbConn"></param>
        public Dictionary<string,List<T>> ExtractEmployeeExcel<T>(OleDbConnection oledbConn, string sheet_name)
        {


            Dictionary<string, List<T>> dic = new Dictionary<string, List<T>>();

            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet dsEmployeeInfo = new DataSet();

            cmd.Connection = oledbConn;
            cmd.CommandType = CommandType.Text;

            List<T> assets = new List<T>();
            cmd.CommandText = "Select * from [" + sheet_name + "$]";

            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(dsEmployeeInfo, sheet_name);

            DataTable data = dsEmployeeInfo.Tables[sheet_name];

            T entity;

            int i = 0;
            foreach (DataRow row in data.Rows)
            {
                entity= Activator.CreateInstance<T>();
               
                foreach (PropertyInfo pinfo in entity.GetType().GetProperties())
                {
                    if (pinfo.Name == "RowIndex")
                    {
                        pinfo.SetValue(entity, (i + 2).ToString());
                    }
                    else
                    {
                        pinfo.SetValue(entity, row.Field<object>(pinfo.Name.Replace("_", " ")) != null ? row.Field<object>(pinfo.Name.Replace("_", " ")).ToString() : string.Empty);
                    }
                }
                assets.Add(entity);
             
            }
                       
            dic.Add(sheet_name,assets);

            return dic;

            //e//ntity.SetValue(entity, Convert.ChangeType(row.ItemArray[k], type.PropertyType, null));
            //var dsEmployeeInfoList = dsEmployeeInfo.Tables[0].AsEnumerable().Select(s => new 
            //{
            //Name_in_English = Convert.ToString(s["Name_in_English"] != DBNull.Value ? s["Name_in_English"] : ""),
            //    Trustee_code = Convert.ToString(s["Trustee_code"] != DBNull.Value ? s["Trustee_code"] : ""),
            //    Trustee_Approval_No = Convert.ToString(s["Trustee_Approval_No"] != DBNull.Value ? s["Trustee_Approval_No"] : ""),
            //    Trustee_client_no = Convert.ToString(s["Trustee_client_no"] != DBNull.Value ? s["Trustee_client_no"] : ""),
            //    Scheme_name = Convert.ToString(s["Scheme_name"] != DBNull.Value ? s["Scheme_name"] : ""),
            //    Scheme_code = Convert.ToString(s["Scheme_code"] != DBNull.Value ? s["Scheme_code"] : ""),
            //    Scheme_Registration_No = Convert.ToString(s["Scheme_Registration_No"] != DBNull.Value ? s["Scheme_Registration_No"] : "")
            //}).ToList();
            //return dsEmployeeInfoList ;
        }

        public List<T> GetExcelDataBySheetName<T>(OleDbConnection oledbConn, string sheet_name)
        {
            OleDbCommand cmd = new OleDbCommand(); ;
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet ds = new DataSet();

            cmd.Connection = oledbConn;
            cmd.CommandType = CommandType.Text;

            List<T> assets = new List<T>();
            cmd.CommandText = "Select * from [" + sheet_name + "$]";

            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(ds, sheet_name);
            DataTable data = ds.Tables[sheet_name];

            T entity;

            int i = 0;
            foreach (DataRow row in data.Rows)
            {
                entity = Activator.CreateInstance<T>();

                foreach (PropertyInfo pinfo in entity.GetType().GetProperties())
                {
                    if (pinfo.Name == "RowIndex")
                    {
                        pinfo.SetValue(entity, (i + 2).ToString());
                    }
                    else
                    {
                        pinfo.SetValue(entity, row.Field<object>(pinfo.Name.Replace("_", " ")) != null ? row.Field<object>(pinfo.Name.Replace("_", " ")).ToString() : string.Empty);
                    }
                }
                assets.Add(entity);
            }
            return assets;
        }

        public object[,] GetExcelDataByRange(string sheet_name,string startCell,string endCell)
        {
            string Import_FileName = path;
            var app = new excel.Application();
            var wb = app.Workbooks.Open(path);
            var sht = wb.Worksheets[sheet_name];
            object[,] datas = sht.Range[startCell+":"+ endCell].Value2;
            return datas;
        }


        public  string SplitCamelCase( string str)
        {
            return Regex.Replace(
                Regex.Replace(
                    str,
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1_$2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1_$2"
            );
        }

        public IList<Notepad> NotepadExcel(OleDbConnection oledbConn, string sheetName)
        {
            OleDbCommand cmd = new OleDbCommand(); 
            OleDbDataAdapter oleda = new OleDbDataAdapter();
            DataSet dsEmployeeInfo = new DataSet();

            cmd.Connection = oledbConn;
            cmd.CommandType = CommandType.Text;

            cmd.CommandText = "Select * from [" + sheetName + "$]";

            oleda = new OleDbDataAdapter(cmd);
            oleda.Fill(dsEmployeeInfo);

            var dsEmployeeInfoList = dsEmployeeInfo.Tables[0].AsEnumerable().Select(s => new Notepad
            {
                Notepad_Key_Words = Convert.ToString(s["Notepad Key Words"] != DBNull.Value ? s["Notepad Key Words"] : "")
             
            }).ToList();

            return dsEmployeeInfoList;
        }







    }
}
