(function () {
    "use strict";

    var default_page = "http://en.wikipedia.org/wiki/Windows_8";
    var nav = WinJS.Navigation;

    // Function that decides whether it is an internal link or an external link
    // and navigates internally if it is an internal link

    function navigate(e) {
        var target = e.currentTarget.href;
        if (target.search("wikipedia.org") >= 0) {

            // This is an internal navigation event since the URL contains wikipedia.org

            e.preventDefault(); 
            nav.navigate("/pages/article.html", { url: target });
        }
    }

    // Render the target url in the page

    function render(url) {

        // Display our busy wait spinner

        articlePageTitle.innerHTML = "";
        article.innerHTML = "<img class='splash' src='/images/ajax-loader.gif'>";

        // Retrieve the wikipedia page given the URL and render the results

        wikipedia.parse(url).then(function (obj) {

            articlePageTitle.innerHTML = obj.title;
            article.innerHTML = obj.html;
            article.focus(); // ensure that keyboard focus is on the <div> element

            // Bind event handler to <a> elements so that we can have correct
            // internal / external navigation

            $('div.mw-body a').click(navigate);
        });
    }

    WinJS.UI.Pages.define("/pages/article.html", {

        // Show default_page if there aren't any arguments passed

        ready: function (element, options) {
            var url = default_page;
            if (options != null && options.url != null) {
                url = options.url;
            }
            render(url);
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