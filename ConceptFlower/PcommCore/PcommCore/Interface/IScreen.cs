using PcommCore.Common;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace PcommCore.Interface
{
    public interface IScreen
    {
        bool WaitForScreen(ScreenDes screenKeyWords, int timeOut);
        /// <summary>
        /// Get one key and skip to another screen, 
        /// if skip success will return true,otherwise return false.
        /// </summary>
        /// <param name="key">KeyBoard's key</param>
        /// <returns></returns>
        bool isSkipScreen(out string Msg);
        /// <summary>
        /// The SetText method sends a string to the presentation space for the connection
        /// associated with the autECLPS object.
        /// </summary>
        /// <param name="Text">Content to send</param>
        /// <param name="Row">The row at which to begin the retrieval 
        /// from the presentation space</param>
        /// <param name="Col">The column position at which to begin the retrieval 
        /// from the presentation space.</param>
        void SetText(string Text, int Row, int Col);
        void SendKey(string key);
        CursorPos SearchText(string text);
        void SetCursorPos(int Row, int Col);
        string GetTextRect(int startRow, int startColumn, int endRow, int endColumn);
        string GetScreenContent();
        Bitmap CaptureAS400BlackScreen();
        List<string> pcommSessionNames();
    }
}
