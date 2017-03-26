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
            throw new NotImplementedException();
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
