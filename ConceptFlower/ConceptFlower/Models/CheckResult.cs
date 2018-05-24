using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Models
{
   public class CheckResult
    {

        public string Level { get; set; }

        public TransferCase CaseItem { get; set; }

        private bool _status = true;
        public bool Status
        {
            get { return _status; }
            set { _status = value; }
        }

        //public bool Status { get; set; }

        public string Meassage { get; set; }

        public string OperationFlag { get; set; }

        private string _color = "Red";
        public string Color
        {
            get { return _color; }
            set { _color = value; }
        }

    }
}
