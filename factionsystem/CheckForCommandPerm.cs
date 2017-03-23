using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using factionsystem.Db;
using GTANetworkServer;

namespace factionsystem
{
    class CheckForCommandPerm
    {
        public CheckForCommandPerm()
        {
            API.shared.onChatCommand += Shared_onChatCommand;
        }

        private void Shared_onChatCommand(Client sender, string command, CancelEventArgs cancel)
        {
            //First get all the faction methods.
            var factionMethods = Assembly.GetExecutingAssembly().GetTypes()
                      .SelectMany(t => t.GetMethods())
                      .Where(m => m.GetCustomAttributes(typeof(FactionCommandAttribute), false).Length > 0)
                      .ToArray();

            //Now find the actual function with same command name.
            foreach (var fac in factionMethods)
            {
                CommandAttribute cmd = (CommandAttribute)fac.GetCustomAttributes(typeof(CommandAttribute), false).First();
                if (cmd.CommandString == command)
                {
                    //TODO: code to check if user has permissions to this command by getting their faction/div and then comparing.
                    return;
                }
            }
        }
    }
}
