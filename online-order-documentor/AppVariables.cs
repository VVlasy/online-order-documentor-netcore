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

    }
}
