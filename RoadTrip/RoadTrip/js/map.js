(function () {
    var map = null;
    var locations = null; // locations of places on the map

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
            
            // Bind handler for map resized event
            Microsoft.Maps.Events.addHandler(map, 'viewchangeend', function () {
                // Compute from bounds
                var locationRect = map.getBounds();
                var halfWidth = locationRect.width / 2;
                var halfHeight = locationRect.height / 2;
                var bbox = [locationRect.center.latitude - halfHeight, locationRect.center.longitude - halfWidth, locationRect.center.latitude + halfHeight, locationRect.center.longitude + halfWidth];
                var message = {
                    action: 'map_resized',
                    bbox: bbox
                };

                // Post message back to parent frame
                window.parent.postMessage(message, 'ms-appx://' + document.location.host);
            });
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
        locations = message.locations; // assign to module scoped variable for later reference
        var locations = message.locations;
        for (var index in locations) {
            var map_location = new Microsoft.Maps.Location(locations[index].Latitude, locations[index].Longitude);
            var pushpin = new Microsoft.Maps.Pushpin(map_location);

            // Bind an event handler to the pushpin to replace it with an InfoBox when the user clicks on it.
            Microsoft.Maps.Events.addHandler(pushpin, 'click', (function (location) {
                return function () {
                    var options = {
                        title: location.Title,
                        description: location.Address,
                        titleClickHandler: function () {
                            console.log("Navigating to: " + location.Url);
                        }
                    };
                    var infobox = new Microsoft.Maps.Infobox(new Microsoft.Maps.Location(location.Latitude, location.Longitude), options);
                    map.entities.push(infobox);

                    console.log("clicked on: ");
                }
            })(locations[index]));

            map.entities.push(pushpin);
        }
    };

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