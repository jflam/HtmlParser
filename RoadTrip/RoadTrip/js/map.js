(function () {
    var credentials = "ArPzSbmnfYJucWzarJwB7Gk29sfWPy97C6rpz-Rm1xTO_AWa-_hCg-5Oo-mAHkcR";
    var map = null;

    $(document).ready(function () {
        var mapOptions = {
            // TODO: secure these credentials
            credentials: credentials,
            center: new Microsoft.Maps.Location(47.733762, -122.146974),
            mapTypeId: Microsoft.Maps.MapTypeId.road,
            zoom: 7,
            showLogo: false
        };
        map = new Microsoft.Maps.Map($("#map")[0], mapOptions);
    });

    search = function (query) {
        var searchRequest = 'http://dev.virtualearth.net/REST/v1/Locations/' + query + '?output=json&jsonp=search_callback&key=' + credentials;
        var mapscript = document.createElement('script');
        mapscript.type = 'text/javascript';
        mapscript.src = searchRequest;
        document.getElementById('map').appendChild(mapscript);
    };

    search_callback = function (result) {
        var bbox = result.resourceSets[0].resources[0].bbox;
        var topLeft = new Microsoft.Maps.Location(bbox[0], bbox[1]);
        var bottomRight = new Microsoft.Maps.Location(bbox[2], bbox[3]);
        var rect = new Microsoft.Maps.LocationRect.fromLocations(topLeft, bottomRight);
        map.setView({ bounds: rect });
    };

    receive_message = function (e) {
        search("new york, ny");
    };
    
    window.addEventListener("message", receive_message, false);
})();