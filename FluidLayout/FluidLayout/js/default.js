// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509

var my_data = new WinJS.Binding.List([
        { title: 'First', size: 'small' },
        { title: 'Second', size: 'medium' },
        { title: 'Third', size: 'large' },
        { title: 'First', size: 'small' },
        { title: 'Second', size: 'medium' },
        { title: 'Third', size: 'large' },
        { title: 'First', size: 'small' },
        { title: 'Second', size: 'medium' },
        { title: 'Third', size: 'large' }
    ]);


function item_template(item_promise) {
    return item_promise.then(function (current_item) {
        var result = document.createElement('div');
        result.className = current_item.data.size;

        var title = document.createElement("h4");
        title.innerText = current_item.data.title;
        result.appendChild(title);

        return result;
    });
};

(function () {
    "use strict";

    var app = WinJS.Application;

    function groupInfo() {
        return {
            multiSize: true,
            slotWidth: 310,
            slotHeight: 80
        };
    }

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize 
                // your application here.
            } else {
                // TODO: This application has been reactivated from suspension. 
                // Restore application state here.
            }
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
