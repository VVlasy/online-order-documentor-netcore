using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "Pricing")]
    public class AlzaItemPricing
    {
        [XmlElement(ElementName = "PriceWithFee")]
        public string PriceWithFee { get; set; }

        [XmlElement(ElementName = "PriceWithoutFee")]
        public string PriceWithoutFee { get; set; }

        [XmlElement(ElementName = "RecycleFee")]
        public string RecycleFee { get; set; }

        [XmlElement(ElementName = "CopyrightFee")]
        public string CopyrightFee { get; set; }

        [XmlElement(ElementName = "Currency")]
        public string Currency { get; set; }
    }

    [XmlRoot(ElementName = "Storage")]
    public class AlzaItemStorage
    {
        [XmlElement(ElementName = "StoredQuantity")]
        public string StoredQuantity { get; set; }
    }

    [XmlRoot(ElementName = "Product")]
    public class AlzaItemProduct
    {

        [XmlElement(ElementName = "Name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "DealerCode")]
        public string DealerCode { get; set; }

        [XmlElement(ElementName = "PartNumber")]
        public string PartNumber { get; set; }

        [XmlElement(ElementName = "Ean")]
        public string Ean { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class AlzaItem
    {
        [XmlElement(ElementName = "Pricing")]
        public AlzaItemPricing Pricing{ get; set; }

        [XmlElement(ElementName = "Storage")]
        public AlzaItemStorage Storage { get; set; }

        [XmlElement(ElementName = "Product")]
        public AlzaItemProduct Product { get; set; }
    }

    [XmlRoot(ElementName = "items")]
    public class AlzaSupplierXmlModel
    {
        [XmlElement(ElementName = "item")]
        public List<AlzaItem> Items { get; set; }
    }
}
