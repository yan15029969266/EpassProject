using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcommCore.Common;
using PcommCore.Screen;
using System.Collections.Generic;
/*
* ==============================================================================
*
* File name: PcommCoreTests
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 4:22:09 PM
*
* Author: Haley X L Zhang
* Company: Chinasoft International
*
* ==============================================================================
*/

namespace PcommCore.Tests
{
    [TestClass()]
    public class PcommCoreTests
    {
        [TestMethod()]
        public void PcommCoreSkipToHomeScreen()
        {
            PcommCore pcommCore = new PcommCore("A");
            pcommCore.SkipToHomeScreen<IBM_i_Main_MenuScreen>();
        }

        [TestMethod()]
        public void PcommCoreTest()
        {
            PcommCore pcommCore = new PcommCore("A");
            pcommCore.LinkToScreen<IBM_i_Main_MenuScreen>((IBM_i_Main_MenuScreen) =>
            {


                IBM_i_Main_MenuScreen.SetCursorPos(2, 4);
               List<string> names= IBM_i_Main_MenuScreen.pcommSessionNames();
                string errorMsg = string.Empty;
                string message = IBM_i_Main_MenuScreen.GetTextRect(7, 6, 7, 80);
                IBM_i_Main_MenuScreen.SetText("1", 20, 7);
                bool isSkip = IBM_i_Main_MenuScreen.isSkipScreen(out errorMsg);
                return isSkip;
            }).LinkToScreen<User_TasksScreen>((User_TasksScreen) =>
            {
                User_TasksScreen.SetText("1", 20, 7);
                return true;
            }).LinkToScreen<CommonScreen>((commonScreen) =>
            {
                commonScreen.SendKey(KeyBoard.PA1);
                commonScreen.SendKey(KeyBoard.PF3);
                return false;
            });
        }



        [TestMethod()]
        public void PcommCoreTestsSetCursorPos()
        {
            PcommCore pcommCore = new PcommCore("A");
            pcommCore.LinkToScreen<IBM_i_Main_MenuScreen>((IBM_i_Main_MenuScreen) =>
            {


              CursorPos point=  IBM_i_Main_MenuScreen.SearchText("User tasks");

                return true;
            });
        }



    }
}