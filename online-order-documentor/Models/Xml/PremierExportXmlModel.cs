using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "que_txt")]
    public class que_txt
    {

        [XmlElement(ElementName = "sloupec01")]
        public string Sloupec01 { get; set; }

        [XmlElement(ElementName = "sloupec02")]
        public string Sloupec02 { get; set; }

        [XmlElement(ElementName = "sloupec03")]
        public string Sloupec03 { get; set; }

        [XmlElement(ElementName = "sloupec04")]
        public string Sloupec04 { get; set; }

        [XmlElement(ElementName = "sloupec05")]
        public string Sloupec05 { get; set; }

        [XmlElement(ElementName = "sloupec06")]
        public string Sloupec06 { get; set; }

        [XmlElement(ElementName = "sloupec07")]
        public string Sloupec07 { get; set; }

        [XmlElement(ElementName = "sloupec08")]
        public string Sloupec08 { get; set; }

        [XmlElement(ElementName = "sloupec09")]
        public string Sloupec09 { get; set; }

        [XmlElement(ElementName = "sloupec10")]
        public string Sloupec10 { get; set; }

        [XmlElement(ElementName = "sloupec11")]
        public string Sloupec11 { get; set; }
    }

    [XmlRoot(ElementName = "VFPData")]
    public class PremierExportXmlModel
    {

        [XmlElement(ElementName = "que_txt")]
        public List<que_txt> que_txt { get; set; }
    }
}
