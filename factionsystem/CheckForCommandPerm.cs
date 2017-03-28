using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using factionsystem.Db;
using GTANetworkServer;

namespace factionsystem
{
    class CheckForCommandPerm : Script
    {
        public CheckForCommandPerm()
        {
            API.onChatCommand += Shared_onChatCommand;
        }

        private void Shared_onChatCommand(Client sender, string command, CancelEventArgs cancel)
        {
            //First get all the faction methods.
            var factionMethods = Assembly.GetExecutingAssembly().GetTypes()
                      .Where(x => x.BaseType == typeof(Script))
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(FactionCommandAttribute), false).Length > 0)
                      .ToArray();

            //Now find the actual function with same command name.
            foreach (var fac in factionMethods)
            {
                CommandAttribute cmd = (CommandAttribute)fac.GetCustomAttributes(typeof(CommandAttribute), false).First();
                if (cmd.CommandString == command.Remove(0, 1))
                {
                    //We will cancel by default and remove the cancel later if has perms.
                    cancel.Cancel = true;
                    cancel.Reason = "You don't have enough permissions to access this command.";

                    using (FactionsDbContext dbContext = new FactionsDbContext())
                    {
                        //First of all, get the user object that corrsponds to this user or create new.
                        var userName = API.getPlayerName(sender);
                        var user = dbContext.Users.SingleOrDefault(x => x.Username == userName) ??
                                   dbContext.Users.Add(new User() {Username = userName});
                        //Check div perms for this command.
                        if(user.Division != null)
                        {
                            var cmds = user.Division.Commands.Split(',');
                            foreach (var dCmd in cmds)
                            {
                                if (dCmd == cmd.CommandString)
                                {
                                    cancel.Cancel = false;
                                    break;
                                }
                            }
                        }

                        if(cancel.Cancel)
                            API.sendChatMessageToPlayer(sender, "You don't have enough permissions to access this command.");

                        dbContext.SaveChanges();
                    }
                }
            }
        }
    }
}
