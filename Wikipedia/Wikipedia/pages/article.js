// For an introduction to the Page Control template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkId=232511
(function () {
    "use strict";

    var default_page = "http://en.wikipedia.org/wiki/Windows_8";
    var backstack = [];

    function render(url) {
        articlePageTitle.innerHTML = "";
        article.innerHTML = "<img class='splash' src='/images/ajax-loader.gif'>";
        wikipedia.parse(url).then(function (obj) {

            articlePageTitle.innerHTML = obj.title;
            article.innerHTML = obj.html;

            // Bind event handler to overload the click event on an <a> element
            // TODO: does this have to be bound to the handler or can i let it bubble?

            $('div.mw-body a').click(function (e) {
                e.preventDefault();
                var target = e.currentTarget.href;

                // Push the current url (captured in this lambda) onto the back stack
                backstack.push(url);
                render(target);
            });
        });
    }

    function go_back() {
        if (backstack.length > 0) {
            var url = backstack.pop();
            render(url);
        }
    }

    WinJS.UI.Pages.define("/pages/article.html", {
        // This function is called whenever a user navigates to this page. It
        // populates the page elements with the app's data.
        ready: function (element, options) {
            render(default_page);
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
