/// <reference path="../types-gtanetwork/index.d.ts" />

//Events.
API.onServerEventTrigger.connect((eventName, args) => {
    switch (eventName) {
        case 'showmanagefaction':
            showCefWindow("ClientSide/manageFactions.html");
        break;
    }
    latestArgs = args; //We will always take a copy of latest sent args.
});

var browser = null;
var latestArgs;

function showCefWindow(path: string) {
    var res = API.getScreenResolution();
    browser = API.createCefBrowser(1000, 320);
    API.waitUntilCefBrowserInit(browser);
    API.setCefBrowserPosition(browser, (res.Width / 2) - (1000 / 2),
        (res.Height / 2) - (320 / 2));
    API.loadPageCefBrowser(browser, path);
    API.showCursor(true);
    API.setCanOpenChat(false);
}