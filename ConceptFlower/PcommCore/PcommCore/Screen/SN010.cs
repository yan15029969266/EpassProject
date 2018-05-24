using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
   public class SN010 :ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public SN010()
        {
            ContentTag tag = new ContentTag("SN010", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public string getClaimLodgmentDate()
        {

            return base.GetText(14,62,18).Trim();

        }

        public void setNotificationDate(string value,int row=10,int col=28)
        {

            base.SetText(value,row,col);
        }

        public void setTerminationDate(string value, int row = 11, int col = 28)
        {

            base.SetText(value, row, col);
        }


        public void setTerminationReason(string value="TP", int row = 12, int col = 28)
        {
            base.SetText(value, row, col);
        }


        public void setX()
        {
            base.SetText("X", 14, 40);
        }


        public void SetAutomatedRollover()

        {
            base.SetText("N", 21, 34);
        }


        public void SetEnter()

        {
            base.Enter(21,34);
        }
        public string GetErrorCode()
        {
            //Invalid member status
            return GetText(24, 2, 40).Trim();
        }
    }
}
