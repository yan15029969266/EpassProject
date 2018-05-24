using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
    class SN521 : ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SN521()
        {
            ContentTag tag = new ContentTag("SN521", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void SetHkid(string value, int row = 11, int col = 12)
        {
            base.SetText(value, row, col);

        }


        public void SetOption(string value = "F", int row = 21, int col = 40)
        {


            base.SetText(value, row, col);
        }

        public void SetEnter()
        {
            base.Enter(21, 40);

        }

    }
}
