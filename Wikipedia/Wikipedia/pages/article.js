(function () {
    "use strict";

    var app = WinJS.Application;

    // Render the target url in the page

    function render(obj) {

        // Display our busy wait spinner
        // TODO: figure out where to put this back in

        //articlePageTitle.innerHTML = "";
        //article.innerHTML = "<img class='splash' src='/images/ajax-loader.gif'>";

        // Retrieve the wikipedia page given the URL and render the results

        articlePageTitle.innerHTML = obj.title;
        article.innerHTML = obj.html;
        article.focus(); // ensure that keyboard focus is on the <div> element

        // Bind event handler to <a> elements to the global navigation handler 

        $('div.mw-body a').click(app.navigate);
    }

    WinJS.UI.Pages.define("/pages/article.html", {

        // Render the wikipedia article object 

        ready: function (element, obj) {
            render(obj);
        },

        updateLayout: function (element, viewState, lastViewState) {
            /// <param name="element" domElement="true" />
            /// <param name="viewState" value="Windows.UI.ViewManagement.ApplicationViewState" />
            /// <param name="lastViewState" value="Windows.UI.ViewManagement.ApplicationViewState" />

            // TODO: Respond to changes in viewState.
        },

        unload: function () {
            // TODO: Respond to navigations away from this page.
        }
    });
})();