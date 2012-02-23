// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

var locations = new WinJS.Binding.List([
    { location: "Seattle, WA" },
    { location: "Portland, OR" }
]);

(function () {
    "use strict";

    var app = WinJS.Application;

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize 
                // your application here.
                $("#list")[0].winControl.addEventListener("iteminvoked", function (eventObject) {
                    eventObject.detail.itemPromise.then(function (invokedItem) {
                        document.frames["map"].postMessage(invokedItem.data.location, "ms-appx-web://" + document.location.host);
                        console.log("clicked on: " + invokedItem.data.location);
                    });
                }, false);
            } else {
                // TODO: This application has been reactivated from suspension. 
                // Restore application state here.
            }

            //$("#bob").click(function () {
            //    // Post something to the child window
            //    document.frames["map"].postMessage("", "ms-appx-web://" + document.location.host);

            //    // add some crap
            //});
            WinJS.UI.processAll();
        }
    };

    app.oncheckpoint = function (eventObject) {
        // TODO: This application is about to be suspended. Save any state
        // that needs to persist across suspensions here. You might use the 
        // WinJS.Application.sessionState object, which is automatically
        // saved and restored across suspension. If you need to complete an
        // asynchronous operation before your application is suspended, call
        // eventObject.setPromise(). 
    };

    app.start();
})();
