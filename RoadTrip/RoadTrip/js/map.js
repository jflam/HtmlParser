(function () {
    var map = null;

    get_map = function (message) {
        if (map == null) {
            var mapOptions = {
                credentials: message.credentials,
                center: new Microsoft.Maps.Location(47.733762, -122.146974),
                mapTypeId: Microsoft.Maps.MapTypeId.road,
                zoom: 7,
                showLogo: false
            };
            map = new Microsoft.Maps.Map($("#map")[0], mapOptions);
        }
        return map;
    }

    zoom_to = function (message) {
        var topLeft = new Microsoft.Maps.Location(message.bbox[0], message.bbox[1]);
        var bottomRight = new Microsoft.Maps.Location(message.bbox[2], message.bbox[3]);
        var rect = new Microsoft.Maps.LocationRect.fromLocations(topLeft, bottomRight);
        get_map(message).setView({ bounds: rect });
    };

    drop_pins = function (message) {
        var map = get_map(message);
        map.entities.clear();
        var locations = message.locations;
        for (var index in locations) {
            var map_location = new Microsoft.Maps.Location(locations[index].Latitude, locations[index].Longitude);
            var pushpin = new Microsoft.Maps.Pushpin(map_location);
            map.entities.push(pushpin);
        }
    }

    receive_message = function (e) {
        var message = e.data;
        switch (e.data.action) {
            case 'zoom_to':
                zoom_to(message);
                break;
            case 'drop_pins':
                drop_pins(message);
                break;
            default:
                console.log('invalid action: ' + message.action);
                break;
        }
    };
    
    window.addEventListener("message", receive_message, false);
})();