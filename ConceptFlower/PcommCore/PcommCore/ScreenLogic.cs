/*
* ==============================================================================
*
* File name: ScreenLogic
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 3:42:30 PM
*
* Author: Haley X L Zhang
* Company: Chinasoft International
*
* ==============================================================================
*/
using PcommCore.Common;
using PcommCore.Interface;
using System;
using System.Reflection;
using System.Drawing;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Collections.Generic;
using System.Data;

namespace PcommCore
{
    public class ScreenLogic : IScreen
    {
        private dynamic connListObj = null;
        private dynamic renderObj = null;
        private dynamic autECLOIAObj = null;
        private Type connListType = Type.GetTypeFromProgID("PCOMM.autECLConnList");
        private Type autECLPSType = Type.GetTypeFromProgID("PCOMM.autECLPS");
        private Type screenDescType = Type.GetTypeFromProgID("PCOMM.autECLScreenDesc");
        private Type autECLOIAType = Type.GetTypeFromProgID("PCOMM.autECLOIA");
        private const string PCOMMAINWINDOWCLASSNAME = "PCSWS:Main:00400000";
        private const string PCOMBODYWINDOWCLASSNAME = "PCSWS:Pres:00400000";
        private const string PCOMSYSTEMINFORMATIONWINDOWCLASSNAME = "PCSWS:Oia:00400000";
        public static string sessionName = "A";
        private const string NORMALINFORMATION = "(C) COPYRIGHT IBM CORP. 1980, 2009.";
        public ScreenLogic()
        {
            connListObj = Activator.CreateInstance(connListType);
            renderObj = Activator.CreateInstance(autECLPSType);
            autECLOIAObj = Activator.CreateInstance(autECLOIAType);
            connListObj.Refresh();
            renderObj.SetConnectionByName(sessionName);
            autECLOIAObj.SetConnectionByName(sessionName);
        }
        public string GetTextRect(int startRow, int startColumn, int endRow, int endColumn)
        {
            var obj = renderObj.GetTextRect(startRow, startColumn, endRow, endColumn);
            return obj == null ? string.Empty : obj.ToString();
        }


        public List<string> pcommSessionNames()
        {
            List<string> pcommSessionList = new List<string>();
            connListObj.Refresh();
            int count = connListObj.Count;
            for (int i = 1; i <= count; i++)
            {
                pcommSessionList.Add(connListObj(i).Name);
            }
            return pcommSessionList;
        }


        public bool WaitForScreen(ScreenDes screenKeyWords, int timeOut)
        {
            autECLOIAObj.WaitForAppAvailable(timeOut);
            autECLOIAObj.WaitForInputReady(timeOut);
            object flag = renderObj.WaitForScreen(screenKeyWords.ScreenDesc, timeOut);
            return flag == null ? false : Convert.ToBoolean(flag);
        }

        public string GetText(int Row, int Col, int Len)
        {
            dynamic obj = renderObj.GetText(Row, Col, Len);
            return obj == null ? string.Empty : obj.ToString();
        }



        //There have an update.
        public bool isSkipScreen(out string msg)
        {
            msg = string.Empty;
            bool isSkip = CompareCaptureImage(() =>
            {
                renderObj.SendKeys(KeyBoard.Enter);
                return GetScreenAvalible();
            });

            msg = GetText(24, 2, 24).Trim();
            if ((isSkip && string.IsNullOrEmpty(msg)) || NORMALINFORMATION.Contains(msg)) { return true; }
            else { return false; }
        }
        public void SendKey(string key)
        {
            renderObj.SendKeys(key);
        }

        public CursorPos SearchText(string text)
        {
            CursorPos point = new CursorPos();
            int row = 1, col = 1;
            bool isFind = renderObj.SearchText(text, 1, ref row, ref col);
            if (isFind)
            {
                point.IsMatched = true;
                point.Row = row;
                point.Col = col;
            }
            return point;
        }

        public void SetCursorPos(int Row, int Col)
        {
            if (autECLOIAObj.WaitForAppAvailable(60000))
            {
                if (autECLOIAObj.WaitForInputReady(60000))
                {
                    renderObj.SetCursorPos(Row, Col);
                    renderObj.WaitForCursor(Row, Col, 10000);
                    autECLOIAObj.WaitForInputReady(60000);
                    renderObj.SendKeys(KeyBoard.Left);
                    autECLOIAObj.WaitForInputReady(60000);
                    renderObj.SendKeys(KeyBoard.Right);
                }
            }
        }

        public void SetText(string Text, int Row, int Col)
        {

            renderObj.SetText(Text, Row, Col);

        }
        public string GetScreenContent()
        {
            StringBuilder buffer = new StringBuilder();
            int count = 0;
            foreach (char character in GetText())
            {
                buffer.Append(character);
                count++;
                if (count == 80)
                {
                    buffer.Append("\r\n");
                    count = 0;
                }
            }
            return buffer.ToString();
        }

        public Bitmap CaptureAS400BlackScreen()
        {
            IntPtr as400HandleID = Win32.FindWindow(PCOMMAINWINDOWCLASSNAME, null);
            IntPtr hWnd = IntPtr.Zero;
            hWnd = Win32.FindWindowEx(as400HandleID, IntPtr.Zero, PCOMBODYWINDOWCLASSNAME, null);
            Win32.Rect rect = new Win32.Rect();
            Win32.GetWindowRect(hWnd, out rect);
            IntPtr hscrdc = Win32.GetWindowDC(hWnd);
            IntPtr hbitmap = Win32.CreateCompatibleBitmap(hscrdc, (rect.right - rect.left), rect.bottom - rect.top);
            IntPtr hmemdc = Win32.CreateCompatibleDC(hscrdc);
            Win32.SelectObject(hmemdc, hbitmap);
            Win32.PrintWindow(hWnd, hmemdc, 0);
            Bitmap image = Bitmap.FromHbitmap(hbitmap);
            Win32.DeleteDC(hscrdc);
            Win32.DeleteDC(hmemdc);
            return image;

        }

        #region 私有方法
        private string GetText()
        {
            var obj = renderObj.GetText();
            return obj == null ? string.Empty : obj.ToString();
        }

        private bool GetScreenAvalible()
        {

            if (autECLOIAObj.WaitForAppAvailable(60000))
            {
                if (autECLOIAObj.WaitForInputReady(60000))
                {
                    return true;
                }
            }
            return false;
        }

        private static bool CompareCaptureImage(Func<bool> action)
        {
            // IntPtr as400HandleID = Win32.FindWindow(PCOMMAINWINDOWCLASSNAME, null);
            //  IntPtr hWnd = IntPtr.Zero;
            // hWnd = Win32.FindWindowEx(as400HandleID, IntPtr.Zero, PCOMSYSTEMINFORMATIONWINDOWCLASSNAME, null);
            //  Bitmap imageBefore = CaptureScreen(hWnd);
            bool executeResult = action.Invoke();
            return executeResult;
            // Bitmap imageAfter = CaptureScreen(hWnd);
            // return ImageCompareString(imageBefore, imageAfter);
        }
        private static bool ImageCompareString(Bitmap firstImage, Bitmap secondImage)
        {
            MemoryStream ms = new MemoryStream();
            firstImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String firstBitmap = Convert.ToBase64String(ms.ToArray());
            ms.Position = 0;
            secondImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            String secondBitmap = Convert.ToBase64String(ms.ToArray());
            if (firstBitmap.Equals(secondBitmap))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static Bitmap CaptureScreen(IntPtr hWnd)
        {
            Win32.Rect rect = new Win32.Rect();
            Win32.GetWindowRect(hWnd, out rect);
            IntPtr hscrdc = Win32.GetWindowDC(hWnd);
            IntPtr hbitmap = Win32.CreateCompatibleBitmap(hscrdc, (rect.right - rect.left) / 2, rect.bottom - rect.top);
            IntPtr hmemdc = Win32.CreateCompatibleDC(hscrdc);
            Win32.SelectObject(hmemdc, hbitmap);
            Win32.PrintWindow(hWnd, hmemdc, 0);
            Bitmap image = Bitmap.FromHbitmap(hbitmap);
            Win32.DeleteDC(hscrdc);
            Win32.DeleteDC(hmemdc);
            return image;
        }

        public bool Exit()
        {
            SendKey(KeyBoard.PF3);
            string msg;
            return isSkipScreen(out msg);
        }


        public void SetF2()
        {
            SendKey(KeyBoard.PF2);
        }



        public bool Enter(int row, int col)
        {
            SetCursorPos(row, col);
            SendKey(KeyBoard.Enter);
            string msg;
            return isSkipScreen(out msg);
        }

      
        public bool Enter()
        {
            SendKey(KeyBoard.Enter);
            if (this.ReadRow(24).Contains("Press <ENTER> to confirm!"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public bool OverviewWarning()
        {
            SendKey(KeyBoard.PF13);
            string msg;
            return isSkipScreen(out msg);
        }

        public bool NextPage()
        {
            SendKey(KeyBoard.PF9);
            string msg;
            return isSkipScreen(out msg);
        }


        public bool IsHaveNext()
        {
            return GetTextRect(22, 72, 22, 80).Equals("More...");
        }

        public bool IsBottom()
        {
            return GetTextRect(22, 74, 22, 80).Trim().Equals("Bottom");
        }




        public bool NotepadHeaderIsHaveNext()
        {
            return GetTextRect(20, 72, 20, 80).Equals("More...");
        }

        public bool NotepadHeaderIsBottom()
        {
            return GetTextRect(20, 74, 20, 80).Trim().Equals("Bottom");
        }



        public void PageDown()
        {
            SendKey(KeyBoard.PageDown);
        }


        public void PageUp()
        {
            SendKey(KeyBoard.PageUp);
        }

        public string ReadRow(int index)
        {
            return GetTextRect(index, 1, index, 80);
        }

        public List<string> ReadRows(int start, int end)
        {
            List<string> list = new List<string>();
            for (int i = start; i <= end; i++)
            {
                list.Add(ReadRow(i));
            }
            return list;
        }


        public bool GoToMenu(string menu)
        {
            var a = SearchText(menu);
            if (a.IsMatched)
            {
                SetCursorPos(a.Row, a.Col);
                Enter();
                return true;
            }
            return false;
        }

        #endregion
    }
}
