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
            return base.SearchText("NT").IsMatched;
        }
    }
}
