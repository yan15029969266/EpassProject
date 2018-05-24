using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConceptFlower.Models
{
    [Serializable]
    //[XmlRoot("TransferCase")]
    //[XmlArrayItem("TransferCase")]
    [XmlType("TransferCase")]
    public class TransferCase
    {
        public string SenderSchRegNo { get; set; }
        public string ReceiverSchRegNo { get; set; }
        public string TransferCaseNo { get; set; }
        public string NewTRIntRefNo { get; set; }
        public string SubmissionType { get; set; }
        public string NewTRAprvlNo { get; set; }
        public string NewSchRegNo { get; set; }
        public string OriTRAprvlNo { get; set; }
        public string OriSchRegNo { get; set; }
        public string RequestReceiptDt { get; set; }
        public string OriSchAcctMemNo { get; set; }
        public string MemHKIDNo { get; set; }
        [XmlElementAttribute(IsNullable = true)]
        public string MemPassportNo { get; set; }
        public string MemHKIDCheckDigit { get; set; }
        public string MemEngName { get; set; }
        public string MemChiName { get; set; }
        public string RequestFormType { get; set; }
        [XmlElementAttribute(IsNullable = true)]
        public string TransferEEMC { get; set; } 
        //[XmlIgnore]
        [XmlElementAttribute(IsNullable = true)] 
        public string TransferEEVC { get; set; }
        [XmlElementAttribute(IsNullable = true)]
        public string TransferFRMC { get; set; }
        [XmlElementAttribute(IsNullable = true)]
        public string TransferFRVC { get; set; }
        public string TransferMCAndVC { get; set; }
        public string SEPCessationEffDt { get; set; }
        public string LastContPaidUpDt { get; set; }

        public string FormImageCode { get; set; }
    }
}
