using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Log
{
    public class LogHelper
    {
        private static readonly log4net.ILog logInfo = log4net.LogManager.GetLogger("loginfo");
        private static readonly log4net.ILog logError = log4net.LogManager.GetLogger("logerror");

        public static void Info(string info)
        {
            Task.Run(() => {
                if (logInfo.IsInfoEnabled)
                {
                    logInfo.Info(info);
                }
            });
        }

        public static void Debug(string info)
        {
            Task.Run(() => {
                if (logInfo.IsDebugEnabled)
                {
                    logInfo.Debug(info);
                }
            });
        }

        public static void Error(string info, Exception se)
        {
            Task.Run(() =>
            {
                if (logError.IsErrorEnabled)
                {
                    logError.Error(info, se);
                }
            });
        }

        public static  void BindingProcess(ObservableCollection<ProcessMsg> LogList)
        {
            var dispatcher = App.Current.Dispatcher;

            ProcessLogProxy.MessageAction = ((x) =>
            {
                dispatcher.Invoke(() =>
                {
                    LogList.Add(x);
                });
            });

            ProcessLogProxy.Info = ((x) =>
            {
                dispatcher.Invoke(() =>
                {
                    LogList.Add(new ProcessMsg(x));
                });
            });

            ProcessLogProxy.Message = ((x, y) =>
            {
                dispatcher.Invoke(() =>
                {
                    LogList.Add(new ProcessMsg(x, y));
                });
            });

            ProcessLogProxy.Debug = ((x, y, z) =>
            {
                dispatcher.Invoke(() =>
                {
                    LogList.Add(new ProcessMsg(x, y, z));
                });
            });
        }

    }
}
