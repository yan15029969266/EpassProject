using ConceptFlower.BLL;
using ConceptFlower.Models;
using ConceptFlower.Static;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinX.virtualAccount;
using WinX.Web;

namespace ConceptFlowerTests.Static
{
    [TestClass()]
    public class GwisTest
    {
        [TestMethod()]
        public void TestNoDoc()
        {
            gwis.testWeb page = new gwis.testWeb();
            if(page.WaitForCreate())
            {
                page.btnSearch.click();
            }
        }
        [TestMethod()]
        public void TestSearch()
        {
            gwis.Search search = new gwis.Search();
            if(search.WaitForCreate())
            {

            }
        }
        [TestMethod]
        public void TestDistnic()
        {
            string ErrorCode = "13,";
            List<string> errorCodeList = ErrorCode.Split(',').ToList();
            errorCodeList = errorCodeList.Where(t => t != "").ToList();
            errorCodeList = errorCodeList.Distinct().ToList();
            string erroCode = string.Empty;
            foreach (string code in errorCodeList)
            {
                erroCode = erroCode == "" ? code : erroCode + "," + code;
            }
            ErrorCode = erroCode;
        }
        [TestMethod]
        public void TestRunCaseGwis()
        {
            ConversionExcelDataToList clit = new ConversionExcelDataToList();
            clit.path = @"L:\GZT_RBT\Private\Production\2B\SA Level\2b Logging\Jasmine\Robot\Project Doc\Epass Stage 1 & 2 automation tool.xlsx";
            Dictionary<string, List<ExcelWhlist>> ewhlistdic = new Dictionary<string, List<ExcelWhlist>>();
            OleDbConnection objExcelCon = clit.openExcel();
            ewhlistdic = clit.ExtractEmployeeExcel<ExcelWhlist>(objExcelCon, "White List");
            ExcelWhlist passModel = ewhlistdic["White List"].ToList().Where(x => x.Clean_Case == "Y").ToList().FirstOrDefault();
            GwisOperationLogic gwisOperation = new GwisOperationLogic();
            gwisOperation.SetOption(passModel, "MANULIFE");
        }
        [TestMethod]
        public void TestEpassGwis()
        {
            string remark = @"Ext. PC received, but Missing/Incorrect member name, Please be reminded to submit Statement to certify signature if necessary next time, L/O 2016/6/16 to trustee";
            ConversionExcelDataToList clit = new ConversionExcelDataToList();
            clit.path = @"L:\GZT_RBT\Private\Production\2B\SA Level\2b Logging\Jasmine\Robot\Project Doc\Epass Stage 1 & 2 automation tool.xlsx";
            Dictionary<string, List<ExcelWhlist>> ewhlistdic = new Dictionary<string, List<ExcelWhlist>>();
            OleDbConnection objExcelCon = clit.openExcel();
            ewhlistdic = clit.ExtractEmployeeExcel<ExcelWhlist>(objExcelCon, "White List");
            ExcelWhlist passModel = ewhlistdic["White List"].ToList().Where(x => x.Clean_Case == "N").ToList().FirstOrDefault();
            GwisOperationLogic gwisOperation = new GwisOperationLogic();
            gwisOperation.EpassUpadateGwis(passModel, remark);
        }
    }
}
