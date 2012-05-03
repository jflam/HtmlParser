// For an introduction to the Blank template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232509
(function () {
    "use strict";

    var app = WinJS.Application;
    var activation = Windows.ApplicationModel.Activation;
    WinJS.strictProcessing();

    app.render_wikipedia = function () {
        $.ajax("http://en.wikipedia.org/wiki/Portsmouth").then(
            function (response) {
                // hack around with the htmlparser
                var handler = new Tautologistics.NodeHtmlParser.DefaultHandler(function (error, dom) {
                    //if (error)
                    //    // error
                    //else
                    //    // parsing done ... do something
                });
                var parser = new Tautologistics.NodeHtmlParser.Parser(handler);
                parser.parseComplete(response);

                // handler.dom has the interesting things in it
                var dom = handler.dom;

                // now let's use soupselect ...
                var images = SoupSelect.select(dom, "img");
                var tags = "";
                for (var index in images) {
                    var image = images[index];
                    image.attribs.src = "http:" + image.attribs.src;
                    tags += "<img src='" + image.attribs.src + "'><br>";
                }

                var html = window.toStaticHTML(tags);
                article.innerHTML = html;

                //// TODO: remove hack while we wait for a fix for this bug
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

            var button = document.getElementById("render_wikipedia");
            button.addEventListener("click", function () {
                app.render_wikipedia();
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
