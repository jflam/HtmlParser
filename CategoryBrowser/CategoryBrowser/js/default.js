// For an introduction to the Grid template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=232446
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var nav = WinJS.Navigation;
    var default_category = "http://en.wikipedia.org/wiki/Category:Microsoft_Windows";

    WinJS.strictProcessing();

    app.addEventListener("activated", function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
                // TODO: read the html page and do stuff
                $.ajax(default_category).then(function (response) {
                    var parser = new Tautologistics.NodeHtmlParser.Parser(app.handler);
                    parser.parseComplete(response);

                    var dom = app.handler.dom;

                    // Select the pages
                    var pages = SoupSelect.select(dom, "div#mw-pages div.mw-content-ltr");
                    var a = SoupSelect.select(pages, 'a');
                    var pagesGroup = Data.groups.getItemFromKey("Articles");
                    var lightGray = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsQAAA7EAZUrDhsAAAANSURBVBhXY7h4+cp/AAhpA3h+ANDKAAAAAElFTkSuQmCC";

                    // Now populate the data array with the elements
                    for (var i = 0; i < a.length; i++) {
                        var newItem = { group: pagesGroup, title: a[i].attribs.title, subtitle: a[i].attribs.title, description: a[i].attribs.title, content: a[i].attribs.href, backgroundImage: lightGray };
                        Data.items.push(newItem);
                    }
                });
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }


            if (app.sessionState.history) {
                nav.history = app.sessionState.history;
            }
            args.setPromise(WinJS.UI.processAll().then(function () {
                if (nav.location) {
                    nav.history.current.initialPlaceholder = true;
                    return nav.navigate(nav.location, nav.state);
                } else {
                    return nav.navigate(Application.navigator.home);
                }
            }));

            app.handler = new Tautologistics.NodeHtmlParser.DefaultHandler(function (error, dom) {
            });
        }
    });

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. If you need to 
        // complete an asynchronous operation before your application is 
        // suspended, call args.setPromise().
        app.sessionState.history = nav.history;
    };

    app.start();
})();
