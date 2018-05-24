using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
   public  class SG756 :ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public SG756()
        {
            ContentTag tag = new ContentTag("SG756", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void Set_SchemeId(string value ,int Row=4,int col=12)
        {


            base.SetText(value,Row,col);
        }

        public void Set_Id_No(string value, int row=5, int col=22)
        {

            base.SetText(value, row, col);

        }
        
        public void Set_Form(string value, int row=8, int col=9)
        {

            base.SetText(value, row, col);

        }
        public void Set_Case_Number(string value, int row=8, int col=27)
        {
            base.SetText(value, row, col);

        }

        public void Set_option(string value ="A", int row = 21, int col = 18)
        {
            base.SetText(value, row, col);

        }


        public void Set_Enter(int row = 21, int col = 18)
        {
            base.Enter(row, col);

        }

        public string GetWarningMessage(int row=24,int col=2)
        {
            return base.GetText(row,col,40).Trim();
        }


    }
}
