using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models.PremierApi
{
    public class PremierApiVersion
    {
        [JsonProperty("ApiComPrem.dll")]
        public ApiComPremDll ApiComPremDll { get; set; }

        [JsonProperty("ApiComSQL.dll")]
        public ApiComSQLDll ApiComSQLDll { get; set; }
        public VerzeInSQL VerzeInSQL { get; set; }
    }

    public class ApiComPremDll
    {
        public string verze { get; set; }
        public string datum { get; set; }
    }

    public class ApiComSQLDll
    {
        public string verze { get; set; }
        public string datum { get; set; }
    }

    public class VerzeInSQL
    {
        public string verze { get; set; }
    }
}
