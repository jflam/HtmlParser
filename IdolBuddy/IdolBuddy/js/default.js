// For an introduction to the Grid template, see the following documentation:
// http://go.microsoft.com/fwlink/?LinkID=232446

// Database of American Idol Twitter IDs
var idols = new WinJS.Binding.List([
    { name: 'Colton Dixon', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/colton_dixon.jpg', twitter: 'CDixonAI11', facebook: 'CDixonAI11' },
    { name: 'Deandre Brackensick', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/deandre_brackensick.jpg', twitter: 'BrackensickAI11', facebook: 'BrackensickAI11' },
    { name: 'Elise Testone', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/elise_testone.jpg', twitter: 'ETestoneAI11', facebook: 'ETestoneAI11' },
    { name: 'Erika Van Pelt', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/erika_van_pelt.jpg', twitter: 'EVanPeltAI11', facebook: 'EVanPeltAI11' },
    { name: 'Heejun Han', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/heejun_han.jpg', twitter: 'HHanAI11', facebook: 'HHanAI11' },
    { name: 'Hollie Cavanagh', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/hollie_cavanagh.jpg', twitter: 'CavanaghAI11', facebook: 'CavanaghAI11' },
    { name: 'Jeremy Rosado', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/jeremy_rosado.jpg', twitter: 'JRosadoAI11', facebook: 'JRosadoAI11' },
    { name: 'Jermaine Jones', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/jermaine_jones.jpg', ttwitter: 'JJonesAI11', facebook: 'JJonesAI11' },
    { name: 'Jessica Sanchez', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/jessica_sanchez.jpg', ttwitter: 'JSanchezAI11', facebook: 'JSanchezAI11' },
    { name: 'Joshua Ledet', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/joshua_ledet.jpg', ttwitter: 'JLedetAI11', facebook: 'JLedetAI11' },
    { name: 'Philip Phillips', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/phillip_phillips.jpg', ttwitter: 'PPhillipsAI11', facebook: 'PPhillipsAI11' },
    { name: 'Shannon Magrane', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/shannon_magrane.jpg', ttwitter: 'SMagraneAI11', facebook: 'SMagraneAI11' },
    { name: 'Skylar Laine', photo: 'http://media.americanidol.com/contestants/season_11/5a9dkg8g6e/top_24/skylar_laine.jpg', ttwitter: 'SLaineAI11', facebook: 'SLaineAI11' }
]);

function item_template(item_promise) {
    return item_promise.then(function (current_item) {
        var result = document.createElement('div');
        result.className = 'idol-item';

        var image = document.createElement('img');
        image.className = 'idol-photo';
        image.src = current_item.data.photo;
        result.appendChild(image);

        var overlay = document.createElement('div');
        overlay.className = 'idol-overlay';

        var caption = document.createElement('h4');
        caption.className = 'idol-title';
        caption.textContent = current_item.data.name;
        overlay.appendChild(caption);

        var subtitle = document.createElement('h6');
        subtitle.className = 'idol-subtitle';
        if (current_item.data.twitter_user_info != undefined) {
            subtitle.textContent = current_item.data.twitter_user_info.followers_count;
        }
        overlay.appendChild(subtitle);

        result.appendChild(overlay);
        return result;
    });
};

var follower_count_query = 'https://api.twitter.com/1/users/show.json?screen_name=';

function get_twitter_user_info(name) {
    return WinJS.xhr({
        url: follower_count_query + name,
    });
};

(function () {
    "use strict";

    var app = WinJS.Application;

    // TODO: there is a lot to analyze in this function. especially around errors. what should this do? this is hard to
    // truly understand the nuances of the function.
    app.update_data = function () {
        WinJS.Promise.join(idols.map(function (idol) {
            return get_twitter_user_info(idol.twitter).then(function (response) {
                idol.twitter_user_info = JSON.parse(response.responseText).followers_count;
                return idol.twitter_user_info;
            });
        })).done(function (followers) {
            // trigger the re-bind
            id('idolsListView').winControl.forceLayout();
        });
    };

    app.onactivated = function (eventObject) {
        if (eventObject.detail.kind === Windows.ApplicationModel.Activation.ActivationKind.launch) {
            if (eventObject.detail.previousExecutionState !== Windows.ApplicationModel.Activation.ApplicationExecutionState.terminated) {
                // TODO: This application has been newly launched. Initialize 
                // your application here.
                app.update_data();
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
