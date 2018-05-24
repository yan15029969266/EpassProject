using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PcommCore.Screen
{
  public   class SG761 :ScreenLogic
    {



        public ScreenDes screenDes = new ScreenDes();
        public SG761()
        {
            ContentTag tag = new ContentTag("SG761", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }



        public void setIncorrectIdentityno(string value="Y",int row=12,int col=76)

        {
            base.SetText(value,row,col);


        }


        public string GetIncorrectIdentityno( int row = 12, int col = 76)

        {
           return base.GetText( row, col,60).Trim();


        }


        public void setIncorrect_mbrship(string value = "Y", int row = 14, int col = 76)

        {
            base.SetText(value, row, col);


        }

        public string GetIncorrect_mbrship( int row = 14, int col = 76)

        {
            return base.GetText(row, col, 60).Trim();


        }


        public void setIncorrectOriginal_Scheme_ID(string value = "Y", int row = 16, int col = 76)

        {
            base.SetText(value, row, col);


        }


        public string GetIncorrectOriginal_Scheme_ID(int row = 16, int col = 76)

        {
            return base.GetText(row, col, 60).Trim();


        }




        public void setMismatch_member_signature(string value = "Y", int row = 17, int col = 76)

        {
            base.SetText(value, row, col);


        }



        public string GetMismatch_member_signature(int row = 17, int col = 76)

        {
            return base.GetText(row, col, 60).Trim();


        }


        public void SetEnter(int row = 17, int col = 76)
        {
            base.Enter(row,col);
        }

        public void SetCodeInSG761(string[] codes, PcommCore pcommCore)
        {
            for (int i = 0; i < codes.Length; i++)
            {
                if (codes[i].Length == 1)
                {
                    codes[i] = "0" + codes[i];
                }
            }
            List<CodeModel> codeList = new List<CodeModel>();
            Thread.Sleep(500);
            //List<string> msgList = base.ReadRows(9, 22);
            for(int row =12;row<22;row++)
            {
                CodeModel model = new CodeModel
                {
                    code = base.GetText(row, 4, 2).Trim(),
                    row = row
                };
                codeList.Add(model);
            }
            var selectedList=codeList.Where(t => codes.Contains(t.code));
            foreach(var mode in selectedList)
            {
                SetText("Y", mode.row, 76);
            }
            if(IsHaveNext())
            {
                PageDown();
                SetCodeInSG761(codes, pcommCore);
                //List
            }
            else//如果是最后一页则对Ｃｏｄｅ进行操作
            {
                CodeModel code99 = codeList.Where(t => t.code == "99").FirstOrDefault();
                if(code99!=null)
                {
                    SetText("X", code99.row, 60);
                    SetText("Y", code99.row, 76);
                    SendKey(KeyBoard.Enter);
                    //PcommCore.PcommCore pcommCore = new PcommCore.PcommCore("A");
                    SG763 sg763 = pcommCore.GetScreen<SG763>();
                    sg763.SetRemark("Please be reminded to submit statement to certify signature if necessary next time.");
                    SetEnter();
                }
            }
        }

        public new bool IsHaveNext()
        {
            return GetTextRect(22, 72, 22, 80).Trim().Equals("More...");
        }

    }
    public class CodeModel
    {
        public string code;
        public int row;
    }
}
