/*
* ==============================================================================
*
* File name: User_TaskScrenn
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 5:00:40 PM
*
* Author: Haley X L Zhang
* Company: Chinasoft International
*
* ==============================================================================
*/
using PcommCore.Common;

namespace PcommCore.Screen
{
    public  class User_TasksScreen:ScreenLogic
    {
        public ScreenDes screenDes = new ScreenDes();
        public User_TasksScreen() {
            ContentTag tag = new ContentTag("User Tasks", 1, 36, 1, 45);
            screenDes.AddTag(tag);
        }
    }
}
