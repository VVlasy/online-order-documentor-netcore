using Microsoft.EntityFrameworkCore;
using online_order_documentor_netcore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace online_order_documentor_netcore
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
        : base(options)
        { }

        public DbSet<FeedFilter> FeedFilters { get; set; }
    }
}
