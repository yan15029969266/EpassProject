using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
    public class S0017 :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public S0017()
        {
            ContentTag tag = new ContentTag("S0017", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public void SetCompany(string value,int row=3, int col=46)
        {
            base.SetText(value,row,col);
        }

        public bool GotoMemberDetails(string value = "Member Details")
        {
          return  base.GoToMenu(value);
        }

        public bool GotoePass(string value = "ePass")
        {
            return base.GoToMenu(value);
        }


    }
}
