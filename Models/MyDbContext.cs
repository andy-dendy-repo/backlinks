using Backlinks_LE.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Backlinks_LE.Models
{
    public class MyDbContext : DbContext
    {
        private string _connectionString;
        public MyDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connectionString);
        }
        public DbSet<DataBacklinkRow> DataBacklinkRow { get; set; }
        public DbSet<DataTagA> DataTagA { get; set; }
        public DbSet<DataListedInfo> DataListedInfo { get; set; }
    }
}
