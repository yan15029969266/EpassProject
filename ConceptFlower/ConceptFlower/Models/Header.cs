using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConceptFlower.Models
{
    [XmlRootAttribute("Header")]
    public class Header
    {
        [XmlElementAttribute("SenderID")]
        public string SenderID;
        [XmlElementAttribute("RecipientID", IsNullable = false)]
        public string RecipientID { get; set; }
        public string MessageType { get; set; }
        public string VersionNo { get; set; }
        public string MessageRef { get; set; }
        public string Timestamp { get; set; }

    }
}
