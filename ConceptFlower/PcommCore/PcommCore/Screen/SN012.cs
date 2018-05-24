using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
 public    class SN012 :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SN012()
        {
            ContentTag tag = new ContentTag("SN012", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void Set_Payment_Option(string value,int row=7, int col=52)
        {

            base.SetText(value,row,col);


        }


        public void Set_Case_Number(string value, int row = 9, int col = 16)
        {

            base.SetText(value, row, col);


        }



        public void Set_Withdrawal_Ground(string value, int row = 9, int col = 78)
        {

            base.SetText(value, row, col);


        }



        public void Set_Employer_Scheme_Transfer(string value, int row = 12, int col = 40)
        {

            base.SetText(value, row, col);


        }


        public string getErrorMessage(int row=24, int col=2)
        {
            return base.GetText(24, 2, 40).Trim();
        }


        public void SetEnter(int row =24,int col=3)
        {

            base.Enter(row,col);
        }


        public bool ContainMessage_confirm()
        {
          //  SendKey(KeyBoard.Enter);
            if (this.ReadRow(22).Contains("Press <ENTER> again to confirm"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
