using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Providers
{
    public class FtpProvider : IStorageProvider
    {
        public Stream Download()
        {
            throw new NotImplementedException();
        }

        public void Upload(Stream file, string name)
        {
            string uploadUrl = string.Format("{0}//{1}", string.Format("ftp://{0}", AppVariables.FtpHost), name);
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(uploadUrl);
            request.Method = WebRequestMethods.Ftp.UploadFile; 
            request.Credentials = new NetworkCredential(AppVariables.FtpUsername, AppVariables.FtpPassword);
            request.Proxy = null;
            request.KeepAlive = true;
            request.UseBinary = true;
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // Copy the contents of the file to the request stream.  
            Stream requestStream = request.GetRequestStream();
            file.CopyTo(requestStream);
            requestStream.Close();
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine(resp.StatusCode);
            }
        }
        public void ClearFolder(string name)
        {
            string folderUrl = string.Format("{0}//{1}/", string.Format("ftp://{0}", AppVariables.FtpHost), name);

            foreach (string folder in GetFilesInFolder(name))
            {
                try
                {
                    WebRequest request = WebRequest.Create(string.Format("{0}{1}", folderUrl, folder));
                    request.Method = WebRequestMethods.Ftp.DeleteFile;
                    request.Credentials = new NetworkCredential(AppVariables.FtpUsername, AppVariables.FtpPassword);

                    using (var resp = (FtpWebResponse)request.GetResponse())
                    {
                        Console.WriteLine(resp.StatusCode);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    continue;
                }
            }
        }

        public void CreateFolder(string name)
        {
            string folderUrl = string.Format("{0}//{1}/", string.Format("ftp://{0}", AppVariables.FtpHost), name);
            WebRequest request = WebRequest.Create(folderUrl);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(AppVariables.FtpUsername, AppVariables.FtpPassword);

            using (var resp = (FtpWebResponse)request.GetResponse())
            {
                Console.WriteLine(resp.StatusCode);
            }
        }
        public bool FolderExists(string name)
        {
            try
            {
                string folderUrl = string.Format("{0}//{1}/", string.Format("ftp://{0}", AppVariables.FtpHost), name);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(folderUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(AppVariables.FtpUsername, AppVariables.FtpPassword);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                }
                return true;
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public string[] GetFilesInFolder(string name)
        {
            try
            {
                string folderUrl = string.Format("{0}//{1}/", string.Format("ftp://{0}", AppVariables.FtpHost), name);

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(folderUrl);
                request.Method = WebRequestMethods.Ftp.ListDirectory;
                request.Credentials = new NetworkCredential(AppVariables.FtpUsername, AppVariables.FtpPassword);
                
                var response = (FtpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(responseStream);
                string names = reader.ReadToEnd();

                reader.Close();
                response.Close();

                return names.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
            catch (WebException ex)
            {
                Console.WriteLine(ex);
                return new string[] { };
            }
        }
    }
}
