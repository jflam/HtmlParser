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

    function render(url) {
        articlePageTitle.innerHTML = "";
        article.innerHTML = "<img class='splash' src='/images/ajax-loader.gif'>";
        wikipedia.parse(url).then(function (obj) {

            articlePageTitle.innerHTML = obj.title;
            article.innerHTML = obj.html;

            // Bind event handler to overload the click event on an <a> element

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