using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
    public class SH655 : ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SH655()
        {
            ContentTag tag = new ContentTag("SH655", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public bool IsNTmatched()
        {
            bool isMatch = false;
            string member = string.Empty;
            for(int i=13;i<21;i++)
            {
                member = GetText(i, 15, 2);
                if(member=="NT")
                {
                    isMatch = true;
                    break;
                }
            }
            //return base.SearchText("NT").IsMatched;
            return isMatch;
        }
    }
}
