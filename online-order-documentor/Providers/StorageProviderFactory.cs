using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Providers
{
    public static class StorageProviderFactory
    {
        public static IStorageProvider Create()
        {
            return new FtpProvider();
        }
    }
}
