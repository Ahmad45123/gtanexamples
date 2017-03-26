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

                case "getDivsForFaction":
                    var getDivFac = (string) arguments[0];
                    var divFac = dbContext.Factions.Single(x => x.FactionName == getDivFac);
                    string[] divs = divFac.Divisions.Select(x => x.DivisionName).ToArray();
                    API.triggerClientEvent(sender, "ShowDivs", string.Join(",", divs));
                    break;

                case "NewDivision":
                    var facname = (string) arguments[0];
                    var divname = (string) arguments[1];
                    var newdivFac = dbContext.Factions.Single(x => x.FactionName == facname);
                    var existDiv = newdivFac.Divisions.SingleOrDefault(x => x.DivisionName == divname);
                    if (existDiv == null)
                    {
                        newdivFac.Divisions.Add(new Division() {DivisionName = divname});
                        API.sendChatMessageToPlayer(sender,
                            $"Division ~r~{arguments[1]}~w~ was sucessfully saved to ~r~{facname}~w~.");
                        API.triggerClientEvent(sender, "DivisionSaved", facname, divname);
                    }
                    else
                        API.sendChatMessageToPlayer(sender, $"Division with that name already exists in faction ~r~{facname}~w~.");
                    break;

                case "EditDivision":
                    var edfacName = (string)arguments[0];
                    var edDivOldName = (string)arguments[1];
                    var edDivNewName = (string)arguments[2];
                    var edFac = dbContext.Factions.Single(x => x.FactionName == edfacName);
                    var edMakeSureDoesntExist = edFac.Divisions.SingleOrDefault(x => x.DivisionName == edDivNewName);
                    if (edMakeSureDoesntExist == null)
                    {
                        edFac.Divisions.Single(x => x.DivisionName == edDivOldName).DivisionName = edDivNewName;
                        API.triggerClientEvent(sender, "DivisionEdited", edfacName, edDivOldName, edDivNewName);
                        API.sendChatMessageToPlayer(sender, $"The div ~r~{edDivOldName}~w~ was renamed to ~r~{edDivNewName}~w~ from the faction ~r~{edfacName}~w~.");
                    }
                    else
                        API.sendChatMessageToPlayer(sender, "Division with that name already exists.");
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
