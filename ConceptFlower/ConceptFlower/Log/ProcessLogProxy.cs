using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Log
{
    public static class ProcessLogProxy
    {
        public static Action<ProcessMsg> MessageAction;
        public static Action<string, string, int> Debug;
        public static Action<string, string> Message;
        public static Action<string> Info;
    }
}
