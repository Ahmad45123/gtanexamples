using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace factionsystem.Db
{
    class FactionsDbContext : DbContext
    {
        public DbSet<Faction> Factions { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<User> Users { get; set; }

        public FactionsDbContext() : base("FactionsSystem") //DbName.
        {
        }
    }
}
