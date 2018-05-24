/*
* ==============================================================================
*
* File name: PcommCore
* Description: Base on Personal Communications for Windows, Version 6.0
* Host Access Class Library
*
* Version: 1.0
* Created: 12/21/2017 3:56:08 PM
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

namespace PcommCore
{
    public class PcommCore : IPcomm
    {

        private const string skipKeyBoard = KeyBoard.Enter;

        private PcommCore() { }
        public PcommCore(string sessionName)
        {
            ScreenLogic.sessionName = sessionName;
        }

        public IPcomm LinkToScreen<IScreen>(Predicate<IScreen> action) where IScreen : new()
        {
            //if (action != null)
            //{
            //    dynamic instance = new IScreen();
            //    dynamic screenDes = instance.screenDes;
            //    bool isWaitSucess = (bool)instance.WaitForScreen(screenDes, 1000);
            //    //  bool waitForSuccess = IBM_i_Main_MenuScreen.WaitForScreen(screenDes, 1000);
            //    if (isWaitSucess)
            //    {
            //        bool result = action.Invoke(instance);
            //        if (result)
            //        {
            //            return this;
            //        }
            //        else
            //        {
            //            return null;
            //        }
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}
            //else
            //{
            //    return null;
            //}

            bool isExecuteActionSuccess = false;
            dynamic instance = new IScreen();
            dynamic screenDes = instance.screenDes;
            bool isWaitSucess = (bool)instance.WaitForScreen(screenDes, 1000);
            if (isWaitSucess)
            {
                isExecuteActionSuccess = action.Invoke(instance);
            }
            if (isExecuteActionSuccess)
            {
                return this;
            }
            else
            {
                throw new Exception("Screen " + instance.GetType().Name + " run failed.");
            }
        }

        public IPcomm SkipToHomeScreen<IScreen>() where IScreen : new()
        {
            dynamic instance = new IScreen();
            dynamic screenDes = instance.screenDes;
            bool isWaitSucess = (bool)instance.WaitForScreen(screenDes, 1000);
            while (isWaitSucess == false)
            {
                instance.SendKey(KeyBoard.PA1);
                instance.SendKey(KeyBoard.PF3);
                isWaitSucess = (bool)instance.WaitForScreen(screenDes, 1000);

            }
            return this;
        }

        public T GetScreen<T>() where T : IScreen, new()
        {
            dynamic instance = new T();
            dynamic screenDes = instance.screenDes;
            bool isWaitSucess = false;

            int? counter = default(int);

            while (!isWaitSucess && counter <= 5)
            {
                isWaitSucess = (bool)instance.WaitForScreen(screenDes, 1000);
                counter++;
            }

            return instance;
        }







        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
