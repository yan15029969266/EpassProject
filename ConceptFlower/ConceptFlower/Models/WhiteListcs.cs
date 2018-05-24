﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Models
{
    public class WhiteListcs
    {
        public WhiteListcs()
        {
            ErrorCode = new StringBuilder();
        }
        public string PM_AC { get; set; } = string.Empty;

        public string Company { get; set; } = string.Empty;

        public string Membership_No { get; set; } = string.Empty;
        public string ERID { get; set; } = string.Empty;
        public string Case_Number { get; set; } = string.Empty;
        public string HKID { get; set; } = string.Empty;
        public string Name_in_AS400 { get; set; } = string.Empty;
        public string Name_in_XML { get; set; } = string.Empty;
        public string Name_Pass { get; set; } = string.Empty;
        public string Signature_Pass { get; set; } = string.Empty;
        public string Notepad_Pass { get; set; } = string.Empty;
        public string Clean_Case { get; set; } = string.Empty;

        private StringBuilder _unclean_case = new StringBuilder();
        public StringBuilder Unclean_Reason
        {
            

            get { return _unclean_case; }
            set { _unclean_case = value; }
        }
        public StringBuilder ErrorCode { get; set; }
        public string Addr_In_Form { get; set; }
        public string Withdraw_NT_PM { get; set; } = string.Empty;
        public string Letter_date { get; set; } = string.Empty;
        public string Process_result { get; set; } = string.Empty; 
    }
}
