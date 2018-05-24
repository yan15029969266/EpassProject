using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PcommCore.Screen
{
    public class SH795 : ScreenLogic
    {

        public List<string> Message { get; set; }

        public bool is_findcase { get; set; }

        public string bank_Flag { get; set; }

        public ScreenDes screenDes = new ScreenDes();

        public List<string> rowList { get; set; }

        public SH795()
        {
            Message = new List<string>();
            ContentTag tag = new ContentTag("SH795", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public void SelectSchemeID(string id, string scheme_Name, int selCol = 3, int stsCol = 66)
        {
            is_findcase = true;


            if (base.IsHaveNext())
            {
                if (id.StartsWith("2") && scheme_Name.StartsWith("HSBC"))
                {
                  
                    CursorPos point = base.SearchText(id);
                    if (point.IsMatched)
                    {
                        base.SetText("Y", selCol, point.Row);

                        var sts = base.GetText(point.Row, stsCol, 3).Trim();

                        if (sts == "CU" || sts == "NO")
                        {
                            base.Enter(point.Row, point.Col);
                            return;
                        }
                        else
                        {
                            Message.Add("HSBC Member a/c status is " + sts + " ,pls manual checking");
                            return;
                        }
                    }
                    else
                    {
                        is_findcase = false;
                        rowList.AddRange(base.ReadRows(9, 22));
                        base.PageDown();
                        SelectSchemeID(id, scheme_Name, selCol, stsCol);
                    }
                }
                else if (id.StartsWith("3") && scheme_Name.StartsWith("HANG"))
                {

                   
                    CursorPos point = base.SearchText(id);
                    if (point.IsMatched)
                    {
                        base.SetText("Y", selCol, point.Row);

                        var sts = base.GetText(point.Row, stsCol, 3).Trim();

                        if (sts == "CU" || sts == "NO")
                        {
                            base.Enter(point.Row, point.Col);
                            return;
                        }
                        else
                        {
                            Message.Add("HANG Member a/c status is " + sts + " ,pls manual checking" + "id ="+ id + "scheme_Name =" + scheme_Name);
                            return;
                        }
                    }
                    else
                    {
                        is_findcase = false;
                        rowList.AddRange(base.ReadRows(9, 22));
                        base.PageDown();
                        SelectSchemeID(id, scheme_Name, selCol, stsCol);
                    }
                }
                else
                {
                    is_findcase = false;
                    rowList.AddRange(base.ReadRows(9, 22));
                    base.PageDown();
                    SelectSchemeID(id, scheme_Name, selCol, stsCol);
                    checked_MembershipNo();
                    Message.Add("Member input info in the column which is different company, pls manual checking" );
                    return;
                }
            }
            else if (base.IsBottom())
            {

                if (id.StartsWith("2") && scheme_Name.StartsWith("HSBC"))
                {
                
                    CursorPos point = base.SearchText(id);
                    if (point.IsMatched)
                    {
                        base.SetText("Y", selCol, point.Row);

                        var sts = base.GetText(point.Row, stsCol, 3).Trim();

                        if (sts == "CU" || sts == "NO")
                        {
                            base.Enter(point.Row, point.Col);
                            return;
                        }
                        else
                        {
                            Message.Add("HSBC Member a/c status is " + sts + " ,pls manual checking");
                            checked_MembershipNo();
                            return;
                        }
                    }
                    else
                    {
                        is_findcase = false;
                        rowList.AddRange(base.ReadRows(9, 22));
                        checked_MembershipNo();
                        return;                      
                    }
                }
                else if (id.StartsWith("3") && scheme_Name.StartsWith("HANG"))
                {
                   
                    CursorPos point = base.SearchText(id);
                    if (point.IsMatched)
                    {
                        base.SetText("Y", selCol, point.Row);

                        var sts = base.GetText(point.Row, stsCol, 3).Trim();

                        if (sts == "CU" || sts == "NO")
                        {
                            base.Enter(point.Row, point.Col);
                            return;
                        }
                        else
                        {
                            Message.Add("HANG Member a/c status is " + sts + " ,pls manual checking");
                            checked_MembershipNo();
                            return;
                        }
                    }
                    else
                    {
                        is_findcase = false;
                        rowList.AddRange(base.ReadRows(9, 22));
                        checked_MembershipNo();
                        return;                                  
                    }
                }
                else
                {
                    rowList.AddRange(base.ReadRows(9, 22));
                    Message.Add("Member input info in the column which is different company, pls manual checking");
                    checked_MembershipNo();
                    return;
                }
            }
        }

        public void checked_MembershipNo()
        {     
            var schemeId = string.Empty;
            rowList.ForEach(x =>
            {
                if (x.Contains("PR01"))
                {
                    if (x.Split(' ').Distinct().ToList()[1].StartsWith("2"))
                    {
                        Message.Add("HSBC Member a/c  is " + x + " membership no/ERID match with another personal a/c with different scheme, the reject scheme name differ ");
                    }
                    else if(x.Split(' ').Distinct().ToList()[1].StartsWith("3"))
                    {
                        Message.Add("HANG SENG Member a/c  is " + x + " membership no/ERID match with another personal a/c with different scheme, the reject scheme name differ ");
                    }
                    else
                    {
                        Message.Add("invalid Member a/c  is " + x + " membership no/ERID unmatch with  personal a/c with different scheme, the reject scheme name differ ");
                    }
                }

            });           
        }

        public void checked_MembershipNo(List<SH795AccoutModel> accountList)
        {
            var schemeId = string.Empty;
            accountList.ForEach(model =>
            {
                if (model.PayCID.Contains("PR01"))
                {
                    if (model.MembershipNoInXml.StartsWith("2"))
                    {
                        Message.Add("HSBC Member a/c  is " + model.SchemeID + " membership no/ERID match with another personal a/c with different scheme, the reject scheme name differ ");
                    }
                    else if (model.MembershipNoInXml.StartsWith("3"))
                    {
                        Message.Add("HANG SENG Member a/c  is " + model.SchemeID + " membership no/ERID match with another personal a/c with different scheme, the reject scheme name differ ");
                    }
                    else
                    {
                        Message.Add("invalid Member a/c  is " + model.SchemeID + " membership no/ERID unmatch with  personal a/c with different scheme, the reject scheme name differ ");
                    }
                }

            });
        }

        public List<SH795AccoutModel> GetAccountList(List<SH795AccoutModel> list,int pageIndex)
        {
            //List<SH795AccoutModel> list = new List<SH795AccoutModel>();
            Regex reg = new Regex(@"\s{2,}");
            if (base.IsHaveNext())
            {
                List<string> msgList = base.ReadRows(9, 22);
                int row = 9;
                foreach(string msg in msgList)
                {
                    //var strList = reg.Split(msg.Trim());
                    //if(strList.Count()<6)
                    //{
                    //    continue;
                    //}
                    if(string.IsNullOrEmpty(msg.Substring(5, 10).Trim()))
                    {
                        continue;
                    }
                    SH795AccoutModel model = new SH795AccoutModel
                    {
                        pageIndex=pageIndex,
                        rowIndex = row,
                        //SchemeID = strList[0],
                        //EmployerName = strList[1],
                        //PayCID = strList[2],
                        //Client = strList[3].Split(' ')[0],
                        //Sts = strList[3].Split(' ')[1],
                        //DJS = Convert.ToDateTime(strList[5])
                        SchemeID = msg.Substring(5, 10).Trim(),
                        EmployerName = msg.Substring(15, 31).Trim(),
                        PayCID = msg.Substring(46, 10).Trim(),
                        Client = msg.Substring(56, 9).Trim(),
                        Sts = msg.Substring(65, 4).Trim(),
                        DJS = Convert.ToDateTime(msg.Substring(69).Trim())
                    };
                    list.Add(model);
                    row++;
                }
                base.PageDown();
                pageIndex++;
                GetAccountList(list, pageIndex);
            }
            else
            {
                List<string> msgList = base.ReadRows(9, 22);
                int row = 9;
                foreach (string msg in msgList)
                {
                    //var strList = reg.Split(msg.Trim());
                    //if (strList.Count() < 6)
                    //{
                    //    continue;
                    //}
                    if (string.IsNullOrEmpty(msg.Substring(5, 10).Trim()))
                    {
                        continue;
                    }
                    SH795AccoutModel model = new SH795AccoutModel
                    {
                        pageIndex=pageIndex,
                        rowIndex = row,
                        SchemeID=msg.Substring(5, 10).Trim(),
                        EmployerName=msg.Substring(15, 31).Trim(),
                        PayCID=msg.Substring(46, 10).Trim(),
                        Client=msg.Substring(56, 9).Trim(),
                        Sts=msg.Substring(65, 4).Trim(),
                        DJS= Convert.ToDateTime(msg.Substring(69).Trim())
                    };
                    list.Add(model);
                    row++;
                }
            }
            return list;
        }
        public void SelectCase(SH795AccoutModel model,string scheme_Name, List<SH795AccoutModel> entityList, int selCol = 3, int stsCol = 66)
        {
            is_findcase = true;
            for(int i=1;i<model.pageIndex;i++)
            {
                //SendKey(KeyBoard.PF3);
                base.PageDown();
            }
            base.SetText("Y", model.rowIndex, selCol);
            SendKey(KeyBoard.Enter);
            return;
            //if (model.MembershipNoInXml.StartsWith("2") && scheme_Name.StartsWith("HSBC"))
            //{
            //    //base.SetText("Y", model.rowIndex, selCol);
            //    //SendKey(KeyBoard.Enter);
            //    //return;
            //    //if (model.Sts == "CU" || model.Sts == "NO")
            //    //{
            //        base.SetText("Y", model.rowIndex, selCol);
            //        SendKey(KeyBoard.Enter);
            //        return;
            //    //}
            //    //else
            //    //{
            //    //    Message.Add("HSBC Member a/c status is " + model.Sts + " ,pls manual checking");
            //    //    checked_MembershipNo(entityList);
            //    //    return;
            //    //}

            //}
            //else if (model.MembershipNoInXml.StartsWith("3") && scheme_Name.StartsWith("HANG"))
            //{
            //    //base.SetText("Y", model.rowIndex, selCol);
            //    //SendKey(KeyBoard.Enter);
            //    ////base.Enter(model.rowIndex, selCol);
            //    ////return;

            //    //if (model.Sts == "CU" || model.Sts == "NO")
            //    //{
            //        base.SetText("Y", model.rowIndex, selCol);
            //        SendKey(KeyBoard.Enter);
            //    //    return;
            //    //}
            //    //else
            //    //{
            //    //    Message.Add("HANG Member a/c status is " + model.Sts + " ,pls manual checking");
            //    //    checked_MembershipNo(entityList);
            //    //    return;
            //    //}
            //}
            //else
            //{
            //    Message.Add("Member input info in the column which is different company, pls manual checking");
            //    checked_MembershipNo(entityList);
            //    return;
            //}
        }
        public bool SelectCase(SH795AccoutModel model, string scheme_Name , int selCol = 3, int stsCol = 66)
        {
            is_findcase = false; ;
            for (int i = 1; i < model.pageIndex; i++)
            {
                //SendKey(KeyBoard.PF3);
                base.PageDown();
            }
            //if (model.Sts == "CU" || model.Sts == "NO")
            //{
                base.SetText("Y", model.rowIndex, selCol);
                SendKey(KeyBoard.Enter);
                is_findcase = true;
            //}
            //if (model.MembershipNoInXml.StartsWith("2") && scheme_Name.StartsWith("HSBC"))
            //{
            //    //base.SetText("Y", model.rowIndex, selCol);
            //    //SendKey(KeyBoard.Enter);
            //    //return;

            //}
            //else if (model.MembershipNoInXml.StartsWith("3") && scheme_Name.StartsWith("HANG"))
            //{
            //    //
            //    //SendKey(KeyBoard.Enter);
            //    ////base.Enter(model.rowIndex, selCol);
            //    //return;

            //    if (model.Sts == "CU" || model.Sts == "NO")
            //    {
            //        base.SetText("Y", model.rowIndex, selCol);
            //        SendKey(KeyBoard.Enter);
            //        is_findcase = true; ;
            //    }
            //}
            //else
            //{
            //    Message.Add("Member input info in the column which is different company, pls manual checking");
            //}
            return is_findcase;
        }
    }
    public class SH795AccoutModel
    {
        public int pageIndex { get; set; }
        public int rowIndex { get; set; }
        public string SchemeID { get; set; }
        public string EmployerName  { get; set; }
        public string PayCID { get; set; }
        public string Client { get; set; }
        public string Sts { get; set; }
        public DateTime DJS { get; set; }
        public string SchemeIDInXml { get; set; }
        public string MembershipNoInXml { get; set; }
    }
}
