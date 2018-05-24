using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
 public    class SG763 :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SG763()
        {
            ContentTag tag = new ContentTag("SG763", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public void SetRemark(string value,int row=12 ,int col=11)
        {

            base.SetText(value,row,col);
        }

        public void SetEnter(string value, int row = 12, int col = 11)
        {
            base.Enter(row,col);
        }





    }
}
