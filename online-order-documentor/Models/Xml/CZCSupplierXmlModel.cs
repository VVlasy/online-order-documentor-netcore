using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "SHOPITEM")]
    public class CZCShopItem
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "EAN")]
        public string EAN { get; set; }

        [XmlElement(ElementName = "QUANTITY")]
        public string Quantity { get; set; }

        [XmlElement(ElementName = "PRICE")]
        public string Price { get; set; }

        [XmlElement(ElementName = "LIST_PRICE")]
        public string ListPrice { get; set; }

        [XmlElement(ElementName = "WEIGHT")]
        public string Weight { get; set; }

        [XmlElement(ElementName = "RECYCLE_FEE")]
        public string RecycleFee { get; set; }

        [XmlElement(ElementName = "SIZE_Y")]
        public string SizeY { get; set; }

        [XmlElement(ElementName = "SIZE_X")]
        public string SizeX { get; set; }

        [XmlElement(ElementName = "SIZE_Z")]
        public string SizeZ { get; set; }
    }

    [XmlRoot(ElementName = "SHOP")]
    public class CZCSupplierXmlModel
    {
        [XmlElement(ElementName = "SHOPITEM")]
        public List<CZCShopItem> ShopItems { get; set; }
    }
}
