using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Log
{
    public class ProcessMsg
    {
        public string Color { get; set; }
        public string Msg { get; set; }
        public int Level { get; set; }

        public ProcessMsg()
        {
            this.Color = "Black";
        }
        public ProcessMsg(string msg)
        {
            this.Color = "Black";
            this.Msg = msg;
            this.Level = 1;
        }

        public ProcessMsg(string msg, string clr)
        {
            this.Color = clr;
            this.Msg = msg;
            this.Level = 1;
        }

        public ProcessMsg(string msg, string clr, int level)
        {
            this.Color = clr;
            this.Msg = msg;
            this.Level = level;
        }
    }
}
