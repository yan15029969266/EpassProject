using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
   public  class SM794  :ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SM794()
        {
            ContentTag tag = new ContentTag("SM794", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public void SetIdentifier(string value,int row=20,int col=14)
        {

            base.SetText(value,row,col);
        }

        public void SetEmpl_ID(string value, int row=4, int col=11)
        {

            base.SetText(value, row, col);
        }


        public void SetSelectOption(string value="D", int row=22, int col=18)
        {
            base.SetText(value, row, col);
        }

        public void SM794Enter(int row=22,int col=18)
        {
            SetCursorPos(row, col);
            SendKey(KeyBoard.Enter);
            //base.Enter(row,col);
        }

    }
}
