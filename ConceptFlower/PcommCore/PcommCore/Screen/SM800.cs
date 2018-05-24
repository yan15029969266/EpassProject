using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PcommCore.Screen
{
  public  class SM800 :ScreenLogic
    {


        public ScreenDes screenDes = new ScreenDes();
        public SM800()
        {
            ContentTag tag = new ContentTag("SM800", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public  Dictionary<string,string>  GetMemberChineseInformation()
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();

            string str = base.GetText(13, 14, 40).Trim();
            Regex reg = new Regex(@"[\u4E00-\u9FA5]+");
            string chinesename = String.Empty;
            if (reg.IsMatch(str))
            {
                chinesename = reg.Match(str).Value;
            }
            dic.Add("CHINESENAME",chinesename);
            return dic;
        }

        public void SetF3()

        {
            base.Exit();
        }

    }
}
