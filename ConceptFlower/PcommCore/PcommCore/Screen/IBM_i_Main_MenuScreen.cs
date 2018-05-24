/*
* ==============================================================================
*
* File name: IBM_i_Main_MenuScreen
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 4:29:31 PM
*
* Author: Haley X L Zhang
* Company: Chinasoft International
*
* ==============================================================================
*/
using PcommCore.Common;

namespace PcommCore.Screen
{
    public class IBM_i_Main_MenuScreen : ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public IBM_i_Main_MenuScreen()
        {
            ContentTag tag = new ContentTag("IBM i Main Menu", 1, 33, 1, 47);
            screenDes.AddTag(tag);

            throw new System.Exception("this is test ");

        }    
    }
}
