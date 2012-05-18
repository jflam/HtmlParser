// Contains all of the code for the wikipedia class

(function () {
    "use strict";

    // HtmlParser object
    var handler = new Tautologistics.NodeHtmlParser.DefaultHandler(function (error, dom) {
        // TODO: Error handling in the parser 
        //if (error)
        //    // error
        //else
        //    // parsing done ... do something
    });
    
    var activation = Windows.ApplicationModel.Activation;
    var default_page = "http://en.wikipedia.org/wiki/Windows_8";
    var xhrRequest = null;

    WinJS.strictProcessing();

    // This is an internal function that walks all of the descendents
    // of node and generates a string. This function is currently
    // hard-coded to fix up wikipedia img tags to include http:
    // In a future revision of this function, it will be generalized
    // to accept a list of lambdas that will be iteratively applied
    // across each node in the tree. The lambda has the option
    // to mutate the underlying node's attributes.

    function inner_html(node) {
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
    };

    // Parse the Wikipedia URL given and return an object that contains some key pieces of information
    // for the UI: title, tweaked HTML that can be used to inject directly into the UI. Note that this
    // is an asynchronous function that takes a completion handler.

    function parse(url) {
        return new WinJS.Promise(function (complete) {
            $.ajax(url).then(function (response) {
                var parser = new Tautologistics.NodeHtmlParser.Parser(handler);
                parser.parseComplete(response);

                // We need to rewrite all <a> elements to point to a navigation event in the page

                var dom = handler.dom;
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
                var html = inner_html(body);
                var safe_html = window.toStaticHTML(html);

                // Extract the title from the page

                var nodes = SoupSelect.select(dom, "h1.firstHeading span");
                var title = "unknown title";
                if (nodes.length > 0 && nodes[0].children.length > 0) {
                    title = nodes[0].children[0].data;
                }

                // Invoke the completion handler with the JS object that contains the results

                complete({
                    title: title,
                    html: safe_html
                })
            });
        });
    }

    // Callback that is called when the share contract is activated

    function share(e) {
        var request = e.request;
        var selection = window.getSelection();
        if (selection.focusNode != null) {
            request.data = MSApp.createDataPackageFromSelection();
        } else {
            var html = document.getElementById("article").innerHTML;
            html = "<div><style> .editsection, .infobox, .mw-jump { visibility: hidden; display: none; }</style>" + html + "</div>";
            html = Windows.ApplicationModel.DataTransfer.HtmlFormatHelper.createHtmlFormat(html);
            request.data.setHtmlFormat(html);
        }

        var parser = new Tautologistics.NodeHtmlParser.Parser(handler);
        parser.parseComplete(document.getElementById("article").innerHTML);

        var dom = handler.dom;
        var nodes = SoupSelect.select(dom, "h1.firstHeading span");
        var title = "unknown title";
        if (nodes.length > 0 && nodes[0].children.length > 0) {
            title = nodes[0].children[0].data;
        }

        request.data.properties.title = title;
        request.data.properties.description = "Wikipedia article";
    }

    // Function that provides search suggestions to the search charm. This
    // code uses the OpenQuery AJAX API on MediaWiki to retrieve search
    // suggestions. 
    // TODO: make this function robust in the face of no network availability
    
    var suggestionUri = "http://en.wikipedia.org/w/api.php?action=opensearch&search=";

    function search_suggestions(eventObject) {
        var queryText = eventObject.queryText, language = eventObject.language, suggestionRequest = eventObject.request;

        // Indicate that we'll obtain suggestions asynchronously:
        var deferral = suggestionRequest.getDeferral();

        // Cancel the previous suggestion request if it is not finished.
        if (xhrRequest && xhrRequest.cancel) {
            xhrRequest.cancel();
        }

        // Create request to obtain suggestions from service and supply them to the Search Pane.
        xhrRequest = WinJS.xhr({ url: suggestionUri + encodeURIComponent(queryText) });
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
    }

    // Publish functions as part of the wikipedia namespace

    WinJS.Namespace.define("wikipedia", {
        search_suggestions: search_suggestions,
        share: share,
        parse: parse
    });
})();