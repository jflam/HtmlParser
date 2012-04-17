// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;

    app.editor = null;

    app.onactivated = function (args) {
        if (args.detail.kind === activation.ActivationKind.launch) {
            if (args.detail.previousExecutionState !== activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize
                // your application here.
                WL.init();
            } else {
                // TODO: This application has been reactivated from suspension.
                // Restore application state here.
            }
            args.setPromise(WinJS.UI.processAll());
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
    }

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
