using ConceptFlower.Extention;
using ConceptFlower.Models;
using PcommCore.Screen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConceptFlower.BLL
{
    public class SelectCaseFactory
    {
        public static SH795AccoutModel Getcase(TransferCase cs, List<SH795AccoutModel> entityList, WhiteListcs whlist, TrusteInfoModel tsinfo, List<UserInfoSchemeInfo> schemeInfoList)
        {
            SH795AccoutModel model = null;
            Regex reg1 = new Regex(@"^(\d{8})(-|\s)(\d{9})$");
            Regex reg2 = new Regex(@"^(\d{8})(-|\s)(\d{8})$");
            Regex reg3 = new Regex(@"^\d{9}$");
            Regex reg4 = new Regex(@"^\d{8}$");
            Regex reg5 = new Regex(@"^\d{17}$");
            Regex reg6 = new Regex(@"^(\d{9})(-|\s)(\d{8})$");
            Regex reg7 = new Regex(@"^\d{17}$");
            #region first select logic
            try
            {
                if (reg1.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg1(cs, entityList, reg1);
                }
                else if (reg2.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg2(cs, entityList, reg2);
                }
                else if (reg3.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg3(cs, entityList, schemeInfoList,reg3);
                }
                else if (reg4.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg4(cs, entityList, tsinfo, schemeInfoList, reg4);
                }
                else if (reg5.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg5(cs, entityList, reg5);
                }
                else if (reg6.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg6(cs, entityList, reg6);
                }
                else if (reg7.IsMatch(cs.OriSchAcctMemNo))
                {
                    model = GetAccountByReg7(cs, entityList, tsinfo, reg7);
                }
                else
                {
                    //whlist.Clean_Case = "N";
                    //whlist.Unclean_Reason.Append("Member Account or ERID is Error, pls input correct whitelsit \r\n");
                    //if (cs.RequestFormType == "PM")
                    //{
                    //    whlist.ErrorCode.Append("3,5,");
                    //}
                    //else if (cs.RequestFormType == "PC")
                    //{
                    //    whlist.ErrorCode.Append("3,");
                    //}
                }
                
                if(model!=null)
                {
                    string schemeName = schemeInfoList.Where(t => t.SchemeID == model.SchemeID).FirstOrDefault().CaseNO;
                    if(!string.IsNullOrEmpty(schemeName)&&schemeName!=cs.OriSchRegNo)
                    {
                        throw new AccountException("Missing/Incorrect Orig Scheme Reg No (Scheme name)");
                    }
                }
            }
            catch(AccountException ex)
            {
                ErrorHanding(ex.Message, whlist);
            }
            #endregion
            #region The second select logic

            #endregion
            return model;
        }
        private static void ErrorHanding(string errorMsg , WhiteListcs whlist)
        {
            if (errorMsg == "Missing/Incorrect mbrship no under Orig Scheme")
            {
                whlist.ErrorCode.Append("3,");
            }
            if (errorMsg == "Missing/Incorrect Orig Scheme Reg No (Scheme name)")
            {
                whlist.ErrorCode.Append("4,");
            }
            if (errorMsg== "Missing/Incorrect Original Scheme ID no (ERID)")
            {
                whlist.ErrorCode.Append("5,");
            }
            if (errorMsg == "No such member record in Original Scheme")
            {
                whlist.ErrorCode.Append("8,");
            }
            if (errorMsg == "There are MemebershipNo with multiple data is the same")
            {
                whlist.ErrorCode.Append("13,");
            }
            if (errorMsg == "Duplicate request in the NEWD file.")
            {
                whlist.ErrorCode.Append("99,");
            }
            if (errorMsg== "Please Check Value Choice")
            {
                throw new AccountException("Please Check Value Choice");
            }
            whlist.Clean_Case = "N"; 
            whlist.Unclean_Reason.Append(errorMsg+ "\r\n");
        }
        /// <summary>
        /// 12345678-123456789
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg1(TransferCase cs, List<SH795AccoutModel> entityList, Regex reg)
        {
            string SchemeID = reg.Match(cs.OriSchAcctMemNo).Groups[1].Value;
            string MembershipNo = reg.Match(cs.OriSchAcctMemNo).Groups[3].Value;
            SH795AccoutModel acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo.Substring(1)).FirstOrDefault();
            if(acModel!=null)
            {
                acModel.SchemeIDInXml = SchemeID;
                acModel.MembershipNoInXml = MembershipNo;
                //result.SchemeIDInAS400 = acModel.SchemeID;
                //result.MemberShipNoInAS400 = acModel.Client;
            }
            else
            {
                if (cs.RequestFormType == "PM")
                {
                    if (entityList.Where(t => t.SchemeID == SchemeID).Count() > 0)
                    {
                        throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                    }
                    if(entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() > 0)
                    {
                        throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                    }
                }
                else if (cs.RequestFormType == "PC")
                {
                    throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                }
            }
            return acModel;
        }
        /// <summary>
        /// 12345678-12345678
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg2(TransferCase cs, List<SH795AccoutModel> entityList,Regex reg)
        {
            string SchemeID = reg.Match(cs.OriSchAcctMemNo).Groups[1].Value;
            string MembershipNo = reg.Match(cs.OriSchAcctMemNo).Groups[3].Value;
            #region Check 99 code
            if (entityList.Where(t => t.SchemeID == SchemeID).Count() > 0 && entityList.Where(t => t.SchemeID == MembershipNo).Count() > 0)
            {
                throw new AccountException("Duplicate request in the NEWD file.");
            }
            if (entityList.Where(t => t.Client == SchemeID).Count() > 0 && entityList.Where(t => t.Client == SchemeID).Count() > 0)
            {
                throw new AccountException("Duplicate request in the NEWD file.");
            }
            #endregion
            SH795AccoutModel acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo).FirstOrDefault();
            if(acModel != null)
            {
                acModel.SchemeIDInXml = SchemeID;
                acModel.MembershipNoInXml = MembershipNo;
            }
            else
            {
                SchemeID = reg.Match(cs.OriSchAcctMemNo).Groups[3].Value;
                MembershipNo = reg.Match(cs.OriSchAcctMemNo).Groups[1].Value;
                acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo).FirstOrDefault();
                if (acModel != null)
                {
                    acModel.SchemeIDInXml = SchemeID;
                    acModel.MembershipNoInXml = MembershipNo;
                }
                else
                {
                    if (cs.RequestFormType == "PM")
                    {
                        string accout1 = reg.Match(cs.OriSchAcctMemNo).Groups[1].Value;
                        string accout2 = reg.Match(cs.OriSchAcctMemNo).Groups[3].Value;
                        if (entityList.Where(t => t.SchemeID == accout1||t.SchemeID==accout2).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                        }
                        if (entityList.Where(t => t.Client == accout1||t.Client==accout2).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                        }
                    }
                    else if (cs.RequestFormType == "PC")
                    {
                        throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                    }
                }
            }
            return acModel;
        }

        /// <summary>
        /// 123456789
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg3(TransferCase cs, List<SH795AccoutModel> entityList , List<UserInfoSchemeInfo> schemeInfoList, Regex reg)
        {
            SH795AccoutModel acModel = null;
            List<SH795AccoutModel> mylist = entityList.Where(t => t.Client == cs.OriSchAcctMemNo.Substring(1)).ToList();
            if (mylist.Count() > 1)
            {
                List<UserInfoSchemeInfo> list = schemeInfoList.Where(t => t.CaseNO == cs.OriSchRegNo).ToList();
                mylist = mylist.Where(t => list.Where(m => m.SchemeID == t.SchemeID).Count()>0).ToList();
                if (mylist.Count() > 1)
                {
                    throw new AccountException("There are MemebershipNo with multiple data is the same");
                }
                else if(mylist.Count()==1)
                {
                    acModel = mylist[0];
                    acModel.SchemeIDInXml = acModel.SchemeID;
                    acModel.MembershipNoInXml = cs.OriSchAcctMemNo;
                }
                else
                {
                    throw new AccountException("Please Check Value Choice");
                }
                
            }
            else if (mylist.Count() == 1)
            {
                //SchemeID = mylist[0].SchemeID;
                acModel = mylist[0];
                acModel.SchemeIDInXml = acModel.SchemeID;
                acModel.MembershipNoInXml = cs.OriSchAcctMemNo;
            }
            else
            {
                if (cs.RequestFormType == "PM")
                {
                    throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                }
                else if (cs.RequestFormType == "PC")
                {
                    throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                }
            }
            return acModel;
        }

        /// <summary>
        /// 12345678
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg4(TransferCase cs, List<SH795AccoutModel> entityList, TrusteInfoModel tsinfo, List<UserInfoSchemeInfo> schemeInfoList, Regex reg)
        {
            SH795AccoutModel acModel = null;
            IEnumerable<SH795AccoutModel> list1 = entityList.Where(t => t.SchemeID == cs.OriSchAcctMemNo);
            //如果在SchemeID存在
            if (list1.Count() > 0)
            {
                SH795AccoutModel model = list1.OrderByDescending(t => t.DJS).FirstOrDefault();
                //SchemeID = model.SchemeID;
                //MembershipNo = model.Client;
                acModel = model;
                acModel.SchemeIDInXml = model.SchemeID;
                if (tsinfo.Scheme_name.StartsWith("HANG"))
                {
                    acModel.MembershipNoInXml = "3" + acModel.Client;
                }
                else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                {
                    acModel.MembershipNoInXml = "2" + acModel.Client;
                }
            }
            else
            {
                IEnumerable<SH795AccoutModel> list2 = entityList.Where(t => t.Client == cs.OriSchAcctMemNo);
                if (list2.Count() > 1)
                {
                    List<UserInfoSchemeInfo> list = schemeInfoList.Where(t => t.CaseNO == cs.OriSchRegNo).ToList();
                    list2 = list2.Where(t => list.Where(m => m.SchemeID == t.SchemeID).Count() > 0).ToList();
                    if (list2.Count() > 1)
                    {
                        throw new AccountException("There are MemebershipNo with multiple data is the same");
                    }
                    else if (list2.Count() == 1)
                    {
                        acModel = list2.FirstOrDefault();
                        acModel.SchemeIDInXml = acModel.SchemeID;
                        acModel.MembershipNoInXml = cs.OriSchAcctMemNo;
                    }
                    else
                    {
                        throw new AccountException("Please Check Value Choice");
                    }
                }
                else if (list2.Count() == 1)
                {
                    //SchemeID = list2.ElementAtOrDefault(0).SchemeID;
                    //MembershipNo = list2.ElementAtOrDefault(0).Client;
                    acModel = list2.ElementAtOrDefault(0);
                    if (tsinfo.Scheme_name.StartsWith("HANG"))
                    {
                        acModel.MembershipNoInXml = "3" + acModel.Client;
                    }
                    else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                    {
                        acModel.MembershipNoInXml = "2" + acModel.Client;
                    }
                }
                else
                {
                    if (cs.RequestFormType == "PM")
                    {
                        throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                    }
                    else if (cs.RequestFormType == "PC")
                    {
                        throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                    }
                }
            }
            return acModel;
        }

        /// <summary>
        /// 12345678912345678
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg5(TransferCase cs, List<SH795AccoutModel> entityList, Regex reg)
        {
            SH795AccoutModel acModel = null;
            bool isSelect = true;
            string SchemeID = cs.OriSchAcctMemNo.Substring(0, 8);
            string MembershipNo = cs.OriSchAcctMemNo.Substring(8, 9);
            if (entityList.Where(t => t.SchemeID == SchemeID).Count() <= 0 && entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() <= 0)
            {
                SchemeID = cs.OriSchAcctMemNo.Substring(9, 8);
                MembershipNo = cs.OriSchAcctMemNo.Substring(0, 9);
                if (entityList.Where(t => t.SchemeID == SchemeID).Count() <= 0 && entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() <= 0)
                {
                    isSelect = false;
                    throw new AccountException("No such member record in Original Scheme");
                }
            }
            if (isSelect)
            {
                acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo.Substring(1)).FirstOrDefault();
                if (acModel == null)
                {
                    if (cs.RequestFormType == "PM")
                    {
                        if (entityList.Where(t => t.SchemeID == SchemeID).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                        }
                        if (entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                        }
                    }
                    else if (cs.RequestFormType == "PC")
                    {
                        throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                    }
                    //SchemeID = cs.OriSchAcctMemNo.Substring(9, 8);
                    //MembershipNo = cs.OriSchAcctMemNo.Substring(0, 9);
                    //acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo.Substring(1)).FirstOrDefault();
                    //if (acModel == null)
                    //{

                    //}
                }
                else
                {
                    acModel.MembershipNoInXml = MembershipNo;
                }
            }
            return acModel;
        }
        /// <summary>
        /// 123456789-12345678
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg6(TransferCase cs, List<SH795AccoutModel> entityList, Regex reg)
        {
            SH795AccoutModel acModel = null;
            string SchemeID = reg.Match(cs.OriSchAcctMemNo).Groups[3].Value;
            string MembershipNo = reg.Match(cs.OriSchAcctMemNo).Groups[1].Value; ;
            if (entityList.Where(t => t.SchemeID == SchemeID).Count() <= 0 && entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() <= 0)
            {
                throw new AccountException("No such member record in Original Scheme");
            }
            else
            {
                acModel = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo.Substring(1)).FirstOrDefault();
                if (acModel == null)
                {
                    if (cs.RequestFormType == "PM")
                    {
                        if (entityList.Where(t => t.SchemeID == SchemeID).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                        }
                        if (entityList.Where(t => t.Client == MembershipNo.Substring(1)).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                        }
                    }
                    else if (cs.RequestFormType == "PC")
                    {
                        throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                    }
                }
                else
                {
                    acModel.MembershipNoInXml = MembershipNo;
                    acModel.SchemeIDInXml = SchemeID;
                }
            }
            return acModel;
        }

        /// <summary>
        /// 1234567812345678
        /// </summary>
        /// <param name="cs"></param>
        /// <param name="entityList"></param>
        /// <param name="whlist"></param>
        /// <param name="reg"></param>
        /// <returns></returns>
        private static SH795AccoutModel GetAccountByReg7(TransferCase cs, List<SH795AccoutModel> entityList, TrusteInfoModel tsinfo, Regex reg)
        {

            SH795AccoutModel acModel = null;
            string SchemeID = cs.OriSchAcctMemNo.Substring(0, 8);
            string MembershipNo = cs.OriSchAcctMemNo.Substring(8, 8);
            #region Check 99 code
            if (entityList.Where(t => t.SchemeID == SchemeID).Count() > 0 && entityList.Where(t => t.SchemeID == MembershipNo).Count() > 0)
            {
                throw new AccountException("Duplicate request in the NEWD file.");
            }
            if (entityList.Where(t => t.Client == SchemeID).Count() > 0 && entityList.Where(t => t.Client == SchemeID).Count() > 0)
            {
                throw new AccountException("Duplicate request in the NEWD file.");
            }
            #endregion
            SH795AccoutModel model = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo).FirstOrDefault();
            if (model != null)
            {
                //MembershipNo = model.Client;
                acModel = model;
                if (tsinfo.Scheme_name.StartsWith("HANG"))
                {
                    acModel.MembershipNoInXml = "3" + acModel.Client;
                }
                else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                {
                    acModel.MembershipNoInXml = "2" + acModel.Client;
                }
            }
            else
            {
                SchemeID = cs.OriSchAcctMemNo.Substring(8, 8);
                MembershipNo = cs.OriSchAcctMemNo.Substring(0, 8);
                model = entityList.Where(t => t.SchemeID == SchemeID && t.Client == MembershipNo).FirstOrDefault();
                if (model != null)
                {
                    acModel = model;
                    if (tsinfo.Scheme_name.StartsWith("HANG"))
                    {
                        acModel.MembershipNoInXml = "3" + acModel.Client;
                    }
                    else if (tsinfo.Scheme_name.StartsWith("HSBC"))
                    {
                        acModel.MembershipNoInXml = "2" + acModel.Client;
                    }
                }
                else
                {
                    if (cs.RequestFormType == "PM")
                    {
                        string accout1 = cs.OriSchAcctMemNo.Substring(0, 8);
                        string accout2 = cs.OriSchAcctMemNo.Substring(8, 8);
                        if (entityList.Where(t => t.SchemeID == accout1 || t.SchemeID == accout2).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                        }
                        if (entityList.Where(t => t.Client == accout1 || t.Client == accout2).Count() > 0)
                        {
                            throw new AccountException("Missing/Incorrect Original Scheme ID no (ERID)");
                        }
                    }
                    else if (cs.RequestFormType == "PC")
                    {
                        throw new AccountException("Missing/Incorrect mbrship no under Orig Scheme");
                    }
                }
            }
            return acModel;
        }

    }
    public class AccountResult
    {
        public string SchemeIDInXml { get; set; }
        public string SchemeIDInAS400 { get; set; }
        public string MemberShipNoInXml { get; set; }
        public string MemberShipNoInAS400 { get; set; }
    }
    public class UserInfoSchemeInfo
    {
        public string SchemeID { get; set; }
        public string SchemeCode { get; set; }
        public string CaseNO { get; set; }
    }
}
