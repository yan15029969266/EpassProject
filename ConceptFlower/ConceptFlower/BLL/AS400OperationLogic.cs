using ConceptFlower.Log;
using ConceptFlower.Models;
using ConceptFlower.Static;
using PcommCore.Common;
using PcommCore.Screen;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConceptFlower.BLL
{
    public  class AS400OperationLogic
    {



        public ObservableCollection<CheckResult> _resultList { get; set; }


        public Dictionary<string, List<string>> notepadDic { get; set; }
        //trusteInfo
        public Dictionary<string, List<TrusteInfoModel>> trusteInfodDic { get; set; }


        GwisOperationLogic gwisOperation = new GwisOperationLogic();

        public  async void CheckCase(NewRequest slidt ,PcommCore.PcommCore pcommCore)
        {
            // mre = new ManualResetEvent(false);
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

            //  IsLoadEnable = true;
            // IsMatchEnable = false;
            var p = slidt.TransferCase.FirstOrDefault();
            ObservableCollection<ProcessMsg> Logtmp = new ObservableCollection<ProcessMsg>();
            //List<Notepad> keywords = dic["Notepad"].ToList();

            // gwis operation






            pcommCore.SkipToHomeScreen<S0017>();

            //foreach (var cs in slidt.TransferCase)

            //{
            //    _resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "sj672 exsit key word", Status = true, OperationFlag = "未退信" });
            //}

            try
            {
                await STATask.Run(() =>
                {


                    foreach (var cs in slidt.TransferCase)
                    {
                        // gwis match


                        if (!gwisOperation.SearchCase(cs,null))

                        {
                            ProcessLogProxy.Debug("gwis can't find the member", "Red", 3);
                        }

                        pcommCore.LinkToScreen<S0017>((S0017) =>
                        {
                            ProcessLogProxy.Debug("SOO17 screeen Set Company", "Green", 1);
                            S0017.SetCompany((cs.OriSchAcctMemNo.Split('-'))[0].ToCharArray().FirstOrDefault().ToString());
                            return S0017.GotoMemberDetails();
                        }).LinkToScreen<S0018>((S0018) =>
                        {
                            ProcessLogProxy.Debug("S0018 screeen Go to MemberSchemeMaint", "Green", 1);
                            return S0018.GotoMemberSchemeMaint();
                        }).LinkToScreen<SM794>((SM794) =>
                        {
                            ProcessLogProxy.Debug("SM794 screeen Set Identifier", "Green", 1);
                            SM794.SetIdentifier(cs.MemHKIDNo);

                            SM794.SetSelectOption("D");

                            ProcessLogProxy.Debug("SM794 screeen Set option D", "Green", 1);
                            return true;
                        }).LinkToScreen<SH795>((SH795) =>
                        {
                            ProcessLogProxy.Debug("SH795 Select  SechemeId", "Green", 1);

                            //SH795.SelectSchemeID(cs.OriSchAcctMemNo.Split('-')[0]);
                            return true;
                        });

                        // go to SM799
                        SM799 sm799 = pcommCore.GetScreen<SM799>();

                        var id = cs.MemHKIDNo + "(" + cs.MemHKIDCheckDigit + ")";
                        var dic = sm799.GetMemberInformation();

                        var englishName = dic["SURNAME"] + " " + dic["TAI MAN"];

                        if (dic["BANKRUPTCY"] == "NO" &&
                        cs.MemEngName == englishName &&
                        id == dic["IDNO"].ToString())
                        {
                            ProcessLogProxy.Debug("SM799 compare with xml data ", "Green", 1);
                            sm799.SetX();
                        }
                        else
                        {
                            _resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "SM799 screen " + dic["MPFCOMPLANT"].ToString() + dic["SENSFLAG"].ToString(), Status = true, OperationFlag = "未退信" });
                            slidt.TransferCase.Remove(cs);
                            ProcessLogProxy.Debug("SM799 screen " + dic["MPFCOMPLANT"].ToString() + dic["SENSFLAG"].ToString(), "Red", 3);
                            continue;
                        }

                        SM800 sm800 = pcommCore.GetScreen<SM800>();

                        ProcessLogProxy.Debug("SM800  get Chinese Name  ", "Green", 1);

                        if (sm800.GetMemberChineseInformation()["CHINESENAME"].ToString() == cs.MemChiName)
                        {
                            sm800.SendKey(KeyBoard.PF3);
                        }
                        else
                        {
                            _resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "SM800 screen  Chinese Name unmatched" + sm800.GetMemberChineseInformation()["CHINESENAME"].ToString(), Status = true, OperationFlag = "未退信" });
                            ProcessLogProxy.Debug("SM800 screen  Chinese Name unmatched" + sm800.GetMemberChineseInformation()["CHINESENAME"].ToString(), "Red", 3);
                            slidt.TransferCase.Remove(cs);
                            continue;
                        }

                        // return SM799 Screen
                        SM799 s_sm7799 = pcommCore.GetScreen<SM799>();
                        s_sm7799.Set_F20();
                        //
                        SJ671 sj671 = pcommCore.GetScreen<SJ671>();
                        sj671.ClearSchemeID();
                        sj671.SetPrintALL();
                        sj671.Enter(21, 69);


                        if (sj671.SelectSchemeID(notepadDic["NotePad"], 10, pcommCore))
                        {
                            ProcessLogProxy.Debug("sj672 exsit key word ", "Red", 3);

                            _resultList.Add(new CheckResult { Level = "Warn", CaseItem = cs, Meassage = "sj672 exsit key word", Status = true, OperationFlag = "未退信" });
                            slidt.TransferCase.Remove(cs);
                            continue;
                        }
                        else
                        {
                            pcommCore.SkipToHomeScreen<S0017>();
                        }
                    }

                    ProcessLogProxy.Debug("xml data check completed ", "Green", 1);
                    pcommCore.SkipToHomeScreen<S0017>();

                });

                //if (_resultList.Count > 0)
                //{
                //    IsEpassEnable = true;
                //    ResultList = _resultList;
                //}
                //IsRunEnable = true;

            }
            catch (Exception e)
            {
                ProcessLogProxy.Debug(e.Message, "Red", 3);
            }
        }



















    }
}
