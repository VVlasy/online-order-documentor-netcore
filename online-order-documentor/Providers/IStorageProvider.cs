using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Providers
{
    public interface IStorageProvider
    {
        void Upload(Stream file, string name);

        Stream Download(string name);

        void CreateFolder(string name);

        void ClearFolder(string name);

        bool FolderExists(string name);

        string[] GetFilesInFolder(string name);
    }
}
