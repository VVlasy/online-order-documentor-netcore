using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace online_order_documentor_netcore.Providers
{
    public class SplitFilesFtpProvider : FtpProvider
    {
        public override Stream Download(string name)
        {
            if (!name.EndsWith(".xml"))
            {
                return base.Download(name);
            }

            string nameOfFile = name;
            string folderOfFile = string.Empty;

            int index = name.LastIndexOf("/");
            if (index >= 0)
            {
                folderOfFile = name.Substring(0, index);
                nameOfFile = name.Substring(index + 1, name.Length - (index + 1));
            }

            IEnumerable<string> files = GetFilesInFolder(folderOfFile).Where(x => x.StartsWith(nameOfFile.Replace(".xml", string.Empty)) && x.EndsWith(".xml"));

            if (files.Count() == 1)
            {
                return base.Download(name);
            }

            XmlDocument resultXml = new XmlDocument();

            foreach (string filename in files)
            {
                XmlDocument srcFile = new XmlDocument();
                using (var str = base.Download($"{folderOfFile}/{filename}"))
                {
                    srcFile.Load(str);

                    // Detect feed type: (Premier should be the only one)

                    switch (srcFile.LastChild.Name) // TODO: check we are still the same type of xml, if not, throw err
                    {
                        case "VFPData":
                            if (resultXml.ChildNodes.Count == 0) // first time we are filling the result xml
                            {
                                resultXml.AppendChild(resultXml.ImportNode(srcFile.FirstChild, true));
                                var vfpDataNode = resultXml.CreateNode(XmlNodeType.Element, "VFPData", string.Empty);
                                resultXml.AppendChild(vfpDataNode);
                            }

                            foreach (XmlNode childNode in srcFile.LastChild.ChildNodes.Cast<XmlNode>())
                            {
                                string id = childNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "sloupec01").InnerText;
                                int stockAmount = -1;

                                try
                                {
                                    stockAmount = (int)double.Parse(childNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "sloupec04").InnerText.Replace(",", ".").Replace(" ", string.Empty), CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }

                                // check if we already have a node with this id
                                if (resultXml.LastChild.ChildNodes.Cast<XmlNode>().Any(x => x.ChildNodes.Cast<XmlNode>().Any(y => y.Name == "sloupec01" && y.InnerText == id)))
                                {
                                    XmlNode resultFoundNode = resultXml.LastChild.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.ChildNodes.Cast<XmlNode>().Any(y => y.Name == "sloupec01" && y.InnerText == id));
                                    XmlNode resultstockAmountNode = resultFoundNode.ChildNodes.Cast<XmlNode>().FirstOrDefault(x => x.Name == "sloupec04");

                                    int stockAmountResult = -1;

                                    try
                                    {
                                        stockAmountResult = (int)double.Parse(resultstockAmountNode.InnerText.Replace(",", ".").Replace(" ", string.Empty), CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }

                                    resultstockAmountNode.InnerText = $"{stockAmount + stockAmountResult},00";
                                }
                                else
                                {
                                    XmlNode newQueTxtNode = resultXml.CreateNode(XmlNodeType.Element, "que_txt", string.Empty);
                                    newQueTxtNode.InnerXml = childNode.InnerXml;

                                    resultXml.LastChild.AppendChild(newQueTxtNode);
                                }
                            }

                            break;
                        default:
                            break;
                    }
                }
            }

            MemoryStream resultStream = new MemoryStream();
            resultXml.Save(resultStream);
            resultStream.Position = 0;

            return resultStream;
        }
    }
}
