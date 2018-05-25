using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConceptFlower.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConceptFlower.Models;
using System.Data.OleDb;
using System.Data;
using System.Text.RegularExpressions;
using WinX.virtualAccount;
using ConceptFlower.ViewModel;
using System.Collections;

namespace ConceptFlower.Static.Tests
{
    [TestClass()]
    public class ConversionExcelDataToListTests
    {
        [TestMethod()]
        public void ExtracTrusteInfoExcelTest()
        {
            //string path = @"E:\Epass Stage 1 & 2 automation tool.xlsx";

            //ConversionExcelDataToList clit = new ConversionExcelDataToList();

            //clit.path = path;
            //OleDbConnection objExcelCon = clit.openExcel();

            //var list1=clit.ExtractEmployeeExcel<ExcelWhlist>(objExcelCon, "White List");

            //var list = clit.NotepadExcel(objExcelCon, "Notepad");

            //string str = "In Short.";

            string str = "     29999989  PERSONAL ACCOUNT HOLDER        PR01          19396019    CU     2017/11/30 ";
            Regex reg = new Regex(@"\s{2,}");
            var strList = reg.Split(str.Trim());
            string a = "撒地方撒地方，士大夫撒";
            Regex reg1 = new Regex(@"[\u4e00-\u9fa5\S]+");
            Regex reg2 = new Regex(@"[a-zA-Z\d]+");
            if(reg1.IsMatch(a)&&!reg2.IsMatch(a))
            {

            }
            string b = "撒地方撒地方，士大夫撒sdfsfds";
            if (reg1.IsMatch(b) && !reg2.IsMatch(b))
            {

            }
            string result = reg1.Replace(a, "");
            //var ss = str.Split(' ');

            //var a= str.Replace(" ","_");
            //var a= clit.SplitCamelCase(str);



        }


        [TestMethod()]
        public void readexcel()
        {
            List<WhiteListcs> wlist = new List<WhiteListcs>();
            ExcelEditor excelHandel = new ExcelEditor();

            //int index = 1;

            //WhiteListcs w1 = new WhiteListcs();

            //w1.Name_in_AS400 = "test";
            //w1.Case_Number = "5454579859";
            //w1.Clean_Case = "Y";
            //w1.ERID = "21456789";
            //w1.ExtentionNo = "81634";
            //w1.Name_Pass = "matched";
            //w1.Membership_No = "12345678-123456789";
            //w1.PM_AC = "PC";
            //w1.Signature_Pass = "Pass";
            //w1.Unclean_Reason.Append("sfdtrtyrturtutyuityurt");
            //w1.Withdraw_NT_PM = "NT";
            //w1.Name_in_XML = "test";

            //wlist.Add(w1);

            //WhiteListcs w2 = new WhiteListcs();

            //w2.Name_in_AS400 = "test";
            //w2.Case_Number = "5454579859";
            //w2.Clean_Case = "Y";
            //w2.ERID = "21456789";
            //w2.ExtentionNo = "81634";
            //w2.Name_Pass = "matched";
            //w2.Membership_No = "12345678-123456789";
            //w2.PM_AC = "PC";
            //w2.Signature_Pass = "Pass";
            //w2.Unclean_Reason.Append("sfdtrtyrturtutyuityurt");
            //w2.Withdraw_NT_PM = "NT";
            //w2.Name_in_XML = "test";
  
            //wlist.Add(w2);


            //if (wlist.Count > 0)
            //{
            //    var excel = excelHandel.Open(@"E:\Epass Stage 1 & 2 automation tool.xlsx");

            //    DataTable dt = ListToDatatableHelper.ToDataTable<WhiteListcs>(wlist);

            //    excel.SaveDataTableToExcel(dt, @"E:\Epass Stage 1 & 2 automation tool.xlsx", "White List");


            //}


            List<ExcelWhlist> ewlist = new List<ExcelWhlist>();

            ExcelWhlist ew = new ExcelWhlist();

            ew.Name_in_AS400 = "test";
            ew.Case_Number = "5454579859";
            ew.Clean_Case = "Y";
            ew.ERID = "21456789";
            //ew.ExtentionNo = "81634";
            ew.Name_Pass = "matched";
            ew.Membership_No = "12345678-123456789";
            ew.PM_AC = "PC";
            ew.Signature_Pass = "Pass";
            ew.Unclean_Reason = "fdsdsdsd";
            ew.Withdraw_NT_PM = "NT";
            ew.Name_in_XML = "test";
            ew.RowIndex = "2";
            ew.Process_result = "Processed";
            //ew.Message_Mark = "Y";
            ewlist.Add(ew);

            if (ewlist.Count > 0)
            {
                var excel = excelHandel.Open(@"E:\Epass Stage 1 & 2 automation tool.xlsx");
                excel.SetSpecifiedToExcel(ewlist, @"E:\Epass Stage 1 & 2 automation tool.xlsx", "White List");
            }





        }

        [TestMethod]
        public void Test1()
        {
            string a = "     30815471  FULUM PALACE O/B CENTRAL KING  MLY       10246709 TE  2012/06/07 ";
            string b = "     30876919  ELITE GRACE DEVELOPMENT LIMITE MLY       10246709 TE  2015/06/01 ";
            string s1 = a.Substring(5, 10);
            string s2 = a.Substring(15, 31);
            string s3 = a.Substring(46, 10);
            string s4 = a.Substring(56, 9);
            string s5 = a.Substring(65, 4);
            string s6 = a.Substring(69);
            string s11 = b.Substring(5, 10);
            string s22 = b.Substring(15, 31);
            string s33 = b.Substring(46, 10);
            string s44 = b.Substring(56, 9);
            string s55 = b.Substring(65, 4);
            string s66 = b.Substring(69);
        }

        [TestMethod]
        public void TestSaveExcel()
        {
            string ExcelPath = @"D:\ConceptFlower Test\Epass Stage 1 & 2 automation tool.xlsx";
            ExcelEditor excelHandel = new ExcelEditor();
            List<WhiteListcs> wlist = new List<WhiteListcs>();
            WhiteListcs model = new WhiteListcs { PM_AC = "PC", Membership_No = "12345678-123456789", ERID= "21456789" ,Case_Number= "5454579859"
            ,Name_in_AS400="test",Name_in_XML="test",Name_Pass="matched",Clean_Case="Y",Unclean_Reason=new StringBuilder("sdfasdfsfds"),Signature_Pass="Pass"
            ,Withdraw_NT_PM="NT",Process_result="Process"};
            wlist.Add(model);
            var excel = excelHandel.Open(ExcelPath);
            DataTable dt = ListToDatatableHelper.ToDataTable<WhiteListcs>(wlist);

            excel.SaveDataTableToExcel(dt, ExcelPath, "White List");
        }

        [TestMethod]
        public void TestPath()
        {
            string path= @"D:\ConceptFlower Test\Epass Stage 1 & 2 automation tool.xlsx";
            bool isPath= CheckPath(path);
        }

        public bool CheckPath(string path)
        {
            string pattern = @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(path);
        }
        [TestMethod]
        public void TestCodeRejection()
        {
            ConversionExcelDataToList clit = new ConversionExcelDataToList();
            clit.path = @"C:\Users\43743418\Desktop\Project Doc\Epass Stage 1 & 2 automation tool.xlsx";
            OleDbConnection objExcelCon = clit.openExcel();
            List<RejectionCode> PMrejectonList = clit.GetExcelDataBySheetName<RejectionCode>(objExcelCon, "PM rejection code");
            List<RejectionCode> ACrejectonList = clit.GetExcelDataBySheetName<RejectionCode>(objExcelCon, "AC rejection code");
            string remark=GetCodeRejection("AC", new string[] { "1", "2", "3" }, PMrejectonList, ACrejectonList);
        }
        private string GetCodeRejection(string pM_AC, string[] codes, List<RejectionCode> pMrejectonList, List<RejectionCode> aCrejectonList)
        {
            string rejectionReason = string.Empty;
            IEnumerable<RejectionCode> selectedCodes;
            if (pM_AC == "PM")
            {
                selectedCodes = pMrejectonList.Where(t => codes.Contains(t.Code) || t.Code == "99");
            }
            else
            {
                selectedCodes = aCrejectonList.Where(t => codes.Contains(t.Code) || t.Code == "99");
            }
            foreach (RejectionCode code in selectedCodes)
            {
                rejectionReason = string.IsNullOrEmpty(rejectionReason) ? code.Description : rejectionReason + ", " + code.Description;
            }
            return rejectionReason;
        }

        [TestMethod]
        public void TestRange()
        {
            List<ModelTest> list = new List<ModelTest>();
            for (int i=0;i<100;i++)
            {
                ModelTest t = new ModelTest { A = i, B = "Test" };
                list.Add(t);
            }
            List<ModelTest> result=TestList(list);
        }
        private List<ModelTest> TestList(List<ModelTest> list)
        {
            ArrayList arrayList = new ArrayList();//声明一个集合对象

            Random r = new Random();//声明一个随机对象
            for (int i = 0; i < 20; i++)
            {
                int number = r.Next(0, list.Count() - 1);//生成一个随机数，0-9
                while (arrayList.Contains(number))//判断集合中有没有生成的随机数，如果有，则重新生成一个随机数，直到生成的随机数list集合中没有才退出循环
                {
                    number = r.Next(0, list.Count() - 1);
                }
                arrayList.Add(number);//将生成的随机数添加到集合对象中
            }
            List<ModelTest> resultList = new List<ModelTest>();
            arrayList.Sort();
            foreach (int index in arrayList)
            {
                resultList.Add(list[index]);
            }
            return resultList;
        }
    }
    public class ModelTest
    {
        public int A { get; set; }
        public string B { get; set; }
    }
}