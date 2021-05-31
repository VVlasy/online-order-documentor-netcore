using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore.Models
{
    public class FeedFilter
    {
        [Key]
        public int Id { get; set; }
    }
}
