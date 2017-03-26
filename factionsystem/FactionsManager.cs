using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using factionsystem.Db;
using GTANetworkServer;

namespace factionsystem
{
    class FactionsManager : Script
    {
        public FactionsManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            var dbContext = new FactionsDbContext();
            switch (eventName)
            {
                case "deleteFaction":
                    var faction = (string)arguments[0];
                    var fac = dbContext.Factions.SingleOrDefault(x => x.FactionName == faction);
                    if (fac == null) break;
                    dbContext.Factions.Remove(fac);
                    API.sendChatMessageToPlayer(sender, $"Faction ~r~'{faction}'~w~ was deleted.");
                    break;

                case "NewFaction":
                    var factionName = (string) arguments[0];
                    var existFac = dbContext.Factions.SingleOrDefault(x => x.FactionName == factionName);
                    if (existFac == null)
                    {
                        Faction newFac = new Faction {FactionName = factionName};
                        dbContext.Factions.Add(newFac);
                        API.triggerClientEvent(sender, "FactionSaved", factionName);
                        API.sendChatMessageToPlayer(sender, $"Faction with name ~r~{factionName}~w~ was successfully created.");
                    }
                    else
                        API.sendChatMessageToPlayer(sender, "Faction with that name already exists.");
                    break;
                case "EditFaction":
                    var oldName = (string)arguments[0];
                    var newName = (string)arguments[1];
                    var makeSureDoesntExist = dbContext.Factions.SingleOrDefault(x => x.FactionName == newName);
                    if (makeSureDoesntExist == null)
                    {
                        dbContext.Factions.Single(x => x.FactionName == oldName).FactionName = newName;
                        API.triggerClientEvent(sender, "FactionEdited", oldName, newName);
                        API.sendChatMessageToPlayer(sender, $"Faction ~r~{oldName}~w~ was renamed to ~r~{newName}~w~.");
                    }
                    else
                        API.sendChatMessageToPlayer(sender, "Faction with that name already exists.");
                    break;
            }
            dbContext.SaveChanges();
        }

        [Command("managefactions")]
        public void ManageFactions(Client player)
        {
            //Get the existing factions
            var dbContext = new FactionsDbContext();
            string[] facs = dbContext.Factions.Select(x => x.FactionName).ToArray();

            //Trigger the event of creating the div.
            API.triggerClientEvent(player, "showmanagefaction", string.Join(",", facs));
        }
    }
}
