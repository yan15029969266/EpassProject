using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcommCore.Common;
using PcommCore.Screen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                List<string> names = IBM_i_Main_MenuScreen.pcommSessionNames();
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


                CursorPos point = IBM_i_Main_MenuScreen.SearchText("User tasks");

                return true;
            });
        }

        [TestMethod]
        public void SM799SkipToSJ671()

        {
            PcommCore pcommCore = new PcommCore("A");
            SM799 s_sm7799 = pcommCore.GetScreen<SM799>();
            //s_sm7799.Set_F2();
            s_sm7799.Set_ShiftF8();
        }

        [TestMethod]
        public void TestNO()
        {
            Regex reg1 = new Regex(@"^(\d{8})(\D)(\d{9})$");
            string a = "12345678-123456789";
            string b = "12345678 123456789";
            string c = @"12345678/123456789";
            if (reg1.IsMatch(a))
            {
                string a1 = reg1.Match(a).Groups[1].Value;
                string a2 = reg1.Match(a).Groups[3].Value;
            }
            if (reg1.IsMatch(b))
            {
                string b1 = reg1.Match(b).Groups[1].Value;
                string b2 = reg1.Match(b).Groups[3].Value;
            }
            if (reg1.IsMatch(c))
            {
                string c1 = reg1.Match(c).Groups[1].Value;
                string c2 = reg1.Match(c).Groups[3].Value;
            }
        }
        [TestMethod]
        public void TestSubString()
        {
            string a = "12345678123456789";
            string SchemeID = a.Substring(0, 8);
            string MembershipNo = a.Substring(8, 9);
        }
        [TestMethod]
        public void Distinct()
        {
            StringBuilder sb = new StringBuilder("1,1,2,3,3,5,");
            List<string> errorCodeList = sb.ToString().Split(',').ToList();
            errorCodeList = errorCodeList.Where(t => t != "").ToList();
            errorCodeList = errorCodeList.Distinct().ToList();
            string erroCode = string.Empty;
            foreach(string code in errorCodeList)
            {
                erroCode = erroCode == "" ? code : erroCode + "," + code;
            }
            sb = new StringBuilder(erroCode);
        }
        [TestMethod]
        public void GetChineseChar()
        {
            string s = "郭麗玲                               ";
            Regex reg = new Regex(@"[\u4E00-\u9FA5]+");
            string chineseStr = reg.Match(s).Value;
        }

        [TestMethod]
        public void TestSelectCaseInSN018()
        {
            PcommCore pcommCore = new PcommCore("A");
            SN018 sn018 = pcommCore.GetScreen<SN018>();
            sn018.SelectClientNO("98039761");
        }
        [TestMethod]
        public void TestCode()
        {
            string[] sArray = { "1", "2", "3", "15" };
            for(int i=0;i<sArray.Length; i++)
            {
                if (sArray[i].Length == 1)
                {
                    sArray[i] = "0" + sArray[i];
                }
            }
        }
    }
}