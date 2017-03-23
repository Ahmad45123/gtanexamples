using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace factionsystem.Db
{
    [Table("Factions")]
    public class Faction
    {
        [Key]
        public int FactionId { get; set; }
        public string FactionName { get; set; }

        //This is to mantain that divisions are related to this.
        public virtual ICollection<Division> Divisions { get; set; }
    }

    [Table("Divisions")]
    public class Division
    {
        [Key]
        public int DivisionId { get; set; }
        public string DivisionName { get; set; }

        [ForeignKey("Faction")]
        public int FactionId { get; set; }

        // This will keep track of the faction this division belongs too
        public virtual Faction Faction { get; set; }
    }
}
