using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using factionsystem.Db;
using GTANetworkServer;

namespace factionsystem
{
    class AdminCommands : Script
    {
        [Command("createfaction", GreedyArg = true)]
        public void CreateFaction(Client player, string factionName)
        {
            var dbContext = new FactionsDbContext();
            if (dbContext.Factions.Count(x => x.FactionName == factionName) == 0)
            {
                //Create faction.
                Faction fac = new Faction() {FactionName = factionName};
                dbContext.Factions.Add(fac);
                dbContext.SaveChanges();
                API.sendChatMessageToPlayer(player, $"Faction with name \"{factionName}\" was sucessfully created.");
            }
            else
             API.sendChatMessageToPlayer(player, "Faction with that name already exists.");
        }

        }
    }
}
