using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkServer;

namespace factionsystem
{
    class FactionCommands
    {
        //Commands that are faction and can be assigned will be here.
        [Command("one"), FactionCommand]
        public void OneCommand(Client player)
        {
            API.shared.sendChatMessageToPlayer(player, "One Works!");
        }

        [Command("two"), FactionCommand]
        public void TwoCommand(Client player)
        {
            API.shared.sendChatMessageToPlayer(player, "Two Works!");
        }

        [Command("three"), FactionCommand]
        public void ThreeCommand(Client player)
        {
            API.shared.sendChatMessageToPlayer(player, "Three Works!");
        }
    }
}
