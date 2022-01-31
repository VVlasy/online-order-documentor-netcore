using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName ="SHOPITEM")]
    public class MallShopItem
    {
        [XmlElement(ElementName = "ID")]
        public string ID { get; set; }

        [XmlElement(ElementName = "NAME")]
        public string Name { get; set; }

        [XmlElement(ElementName = "EAN")]
        public string EAN { get; set; }

        [XmlElement(ElementName = "STOCK")]
        public string Stock { get; set; }

        [XmlElement(ElementName = "PURCHASEPRICE")]
        public string PurchasePrice { get; set; }

        [XmlElement(ElementName = "PRICE_NO_VAT")]
        public string PriceNoVAT { get; set; }
        
        [XmlElement(ElementName = "sloupec07")]
        public string TODOSloupec07 { get; set; }
    }

    [XmlRoot(ElementName = "SHOP")]
    public class MallSupplierXmlModel
    {
        [XmlElement(ElementName ="SHOPITEM")]
        public List<MallShopItem>  ShopItems { get; set; }
    }
}
