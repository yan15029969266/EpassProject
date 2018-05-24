using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
 public    class SN014 :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SN014()
        {
            ContentTag tag = new ContentTag("SN014", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void SetEnter(int row =1 ,int col =1)
        {


            base.Enter(row,col);

        }





    }
}
