using ConceptFlower.Command;
using ConceptFlower.Log;
using ConceptFlower.Models;
using ConceptFlower.Static;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Forms;
using PcommCore.Common;
using PcommCore.Screen;
using PcommCore;
using PcommCore.Interface;
using System.Data.OleDb;
using System.Windows.Media;
using WinX.virtualAccount;
using System.Data;
using mshtml;
using System.Net;
using System.IO;
using System.Xml;
using ConceptFlower.BLL;
using System.Text.RegularExpressions;
using ConceptFlower.Extention;
using System.Collections;

namespace ConceptFlower.ViewModel
{
    public class ConceptFlowerViewModel : ViewModelBase
    {

        NewRequest slidt = new NewRequest();


        // keyword Dictionary
        Dictionary<string, List<string>> notepadDic = new Dictionary<string, List<string>>();
        //trusteInfo
        Dictionary<string, List<TrusteInfoModel>> trusteInfodDic = new Dictionary<string, List<TrusteInfoModel>>();
        // White List
        //Dictionary<string ,List<WhiteListcs>> whlistdic = new Dictionary<string, List<WhiteListcs>>();

        Dictionary<string, List<ExcelWhlist>> ewhlistdic = new Dictionary<string, List<ExcelWhlist>>();

        // gwis object
        ExcelEditor excelHandel = new ExcelEditor();
        ObservableCollection<ProcessMsg> _logList = new ObservableCollection<ProcessMsg>();
        ObservableCollection<CheckResult> _resultList = new ObservableCollection<CheckResult>();

        // oled read excel data object 
        ConversionExcelDataToList clit = new ConversionExcelDataToList();

        List<WhiteListcs> wlist = new List<WhiteListcs>();
        GwisOperationLogic gwisOperation = new GwisOperationLogic();

        AS400OperationLogic as400logic = new AS400OperationLogic();

        PcommCore.PcommCore pcommCore = new PcommCore.PcommCore("A");

        RelayCommand<object> _readXmlCommand = null;

        RelayCommand<object> _readexcelCommand = null;

        RelayCommand<object> _checkCaseCommand = null;

        RelayCommand<object> _runCaseCommand = null;

        RelayCommand<object> _epassCaseCommand = null;

        ProcessMsg p = new ProcessMsg();
        public ConceptFlowerViewModel()
        {
            LogHelper.BindingProcess(_logList);
            //IsRunEnable = true;
        }

        private bool _isEpassEnable = true;


        public bool IsEpassEnable
        {

            get
            {
                return _isEpassEnable;
            }

            set
            {
                if (_isEpassEnable != value)
                {
                    _isEpassEnable = value;

                    this.RaisePropertyChanged(() => IsEpassEnable);
                }
            }
        }

        private bool _ischeckEnable = true;


        public bool IscheckEnable
        {
            get
            {
                return _ischeckEnable;
            }
            set
            {

                if (_ischeckEnable != value)
                {
                    _ischeckEnable = value;

                    this.RaisePropertyChanged(() => IscheckEnable);
                }
            }
        }

        private bool _isRunEnable = true;

        public bool IsRunEnable
        {

            get
            {
                return _isRunEnable;
            }

            set
            {

                if (_isRunEnable != value)
                {
                    _isRunEnable = value;

                    this.RaisePropertyChanged(() => IsRunEnable);
                }
            }
        }


        public new ICommand CheckCaseCommand
        {
            get
            {
                if (_checkCaseCommand == null)

                    _checkCaseCommand = new RelayCommand<object>((p) => this.CheckCase());

                return _checkCaseCommand;
            }
        }

        public static ManualResetEvent mre = null;


        private List<TransferCase> TestList(List<TransferCase> list)
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
            List<TransferCase> resultList = new List<TransferCase>();
            arrayList.Sort();
            foreach (int index in arrayList)
            {
                resultList.Add(list[index]);
            }
            return resultList;
        }

        private async void CheckCase()
        {
            ProcessLogProxy.Debug("Start Checking Xml Data", "Green", 1);
            wlist = new List<WhiteListcs>();
            if (slidt.Header == null && slidt.TransferCase == null)
            {
                MessageBox.Show("xml data not to load");

                return;
            }

            if (slidt.TransferCase.Count == 0)
            {
                MessageBox.Show("no xml data to load");

                return;
            }

            var p = slidt.TransferCase.FirstOrDefault();
            ObservableCollection<ProcessMsg> Logtmp = new ObservableCollection<ProcessMsg>();
            List<string> keywords = notepadDic["Notepad"];
            List<UserInfoSchemeInfo> schemeInfoList = GetSchemeList();
            // gwis operation
            pcommCore.SkipToHomeScreen<S0017>();
            await STATask.Run(() =>
            {
                try
                {
                    int index = 1;
                    List<TransferCase> runList = slidt.TransferCase.Where(t => t.RequestFormType == "PC" || t.RequestFormType == "PM").ToList();
                    if (slidt.TransferCase.Count() > 20)
                    {
                        runList = TestList(slidt.TransferCase);
                    }
                    //foreach (var cs in slidt.TransferCase)
                    foreach (var cs in runList)
                    {
                        ProcessLogProxy.Debug(cs.TransferCaseNo + " is checking", "Green", 1);
                        WhiteListcs whlist = new WhiteListcs();
                        whlist.Clean_Case = "Y";
                        whlist.Case_Number = cs.TransferCaseNo;
                        whlist.PM_AC = cs.RequestFormType;
                        //   bug  whlist.Membership_No = cs.OriSchAcctMemNo;
                        whlist.Name_in_XML = cs.MemEngName;
                        whlist.HKID = cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")";
                        SH795AccoutModel acModel = null;
                        #region 下载图片到excel
                        string filePath = Path.GetDirectoryName(ExcelPath) + @"\Image\" + cs.TransferCaseNo + ".jpg";
                        Util.Base64SaveFlie(cs.FormImageCode, filePath);
                        whlist.Addr_In_Form = filePath;
                        #endregion
                        try
                        {
                            if (!gwisOperation.SearchCase(cs, trusteInfodDic))
                            {
                                whlist.Signature_Pass = gwisOperation.Member;
                                if (gwisOperation.Member != null && gwisOperation.Member.Trim() == "Unmatched")
                                {

                                    whlist.ErrorCode.Append("6,");
                                }
                                else if (gwisOperation.Member != null && gwisOperation.Member.Trim() == "No MIP")
                                {
                                    whlist.ErrorCode.Append("7,");
                                }
                                ProcessLogProxy.Debug("Can't find item in gwis", "Red", 3);
                                ProcessLogProxy.Debug("This case has been checked in error.", "Red", 1);
                                continue;
                            }
                            else
                            {
                                whlist.Signature_Pass = gwisOperation.Member;
                                bool isSelectCase = false;
                                //whlist.Name_Pass = gwisOperation.Member;

                                var tsinfo = trusteInfodDic["Trustee info"].Where(x => x.Scheme_Registration_No == cs.OriSchRegNo).FirstOrDefault();
                                pcommCore.SkipToHomeScreen<S0017>();
                                pcommCore.LinkToScreen<S0017>((S0017) =>
                                {
                                    ProcessLogProxy.Debug("SOO17 screeen Set Company", "Green", 1);
                                    string companyNum = string.Empty;
                                    //(cs.OriSchAcctMemNo.Split('-'))[0].ToCharArray().FirstOrDefault().ToString();

                                    if (tsinfo.Scheme_name.StartsWith("HANG"))
                                    {
                                        whlist.Company = "HANG";
                                        companyNum = "3";
                                    }
                                    else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                                    {
                                        whlist.Company = "HSBC";
                                        companyNum = "2";
                                    }

                                    S0017.SetCompany(companyNum);
                                    return S0017.GotoMemberDetails();
                                }).LinkToScreen<S0018>((S0018) =>
                                {
                                    ProcessLogProxy.Debug("S0018 screeen Go to MemberSchemeMaint", "Green", 1);
                                    return S0018.GotoMemberSchemeMaint();
                                }).LinkToScreen<SM794>((SM794) =>
                                {
                                    ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                                    SM794.SetIdentifier(cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")");
                                    SM794.SetSelectOption("D");
                                    ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                                    SM794.SM794Enter();
                                    return true;

                                }).LinkToScreen<SH795>((SH795) =>
                                {
                                    List<string> schemeList = new List<string>();
                                    List<string> clientList = new List<string>();
                                    isSelectCase = CkeckOriSchAcctMemNo(cs, whlist, tsinfo, SH795, schemeInfoList, out acModel);
                                    SH795.Message.ToList().ForEach(x =>
                                        {
                                            whlist.Unclean_Reason.Append(x + "\r\n");
                                        });

                                    return true;
                                });
                                if (!isSelectCase)
                                {
                                    throw new IgnoreCaseException("Can't select case in SH795");
                                }
                                // go to SM799
                                SM799 sm799 = pcommCore.GetScreen<SM799>();
                                var dic = sm799.GetMemberInformation();
                                try
                                {
                                    CheckCaseDetail(dic, cs, whlist, sm799);
                                }
                                catch
                                {
                                    throw new Exception("An unknown error occurred in SH799 or SH800");
                                }
                                // return SM799 Screen
                                SM799 s_sm7799 = pcommCore.GetScreen<SM799>();
                                s_sm7799.Set_ShiftF8();
                                try
                                {
                                    SJ671 sj671 = pcommCore.GetScreen<SJ671>();
                                    sj671.SetStoAll();
                                    sj671.ClearSchemeID();
                                    sj671.SendKey(KeyBoard.Enter);
                                    if (sj671.NotePadPass(keywords, pcommCore))
                                    {
                                        whlist.Notepad_Pass = "Y";
                                        pcommCore.SkipToHomeScreen<S0017>();

                                    }
                                    else
                                    {
                                        ProcessLogProxy.Debug("sj672 exsit key word ", "Red", 3);
                                        whlist.Notepad_Pass = "N";
                                        whlist.Unclean_Reason.Append("Notepad unclean\r\n");
                                    }

                                    sj671.Message.ToList().ForEach(x =>
                                    {

                                        whlist.Unclean_Reason.Append(x + "\r\n");

                                    });
                                }
                                catch
                                {
                                    throw new Exception("An unknown error occurred in sj671");
                                }
                                if (cs.RequestFormType == "PM" && cs.TransferMCAndVC.ToUpper() == "WithDraw".ToUpper())
                                {
                                    if (IsNTWithDraw(pcommCore, cs, whlist, acModel))
                                    {
                                        whlist.Withdraw_NT_PM = "Y";
                                    }

                                }
                            }
                            #region whlist ErrorCode去重调整格式
                            if (!string.IsNullOrEmpty(whlist.ErrorCode.ToString()))
                            {
                                List<string> errorCodeList = whlist.ErrorCode.ToString().Split(',').ToList();
                                errorCodeList = errorCodeList.Where(t => t != "").ToList();
                                errorCodeList = errorCodeList.Distinct().ToList();
                                string erroCode = string.Empty;
                                foreach (string code in errorCodeList)
                                {
                                    erroCode = erroCode == "" ? code : erroCode + "," + code;
                                }
                                whlist.ErrorCode = new StringBuilder(erroCode);

                            }
                            #endregion
                            wlist.Add(whlist);
                        }
                        catch (IgnoreCaseException ex)
                        {
                            whlist.Clean_Case = "N/A";
                            whlist.Unclean_Reason.Append(ex.Message + "\r\n");
                            #region whlist ErrorCode去重调整格式
                            if (!string.IsNullOrEmpty(whlist.ErrorCode.ToString()))
                            {
                                List<string> errorCodeList = whlist.ErrorCode.ToString().Split(',').ToList();
                                errorCodeList = errorCodeList.Where(t => t != "").ToList();
                                errorCodeList = errorCodeList.Distinct().ToList();
                                string erroCode = string.Empty;
                                foreach (string code in errorCodeList)
                                {
                                    erroCode = erroCode == "" ? code : erroCode + "," + code;
                                }
                                whlist.ErrorCode = new StringBuilder(erroCode);

                            }
                            #endregion
                            wlist.Add(whlist);
                            ProcessLogProxy.Debug(ex.Message, "Red", 3);
                            ProcessLogProxy.Debug("This case has been checked in error.", "Red", 1);
                            continue;
                        }
                        catch (Exception ex)
                        {
                            whlist.Clean_Case = "N";
                            whlist.Unclean_Reason.Append(ex.Message + "\r\n");
                            #region whlist ErrorCode去重调整格式
                            if (!string.IsNullOrEmpty(whlist.ErrorCode.ToString()))
                            {
                                List<string> errorCodeList = whlist.ErrorCode.ToString().Split(',').ToList();
                                errorCodeList = errorCodeList.Where(t => t != "").ToList();
                                errorCodeList = errorCodeList.Distinct().ToList();
                                string erroCode = string.Empty;
                                foreach (string code in errorCodeList)
                                {
                                    erroCode = erroCode == "" ? code : erroCode + "," + code;
                                }
                                whlist.ErrorCode = new StringBuilder(erroCode);

                            }
                            #endregion
                            wlist.Add(whlist);
                            ProcessLogProxy.Debug(ex.Message, "Red", 3);
                            ProcessLogProxy.Debug("This case has been checked in error.", "Red", 1);
                            continue;
                        }
                        ProcessLogProxy.Debug("This case has been checked out.", "Green", 1);
                    }

                    ProcessLogProxy.Debug("xml data check completed ", "Green", 1);
                    pcommCore.SkipToHomeScreen<S0017>();


                    if (wlist.Count > 0)
                    {
                        var excel = excelHandel.Open(this.ExcelPath);

                        DataTable dt = ListToDatatableHelper.ToDataTable<WhiteListcs>(wlist);

                        excel.SaveDataTableToExcel(dt, this.ExcelPath, "White List");
                    }

                }
                catch (Exception e)
                {
                    ProcessLogProxy.Debug(e.Message, "Red", 3);
                }
            });

            IsEpassEnable = true;
            IsRunEnable = true;

        }

        private List<UserInfoSchemeInfo> GetSchemeList()
        {
            OleDbConnection objExcelCon = clit.openExcel();
            List<TrusteInfoModel> trusteeInfos = clit.GetExcelDataBySheetName<TrusteInfoModel>(objExcelCon, "Trustee info");
            List<UserInfoSchemeInfo> list = new List<UserInfoSchemeInfo>();
            object[,] datas = clit.GetExcelDataByRange("Useful info", "A2", "C5");
            for (int i = 1; i <= datas.GetLength(0); i++)
            {
                string schemeCode = datas[i, 1].ToString();
                TrusteInfoModel trusteeInfo = trusteeInfos.Where(t => t.Scheme_code == schemeCode).FirstOrDefault();
                string CaseNO = string.Empty;
                if (trusteeInfo != null)
                {
                    CaseNO = trusteeInfo.Scheme_Registration_No;
                }
                UserInfoSchemeInfo model1 = new UserInfoSchemeInfo { SchemeCode = datas[i, 1].ToString(), SchemeID = datas[i, 2].ToString(), CaseNO = CaseNO, IsFirst = false };
                UserInfoSchemeInfo model2 = new UserInfoSchemeInfo { SchemeCode = datas[i, 1].ToString(), SchemeID = datas[i, 3].ToString(), CaseNO = CaseNO, IsFirst = true };
                list.Add(model1);
                list.Add(model2);
            }
            return list;
        }

        private bool CkeckOriSchAcctMemNo(TransferCase cs, WhiteListcs whlist, TrusteInfoModel tsinfo, SH795 sh795, List<UserInfoSchemeInfo> schemeInfoList, out SH795AccoutModel accountModel)
        {
            //获取list实体
            List<SH795AccoutModel> entityList = new List<SH795AccoutModel>();
            //string SchemeID = "";
            //string MembershipNo = "";
            SH795AccoutModel acModel = null;
            try
            {
                entityList = sh795.GetAccountList(entityList, 1);
                List<SH795AccoutModel> entityListPR01 = entityList.Where(t => t.PayCID.Trim() == "PR01").ToList();
                List<SH795AccoutModel> entityListOther = entityList.Where(t => t.PayCID.Trim() != "PR01").ToList();
                try
                {
                    //补全SchmeID和 MemberShipNo
                    acModel = SelectCaseFactory.Getcase(cs, entityListPR01, whlist, tsinfo, schemeInfoList);
                    if (acModel == null)
                    {
                        accountModel = acModel;
                        return false;
                    }
                    else
                    {
                        SelectCaseBySingleAC(cs, whlist, acModel, tsinfo, entityList, sh795);
                    }
                }
                catch (AccountException ex)
                {
                    if (ex.Message == "Please Check Value Choice")
                    {
                        bool isSelected = SelectCaseByACList(cs, whlist, tsinfo, entityListOther, sh795);
                        if (!isSelected)
                        {
                            if (tsinfo.Scheme_name.StartsWith("HANG"))
                            {
                                acModel = entityList.Where(t => t.PayCID == "PR01" && t.SchemeID.StartsWith("3")).FirstOrDefault();
                            }
                            else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                            {
                                acModel = entityList.Where(t => t.PayCID == "PR01" && t.SchemeID.StartsWith("2")).FirstOrDefault();
                            }
                            if (acModel != null)
                            {
                                whlist.Clean_Case = "N";
                                whlist.Unclean_Reason.Append("Missing/Incorrect Orig Scheme Reg No (Scheme name) \r\n");
                                whlist.ErrorCode.Append("4,");
                                SelectCaseBySingleAC(cs, whlist, acModel, tsinfo, entityList, sh795);

                            }
                            else
                            {
                                accountModel = acModel;
                                return false;
                            }
                        }
                        else
                        {
                            whlist.Clean_Case = "N";
                            whlist.ErrorCode.Append("32,");
                            whlist.Unclean_Reason.Append("Sec.II not SelfEmp or Personal or Cont a/c for trf.\r\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " in SH795");
            }
            accountModel = acModel;
            return true;
        }

        private bool SelectCaseByACList(TransferCase cs, WhiteListcs whlist, TrusteInfoModel tsinfo, List<SH795AccoutModel> entityList, SH795 sh795)
        {
            bool isFindCase = false;
            sh795.SendKey(KeyBoard.PF3);
            pcommCore.LinkToScreen<SM794>((SM794) =>
            {
                ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                SM794.SetIdentifier(cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")");
                SM794.SetSelectOption("D");
                ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                SM794.SM794Enter();
                return true;

            }).LinkToScreen<SH795>((SH795Screen) =>
            {

                foreach (SH795AccoutModel acModel in entityList)
                {
                    if (SH795Screen.SelectCase(acModel))
                    {
                        SM799 sm799 = pcommCore.GetScreen<SM799>();
                        string title = sm799.GetText(3, 58, 20).Trim();
                        if (title.ToLower() == "value choice")
                        {
                            isFindCase = true;
                            if (acModel.Sts == "TE")
                            {
                                whlist.ErrorCode.Append("9,");
                            }
                            break;
                        }
                        else
                        {
                            sh795.SendKey(KeyBoard.PF3);
                            Thread.Sleep(500);
                            SM794 sm794 = pcommCore.GetScreen<SM794>();
                            sm794.SendKey(KeyBoard.PF3);
                            Thread.Sleep(500);
                            pcommCore.LinkToScreen<S0018>((S0018) =>
                            {
                                ProcessLogProxy.Debug("S0018 screeen Go to MemberSchemeMaint", "Green", 1);
                                return S0018.GotoMemberSchemeMaint();
                            }).LinkToScreen<SM794>((SM794) =>
                            {
                                ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                                SM794.SetIdentifier(cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")");
                                SM794.SetSelectOption("D");
                                ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                                SM794.SM794Enter();
                                return true;

                            });
                        }
                    }
                    Thread.Sleep(1000);
                }
                return true;
            });
            return isFindCase;
        }

        private void SelectCaseBySingleAC(TransferCase cs, WhiteListcs whlist, SH795AccoutModel acModel, TrusteInfoModel tsinfo, List<SH795AccoutModel> entityList, SH795 sh795)
        {
            if ((acModel.Client.StartsWith("2") && tsinfo.Scheme_name.StartsWith("HANG")) || (acModel.Client.StartsWith("3") && tsinfo.Scheme_name.StartsWith("HSBC")))
            {
                whlist.Clean_Case = "N";
                whlist.Unclean_Reason.Append("Member Account or ERID is Error, pls input correct whitelsit \r\n");
                whlist.ErrorCode.Append("4,");
                //return true;
            }
            if (acModel.Sts == "TE")
            {
                whlist.Clean_Case = "N";
                whlist.Unclean_Reason.Append("Member Account or ERID is Error, pls input correct whitelsit \r\n");
                whlist.ErrorCode.Append("9,");
                //return false;
            }
            //根据SchemeID和MemberShipNO来进入Case
            sh795.SendKey(KeyBoard.PF3);
            pcommCore.LinkToScreen<SM794>((SM794) =>
            {
                ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                SM794.SetIdentifier(cs.MemHKIDNo);
                SM794.SetSelectOption("D");
                ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                SM794.SM794Enter();
                return true;

            }).LinkToScreen<SH795>((SH795Screen) =>
            {
                SH795Screen.SelectCase(acModel);
                return true;
            });
            // PM PC ERID blank 
            if (cs.RequestFormType.ToUpper() == "PC" || cs.RequestFormType.ToUpper() == "PM")
            {
                whlist.ERID = acModel.SchemeID;
                whlist.Membership_No = acModel.MembershipNoInXml;
            }
            else
            {
                whlist.Clean_Case = "N";
            }
        }

        private void CheckCaseDetail(Dictionary<string, string> dic, TransferCase cs, WhiteListcs whlist, SM799 sm799)
        {

            string SM799Tips = sm799.GetText(2, 50, 29).Trim();
            if (SM799Tips.Contains("MPFA"))
            {
                whlist.Clean_Case = "N";
                whlist.Unclean_Reason.Append(SM799Tips + "/r/n");
            }
            string sensitiveFlag = sm799.GetText(4, 77, 2).Trim();
            if (!string.IsNullOrEmpty(sensitiveFlag))
            {
                whlist.Clean_Case = "N";
                whlist.Unclean_Reason.Append(sm799.GetText(4, 60, 20).Trim());
            }

            var id = cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")";
            //whlist.HKID = id;

            var englishName = dic["SURNAME"] + " " + dic["FIRATNAME"];

            whlist.Name_in_AS400 = englishName;

            whlist.Name_in_XML = cs.MemEngName;


            if (dic["BANKRUPTCY"] == "N" &&
                cs.MemEngName.ToUpper() == englishName.ToUpper() &&
                id == dic["IDNO"].ToString())
            {
                //whlist.Clean_Case = "Y";
                whlist.Name_Pass = "Matched";
                ProcessLogProxy.Debug("SM799 compare with xml data ", "Green", 1);
                sm799.SetX();
                sm799.SendKey(KeyBoard.Enter);
            }
            else
            {
                if (id != dic["IDNO"].ToString())
                {
                    whlist.ErrorCode.Append("1,");
                    ProcessLogProxy.Debug("SM799 screen IDNO is error", "Red", 3);
                }
                if (cs.MemEngName != englishName)
                {
                    whlist.Name_Pass = "Unmatched";
                    whlist.ErrorCode.Append("2,");
                    ProcessLogProxy.Debug("SM799 screen name is error", "Red", 3);
                }
                whlist.Clean_Case = "N";
                //if (cs.RequestFormType == "PM")
                //{
                //    if (cs.TransferMCAndVC == "ALL" && IsNTWithDraw(pcommCore, cs))
                //    {
                //    //    whlist.ErrorCode.Append("14");
                //    //    whlist.Withdraw_NT_PM = " member has NT balance but XML file choose ALL instead of withdraw ";
                //    }
                //}
                //whlist.Unclean_Reason.Append("SM799 screen " + dic["MPFCOMPLANT"].ToString() + dic["SENSFLAG"].ToString() + "unmatched \r\n");

                //ProcessLogProxy.Debug("SM799 screen " + dic["MPFCOMPLANT"].ToString() + dic["SENSFLAG"].ToString(), "Red", 3);
                sm799.SetX();
                sm799.SendKey(KeyBoard.Enter);
            }

            SM800 sm800 = pcommCore.GetScreen<SM800>();

            ProcessLogProxy.Debug("SM800  get Chinese Name  ", "Green", 1);
            string chineseNameInAs400 = sm800.GetMemberChineseInformation()["CHINESENAME"].ToString().Trim();
            if (chineseNameInAs400 == cs.MemChiName.Trim()
                || string.IsNullOrEmpty(cs.MemChiName.Trim())
                || string.IsNullOrEmpty(chineseNameInAs400))
            {
                sm800.SendKey(KeyBoard.Enter);
            }
            else
            {
                whlist.Name_Pass = "Unmatched";
                whlist.Clean_Case = "N";
                whlist.Unclean_Reason.Append("SM800 screen  Chinese Name unmatched" + sm800.GetMemberChineseInformation()["CHINESENAME"].ToString() + "\r\n");
                whlist.ErrorCode.Append("2,");
                ProcessLogProxy.Debug("SM800 screen  Chinese Name unmatched" + sm800.GetMemberChineseInformation()["CHINESENAME"].ToString(), "Red", 3);
                sm800.SendKey(KeyBoard.Enter);
            }
        }
        public bool IsNTWithDraw(PcommCore.PcommCore pcommCore, TransferCase cs, WhiteListcs whlist, SH795AccoutModel acModel)
        {

            bool isWithDraw = false;

            pcommCore.SkipToHomeScreen<S0017>();

            S0017 s0017 = pcommCore.GetScreen<S0017>();

            s0017.GotoMemberDetails();

            S0018 s0018 = pcommCore.GetScreen<S0018>();
            s0018.GotoUnitMovements();

            Thread.Sleep(1000);
            CommonScreen SN521 = pcommCore.GetScreen<CommonScreen>();
            while (SN521.GetText(1, 72, 5) != "SN521")
            {
                Thread.Sleep(1000);
                SN521 = pcommCore.GetScreen<CommonScreen>();
            }
            SN521.SetText(whlist.HKID, 11, 12);
            SN521.SendKey(KeyBoard.Enter);

            SH795 sh795 = pcommCore.GetScreen<SH795>();
            sh795.SelectCase(acModel);

            SH655 sh655 = pcommCore.GetScreen<SH655>();

            isWithDraw = sh655.IsNTmatched();

            return isWithDraw;
        }

        public new ICommand RunCaseCommand
        {
            get
            {
                if (_runCaseCommand == null)

                    _runCaseCommand = new RelayCommand<object>((p) => this.RunCase());

                return _runCaseCommand;
            }
        }


        private async void RunCase()
        {
            ProcessLogProxy.Debug("Start Stage 1&2", "Green", 1);
            OleDbConnection objExcelCon = clit.openExcel();
            ewhlistdic = clit.ExtractEmployeeExcel<ExcelWhlist>(objExcelCon, "White List");
            List<TrusteInfoModel> trusteeInfos = clit.GetExcelDataBySheetName<TrusteInfoModel>(objExcelCon, "Trustee info");
            objExcelCon.Close();
            try
            {
                await STATask.Run(() =>
                {

                    var passList = ewhlistdic["White List"].ToList().Where(x => x.Clean_Case == "Y"
                    &&(string.IsNullOrEmpty(x.Process_result)||x.Process_result== ProcessResult.Result3)).ToList();
                    pcommCore.SkipToHomeScreen<S0017>();
                    foreach (var wl in passList)
                    {
                        ProcessLogProxy.Debug(wl.Case_Number + " starts Satge 1", "Green", 1);
                        var sl = slidt.TransferCase.Where(x => x.TransferCaseNo == wl.Case_Number).FirstOrDefault();

                        if (wl.Clean_Case == "Y" && wl.Case_Number == sl.TransferCaseNo)
                        {
                            pcommCore.LinkToScreen<S0017>((S0017) =>
                            {
                                ProcessLogProxy.Debug("SOO17 screeen Set Company", "Green", 1);
                                //S0017.SetCompany((sl.OriSchAcctMemNo.Split('-'))[0].ToCharArray().FirstOrDefault().ToString());
                                if (wl.Company == "HSBC")
                                {
                                    S0017.SetCompany("2");
                                }
                                else
                                {
                                    S0017.SetCompany("3");
                                }
                                return S0017.GotoMemberDetails();
                            }).LinkToScreen<S0018>((S0018) =>
                            {
                                ProcessLogProxy.Debug("S0018 screeen Go to MemberDetails", "Green", 1);
                                return S0018.GotoMemberTerminations();
                            });
                            SN008 sn008 = pcommCore.GetScreen<SN008>();
                            ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                            sn008.setScheme(wl.ERID);
                            //sn008.setID(sl.MemHKIDNo);
                            sn008.setID(wl.HKID);
                            sn008.setOption();
                            sn008.setEnter();

                            Thread.Sleep(1000);
                            string WarningInSN008 = sn008.GetMessage();
                            //Warning Tips
                            if (WarningInSN008.ToUpper().Contains("Discrepancy update lock".ToUpper()))
                            {
                                pcommCore.SkipToHomeScreen<S0017>();
                                wl.Process_result = ProcessResult.Result3;
                                continue;
                            }
                            ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                            SN010 sn010 = pcommCore.GetScreen<SN010>();
                            if (sn010.GetErrorCode().Contains("Invalid member status"))
                            {
                                ProcessLogProxy.Debug("SN010 Screen Invalid member status", "Red", 3);
                                continue;
                            }
                            if (!string.IsNullOrEmpty(sn010.getClaimLodgmentDate()))
                            {
                                sn010.setNotificationDate(sn010.getClaimLodgmentDate());
                                sn010.setTerminationDate(sn010.getClaimLodgmentDate());
                                sn010.setTerminationReason();
                                sn010.setX();
                                sn010.SetAutomatedRollover();
                                sn010.SetEnter();
                            }
                            else
                            {
                                ProcessLogProxy.Debug("SN010 Screen ClaimLodgmentDate is blank", "Red", 3);
                                continue;
                            }
                            SJ452 sj452 = pcommCore.GetScreen<SJ452>();
                            if (sl.RequestFormType == "PM")
                            {
                                if (!string.IsNullOrEmpty(sj452.Get_PM_RecDate()))
                                {
                                    sj452.SetCcDate(sj452.Get_PM_RecDate());
                                    sj452.set_PM_AC();
                                    sj452.SendKey(KeyBoard.Enter);
                                    //if (sj452.set_Enter_confirm())
                                    //{
                                    //    sj452.set_Enter();
                                    //}
                                    //else
                                    //{
                                    //    ProcessLogProxy.Debug("SJ452 Screen can't find [Press <ENTER> to confirm|] ", "Red", 3);
                                    //   // continue;
                                    //}
                                }
                                else
                                {
                                    ProcessLogProxy.Debug("SN010 Screen PM's Rec date is blank ", "Red", 3);
                                    continue;
                                }
                            }
                            else if (sl.RequestFormType == "PC")
                            {
                                if (!string.IsNullOrEmpty(sj452.Get_AC_RecDate()))
                                {
                                    sj452.Set_AC_CCDate(sj452.Get_AC_RecDate());
                                    sj452.set_PM_AC();
                                    if (sj452.set_Enter_confirm())
                                    {
                                        sj452.SendKey(KeyBoard.Enter);
                                    }
                                    else
                                    {
                                        ProcessLogProxy.Debug("SJ452 Screen can't find [Press <ENTER> to confirm|] ", "Red", 3);
                                        continue;
                                    }
                                }
                                else
                                {
                                    ProcessLogProxy.Debug("SN010 Screen AC's Rec date is blank ", "Red", 3);
                                    continue;
                                }
                            }
                            else
                            {
                                ProcessLogProxy.Debug("SN010 Screen RequestFormType is unknown ", "Red", 3);
                                continue;
                            }
                            SN010 sn0101 = pcommCore.GetScreen<SN010>();
                            sn0101.SendKey(KeyBoard.Enter);
                            Thread.Sleep(5000);
                            bool isSkiped = false;
                            while (!isSkiped)
                            {
                                isSkiped = Stage2Skiped();
                            }
                            ProcessLogProxy.Debug(wl.Case_Number + " starts Satge 2", "Green", 1);
                            SN008 sn0081 = pcommCore.GetScreen<SN008>();
                            if (sn0081.GetMessage() == "Last transaction processed")
                            {
                                sn0081.setOption("D");
                                sn0081.setEnter();
                                SN012 sn012 = pcommCore.GetScreen<SN012>();

                                if (wl.PM_AC == "PM" && wl.Withdraw_NT_PM == "Y")
                                {
                                    sn012.Set_Payment_Option("3");
                                }
                                else
                                {
                                    sn012.Set_Payment_Option("2");
                                }
                                sn012.Set_Case_Number(sl.TransferCaseNo);
                                sn012.Set_Employer_Scheme_Transfer("N");

                                sn012.Set_Withdrawal_Ground("NC");
                                sn012.SetEnter();
                                if (sn012.getErrorMessage().Trim().Contains("Unmatched with NEWD"))
                                {
                                    sn012.SendKey(KeyBoard.Enter);
                                }
                                if (sn012.getErrorMessage() == "Product ID’s don’t match")
                                {
                                    ProcessLogProxy.Debug("SN012 Screen " + sn012.getErrorMessage(), "Red", 3);
                                    continue;
                                }
                                else
                                {
                                    //ProcessLogProxy.Debug("SN012 Screen " + sn012.getErrorMessage(), "Red", 3);
                                    //_resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "SN012 Screen " + sn012.getErrorMessage(), OperationFlag = "未退信" });
                                    SN014 sn014 = pcommCore.GetScreen<SN014>();
                                    //sn014.SetEnter();
                                    sn014.SendKey(KeyBoard.Enter);
                                    SN018 sn018 = pcommCore.GetScreen<SN018>();
                                    TrusteInfoModel trusteeInfo = trusteeInfos.Where(t => t.Trustee_Approval_No == sl.NewTRAprvlNo).FirstOrDefault();
                                    if (trusteeInfo != null && !string.IsNullOrEmpty(trusteeInfo.Trustee_client_no))
                                    {
                                        bool isSelectCase = false;
                                        if (wl.PM_AC == "PM" && wl.Withdraw_NT_PM == "Y")
                                        {
                                            isSelectCase = sn018.SelectClientNOForWithDraw(trusteeInfo.Trustee_client_no, wl.Name_in_XML);
                                        }
                                        else
                                        {
                                            isSelectCase = sn018.SelectClientNO(trusteeInfo.Trustee_client_no);
                                        }
                                        if (isSelectCase)
                                        {
                                            SJ353 sj353 = pcommCore.GetScreen<SJ353>();
                                            // exce 
                                            TrusteInfoModel data = trusteeInfos.Where(t => t.Scheme_Registration_No == sl.NewSchRegNo && t.Trustee_Approval_No == sl.NewTRAprvlNo).FirstOrDefault();
                                            //var data = trusteInfodDic["Trusteeinfo"].Where(x => x.Scheme_Registration_No == sl.NewSchRegNo).FirstOrDefault();
                                            if (data != null)
                                            {
                                                sj353.SelectSchemCode(data.Scheme_code);
                                            }
                                            else
                                            {
                                                ProcessLogProxy.Debug("cant find" + sl.NewSchRegNo, "Red", 3);
                                                continue;
                                            }

                                            SN012 sn0122 = pcommCore.GetScreen<SN012>();

                                            if (sn0122.ContainMessage_confirm())
                                            {
                                                sn0122.SendKey(KeyBoard.Enter);
                                                SN008 sn0083 = pcommCore.GetScreen<SN008>();
                                                sn0083.SendKey(KeyBoard.Enter);

                                                wl.Process_result = sn0083.GetMessage();


                                                if (sn0083.GetMessage().Contains("Last transaction processed") ||
                                                sn0083.GetMessage().Contains("Record already exists"))
                                                {
                                                    // update gwis
                                                    try
                                                    {
                                                        gwisOperation.SetOption(wl, data.In_Short);
                                                    }
                                                    catch(Exception ex)
                                                    {
                                                        ProcessLogProxy.Debug(ex.Message, "Red", 3);
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    ProcessLogProxy.Debug("SN008 Screen cant find" + sn0083.GetMessage(), "Red", 3);
                                                    continue;
                                                }
                                            }
                                            else

                                            {
                                                ProcessLogProxy.Debug("SN012 Screen cant find" + sn0122.getErrorMessage(), "Red", 3);
                                                continue;
                                            }
                                        }

                                        else
                                        {
                                            ProcessLogProxy.Debug("SN018 Screen cant find" + trusteeInfo.Trustee_client_no, "Red", 3);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        ProcessLogProxy.Debug("SN018 Screen can't find Trustee Client No", "Red", 3);
                                        continue;
                                    }

                                }
                            }
                            else
                            {
                                ProcessLogProxy.Debug("SN010 Screen" + sn0081.GetMessage(), "Red", 3);
                                continue;
                            }
                            ProcessLogProxy.Debug(wl.Case_Number + " has completed Satge 1&2 ", "Green", 1);
                            wl.Process_result = ProcessResult.Result1;
                        }
                        else
                        {
                            continue;
                        }
                    }

                    if (passList.Count > 0)
                    {
                        var excel = excelHandel.Open(this.ExcelPath);
                        excel.SetSpecifiedToExcel(passList, this.ExcelPath, "White List");
                    }
                    ProcessLogProxy.Debug("Complete", "Green", 1);
                });

            }
            catch (Exception e)
            {

                ProcessLogProxy.Debug(e.Message, "Red", 3);
                pcommCore.SkipToHomeScreen<S0017>();
            }
        }

        private bool Stage2Skiped()
        {
            CommonScreen screen = pcommCore.GetScreen<CommonScreen>();
            if (screen.GetText(1, 72, 5) == "SN008")
            {
                return true;
            }
            else
            {
                Thread.Sleep(1000);
                return false;
            }
        }

        public new ICommand EpassCaseCommand
        {
            get
            {
                if (_epassCaseCommand == null)

                    _epassCaseCommand = new RelayCommand<object>((p) => this.EpassCase());

                return _epassCaseCommand;
            }
        }


        private async void EpassCase()
        {
            ProcessLogProxy.Debug("Start Epass", "Green", 1);
            OleDbConnection objExcelCon = clit.openExcel();
            ewhlistdic = clit.ExtractEmployeeExcel<ExcelWhlist>(objExcelCon, "White List");
            List<RejectionCode> PMrejectonList = clit.GetExcelDataBySheetName<RejectionCode>(objExcelCon, "PM rejection code");
            List<RejectionCode> ACrejectonList = clit.GetExcelDataBySheetName<RejectionCode>(objExcelCon, "AC rejection code");
            try
            {
                //需要修改
                await STATask.Run(() =>
                 {
                     var epassList = ewhlistdic["White List"].ToList().Where(x => x.Clean_Case == "N").ToList();
                     var Remark = string.Empty;
                     pcommCore.SkipToHomeScreen<S0017>();
                     foreach (var wl in epassList)
                     {
                         try
                         {
                             ProcessLogProxy.Debug(wl.Case_Number + " starts Epass", "Green", 1);
                             var sl = slidt.TransferCase.Where(x => x.TransferCaseNo == wl.Case_Number).FirstOrDefault();

                             if (sl.TransferCaseNo == wl.Case_Number && wl.Clean_Case == "N")
                             {
                                 pcommCore.LinkToScreen<S0017>((S0017) =>
                                {
                                    ProcessLogProxy.Debug("SOO17 screeen Set Company", "Green", 1);
                                    if (wl.Company == "HSBC")
                                    {
                                        S0017.SetCompany("2");
                                    }
                                    else
                                    {
                                        S0017.SetCompany("3");
                                    }
                                    return S0017.GotoePass();
                                }).LinkToScreen<S0018>((S0018) =>
                                {
                                    ProcessLogProxy.Debug("S0018 screeen Goto Rejection_Letter", "Green", 1);
                                    return S0018.GotoRejection_Letter();
                                });
                                 SG756 sg756 = pcommCore.GetScreen<SG756>();
                                 var errorMessage = string.Empty;
                                 //sg756.Set_SchemeId(SelectedResut.CaseItem.OriSchAcctMemNo.Split('-')[0]);
                                 //sg756.Set_Id_No(SelectedResut.CaseItem.MemHKIDNo + "(" + SelectedResut.CaseItem.MemHKIDCheckDigit + ")");
                                 //sg756.Set_Case_Number(SelectedResut.CaseItem.TransferCaseNo);
                                 if (!string.IsNullOrEmpty(wl.ERID.Trim()))
                                 {
                                     sg756.Set_SchemeId(wl.ERID);
                                 }
                                 sg756.Set_Id_No(wl.HKID);
                                 sg756.Set_Case_Number(wl.Case_Number);
                                 if (wl.PM_AC == "PC")
                                 {
                                     sg756.Set_Form("AC");
                                 }
                                 else
                                 {
                                     //sg756.Set_Form(SelectedResut.CaseItem.RequestFormType);
                                     sg756.Set_Form(wl.PM_AC);
                                 }
                                 sg756.Set_option();
                                 sg756.SendKey(KeyBoard.Enter);
                                 bool isCrossCompany = false;
                                 if (sg756.GetWarningMessage().Contains("Scheme not verified"))
                                 {
                                     isCrossCompany = true;
                                     sg756.Set_SchemeId("        ");
                                     sg756.SendKey(KeyBoard.Enter);
                                     sg756.SendKey(KeyBoard.Enter);
                                     sg756.SendKey(KeyBoard.Enter);
                                 }
                                 CommonScreen commnoScreen = pcommCore.GetScreen<CommonScreen>();
                                 int enterNum = 0;
                                 while (enterNum < 6)
                                 {
                                     string screenCode = commnoScreen.GetText(1, 70, 10).Trim().ToUpper();
                                     if (screenCode.Contains("SG761"))
                                     {
                                         break;
                                     }
                                     else
                                     {
                                         commnoScreen.SendKey(KeyBoard.Enter);
                                         enterNum++;
                                     }
                                 }
                                 if (enterNum >= 6)
                                 {
                                     pcommCore.SkipToHomeScreen<S0017>();
                                     continue;
                                 }
                                 //errorMessage = sg756.GetWarningMessage();
                                 //if (errorMessage.Contains("Unmatched with NEWD"))
                                 //{
                                 //    sg756.Set_Enter();
                                 //    sg756.Set_Enter();
                                 //}
                                 //else if (errorMessage.Contains("Case No. already used"))
                                 //{
                                 //    pcommCore.SkipToHomeScreen<S0017>();
                                 //    continue;
                                 //}
                                 SG761 sg761 = pcommCore.GetScreen<SG761>();
                                 string[] codes = wl.ErrorCode.Split(',');
                                 sg761.SetCodeInSG761(codes, pcommCore);
                                 string codeMessage = GetCodeRejection(wl.PM_AC, codes, PMrejectonList, ACrejectonList);
                                 Remark = string.Format("{0} {1} received, but {2}, L/O 2016/6/16 to trustee", "Ext." + Extention, wl.PM_AC, codeMessage, DateTime.Now.ToString("yyyy/mm/dd"));

                                 SG756 sg7561 = pcommCore.GetScreen<SG756>();
                                 if (sg7561.GetWarningMessage() == "Last transaction processed")
                                 {
                                     pcommCore.SkipToHomeScreen<S0017>();
                                 }
                                 else
                                 {
                                     ProcessLogProxy.Debug("Epass is failed on SG756 ,Procedure is exit!" + sg7561.GetWarningMessage(), "Red", 3);
                                     SelectedResut.Status = false;
                                     pcommCore.SkipToHomeScreen<S0017>();
                                     return;
                                 }
                                 pcommCore.LinkToScreen<S0017>((S0017) =>
                                {
                                    ProcessLogProxy.Debug("SOO17 screeen Set Company", "Green", 1);
                                    if (isCrossCompany)
                                    {
                                        if (wl.Company == "HSBC")
                                        {
                                            S0017.SetCompany("3");
                                        }
                                        else
                                        {
                                            S0017.SetCompany("2");
                                        }
                                    }
                                    else
                                    {
                                        if (wl.Company == "HSBC")
                                        {
                                            S0017.SetCompany("2");
                                        }
                                        else
                                        {
                                            S0017.SetCompany("3");
                                        }
                                    }
                                    //S0017.SetCompany((SelectedResut.CaseItem.OriSchAcctMemNo.Split('-'))[0].ToCharArray().FirstOrDefault().ToString());
                                    return S0017.GotoMemberDetails();
                                }).LinkToScreen<S0018>((S0018) =>
                                {
                                    ProcessLogProxy.Debug("S0018 screeen Goto Goto MemberSchemeMaint", "Green", 1);
                                    return S0018.GotoMemberSchemeMaint();
                                });
                                 SM794 sm794 = pcommCore.GetScreen<SM794>();
                                 //sm794.SetEmpl_ID(SelectedResut.CaseItem.OriSchAcctMemNo.Split('-')[0]);
                                 sm794.SetEmpl_ID(wl.ERID);
                                 //sm794.SetIdentifier(SelectedResut.CaseItem.MemHKIDNo);
                                 sm794.SetIdentifier(wl.HKID);
                                 sm794.SetSelectOption();
                                 sm794.SM794Enter();
                                 SM799 sm799 = pcommCore.GetScreen<SM799>();
                                 sm799.Set_F20();
                                 SJ671 sj761 = pcommCore.GetScreen<SJ671>();
                                 sj761.SetPrintALL();
                                 sj761.SendKey(KeyBoard.Enter);
                                 SJ672 sj672 = pcommCore.GetScreen<SJ672>();
                                 Thread.Sleep(500);
                                 sj672.SetRemark(Remark);
                                 Thread.Sleep(500);
                                 sj672.SetEnter();
                                 pcommCore.SkipToHomeScreen<S0017>();
                                 // update gwis
                                 //gwisOperation.EpassUpdateGwis(sl);
                                 gwisOperation.EpassUpadateGwis(wl, Remark);
                                 wl.Letter_date = DateTime.Now.ToString("yyyy/MM/dd");
                             }
                             else
                             {
                                 continue;
                             }
                             ProcessLogProxy.Debug("This case has been completed Epass.", "Green", 1);
                         }
                         catch (Exception e)
                         {
                             //throw (e);
                             continue;
                         }
                     }

                     if (epassList.Count > 0)
                     {
                         var excel = excelHandel.Open(this.ExcelPath);
                         excel.SetSpecifiedToExcel(epassList, this.ExcelPath, "White List");
                     }
                     ProcessLogProxy.Debug("Epass has completed.", "Green", 1);
                     //if (epassList.Count > 0)
                     //{
                     //    var excel = excelHandel.Open(this.ExcelPath);

                     //    DataTable dt = ListToDatatableHelper.ToDataTable<WhiteListcs>(epassList);

                     //    excel.SaveDataTableToExcel(dt, this.ExcelPath, "White List");
                     //}

                 });
            }
            catch (Exception e)
            {
                //ProcessLogProxy.Debug(e.InnerException.Message, "Red", 3);
                pcommCore.SkipToHomeScreen<S0017>();
            }
        }

        private string GetCodeRejection(string pM_AC, string[] codes, List<RejectionCode> pMrejectonList, List<RejectionCode> aCrejectonList)
        {
            for (int i = 0; i < codes.Length; i++)
            {
                if (codes[i].StartsWith("0"))
                {
                    codes[i] = codes[i].Substring(1);
                }
            }
            string rejectionReason = string.Empty;
            IEnumerable<RejectionCode> selectedCodes;
            if (pM_AC == "PM")
            {
                selectedCodes = pMrejectonList.Where(t => codes.Contains(t.Code));
            }
            else
            {
                selectedCodes = aCrejectonList.Where(t => codes.Contains(t.Code));
            }
            foreach (RejectionCode code in selectedCodes)
            {
                rejectionReason = string.IsNullOrEmpty(rejectionReason) ? code.Description : rejectionReason + ", " + code.Description;
            }
            return rejectionReason;
        }

        private string _xtentionNo;

        public string Extention
        {

            get
            {
                return _xtentionNo;
            }

            set
            {

                if (_xtentionNo != value)
                {
                    _xtentionNo = value;

                    this.RaisePropertyChanged(() => Extention);
                }
            }
        }




        private string _xmlPath;

        public string XmlPath
        {

            get
            {
                return _xmlPath;
            }

            set
            {

                if (_xmlPath != value)
                {
                    _xmlPath = value;

                    this.RaisePropertyChanged(() => XmlPath);
                }
            }
        }

        private string _excelPath;

        public string ExcelPath
        {

            get
            {
                return _excelPath;
            }

            set
            {

                if (_excelPath != value)
                {
                    _excelPath = value;

                    this.RaisePropertyChanged(() => ExcelPath);
                }
            }
        }


        public new ICommand ReadExcelCommand
        {
            get
            {
                if (_readexcelCommand == null)

                    _readexcelCommand = new RelayCommand<object>((p) => this.ReadExcel());

                return _readexcelCommand;
            }
        }

        private void ReadExcel()
        {

            //ProcessLogProxy.Debug("Start Read Excel's Processing", "Red", 3);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "Excel文件|*.xls;*.xlsx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                clit.path = openFileDialog.FileName;
                ExcelPath = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            OleDbConnection objExcelCon = clit.openExcel();
            trusteInfodDic = clit.ExtractEmployeeExcel<TrusteInfoModel>(objExcelCon, "Trustee info");
            List<string> list = new List<string>();
            clit.ExtractEmployeeExcel<Notepad>(objExcelCon, "Notepad")["Notepad"].ForEach(x =>
            {
                list.Add(x.Notepad_Key_Words);
            });
            notepadDic.Add("Notepad", list);

        }

        public new ICommand ReadXmlCommand
        {
            get
            {
                if (_readXmlCommand == null)

                    _readXmlCommand = new RelayCommand<object>((p) => this.Readxml());

                return _readXmlCommand;
            }
        }

        private void Readxml()
        {
            //ProcessMsg p = new ProcessMsg();
            //p.Color = "Green";
            //p.Msg = "Testxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
            //p.Level = 1;
            //ProcessLogProxy.MessageAction(p);
            //ProcessLogProxy.Debug("Debug message test", "Red", 3);
            //ProcessLogProxy.Info("Normal message test");
            //ProcessLogProxy.Message("This is a message test", "Blue");

            string filename = string.Empty;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "c:\\";//注意这里写路径时要用c:\\而不是c:\
            //openFileDialog.Filter = "文本文件|*.*|xml文件|*.xml|所有文件|*.*";
            openFileDialog.Filter = "xml文件|*.xml";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                filename = openFileDialog.FileName;
                XmlPath = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            slidt = XMLProcesser.GetObject<NewRequest>(filename);

            foreach (var cs in slidt.TransferCase)

            {
                _resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "sj672 exsit key word", OperationFlag = "未退信" });
            }



        }



        public ObservableCollection<ProcessMsg> LogList
        {
            get
            {
                return _logList;
            }

            set
            {
                if (_logList != value)
                {
                    _logList = value;

                    this.RaisePropertyChanged(() => LogList);
                }
            }
        }

        private CheckResult _selectedResult;


        public CheckResult SelectedResut

        {
            get
            {
                return _selectedResult;
            }

            set
            {
                if (_selectedResult != value)
                {
                    _selectedResult = value;

                    this.RaisePropertyChanged(() => SelectedResut);
                }
            }
        }

        public ObservableCollection<CheckResult> ResultList

        {
            get
            {
                return _resultList;
            }

            set
            {
                if (_resultList != value)
                {
                    _resultList = value;

                    this.RaisePropertyChanged(() => ResultList);
                }
            }
        }

    }
}
