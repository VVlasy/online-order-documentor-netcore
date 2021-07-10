using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models
{
    public class FeedFilter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string FeedName { get; set; }

        public string ControllerName { get; set; }

        public string Filters { get; set; } // store a json of some filter object, extensinble as it can be deserialized as needed
    }
}
