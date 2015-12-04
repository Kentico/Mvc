(function () {
    var geocoder;
    var map;
    var markers = {};
    var bounds = new google.maps.LatLngBounds();
    var mapElement = jQuery('div.js-map');

    function initialize() {
        geocoder = new google.maps.Geocoder();
        var mapOptions = {
            mapTypeId: google.maps.MapTypeId.ROADMAP,
            mapTypeControl: true,
            scaleControl: true,
            panControl: true,
            streetViewControl: true,
            zoomControl: true,
            keyboardShortcuts: false,
            draggable: true,
            zoomControlOptions: {
                style: google.maps.ZoomControlStyle.DEFAULT
            }
        };
        map = new google.maps.Map(mapElement[0], mapOptions);
        jQuery('.js-scroll-to-map').each(function () {
            var address = jQuery(this).attr('data-address');
            addMarker(address);
            jQuery(this).click(function () {
                panToMarker(address);
            });
        });
    }

    function panToMarker(address) {
        jQuery('html, body').animate({
            scrollTop: mapElement.offset().top
        }, 500);
        map.setZoom(17);
        map.panTo(markers[address].position);
    }

    function addMarker(address) {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
                markers[address] = marker;
                bounds.extend(marker.getPosition());
                map.fitBounds(bounds);
            } else {
                console.warn('Geocode was not successful for the following reason: ' + status);
            }
        });
    }

    google.maps.event.addDomListener(window, 'load', initialize);
}());