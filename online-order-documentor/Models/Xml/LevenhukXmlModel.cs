using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Models.Xml
{
    [XmlRoot(ElementName = "PARAMETR")]
    public class PARAMETR
    {

        [XmlAttribute(AttributeName = "TYP")]
        public string TYP { get; set; }

        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "PARAMETRY")]
    public class PARAMETRY
    {

        [XmlElement(ElementName = "PARAMETR")]
        public List<PARAMETR> PARAMETR { get; set; }
    }

    [XmlRoot(ElementName = "POLOZKA")]
    public class POLOZKA
    {

        [XmlElement(ElementName = "NAZEV")]
        public string NAZEV { get; set; }

        [XmlElement(ElementName = "KOD")]
        public string KOD { get; set; }

        [XmlElement(ElementName = "EAN")]
        public string EAN { get; set; }

        [XmlElement(ElementName = "OBRAZEK")]
        public string OBRAZEK { get; set; }

        [XmlElement(ElementName = "CENA_DOPORUCENA")]
        public string CENADOPORUCENA { get; set; }

        [XmlElement(ElementName = "CENA_BEZ_DPH")]
        public string CENABEZDPH { get; set; }

        [XmlElement(ElementName = "CENA_S_DPH")]
        public string CENASDPH { get; set; }

        [XmlElement(ElementName = "POPIS")]
        public string POPIS { get; set; }

        [XmlElement(ElementName = "ZASOBA")]
        public int ZASOBA { get; set; }

        [XmlElement(ElementName = "NACESTE")]
        public int NACESTE { get; set; }

        [XmlElement(ElementName = "ZARUKA")]
        public int ZARUKA { get; set; }

        [XmlElement(ElementName = "HMOTNOST")]
        public string HMOTNOST { get; set; }

        [XmlElement(ElementName = "PARAMETRY")]
        public PARAMETRY PARAMETRY { get; set; }

        [XmlElement(ElementName = "RECYKLACNI_PRISPEVEK")]
        public string RECYKLACNIPRISPEVEK { get; set; }

        [XmlElement(ElementName = "OBRAZKY_DALSI")]
        public OBRAZKYDALSI OBRAZKYDALSI { get; set; }
    }

    [XmlRoot(ElementName = "OBRAZKY_DALSI")]
    public class OBRAZKYDALSI
    {

        [XmlElement(ElementName = "OBRAZEK")]
        public List<string> OBRAZEK { get; set; }
    }

    [XmlRoot(ElementName = "ZBOZI")]
    public class ZBOZI
    {

        [XmlElement(ElementName = "POLOZKA")]
        public List<POLOZKA> POLOZKA { get; set; }
    }

    [XmlRoot(ElementName = "DATA")]
    public class LevenhukXmlModel
    {

        [XmlElement(ElementName = "ZBOZI")]
        public ZBOZI ZBOZI { get; set; }
    }
}
