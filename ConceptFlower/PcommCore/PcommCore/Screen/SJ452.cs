using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
   public  class SJ452 :ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public SJ452()
        {
            ContentTag tag = new ContentTag("SJ452", 1, 72, 1, 76);
            screenDes.AddTag(tag);
           
        }


        public string  Get_PM_RecDate(int row=11 ,int col=13)

        {
            return base.GetText(row,col,11).Trim();

        }

        public void SetCcDate(string value,int row = 11, int col = 24)

        {
             base.SetText(value,row,col);

        }



        public string Get_AC_RecDate(int row = 15, int col = 50)

        {
            return base.GetText(row, col, 11).Trim();

        }

        public void Set_AC_CCDate(string value, int row = 15, int col = 61)

        {
            base.SetText(value, row, col);

        }


        public void set_PM_AC(string value="Y", int row = 20, int col = 17)
        {

            base.SetText(value, row, col);
        }

        public bool set_Enter_confirm()
        {
            if (GetText(24, 2, 30).Trim().Contains("Press <ENTER> to confirm"))
            {
                return true;
            }
            else
            {
                return false;
            }
            //return base.Enter();
        }

        public bool set_Enter(int row = 20, int col = 17)
        {
            return base.Enter(row,col);
        }


        public string get_Message(int row = 24, int col = 02)
        {
            return base.GetText(row,col,30).Trim();
        }


    }
}
