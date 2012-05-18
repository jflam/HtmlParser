(function () {
    "use strict";

    var app = WinJS.Application;
    var nav = WinJS.Navigation;
    var activation = Windows.ApplicationModel.Activation;

    WinJS.strictProcessing();

    app.newly_launched = function (args) {
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.reactivated_from_suspension = function (args) {
    };

    app.search_activation = function (args) {
        args.setPromise(WinJS.UI.processAll().then(function () {
            if (args.detail.queryText === "") {
                // Navigate to your landing page since the user is pre-scoping to your app.
            } else {
                // Display results in UI for eventObject.detail.queryText and eventObject.detail.language.
                // eventObject.detail.language represents user's locale.
                navigateToSearchTerm(args.detail.queryText);
            }
        }));
    };

    function navigateToSearchTerm(search_term) {
        var article_url = "http://en.wikipedia.org/wiki/" + search_term.replace(' ', '_');
        nav.navigate('pages/article.html', { url: article_url });
    }

    // TODO: actually implement this function - right now we just delegate based on the search term.

    function search(eventObject) {
        navigateToSearchTerm(eventObject.queryText);
    };

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                app.newly_launched();
            } else {
                app.reactivated_from_suspension();
            }

            args.setPromise(WinJS.UI.processAll().then(function() {
                // Bind the go back button click event to our navigation event handler
                document.getElementById("go_home").addEventListener("click", function () {
                    nav.navigate("/pages/home.html");
                });

                // Initialize our navigation framework
                if (app.sessionState.history) {
                    nav.history = app.sessionState.history;
                }

                if (nav.location) {
                    nav.history.current.initialPlaceholder = true;
                    return nav.navigate(nav.location, nav.state);
                } else {
                    return nav.navigate(Application.navigator.home);
                }
            }));
        } else if (args.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.search) {
            app.search_activation(args);
        }
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