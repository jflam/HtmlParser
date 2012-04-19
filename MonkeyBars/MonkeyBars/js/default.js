﻿// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.editor = null;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
                // TODO: initialize WL library here
                // WL.init();
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());

            // Bind event handlers 
            document.getElementById('cmdOpenFile').addEventListener('click', app.load_buffer);
            document.getElementById('cmdSaveFile').addEventListener('click', app.save_buffer);
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

    function ensureUnsnapped() {
        // FilePicker APIs will not work if the application is in a snapped state.
        // If an app wants to show a FilePicker while snapped, it must attempt to unsnap first
        return ((Windows.UI.ViewManagement.ApplicationView.value !== Windows.UI.ViewManagement.ApplicationViewState.snapped) ||
            Windows.UI.ViewManagement.ApplicationView.tryUnsnap());
    }
    
    // TODO: figure out how to design commands correctly in WWAs ... need a sample
    app.save_buffer = function () {
        var text = app.editor.getValue();
        app.save_file_locally(text);
    };

    app.load_buffer = function () {
        var text = app.load_file_locally().then(function (text) {
            app.editor.setValue(text);
        });
    };

    // This function will save either locally via a file picker or to SkyDrive (later)
    app.save_file_locally = function (text) {
        if (ensureUnsnapped()) {
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
            savePicker.defaultFileExtension = ".js";
            savePicker.suggestedFileName = "my_code";
            savePicker.fileTypeChoices.insert("Javascript", [".js"]);

            savePicker.pickSaveFileAsync().done(function (file) {
                if (file) {
                    Windows.Storage.FileIO.writeTextAsync(file, text);
                } else {
                    // TODO: Error - when would we ever wind up here without a file?
                }
            });
        }
    };

    app.load_file_locally = function () {
        return new WinJS.Promise(function (complete) {
            if (ensureUnsnapped()) {
                var openPicker = new Windows.Storage.Pickers.FileOpenPicker();
                openPicker.viewMode = Windows.Storage.Pickers.PickerViewMode.list;
                openPicker.suggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.documentsLibrary;
                openPicker.fileTypeFilter.replaceAll([".js"]);

                openPicker.pickSingleFileAsync().done(function (file) {
                    if (file) {
                        Windows.Storage.FileIO.readTextAsync(file).done(complete);
                    } else {
                        // TODO: Error - when would we ever wind up here without a file?
                    }
                });
            }
        });
    };

    app.eval_selection = function () {

        // If the user doesn't have a selection, we execute the current line, otherwise we execute the selection
        var code = null;
        if (app.editor.somethingSelected()) {
            // Eval the selection
            code = app.editor.getSelection();
            var result = eval(code);

        } else {
            // We need to select the entire line that the cursor is on
            var pos = app.editor.getCursor(true);
            code = app.editor.getLine(pos.line);
        }

        // Execute user code
        var result = eval(code);

        // Get the end of the selection
        var selection_end = app.editor.getCursor(false);
        selection_end.ch = null; // end of line
        app.editor.setCursor(selection_end);

        // Insert the result after the end of the selection
        var new_position = null;
        if (typeof (result) === 'function') {
            new_position = app.editor.replaceRange('\n//> created function(s)\n', selection_end);
        } else {
            new_position = app.editor.replaceRange('\n//> ' + result + '\n', selection_end);
        }

        // Make sure the cursor is on the start of the line after the result
        app.editor.setCursor({line: new_position.line + 1, ch: 0});
    };

    app.save_file_to_skydrive = function () {
        // TODO: something here with skydrive
        //var access_token = '6YYd0AdCOAHziZSdhldhX2f5cAkWkPyi';
        //WL.login({
        //    scope: 'wl.skydrive'
        //}).then(
        //    function (response) {
        //        WL.api({
        //            path: 'me/skydrive', // does this work?
        //            method: 'GET'
        //        }).then(
        //            function (response) {
        //                var x = 42;
        //            }
        //        );
        //    },
        //    function (responseFailed) {
        //        // TODO:
        //        var err = 42;
        //    }
        //);
        //var x = 42;
    };
})();
