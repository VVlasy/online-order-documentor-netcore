using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "IMAGES")]
    public class IMAGES
    {
        [XmlElement(ElementName = "IMAGE")]
        public List<string> IMAGE { get; set; }
    }

    [XmlRoot(ElementName = "STOCK")]
    public class STOCK
    {
        [XmlElement(ElementName = "AMOUNT")]
        public int AMOUNT { get; set; }
    }

    [XmlRoot(ElementName = "INFORMATION_PARAMETER")]
    public class INFORMATIONPARAMETER
    {

        [XmlElement(ElementName = "NAME")]
        public string NAME { get; set; }

        [XmlElement(ElementName = "VALUE")]
        public string VALUE { get; set; }
    }

    [XmlRoot(ElementName = "INFORMATION_PARAMETERS")]
    public class INFORMATIONPARAMETERS
    {

        [XmlElement(ElementName = "INFORMATION_PARAMETER")]
        public List<INFORMATIONPARAMETER> INFORMATIONPARAMETER { get; set; }
    }

    [XmlRoot(ElementName = "SHOPITEM")]
    public class SHOPITEM
    {

        [XmlElement(ElementName = "NAME")]
        public string NAME { get; set; }

        [XmlElement(ElementName = "CODE")]
        public string CODE { get; set; }

        [XmlElement(ElementName = "EAN")]
        public string EAN { get; set; }

        [XmlElement(ElementName = "IMAGES")]
        public IMAGES IMAGES { get; set; }

        [XmlElement(ElementName = "PRICE")]
        public string PRICE { get; set; }

        [XmlElement(ElementName = "PRICE_VAT")]
        public string PRICEVAT { get; set; }
        [XmlElement(ElementName = "PURCHASE_PRICE")]
        public string PURCHASEPRICE { get; set; }
        [XmlElement(ElementName = "STANDARD_PRICE")]
        public string STANDARDPRICE { get; set; }
        [XmlElement(ElementName = "MANUFACTURER")]
        public string MANUFACTURER { get; set; } 
[XmlElement(ElementName = "DESCRIPTION")]
        public string DESCRIPTION { get; set; }
        [XmlElement(ElementName = "SHORT_DESCRIPTION")]
        public string SHORTDESCRIPTION { get; set; }

        [XmlElement(ElementName = "STOCK")]
        public STOCK STOCK { get; set; }

        [XmlElement(ElementName = "WEIGHT")]
        public string WEIGHT { get; set; }

        [XmlElement(ElementName = "INFORMATION_PARAMETERS")]
        public INFORMATIONPARAMETERS INFORMATIONPARAMETERS { get; set; }

        [XmlElement(ElementName = "AVAILABILITY_OUT_OF_STOCK")]
        public string AVAILABILITYOUTOFSTOCK { get; set; }

        [XmlElement(ElementName = "AVAILABILITY_IN_STOCK")]
        public string AVAILABILITYINSTOCK { get; set; }
    }

    [XmlRoot(ElementName = "SHOP")]
    public class ShoptetXmlFeed
    {
        [XmlElement(ElementName = "SHOPITEM")]
        public List<SHOPITEM> SHOPITEM { get; set; }
    }

}
