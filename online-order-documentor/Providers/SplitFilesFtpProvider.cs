using online_order_documentor_netcore.Models.Xml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace online_order_documentor_netcore.Providers
{
    public class SplitFilesFtpProvider : FtpProvider
    {
        public string ItemIdColumnName { get; set; } = "sloupec01";

        public string StockColumnName { get; set; } = "sloupec04";

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

            XmlSerializer serializer = new XmlSerializer(typeof(PremierExportXmlModel));
            PremierExportXmlModel resultPremierFeed = new PremierExportXmlModel()
            {
                que_txt = new List<que_txt>()
            };

            foreach (string filename in files.OrderBy(x=>x.Length))
            {
                using (var downloadedFile = base.Download($"{folderOfFile}/{filename}"))
                {
                    using (StreamReader sr = new StreamReader(downloadedFile))
                    {
                        using (XmlReader reader = XmlReader.Create(sr))
                        {
                            var data = (PremierExportXmlModel)serializer.Deserialize(reader);

                            foreach (que_txt item in data.que_txt)
                            {
                                que_txt existingItem = resultPremierFeed.que_txt.FirstOrDefault(x => x[ItemIdColumnName] == item[ItemIdColumnName]);
                                int stockAmount = item.GetColumnAsInt(StockColumnName);
                                item[StockColumnName] = stockAmount.ToString(CultureInfo.InvariantCulture);

                                if (existingItem != null)
                                {
                                    existingItem[StockColumnName] = (existingItem.GetColumnAsInt(StockColumnName) + stockAmount).ToString(CultureInfo.InvariantCulture);
                                    resultPremierFeed.que_txt.Add(existingItem);
                                }
                                else
                                {   
                                    resultPremierFeed.que_txt.Add(item);
                                }
                            }
                        }
                    }
                }
            }

            MemoryStream resultStream = new MemoryStream();
            using (XmlWriter writer = XmlWriter.Create(resultStream))
            {
                serializer.Serialize(writer, resultPremierFeed);
            }

            resultStream.Position = 0;
            
            return resultStream;
        }
    }
}
