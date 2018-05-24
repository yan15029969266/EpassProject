using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace PcommCore.Screen
{
    public class SN018 : ScreenLogic
    {

        public ScreenDes screenDes = new ScreenDes();
        public SN018()
        {
            ContentTag tag = new ContentTag("SN018", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        //public bool SelectSchemeID(string clientno, int selCol = 4, int xCol = 16, int PreservedCol = 39, int NonPresCol = 50)
        //{

        //    CursorPos point = base.SearchText(clientno);

        //    if (point.IsMatched)
        //    {
        //        base.SetText("Y", selCol, point.Row);

        //        base.SetText("X", xCol, point.Row);

        //        base.SetText("100", PreservedCol, point.Row);

        //        base.SetText("100", NonPresCol, point.Row);

        //        base.Enter(selCol,point.Row);

        //        return point.IsMatched;
        //    }
        //    else
        //    {
        //        return point.IsMatched;
        //    }
        //}
        public bool SelectClientNO(string clientNo, string tips="",int selCol = 4, int xCol = 16, int PreservedCol = 39, int NonPresCol = 50)
        {
            CursorPos point = base.SearchText(clientNo);

            if (point.IsMatched)
            {
                base.SetText("Y", point.Row, selCol);

                base.SetText("X", point.Row, xCol);

                base.SetText("100", point.Row, PreservedCol);

                base.SetText("100", point.Row, NonPresCol);

                base.Enter(point.Row, selCol);

                return point.IsMatched;
            }
            else
            {
                string currnetTips = GetText(17, 7, 9);
                if (currnetTips == tips)
                {
                    SendKey(KeyBoard.Reset);
                    return point.IsMatched;
                }
                else
                {
                    PageUp();
                    Thread.Sleep(500);
                    return SelectClientNO(clientNo, currnetTips);
                }
                //if (tips == "+")
                //{
                //    PageUp();
                //    return SelectSchemeID(clientNo);
                //}
                //else
                //{
                //    
                //}
            }
        }
    }
}
