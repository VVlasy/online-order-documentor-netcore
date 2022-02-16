using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "product")]
    public class ExpandoItem
    {
        [XmlElement(ElementName = "code")]
        public string Code { get; set; }

        [XmlElement(ElementName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "ean")]
        public string EAN { get; set; }

        [XmlElement(ElementName = "stock")]
        public string Stock { get; set; }

        [XmlElement(ElementName = "price_vat")]
        public string PriceVat { get; set; }
    }

    [XmlRoot(ElementName = "products")]
    public class ExpandoXmlModel
    {
        [XmlElement(ElementName = "product")]
        public List<ExpandoItem> Products { get; set; }
    }
}
