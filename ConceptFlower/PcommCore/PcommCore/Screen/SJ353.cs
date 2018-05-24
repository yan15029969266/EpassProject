using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
  public   class SJ353 :ScreenLogic
    {

        public List<string> Message { get; set; }
        public ScreenDes screenDes = new ScreenDes();
        public SJ353()
        {
            ContentTag tag = new ContentTag("SJ353", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }


        public void SelectSchemCode(string trustId, int selCol = 6)
        {

            if (base.IsHaveNext())
            {
                CursorPos point = base.SearchText(trustId);

                if (point.IsMatched)

                {
                    base.SetText("Y", selCol, point.Row);

                    base.Enter(point.Row, point.Col);
                }
                else
                {
                    base.PageDown();

                    SelectSchemCode(trustId, selCol);

                }

            }
            else if (base.IsBottom())
            {
                CursorPos point = base.SearchText(trustId);
                base.SetText("Y", selCol, point.Row);

                if (point.IsMatched)

                {
                    base.SetText("Y", point.Row, selCol);

                    base.Enter(point.Row, point.Col);
                }
                else
                {

                    Message.Add("can't find the trustId" + trustId);
                    //throw new Exception("can't find the member");

                }
            }

        }









    }
}
