(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var default_page = "http://en.wikipedia.org/wiki/Windows_8";

    WinJS.strictProcessing();

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // This application has been newly launched. Initialize
                // your application here.
                wikipedia.render(default_page);
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            args.setPromise(WinJS.UI.processAll());

            // Bind the go back button click event to our navigation event handler
            document.getElementById("go_back").addEventListener("click", wikipedia.go_back);

        } else if (args.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.search) {
            args.setPromise(WinJS.UI.processAll().then(function () {
                if (args.detail.queryText === "") {
                    // Navigate to your landing page since the user is pre-scoping to your app.
                } else {
                    // Display results in UI for eventObject.detail.queryText and eventObject.detail.language.
                    // eventObject.detail.language represents user's locale.
                    var query = args.detail.queryText;
                    var article_url = "http://en.wikipedia.org/wiki/" + query.replace(' ', '_');
                    wikipedia.render(article_url);
                }
            }));
        }
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    // Register our application settings page
    WinJS.Application.onsettings = function (e) {
        e.detail.applicationcommands = { "defaults": { title: "Defaults", href: "/settings.html" } };
        WinJS.UI.SettingsFlyout.populateSettings(e);
    };

    // Register our search provider
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onquerysubmitted = wikipedia.search;

    // Register our search suggestions provider
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = wikipedia.search_suggestions;

    // Register our share source handler
    Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView().ondatarequested = wikipedia.share;

    app.start();
})();