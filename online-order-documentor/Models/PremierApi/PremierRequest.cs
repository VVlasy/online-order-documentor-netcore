using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models.PremierApi
{
    public class PremierRequest
    {
        public PremierCommand Command { get; set; }

        public PremierRequest(string inComm)
        {
            Command = new PremierCommand() { InComm = inComm };
        }
    }

    public class PremierCommand
    {
        public string InComm { get; set; }

        public PremierCommandParameters InParam { get; set; }
    }

    public class PremierCommandParameters
    {
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }
}
