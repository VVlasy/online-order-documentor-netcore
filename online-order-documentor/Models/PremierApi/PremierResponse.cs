using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models.PremierApi
{
    public class PremierResponse
    {
        public string Result { get; set; }

        public string CommandIn { get; set; }

        public JObject Data { get; set; }
        //public CommandData Data { get; set; }

        public Dictionary<string, string>[] Error { get; set; }

        public Dictionary<string, string>[] Warning { get; set; }

        public Dictionary<string, string>[] QueryFields { get; set; }
    }

    public class CommandData
    {
    }
}
