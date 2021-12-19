using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore
{
    public static class AppVariables
    {
        public static string FtpHost
        {
            get {
                return Environment.GetEnvironmentVariable("FTP_HOST");
            }
        }
        public static string FtpUsername
        {
            get
            {
                return Environment.GetEnvironmentVariable("FTP_USER");
            }
        }
        public static string FtpPassword
        {
            get
            {
                return Environment.GetEnvironmentVariable("FTP_PASS");
            }
        }
        public static string FtpPhotosFolder
        {
            get
            {
                return Environment.GetEnvironmentVariable("FTP_PHOTOS_FOLDER_PATH");
            }
        }
        public static string FtpAvailabilityXmlPath
        {
            get
            {
                return Environment.GetEnvironmentVariable("FTP_AVAILABILITY_XML_PATH");
            }
        }
        public static string InstaxShopHash
        {
            get
            {
                return Environment.GetEnvironmentVariable("INSTAXSHOP_HASH");
            }
        }
        public static string DigiEshopHash
        {
            get
            {
                return Environment.GetEnvironmentVariable("DIGI_ESHOP_HASH");
            }
        }

        // Premier API

        public static string PremierUrl
        {
            get
            {
                return Environment.GetEnvironmentVariable("PREMIER_URL");
            }
        }

        public static string PremierPort
        {
            get
            {
                return Environment.GetEnvironmentVariable("PREMIER_PORT");
            }
        }

        public static string PremierUsername
        {
            get
            {
                return Environment.GetEnvironmentVariable("PREMIER_USERNAME");
            }
        }

        public static string PremierPassword
        {
            get
            {
                return Environment.GetEnvironmentVariable("PREMIER_PASSWORD");
            }
        }

        public static string PremierIdUj
        {
            get
            {
                return Environment.GetEnvironmentVariable("PREMIER_ID_UJ");
            }
        }
    }
}
