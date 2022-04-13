using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "link")]
    public class Link
    {
        [XmlAttribute(AttributeName = "rel")]
        public string Rel { get; set; }

        [XmlAttribute(AttributeName = "href")]
        public string Href { get; set; }
    }

    [XmlRoot(ElementName = "entry")]
    public class Entry
    {
        [XmlElement(ElementName = "article")]
        public string Article { get; set; }

        [XmlElement(ElementName = "product_title")]
        public string ProductTitle { get; set; }

        [XmlElement(ElementName = "long_name")]
        public string LongName { get; set; }

        [XmlElement(ElementName = "description")]
        public string Description { get; set; }

        [XmlElement(ElementName = "product_url")]
        public string ProductUrl { get; set; }

        [XmlElement(ElementName = "image_url")]
        public string ImageUrl { get; set; }

        [XmlElement(ElementName = "additional_image_url")]
        public List<string> AdditionalImageUrl { get; set; }

        [XmlElement(ElementName = "availability")]
        public string Availability { get; set; }

        [XmlElement(ElementName = "stock_quantity")]
        public string StockQuantity { get; set; }

        [XmlElement(ElementName = "retail_price")]
        public string RetailPrice { get; set; }

        [XmlElement(ElementName = "wsp1_no_vat")]
        public string Wsp1NoVat { get; set; }

        [XmlElement(ElementName = "wsp2_no_vat")]
        public string Wsp2NoVat { get; set; }

        [XmlElement(ElementName = "wsp3_no_vat")]
        public string Wsp3NoVat { get; set; }

        [XmlElement(ElementName = "ean")]
        public string Ean { get; set; }

        [XmlElement(ElementName = "brand")]
        public string Brand { get; set; }

        [XmlElement(ElementName = "product_type")]
        public string ProductType { get; set; }

        [XmlElement(ElementName = "weight")]
        public string Weight { get; set; }

        [XmlElement(ElementName = "length")]
        public string Length { get; set; }

        [XmlElement(ElementName = "width")]
        public string Width { get; set; }

        [XmlElement(ElementName = "height")]
        public string Height { get; set; }
    }

    [XmlRoot(ElementName = "feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class LevenhukNewXmlModel
    {

        [XmlElement(ElementName = "title")]
        public string Title { get; set; }

        [XmlElement(ElementName = "link")]
        public Link Link { get; set; }

        [XmlElement(ElementName = "updated")]
        public string Updated { get; set; }

        [XmlElement(ElementName = "entry")]
        public List<Entry> Entry { get; set; }

        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }

        [XmlAttribute(AttributeName = "g")]
        public string G { get; set; }

        [XmlText]
        public string Text { get; set; }
    }
}
