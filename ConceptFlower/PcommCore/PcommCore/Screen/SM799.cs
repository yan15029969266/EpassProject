using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
  public   class SM799 :ScreenLogic
    {


        public ScreenDes screenDes = new ScreenDes();
        public SM799()
        {
            ContentTag tag = new ContentTag("SM799", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }
        public Dictionary<String,string> GetMemberInformation()
        {
            Dictionary<String, string> dic = new Dictionary<string, string>();

            var idNo=base.GetText(6,11,12).Trim();

            var Surname = base.GetText(8,14,15).Trim();

            var firatName = base.GetText(9,14,40).Trim();

            var mpfcomplant = base.GetText(2,46,30).Trim();

            var SensitiveFlag = base.GetText(4,77,2).Trim();

            var Bankruptcy = base.GetText(8,68,1).Trim();

            dic.Add("IDNO",idNo);
            dic.Add("SURNAME",Surname);
            dic.Add("FIRATNAME",firatName);
            dic.Add("MPFCOMPLANT", mpfcomplant);
            dic.Add("SENSFLAG", SensitiveFlag);
            dic.Add("BANKRUPTCY", Bankruptcy);
            return dic;
        }

        public void SetX(string value="X",int row=20,int Col=33)
        {
            base.SetText(value,row,Col);
        }


        public void Set_F20()

        {

            SendKey(KeyBoard.PF20);
        }


        public void Set_F3()

        {

            base.Exit();
        }

        public void Set_ShiftF8()
        {

            SendKey(KeyBoard.PF20);
        }


    }
}
