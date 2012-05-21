(function () {
    "use strict";

    var app = WinJS.Application;
    var nav = WinJS.Navigation;
    var activation = Windows.ApplicationModel.Activation;
    var default_page = "http://en.wikipedia.org/wiki/Windows_8";

    WinJS.strictProcessing();

    // Navigate to the wikipedia link specified by url. This is an app 
    // scoped function that dispatches to the correct page fragment
    // that contains the markup and code to render that url.

    app.wikipedia_navigate = function (url) {

        // Parse the target url's HTML and navigate to the correct 
        // rendering HTML page.

        wikipedia.parse(url).then(function (obj) {
            if (obj.type == 'article') {
                nav.navigate("/pages/article.html", obj);
            } else if (obj.type == 'images') {
                nav.navigate("/pages/image.html", obj);
            }
        });
    }

    // Navigate to the link specified by url. The function behaves
    // differently based on whether it needs to cancel the default
    // action of the anchor element which will open the target
    // URL using Metro style IE. It uses the simple heuristic of
    // looking to see whether the URL contains wikipedia.org in it.

    app.navigate = function (e) {
        var target = e.currentTarget.href;
        
        // We look for internal URLs based on whether we are going to either wikipedia.org
        // or wikimedia.org (for images and other media). This is not a great heuristic
        // and I will likely need to update this down the road. TODO

        if (target.search("wikipedia.org") >= 0) {
            e.preventDefault();
            app.wikipedia_navigate(target);
        }
    }

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
        app.wikipedia_navigate(article_url);
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

            args.setPromise(WinJS.UI.processAll().then(function () {

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

                    // We are freshly launched, navigate to the default page

                    app.wikipedia_navigate(default_page);
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
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onquerysubmitted = search;

    // Register our search suggestions provider
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = wikipedia.search_suggestions;

    // Register our share source handler
    Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView().ondatarequested = wikipedia.share;

    app.start();
})();