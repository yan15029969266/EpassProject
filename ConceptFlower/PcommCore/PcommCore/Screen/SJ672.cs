using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
    public class SJ672 :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SJ672()
        {
            ContentTag tag = new ContentTag("SJ672", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public string getContent()
        {
           return base.GetScreenContent();
        }



        public void SetRemark(string value , int row = 7, int col = 1)

        {
            base.SetText(value, row, col);

        }


        public void SetEnter()
        {
            base.Enter(7,1);

        }

    }
}
