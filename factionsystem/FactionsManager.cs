using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Reflection;
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
                case "DeleteFaction":
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

                case "GetDivsForFaction":
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

                case "DeleteDivison":
                    var dvfacName = (string)arguments[0];
                    var dvdivName = (string)arguments[1];
                    var dvFac = dbContext.Factions.Single(x => x.FactionName == dvfacName);
                    var dvDiv = dvFac.Divisions.Single(x => x.DivisionName == dvdivName);
                    dbContext.Divisions.Remove(dvDiv);
                    API.sendChatMessageToPlayer(sender, $"Division ~r~{dvdivName}~w~ was sucessfully deleted from faction ~r~{dvfacName}~w~.");
                    break;

                case "GetDivisionCommands":
                    var gdcfacName = (string)arguments[0];
                    var gdcdivName = (string)arguments[1];
                    var gdcFac = dbContext.Factions.Single(x => x.FactionName == gdcfacName);
                    var gdcDiv = gdcFac.Divisions.Single(x => x.DivisionName == gdcdivName);
                    API.triggerClientEvent(sender, "SelectCommandsForDivision", gdcfacName, gdcdivName, gdcDiv.Commands ?? "");
                    break;

                case "SaveCmds":
                    var scfacName = (string)arguments[0];
                    var scdivName = (string)arguments[1];
                    var sccmdList = (string)arguments[2];
                    var scFac = dbContext.Factions.Single(x => x.FactionName == scfacName);
                    var scDiv = scFac.Divisions.Single(x => x.DivisionName == scdivName);
                    scDiv.Commands = sccmdList;
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

            //Get all commands.
            CommandAttribute[] factionMethods = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(x => x.BaseType == typeof(Script))
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(FactionCommandAttribute), false).Length > 0)
                      .Select(y => (CommandAttribute)y.GetCustomAttributes(typeof(CommandAttribute)).First())
                      .ToArray();
            string[] cmds = factionMethods.Select(x => x.CommandString).ToArray();

            //Trigger the event of creating the div.
            API.triggerClientEvent(player, "showmanagefaction", string.Join(",", facs), string.Join(",", cmds));
        }

        [Command("setfaction", GreedyArg = true)]
        public void SetUserFaction(Client player, Client target, string faction)
        {
            using (FactionsDbContext dbContext = new FactionsDbContext())
            {
                //Make sure faction exists first.
                var fac = dbContext.Factions.SingleOrDefault(x => x.FactionName == faction);
                if (fac != null)
                {
                    //Make user object for target ofcourse.
                    var userName = API.getPlayerName(target);
                    var user = dbContext.Users.SingleOrDefault(x => x.Username == userName) ??
                               dbContext.Users.Add(new User() { Username = userName });

                    //Set user faction.
                    user.Faction = fac;
                    user.Division = null; //just reset div in case there is old fac.

                    //Notify.
                    API.sendChatMessageToPlayer(player, $"You have set the player ~r~{target.name}~w~ to the faction ~r~{fac.FactionName}~w~.");
                    API.sendChatMessageToPlayer(target, $"You have beed set to the faction ~r~{fac.FactionName}~w~.");
                }
                else
                    API.sendChatMessageToPlayer(player, "That faction doesn't exist.");

                //Save any changes.
                dbContext.SaveChanges();
            }
        }

        [Command("setdivision", GreedyArg = true)]
        public void SetUserDivision(Client player, Client target, string division)
        {
            using (FactionsDbContext dbContext = new FactionsDbContext())
            {
                //Make user object for target ofcourse.
                var userName = API.getPlayerName(target);
                var user = dbContext.Users.SingleOrDefault(x => x.Username == userName) ??
                           dbContext.Users.Add(new User() { Username = userName });

                //If user does have faction.
                if (user.Faction != null)
                {
                    //Make sure div exists.
                    var div = user.Faction.Divisions.SingleOrDefault(x => x.DivisionName == division);
                    if (div != null)
                    {
                        //Set user faction.
                        user.Division = div;

                        //Notify.
                        API.sendChatMessageToPlayer(player, $"You have set the player ~r~{target.name}~w~ to the division ~r~{div.DivisionName}~w~ in the faction ~r~{user.Faction.FactionName}~w~.");
                        API.sendChatMessageToPlayer(target, $"You have beed set to the division ~r~{div.DivisionName}~w~ in the faction ~r~{user.Faction.FactionName}~w~.");
                    }
                    else
                        API.sendChatMessageToPlayer(player, $"That division doesn't exist for the faction ~r~{user.Faction.FactionName}~w~.");
                }
                else
                    API.sendChatMessageToPlayer(player, "That user doesn't even have any faction assigned.");

                //Save any changes.
                dbContext.SaveChanges();
            }
        }

        [Command("userinfo")]
        public void GetUserInfo(Client player, Client target)
        {
            using (FactionsDbContext dbContext = new FactionsDbContext())
            {
                var userName = API.getPlayerName(target);
                var user = dbContext.Users.SingleOrDefault(x => x.Username == userName);
                if (user != null)
                {
                    API.sendChatMessageToPlayer(player, $"The user ~r~{target.name}~w~ is in the faction ~r~{user.Faction.FactionName ?? "None"}~w~ and the division ~r~{user.Division.DivisionName ?? "None"}~w~.");
                }
                else
                    API.sendChatMessageToPlayer(player, "That user is not even in DB yet.");

                dbContext.SaveChanges();
            }
        }

        [Command("deleteuserfaction")]
        public void DeleteUserInfo(Client player, Client target)
        {
            using (FactionsDbContext dbContext = new FactionsDbContext())
            {
                var userName = API.getPlayerName(target);
                var user = dbContext.Users.SingleOrDefault(x => x.Username == userName);
                if (user != null)
                {
                    user.Faction = null;
                    user.Division = null;
                    API.sendChatMessageToPlayer(player, $"Faction info for {target.name} was deleted sucessfully.");
                }
                else
                    API.sendChatMessageToPlayer(player, "That user is not even in DB yet.");

                dbContext.SaveChanges();
            }
        }
    }
}
