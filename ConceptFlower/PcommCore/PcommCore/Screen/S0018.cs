using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcommCore.Screen
{
    public class S0018 : ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public S0018()
        {
            ContentTag tag = new ContentTag("S0018", 1, 72, 1, 76);
            screenDes.AddTag(tag);
        }

        public bool GotoMemberSchemeMaint(string menue = "Member Scheme Maint.")
        {
            return base.GoToMenu(menue);
        }


        public bool GotoMemberTerminations(string menue = "Member Terminations")
        {
            return base.GoToMenu(menue);
        }





        public bool GotoUnitMovements(string menue = "Unit Movements")
        {
            return base.GoToMenu(menue);
        }



        public bool GotoRejection_Letter(string menue = "Rejection Letter")
        {
            return base.GoToMenu(menue);
        }


    }
}
