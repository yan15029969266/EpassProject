using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
  public  class SN008 :ScreenLogic
    {


        public ScreenDes screenDes = new ScreenDes();
        public SN008()
        {
            ContentTag tag = new ContentTag("SN008", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void setScheme(string value,int row=4 ,int col=11)
        {

            base.SetText(value,row,col);

        }

        public void setID(string value, int row=21, int col=8)
        {

            base.SetText(value, row, col);

        }


        public void setOption(string value="A", int row=22, int col=18)
        {

            base.SetText(value, row, col);

        }


        public void setEnter( int row = 22, int col = 18)
        {
            base.Enter(row,col);
        }


        public string GetMessage(int row=24, int col=2)
        {
           return  base.GetText(24,2,40).Trim();
        }

        public bool ContainMessage_processed()
        {
            SendKey(KeyBoard.Enter);
            if (this.ReadRow(24).Contains("Last transaction processed!"))
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
