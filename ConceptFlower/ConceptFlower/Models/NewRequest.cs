using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ConceptFlower.Models
{
    [Serializable]
    [XmlRoot("NewRequest")]
    public class NewRequest 
    {
        [XmlElementAttribute("Header", IsNullable = false)]
        public Header Header { get; set; }
        //[XmlArray("TransferCase")]
        //[XmlElementAttribute("TransferCase", IsNullable = false)]
        //[XmlArrayItem("TransferCase")]
        [XmlElement("TransferCase")]
        public List<TransferCase> TransferCase { get; set; }
    }
}
