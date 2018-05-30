using mshtml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinX.Web;
using WinX.virtualAccount;
using ConceptFlower.Models;
using System.Net;
using ConceptFlower.Extention;

namespace ConceptFlower.BLL
{
    public class GwisOperationLogic
    {



        public string Member { get; set; }


        //mpf main page
        gwis.MpfTerminationPage mpfTerminationPage = new gwis.MpfTerminationPage();

        // actionbar page
        gwis.SearchActionBar actionpage = new gwis.SearchActionBar();

        // Nodcoument search page
        gwis.Nodcoument blankSearch = new gwis.Nodcoument();

        // remark item page 
        gwis.RemarkWorkItem remarkPage = new gwis.RemarkWorkItem();

        // search page
        gwis.Search spage = new gwis.Search();


        int tableIndex = 0;
        public bool SearchCase(TransferCase ts, Dictionary<string, List<TrusteInfoModel>> trusteInfodDic)
        {

            bool find_case = false;
            //if (actionpage.WaitForCreate())
            //{
            //    if (actionpage.btnCancel != null)
            //    {
            //        actionpage.btnCancel.click();
            //    }
            //}

            if (blankSearch.WaitForCreate())
            {
                blankSearch.btnSearch.click();
            }
            WaitForWebPage(spage);

            spage.txtHKID.value = ts.MemHKIDNo + "(" + ts.MemHKIDCheckDigit + ")";

            spage.btnsearch.click();

            WaitForWebPage(spage);
            int index = -1;
            int rowIndex = 0;
            HTMLTable table = spage.tblSearchResult;
            if (table == null)
            {
                throw new Exception("This search has no return result.");
            }
            #region SelectCase
            List<CaseModel> caseList = new List<CaseModel>();
            foreach (HTMLTableRow tr in spage.tblSearchResult.getElementsByTagName("TR"))
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }
                HTMLTableCell FormType = tr.getElementsByTagName("TD").item(2) as HTMLTableCell;
                HTMLTableCell ERID = tr.getElementsByTagName("TD").item(3) as HTMLTableCell;
                HTMLTableCell PCID = tr.getElementsByTagName("TD").item(14) as HTMLTableCell;
                HTMLTableCell WorkQueue = tr.getElementsByTagName("TD").item(16) as HTMLTableCell;
                CaseModel model = new CaseModel { index = rowIndex, FormType = FormType.innerText, ERID = ERID.innerText, PCID = PCID.innerText, WorkQueue = WorkQueue.innerText };
                caseList.Add(model);
                rowIndex++;
            }
            //Tag of Debug comment line
            caseList = caseList.Where(t => t.WorkQueue.Trim() == "SA-03 Processing by SA" || t.WorkQueue.Trim() == "FU-05 Follow-up").ToList();
            if (caseList == null || caseList.Count < 1)
            {
                throw new IgnoreCaseException("no item could be located in related queue.");
            }
            List<CaseModel> selectCase = caseList.Where(t => t.ERID == ts.OriSchAcctMemNo.Split('-')[0]).ToList();
            if (selectCase == null || selectCase.Count < 1)
            {
                var tsinfo = trusteInfodDic["Trustee info"].Where(x => x.Scheme_Registration_No == ts.OriSchRegNo).FirstOrDefault();
                if (tsinfo.Scheme_name.StartsWith("HANG"))
                {
                    if (ts.RequestFormType == "PC")
                    {
                        selectCase = caseList.Where(t => t.FormType == "HAAC").ToList();
                    }
                    else if (ts.RequestFormType == "PM")
                    {
                        selectCase = caseList.Where(t => t.FormType == "HAPM" && t.PCID == "PR01").ToList();
                    }
                }
                else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                {
                    if (ts.RequestFormType == "PC")
                    {
                        selectCase = caseList.Where(t => t.FormType == "INAC").ToList();
                    }
                    else if (ts.RequestFormType == "PM")
                    {
                        selectCase = caseList.Where(t => t.FormType == "INPM" && t.PCID == "PR01").ToList();
                    }
                }
            }
            if (selectCase == null || selectCase.Count < 1)
            {
                if (ts.RequestFormType == "PC")
                {
                    selectCase = caseList;
                }
                else if (ts.RequestFormType == "PM")
                {
                    selectCase = caseList.Where(t => t.PCID == "PR01").ToList();
                }
                //throw new Exception("Can't find item in gwis");
            }
            foreach (CaseModel model in selectCase)
            {
                HTMLTableRow tr = spage.tblSearchResult.getElementsByTagName("TR").item(model.index) as HTMLTableRow;
                HTMLTableCell td = tr.getElementsByTagName("TD").item(1) as HTMLTableCell;
                var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
                Thread.Sleep(2000);
                t.click();
                Thread.Sleep(3000);
                if (CheckGwisDetailInfo(ts))
                {
                    find_case = true;
                    break;
                }
            }
            #endregion

            #region 注释旧逻辑
            //foreach (HTMLTableRow tr in spage.tblSearchResult.getElementsByTagName("TR"))
            //{
            //    if (rowIndex == 0)
            //    {
            //        rowIndex++;
            //        continue;
            //    }
            //    HTMLTableCell ERIDCell = tr.getElementsByTagName("TD").item(3) as HTMLTableCell;
            //    HTMLTableCell WorkQueue = tr.getElementsByTagName("TD").item(16) as HTMLTableCell;
            //    HTMLTableCell FormType = tr.getElementsByTagName("TD").item(2) as HTMLTableCell;
            //    //string a = ERIDCell.innerText;
            //    //string b = WorkQueue.innerText;
            //    if (ERIDCell.innerText.Trim() != ts.OriSchAcctMemNo.Split('-')[0]/*||WorkQueue.innerText.Trim()!= "SA-03 Processing by SA"*/)
            //    {
            //        continue;
            //    }
            //    else
            //    {
            //        HTMLTableCell td = tr.getElementsByTagName("TD").item(1) as HTMLTableCell;
            //        var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
            //        t.click();
            //        if (CheckGwisDetailInfo(ts))
            //        {
            //            find_case = true;
            //            break;
            //        }
            //    }
            //    #region 注释旧逻辑
            //    //foreach (HTMLTableCell td in tr.getElementsByTagName("TD"))
            //    //{
            //    //    //var t = td as HTMLInputElement;
            //    //    index++;
            //    //    if (index == 0)
            //    //    {
            //    //        td.click();
            //    //        string innerHtml = td.innerHTML;
            //    //        var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
            //    //        //var t = td as HTMLInputElement;
            //    //        t.select();
            //    //        t.click();
            //    //    }
            //    //    else if (index == 1)

            //    //    {
            //    //        var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
            //    //        t.click();
            //    //    }
            //    //    // SA-03 Processing by SA 

            //    //    else if (index == 2)
            //    //    {
            //    //        //var t = td as HTMLInputElement;
            //    //        Thread.Sleep(3000);
            //    //        WaitForWebPage(mpfTerminationPage);

            //    //        var optQueueList = mpfTerminationPage.sltWFCKTOCompMatch.getElementsByTagName("option");

            //    //        foreach (var obj in optQueueList)
            //    //        {
            //    //            HTMLOptionElement option = obj as HTMLOptionElement;

            //    //            if (ts.OriSchAcctMemNo.IndexOf('-') != -1)
            //    //            {
            //    //                if (option.innerText == "Matched" &&
            //    //                                                option.selected == true &&
            //    //                                                mpfTerminationPage.txtERIDInfo.value == ts.OriSchAcctMemNo.Split('-')[0] &&
            //    //                                                mpfTerminationPage.txtCaseNumber.value == ts.TransferCaseNo)
            //    //                {
            //    //                    Member = option.innerText;
            //    //                    find_case = true;
            //    //                }
            //    //                else if (option.selected)
            //    //                {
            //    //                    Member = option.innerText;
            //    //                }
            //    //            }
            //    //            else
            //    //            {
            //    //                Member = "invalid" + ts.OriSchAcctMemNo;
            //    //            }

            //    //        }

            //    //        if (!find_case)
            //    //        {
            //    //            actionpage.btnCancel.click();
            //    //            break;
            //    //        }

            //    //        //foreach (var ahref in t.getElementsByTagName("a"))
            //    //        //{

            //    //        //    foreach (var input in td.GetElementsByTagName("input"))
            //    //        //    {

            //    //        //        var ckbox = input as HTMLInputElement;
            //    //        //        ckbox.click();
            //    //        //    }

            //    //        //    var b = a as HTMLAnchorElement;

            //    //        //    var weburl = b.nodeValue();


            //    //        //    XmlAttribute att = b.attributes["href"];

            //    //        //    var url = att.Value;

            //    //        //    var content = GetPageSource(att.Value);

            //    //        //    if (content.IndexOf("") == -1)
            //    //        //    {
            //    //        //        continue;
            //    //        //    }
            //    //        //    else
            //    //        //    {
            //    //        //        b.click();
            //    //        //    }

            //    //        //    Thread.Sleep(3000);
            //    //        //    WaitForWebPage(mpfTerminationPage);

            //    //        //    var optQueueList = mpfTerminationPage.sltWFCKTOCompMatch.getElementsByTagName("option");

            //    //        //    foreach (var obj in optQueueList)
            //    //        //    {
            //    //        //        HTMLOptionElement option = obj as HTMLOptionElement;
            //    //        //        if (option.innerText == "Matched" &&
            //    //        //            option.selected == true &&
            //    //        //            mpfTerminationPage.txtERIDInfo.value == ts.OriSchAcctMemNo.Split('-')[0] &&
            //    //        //            mpfTerminationPage.txtCaseNumber.value == ts.TransferCaseNo)
            //    //        //        {
            //    //        //            find_case = true;
            //    //        //        }
            //    //        //    }
            //    //        //}
            //    //    }

            //    //}
            //    #endregion
            //    
            //}
            #endregion
            return find_case;
        }

        private bool CheckGwisDetailInfo(TransferCase ts)
        {
            bool result = false;
            if (WaitForWebPage(mpfTerminationPage))
            {
                if (mpfTerminationPage.txtCaseNumber.value != ts.TransferCaseNo)
                {
                    result = false;
                    if (WaitForWebPage(actionpage))
                    {
                        actionpage.btnCancel.click();
                    }
                }
                else
                {
                    mpfTerminationPage.btnGet.click();
                    string selectedText = mpfTerminationPage.sltWFCKSVMember.GetSelectedText();
                    Member = selectedText;
                    if (Member == "Matched")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                    if (WaitForWebPage(actionpage))
                    {
                        actionpage.btnSave.click();
                        actionpage.btnCancel.click();
                    }
                }

            }
            return result;
        }

        public string GetPageSource(string URL)
        {
            Uri uri = new Uri(URL);
            HttpWebRequest hwReq = (HttpWebRequest)WebRequest.Create(uri);
            HttpWebResponse hwRes = (HttpWebResponse)hwReq.GetResponse();
            hwReq.Method = "Get";
            hwReq.KeepAlive = false;
            StreamReader reader = new StreamReader(hwRes.GetResponseStream(), System.Text.Encoding.GetEncoding("GB2312"));

            return reader.ReadToEnd().ToString();
        }
        public void SetOption(ExcelWhlist wl, string newTrustee)
        {
            bool find_case = false;
            if (blankSearch.WaitForCreate())
            {
                blankSearch.btnSearch.click();
            }
            WaitForWebPage(spage);

            spage.txtHKID.value = wl.HKID;

            spage.btnsearch.click();

            WaitForWebPage(spage);
            int index = -1;
            
            HTMLTable table = spage.tblSearchResult;
            if (table == null)
            {
                throw new Exception("This search has no return result.");
            }
            bool isContinute=SelectCase(wl.ERID);
            if(isContinute)
            {
                SetOptionPage(newTrustee);
            }
            else
            {

            }
        }
        private void SetOptionPage(string newTrustee)
        {
            Thread.Sleep(2000);
            WaitForWebPage(mpfTerminationPage);

            var optQueueList = mpfTerminationPage.sltProcessType.getElementsByTagName("option");

            foreach (var obj in optQueueList)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "Member Termination")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }

            var Transactionfrom = mpfTerminationPage.sltWFCKTFCompMatch.getElementsByTagName("option");

            foreach (var obj in Transactionfrom)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "Yes")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }

            var TransferOldTrustee = mpfTerminationPage.sltWFCKTrOldTrustee.getElementsByTagName("option");

            foreach (var obj in TransferOldTrustee)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "HSBC")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }



            var Transferto = mpfTerminationPage.sltWFCKTOCompMatch.getElementsByTagName("option");

            foreach (var obj in Transferto)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "Yes")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }


            var newTrusteeoption = mpfTerminationPage.sltWFCKTrNewTrustee.getElementsByTagName("option");

            foreach (var obj in newTrusteeoption)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText.ToUpper() == newTrustee.ToUpper())
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }



            var Validform = mpfTerminationPage.sltWFCKValidForm.getElementsByTagName("option");

            foreach (var obj in Validform)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "Yes")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }
            WaitForWebPage(actionpage);
            foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "PA - Process Authorization")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }
            Task.Run(() => {
                actionpage.btnSend.click();
            });
            Thread.Sleep(1500);
            gwisTips.PASendTips tips = new gwisTips.PASendTips();
            if(tips.WaitForCreate())
            {
                tips.btnOK.Click();
            }
            //else
            //{
            //    Thread.Sleep(2000);
            //    if (tips.IsCreated() && tips.WaitForCreate())
            //    {
            //        tips.btnOK.Click();
            //    }
            //}

        }
        private bool SelectCase(string erID)
        {
            WaitForWebPage(spage);
            int rowIndex = 0;
            List<CaseModel> caseList = new List<CaseModel>();
            foreach (HTMLTableRow tr in spage.tblSearchResult.getElementsByTagName("TR"))
            {
                if (rowIndex == 0)
                {
                    rowIndex++;
                    continue;
                }
                HTMLTableCell FormType = tr.getElementsByTagName("TD").item(2) as HTMLTableCell;
                HTMLTableCell ERID = tr.getElementsByTagName("TD").item(3) as HTMLTableCell;
                HTMLTableCell PCID = tr.getElementsByTagName("TD").item(14) as HTMLTableCell;
                HTMLTableCell WorkQueue = tr.getElementsByTagName("TD").item(16) as HTMLTableCell;
                CaseModel model = new CaseModel { index = rowIndex, FormType = FormType.innerText, ERID = ERID.innerText, PCID = PCID.innerText, WorkQueue = WorkQueue.innerText };
                caseList.Add(model);
                rowIndex++;
            }
            //Tag of Debug comment line
            caseList = caseList.Where(t => t.WorkQueue.Trim() == "SA-03 Processing by SA" || t.WorkQueue.Trim() == "FU-05 Follow-up").ToList();
            if (caseList == null || caseList.Count < 1)
            {
                throw new Exception("no item could be located in related queue.");
            }
            List<CaseModel> selectCase = caseList.Where(t => t.ERID == erID).ToList();
            CaseModel casemodel = selectCase.FirstOrDefault();
            if(casemodel == null)
            {
                return false;
            }
            else
            {
                HTMLTableRow tr = spage.tblSearchResult.getElementsByTagName("TR").item(casemodel.index) as HTMLTableRow;
                HTMLTableCell td = tr.getElementsByTagName("TD").item(1) as HTMLTableCell;
                var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
                Thread.Sleep(2000);
                t.click();
                Thread.Sleep(500);
                return true;
            }
        }
        #region old logic for stage 1&2(run case)
        //public void SetOption(string newTrustee)
        //{
        //    //MANULIFE
        //    Thread.Sleep(3000);
        //    WaitForWebPage(mpfTerminationPage);

        //    var optQueueList = mpfTerminationPage.sltProcessType.getElementsByTagName("option");

        //    foreach (var obj in optQueueList)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "Member termination")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    var Transactionfrom = mpfTerminationPage.sltWFCKTFCompMatch.getElementsByTagName("option");

        //    foreach (var obj in Transactionfrom)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "YES")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    var TransferOldTrustee = mpfTerminationPage.sltWFCKTrOldTrustee.getElementsByTagName("option");

        //    foreach (var obj in TransferOldTrustee)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "HSBC")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }



        //    var Transferto = mpfTerminationPage.sltWFCKTOCompMatch.getElementsByTagName("option");

        //    foreach (var obj in Transferto)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "YES")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }


        //    var newTrusteeoption = mpfTerminationPage.sltWFCKTrNewTrustee.getElementsByTagName("option");

        //    foreach (var obj in newTrusteeoption)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == newTrustee)
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }



        //    var Validform = mpfTerminationPage.sltWFCKValidForm.getElementsByTagName("option");

        //    foreach (var obj in Validform)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "YES")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "FU - Follow-up")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    actionpage.btnSend.click();
        //}
        #endregion



        public void EpassSetRemark(string remark)
        {
            if (WaitForWebPage(actionpage))
            {
                actionpage.btnRemark.click();

                if (WaitForWebPage(remarkPage))
                {
                    var text = remarkPage.RemarkText.FirstOrDefault() as HTMLTextAreaElement;
                    text.value = remark;
                    remarkPage.btnAdd.click();
                    //remarkPage.Close();
                    Thread.Sleep(1500);
                    remarkPage.btnClose.click();
                }
                //foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
                //{
                //    HTMLOptionElement option = obj as HTMLOptionElement;
                //    if (option.innerText == "FU - Follow-up")
                //    {
                //        option.click();
                //        option.selected = true;
                //        break;
                //    }
                //}
                //actionpage.btnSend.click();

                //actionpage.btnCancel.click();
            }
        }


        public void EpassUpadateGwis(ExcelWhlist wl,string Remark)
        {
            bool find_case = false;
            if (blankSearch.WaitForCreate())
            {
                blankSearch.btnSearch.click();
            }
            WaitForWebPage(spage);

            spage.txtHKID.value = wl.HKID;

            spage.btnsearch.click();

            WaitForWebPage(spage);
            int index = -1;
            int rowIndex = 0;
            HTMLTable table = spage.tblSearchResult;
            if (table == null)
            {
                throw new Exception("This search has no return result.");
            }
            bool isContinute = SelectCase(wl.ERID);
            if (isContinute)
            {
                Thread.Sleep(2000);
                EpassSetRemark(Remark);
                SetEpassPageFirst();
                WaitForWebPage(spage);
                spage.txtHKID.value = wl.HKID;
                spage.btnsearch.click();
                SelectCase(wl.ERID);
                SetEpassPageSecond();
                //SetOptionPage(newTrustee);
            }
            else
            {

            }
            #region SelectCase
            //List<CaseModel> caseList = new List<CaseModel>();
            //foreach (HTMLTableRow tr in spage.tblSearchResult.getElementsByTagName("TR"))
            //{
            //    if (rowIndex == 0)
            //    {
            //        rowIndex++;
            //        continue;
            //    }
            //    HTMLTableCell FormType = tr.getElementsByTagName("TD").item(2) as HTMLTableCell;
            //    HTMLTableCell ERID = tr.getElementsByTagName("TD").item(3) as HTMLTableCell;
            //    HTMLTableCell PCID = tr.getElementsByTagName("TD").item(14) as HTMLTableCell;
            //    HTMLTableCell WorkQueue = tr.getElementsByTagName("TD").item(16) as HTMLTableCell;
            //    CaseModel model = new CaseModel { index = rowIndex, FormType = FormType.innerText, ERID = ERID.innerText, PCID = PCID.innerText, WorkQueue = WorkQueue.innerText };
            //    caseList.Add(model);
            //    rowIndex++;
            //}
            ////Tag of Debug comment line
            //caseList = caseList.Where(t => t.WorkQueue.Trim() == "SA-03 Processing by SA" || t.WorkQueue.Trim() == "FU-05 Follow-up").ToList();
            //if (caseList == null || caseList.Count < 1)
            //{
            //    throw new Exception("no item could be located in related queue.");
            //}
            //List<CaseModel> selectCase = caseList.Where(t => t.ERID == wl.ERID).ToList();
            //if (selectCase == null || selectCase.Count < 1)
            //{
            //    //var tsinfo = trusteInfodDic["Trustee info"].Where(x => x.Scheme_Registration_No == ts.OriSchRegNo).FirstOrDefault();
            //    if (wl.Company.StartsWith("HANG"))
            //    {
            //        if (wl.PM_AC == "PC")
            //        {
            //            selectCase = caseList.Where(t => t.FormType == "HAAC").ToList();
            //        }
            //        else if (wl.PM_AC == "PM")
            //        {
            //            selectCase = caseList.Where(t => t.FormType == "HAPM" && t.PCID == "PR01").ToList();
            //        }
            //    }
            //    else if (wl.Company.StartsWith("HSBC"))
            //    {
            //        if (wl.PM_AC == "PC")
            //        {
            //            selectCase = caseList.Where(t => t.FormType == "INAC").ToList();
            //        }
            //        else if (wl.PM_AC == "PM")
            //        {
            //            selectCase = caseList.Where(t => t.FormType == "INPM" && t.PCID == "PR01").ToList();
            //        }
            //    }
            //}
            //if (selectCase == null || selectCase.Count < 1)
            //{
            //    if (wl.PM_AC == "PC")
            //    {
            //        selectCase = caseList;
            //    }
            //    else if (wl.PM_AC == "PM")
            //    {
            //        selectCase = caseList.Where(t => t.PCID == "PR01").ToList();
            //    }
            //    //throw new Exception("Can't find item in gwis");
            //}
            //foreach (CaseModel model in selectCase)
            //{
            //    HTMLTableRow tr = spage.tblSearchResult.getElementsByTagName("TR").item(model.index) as HTMLTableRow;
            //    HTMLTableCell td = tr.getElementsByTagName("TD").item(1) as HTMLTableCell;
            //    var t = td.getElementsByTagName("INPUT").item(0) as HTMLInputElement;
            //    Thread.Sleep(2000);
            //    t.click();
            //    Thread.Sleep(500);
            //    EpassSetRemark(Remark);
            //    Thread.Sleep(1000);
            //    t.click();
            //    Thread.Sleep(500);
            //    //if (CheckGwisDetailInfo(ts))
            //    //{
            //    //    find_case = true;
            //    //    break;
            //    //}
            //}
            #endregion
        }
        public void SetEpassPageFirst()
        {
            Thread.Sleep(1000);
            WaitForWebPage(mpfTerminationPage);

            var optQueueList = mpfTerminationPage.sltProcessType.getElementsByTagName("option");

            foreach (var obj in optQueueList)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "Member Termination")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }

            var Validform = mpfTerminationPage.sltWFCKValidForm.getElementsByTagName("option");

            foreach (var obj in Validform)
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "No, reason:")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }

            WaitForWebPage(actionpage);

            foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "FU - Follow-up")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }
            actionpage.btnSend.click();

            actionpage.btnCancel.click();
        }
        public void SetEpassPageSecond()
        {
            WaitForWebPage(actionpage);

            foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
            {
                HTMLOptionElement option = obj as HTMLOptionElement;
                if (option.innerText == "LA - Letter Authorization")
                {
                    option.click();
                    option.selected = true;
                    break;
                }
            }

            actionpage.btnSend.click();
        }
        #region old pass logic
        //public void EpassUpdateGwis(TransferCase ts)
        //{
        //    bool find_case = false;

        //    Thread.Sleep(3000);
        //    WaitForWebPage(mpfTerminationPage);

        //    var optQueueList = mpfTerminationPage.sltProcessType.getElementsByTagName("option");

        //    foreach (var obj in optQueueList)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "Member termination")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    var Validform = mpfTerminationPage.sltWFCKValidForm.getElementsByTagName("option");

        //    foreach (var obj in Validform)
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "No, reason:")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }



        //    foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "FU - Follow-up")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    actionpage.btnSend.click();

        //    actionpage.btnCancel.click();

        //    WaitForWebPage(spage);

        //    int index = -1;

        //    foreach (HTMLLIElement tr in spage.tblSearchResult.getElementsByTagName("TR"))
        //    {
        //        tableIndex++;

        //        foreach (var td in tr.getElementsByTagName("TD"))
        //        {
        //            //var t = td as HTMLInputElement;
        //            index++;
        //            if (index == 0)
        //            {
        //                var t = td as HTMLInputElement;
        //                t.select();
        //                t.click();
        //            }
        //            else if (index == 1)
        //            {
        //                var t = td as HTMLAnchorElement;
        //                t.click();
        //            }
        //            else if (index == 2)
        //            {

        //                Thread.Sleep(3000);
        //                WaitForWebPage(mpfTerminationPage);

        //                var optQueueList1 = mpfTerminationPage.sltWFCKTOCompMatch.getElementsByTagName("option");

        //                foreach (var obj in optQueueList1)
        //                {
        //                    HTMLOptionElement option = obj as HTMLOptionElement;
        //                    if (option.innerText == "Matched" &&
        //                        option.selected == true &&
        //                        mpfTerminationPage.txtERIDInfo.value == ts.OriSchAcctMemNo.Split('-')[0] &&
        //                        mpfTerminationPage.txtCaseNumber.value == ts.TransferCaseNo)
        //                    {
        //                        find_case = true;
        //                    }
        //                }

        //                if (!find_case)
        //                {
        //                    actionpage.btnCancel.click();
        //                    break;
        //                }
        //            }
        //        }
        //    }

        //    WaitForWebPage(mpfTerminationPage);

        //    foreach (var obj in actionpage.sltNpi.FirstOrDefault().getElementsByTagName("option"))
        //    {
        //        HTMLOptionElement option = obj as HTMLOptionElement;
        //        if (option.innerText == "FU - Follow-up")
        //        {
        //            option.click();
        //            option.selected = true;
        //            break;
        //        }
        //    }

        //    actionpage.btnSend.click();

        //}
        #endregion


        private bool WaitForWebPage(WebScreen page, int timeInterval = 5, int times = 1)
        {
            bool flag = (page.IsCreated() || page.WaitForCreate(5)) && page.WaitForLoadComplete(20);
            int index = 1;
            while (!flag && index < times)
            {
                flag = page.IsCreated() || page.WaitForCreate(timeInterval);
                index++;
            }
            return flag;
        }

    }
    class CaseModel
    {
        public int index { get; set; }
        public string FormType { get; set; }
        public string ERID { get; set; }
        public string PCID { get; set; }
        public string WorkQueue { get; set; }
    }
}
