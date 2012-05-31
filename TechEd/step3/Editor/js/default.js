// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    WinJS.strictProcessing();

    app.editor = null;
    app.file_token = null;

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
            document.getElementById('cmdOpen').addEventListener('click', app.open_file);
            document.getElementById('cmdSave').addEventListener('click', app.save_file);
        }
    };

    app.onloaded = function (args) {
        app.editor = CodeMirror.fromTextArea(document.getElementById("editor"), {
            lineNumbers: true,
            indentUnit: 4,
            theme: "lesser-dark",
            keyMap: "html_editor"
        });

        CodeMirror.keyMap.html_editor = {
            'Ctrl-Enter': function (cm) {
                // Eval the HTML here and drop it into the <div>
                var html = app.editor.getValue();
                try {
                    preview.innerHTML = html;
                }
                catch (e) {
                    console.log(e.message);
                }
            },
            'Ctrl-S': function (cm) {
                app.save_file(null);
            },
            fallthrough: ["default"],
        };

        var xhrRequest = null;

        // Provide suggestions using an URL that supports the Open Search suggestion format.
        // Scenarios 2-6 introduce different methods of providing suggestions. The registration for the onsuggestionsrequested
        // event is added in a local scope for this sample's purpose, but in the common case, you should place this code in the
        // global scope, e.g. default.js, to run as soon as your app is launched. This way your app can provide suggestions
        // anytime the user brings up the search pane.
        Windows.ApplicationModel.Search.SearchPane.getForCurrentView().onsuggestionsrequested = function (eventObject) {
            var queryText = eventObject.queryText, language = eventObject.language, suggestionRequest = eventObject.request;

            // The deferral object is used to supply suggestions asynchronously for example when fetching suggestions from a web service.
            // Indicate that we'll obtain suggestions asynchronously:
            var deferral = suggestionRequest.getDeferral();

            // This refers to a local package file that contains a sample JSON response.
            // You can update the Uri to a service that supports this standard in order to see suggestions come from a web service.
            var suggestionUri = "http://en.wikipedia.org/w/api.php?action=opensearch&search=";
            // If you are using a webservice,the query string should be encoded into the URI. See example below:
            suggestionUri += encodeURIComponent(queryText);

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
    };

    app.open_file = function (args) {
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
        openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
        openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
        openPicker.fileTypeFilter.replaceAll([".htm", ".zip"]);

        openPicker.pickSingleFileAsync().done(function (file) {
            // Squirrel away a token for future access
            app.file_token = Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.add(file);
            ZipHelper.Zip.open(file).done(function (html) {
                var regex = /\"\/\/(.*?)\"/ig;
                var result = html.replace(regex, "\"http://$1\"");

                var clean_html = window.toStaticHTML(result);
                app.editor.setValue(clean_html);
            });
        });
    };

    app.save_file = function (args) {
        var text = app.editor.getValue();
        if (app.file_token == null) {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
            savePicker.defaultFileExtension = ".htm";
            savePicker.suggestedFileName = "my_html";
            savePicker.fileTypeChoices.insert("HTML", [".htm"]);

            savePicker.pickSaveFileAsync().done(function (file) {
                if (file) {
                    Windows.Storage.FileIO.writeTextAsync(file, text);
                }
            });
        } else {
            Windows.Storage.AccessCache.StorageApplicationPermissions.futureAccessList.getFileAsync(app.file_token).done(function (file) {
                if (file) {
                    Windows.Storage.FileIO.writeTextAsync(file, text);
                }
            });
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

    app.start();
})();
