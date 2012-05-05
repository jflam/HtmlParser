﻿// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
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

    app.parse_comment = function (node) {
        var result = {};
        var comment = node.children[0].children[0].children[0];

        // Isolate the <img> tag's width attribute which is used for indentation
        var single_pixel_gif = comment.children[0].children[0];
        result.comment_indentation = single_pixel_gif.attribs.width / 40;

        // Extract out the author, post time and permalink for the comment
        var comment_head = comment.children[2].children[0].children[0];
        result.comment_user = comment_head.children[0].children[0].data;
        result.comment_time = comment_head.children[1].data;
        result.comment_permalink = comment_head.children[2].attribs.href;
        result.comment_html = "";

        function strip_tags(node) {
            var attributes = "";
            for (var key in node.attribs) {
                attributes += key + "=\"" + node.attribs[key] + "\" ";
            }

            if (node.name != 'font') {
                result.comment_html += "<" + node.name + " " + attributes + ">";
            }

            for (var i = 0; i < node.children.length; i++) {
                var child = node.children[i];
                if (child.type == "text") {
                    result.comment_html += child.data;
                } else {
                    strip_tags(child);
                }
            }

            if (node.name != 'font') {
                result.comment_html += "</" + node.name + ">";
            }
        }

        // Render the comment HTML and strip out <font> tags
        var comment_body = comment.children[2].children[3];
        strip_tags(comment_body);

        return result;
    }

    app.parse_hackernews = function () {
        return new WinJS.Promise(function (complete) {
            $.ajax("http://news.ycombinator.com/item?id=3921052").then(
                function (response) {
                    var parser = new Tautologistics.NodeHtmlParser.Parser(app.handler);
                    parser.parseComplete(response);

                    var dom = app.handler.dom;

                    var body = dom[0].children[1];
                    var center = body.children[0];
                    var table = center.children[0];

                    // summary info
                    var comment_thread = {};

                    var summary = table.children[2].children[0].children[0]; // this is a table
                    comment_thread.title = summary.children[0].children[1].children[0].children[0].data;
                    var details = summary.children[1].children[1];
                    comment_thread.points = details.children[0].children[0].data;
                    comment_thread.user = details.children[2].children[0].data;
                    comment_thread.time = details.children[3].data;
                    comment_thread.comments = [];

                    // parse each comment
                    var comments = table.children[2].children[0].children[4].children;
                    for (var i = 0; i < comments.length; i++) {
                        comment_thread.comments.push(app.parse_comment(comments[i]));
                    }

                    complete(comment_thread);
                });
        });
    };

    app.render_hackernews = function (comment_thread) {
        var html = "";
        html += "<h1 class=\"hn\">" + comment_thread.title + "</h1>";
        html += "<h2 class=\"hn\"><span class=\"emph\">" + comment_thread.points + " | " + comment_thread.comments.length + " comments</span> by " + comment_thread.user + " " + comment_thread.time + "</h2>";
        html += "<div class=\"comments\">";

        // Track indentation levels
        var current_indent = -1;
        for (var i = 0; i < comment_thread.comments.length; i++) {
            var comment = comment_thread.comments[i];
            var indent_diff = comment.comment_indentation - current_indent;
            if (indent_diff > 0) {
                html += "<div class=\"comment\">";
            } else if (indent_diff == 0) {
                html += "</div><div class=\"comment\">";
            } else if (indent_diff == -1) {
                html += "</div></div><div class=\"comment\">";
            } else if (indent_diff == -2) {
                html += "</div></div></div><div class=\"comment\">";
            } else if (indent_diff == -3) {
                html += "</div></div></div></div><div class=\"comment\">";
            }
            current_indent = comment.comment_indentation;

            html += "<span class=\"heading\">";
            html += "<span class=\"dropcap\">" + (i + 1) + "</span>";
            html += "<span class=\"emph\">" + comment.comment_user + "</span>" + comment.comment_time;
            html += "</span><br>";
            html += "<span class=\"comment\">" + comment.comment_html + "</span>";
        }

        html += "</div>";
        return html;
    };

    app.render_wikipedia = function () {
        //var url = "http://en.wikipedia.org/wiki/Portsmouth";
        var url = "http://en.wikipedia.org/wiki/North_American_P-51_Mustang";
        //var url = "http://en.wikipedia.org/wiki/American_Idol";
        $.ajax(url).then(
            function (response) {
                var parser = new Tautologistics.NodeHtmlParser.Parser(app.handler);
                parser.parseComplete(response);

                var dom = app.handler.dom;
                var body = SoupSelect.select(dom, "div.mw-body")[0];
                var html = app.inner_html(body);
                var clean_html = window.toStaticHTML(html);
                article.innerHTML = window.toStaticHTML(clean_html);

                //// TODO: useful hack while we wait for a fix for this bug
                //MSApp.execUnsafeLocalFunction(function () {
                //    article.innerHTML = html;
                //});
            });
    }

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }

            args.setPromise(WinJS.UI.processAll());

            document.getElementById("render_wikipedia").addEventListener("click", function () {
                app.render_wikipedia();
            });

            document.getElementById("render_hackernews").addEventListener("click", function () {
                app.parse_hackernews().then(function (comment_thread) {
                    article.innerHTML = app.render_hackernews(comment_thread);
                });
            });

            // Initialize our soupselect handler
            app.handler = new Tautologistics.NodeHtmlParser.DefaultHandler(function (error, dom) {
                //if (error)
                //    // error
                //else
                //    // parsing done ... do something
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
    };

    app.oncheckpoint = function (args) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // args.setPromise().
    };

    // Register for search 
    Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onquerysubmitted = function (eventObject) {
        WinJS.log && WinJS.log("User submitted the search query: " + eventObject.queryText, "sample", "status");
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
