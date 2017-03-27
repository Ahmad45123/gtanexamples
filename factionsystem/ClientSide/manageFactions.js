/// <reference path="../types-gtanetwork/index.d.ts" />

//Events.
API.onServerEventTrigger.connect((eventName, args) => {
    if (eventName === 'showmanagefaction') {
        factionsList = args;
        showCefWindow("ClientSide/manageFactions.html");
    } else
        browser.call(eventName, ...args); //Event will most likely have a corsponding function in the HTML.
});

var browser = null;
var factionsList;

function showCefWindow(path) {
    var res = API.getScreenResolution();
    browser = API.createCefBrowser(1000, 320);
    API.waitUntilCefBrowserInit(browser);
    API.setCefBrowserPosition(browser, (res.Width / 2) - (1000 / 2),
        (res.Height / 2) - (320 / 2));
    API.loadPageCefBrowser(browser, path);
    API.showCursor(true);
    API.setCanOpenChat(false);
}

//Events that will get called from the managefactions.html.
function documentLoaded() {
    //We wanna populate the list with all the available factions.
    browser.call("loadFactions", factionsList[0], factionsList[1]); //This should be a comma list of factions.
}