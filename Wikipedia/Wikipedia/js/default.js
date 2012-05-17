// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    var backstack = [];

    WinJS.strictProcessing();

    app.handler = null;

    app.inner_html = function (node) {
        var html = "";

        function walk(node) {
            // fix up image tags to pre-pend http:
            if (node.type == "tag" && node.name.toLowerCase() == "img") {
                var src = node.attribs.src;
                if (src.indexOf("//") == 0) {
                    node.attribs.src = "http:" + node.attribs.src;
                }
            }

            var attributes = "";
            for (var key in node.attribs) {
                attributes += key + "=\"" + node.attribs[key] + "\" ";
            }

            if (node.type == "text") {
                html += node.data;
            } else if (node.type == "tag") {
                html += "<" + node.name + " " + attributes + ">";
            }


            if (node.children != null) {
                for (var i = 0; i < node.children.length; i++) {
                    var child = node.children[i];
                    if (child.type == "text") {
                        html += child.data;
                    } else {
                        walk(child);
                    }
                }
            }

            if (node.type == "tag") {
                html += "</" + node.name + ">";
            } 
        };

        walk(node);
        return html;
    }

    app.render_wikipedia = function (url) {
        // TODOO: This callback in a search handler is an example of a swallowing exception event. Doc this in the 
        // errors doc as a simple example of something that even 1st chance exceptions don't catch. Make
        // sure that I understand exactly why this happens.

        // Display loading
        article.innerHTML = "<img class='splash' src='/images/ajax-loader.gif'>";
        $.ajax(url).then(
            function (response) {
                var parser = new Tautologistics.NodeHtmlParser.Parser(app.handler);
                parser.parseComplete(response);

                // We need to rewrite all <a> elements to point to a navigation event in the page
                var dom = app.handler.dom;
                var anchors = SoupSelect.select(dom, "a");
                for (var i = 0; i < anchors.length; i++) {
                    var anchor = anchors[i];

                    if (anchor.attribs.href) {
                        var link = anchor.attribs.href.indexOf("/wiki/", 0);
                        var href = null;
                        if (link >= 0) {
                            if (link == 0) {
                                // relative link to wikipedia
                                href = "http://en.wikipedia.org" + anchor.attribs.href;
                            } else {
                                // absolute link to wikipedia
                                href = "http:" + anchor.attribs.href;
                            }
                            anchor.attribs.href = href;
                        }
                    }
                }

                var body = SoupSelect.select(dom, "div.mw-body")[0];
                var html = app.inner_html(body);

                var clean_html = window.toStaticHTML(html);
                article.innerHTML = window.toStaticHTML(clean_html);

                $('div.mw-body a').click(function (e) {
                    e.preventDefault();
                    var target = e.currentTarget.href;

                    // Push the current url (captured in this lambda) onto the back stack
                    backstack.push(url);
                    app.render_wikipedia(target);
                });
            });
    }

    var default_page = "http://en.wikipedia.org/wiki/Windows_8";

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // This application has been newly launched. Initialize
                // your application here.
                app.render_wikipedia(default_page);
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            args.setPromise(WinJS.UI.processAll());

            // Handle the go back button click
            document.getElementById("go_back").addEventListener("click", function () {
                if (backstack.length > 0) {
                    var url = backstack.pop();
                    app.render_wikipedia(url);
                }
            });
        } else if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.search) {
                // Use setPromise to indicate to the system that the splash screen must not be torn down
                // until after processAll and navigate complete asynchronously.
            eventObject.setPromise(WinJS.UI.processAll().then(function () {
                if (eventObject.detail.queryText === "") {
                    // Navigate to your landing page since the user is pre-scoping to your app.
                } else {
                    // Display results in UI for eventObject.detail.queryText and eventObject.detail.language.
                    // eventObject.detail.language represents user's locale.
                }

                // Navigate to the first scenario since it handles search activation.
                var url = scenarios[0].url;
                return WinJS.Navigation.navigate(url, { searchDetails: eventObject.detail });
            }));
        }

        // Initialize our soupselect handler -- regardless of execution path -- this is a crappy error experience ... need to report this!!!
        app.handler = new Tautologistics.NodeHtmlParser.DefaultHandler(function (error, dom) {
            // TODO: Error handling in the parser 
            //if (error)
            //    // error
            //else
            //    // parsing done ... do something
        });

        // Initialize our share source handler
        var dataTransferManager = Windows.ApplicationModel.DataTransfer.DataTransferManager.getForCurrentView();
        dataTransferManager.addEventListener("datarequested", app.data_requested);
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    app.search_wikipedia = function (query) {
        // URI en.wikipedia.org/w/api.php?action=query&list=search&format=json&srsearch=microsoft&srlimit=10
        var uri = "http://en.wikipedia.org/w/api.php?action=query&list=search&format=json&&srlimit=10&srsearch=";
        var search_uri = uri + encodeURIComponent(query);
        $.ajax(search_uri).then(
            function (response) {
                var x = 42;
            });
    };

    // Callback that is called when the share contract is activated
    app.data_requested = function (e) {
        var request = e.request;
        var html = null;
        var selection = window.getSelection();
        if (selection.focusNode != null) {
            request.data = MSApp.createDataPackageFromSelection();
        } else {
            var range = document.createRange();
            var element = document.getElementById("article");
            range.selectNode(element);
            request.data = MSApp.createDataPackage(range);
        }

        var parser = new Tautologistics.NodeHtmlParser.Parser(app.handler);
        parser.parseComplete(document.getElementById("article").innerHTML);

        var dom = app.handler.dom;
        var nodes = SoupSelect.select(dom, "h1.firstHeading span");
        var title = "unknown title";
        if (nodes.length > 0 && nodes[0].children.length > 0) {
            title = nodes[0].children[0].data;
        }
        request.data.properties.title = title;
        request.data.properties.description = "Wikipedia article";
    }

    // Register for search 
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onquerysubmitted = function (eventObject) {
        // TODO: what is the heuristic for mapping to URL? Replace spaces with underscores?
        // Do I do a search and return the first search result?
        // TODO: navigate to the search results page ...?
        // How do I do navigation using this framework?
        var query = eventObject.queryText;
        // app.search_wikipedia(query);
        var article_url = "http://en.wikipedia.org/wiki/" + query.replace(' ', '_');
        app.render_wikipedia(article_url);
    };

    var xhrRequest;

    // Register and initialize our search suggestions provider
    // Provide suggestions using an URL that supports the Open Search suggestion format.
    // This code should be placed in your apps global scope, e.g. default.js, and run as soon as your app is launched.
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = function (eventObject) {
        var queryText = eventObject.queryText, language = eventObject.language, suggestionRequest = eventObject.request;

        // Indicate that we'll obtain suggestions asynchronously:
        var deferral = suggestionRequest.getDeferral();
        var suggestionUri = "http://en.wikipedia.org/w/api.php?action=opensearch&search=" + encodeURIComponent(queryText);

        // Cancel the previous suggestion request if it is not finished.
        if (xhrRequest && xhrRequest.cancel) {
            xhrRequest.cancel();
        }

        // Create request to obtain suggestions from service and supply them to the Search Pane.
        xhrRequest = WinJS.xhr({ url: suggestionUri });
        xhrRequest.done(
            function (request) {
                if (request.responseText) {
                    var parsedResponse = JSON.parse(request.responseText);
                    if (parsedResponse && parsedResponse instanceof Array) {
                        var suggestions = parsedResponse[1];
                        if (suggestions) {
                            suggestionRequest.searchSuggestionCollection.appendQuerySuggestions(suggestions);
                            WinJS.log && WinJS.log("Suggestions provided for query: " + queryText, "sample", "status");
                        } else {
                            WinJS.log && WinJS.log("No suggestions provided for query: " + queryText, "sample", "status");
                        }
                    }
                }

                deferral.complete(); // Indicate we're done supplying suggestions.
            },
            function (error) {
                WinJS.log && WinJS.log("Error retrieving suggestions for query: " + queryText, "sample", "status");
                // Call complete on the deferral when there is an error.
                deferral.complete();
            });
    };

    app.start();
})();