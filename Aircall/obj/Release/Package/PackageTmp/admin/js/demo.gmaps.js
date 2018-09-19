// Defining some global variables
var map, geocoder, marker, infowindow, zoom, bounds;
var icounter = 0;
var DemoGMaps = function () {

    function getZoomLevel() {
        zoom = 8;
    }

    var mapMarker = function () {
        bounds = new google.maps.LatLngBounds();
        var options = {
            zoom: 5,
            minZoom: 5,
            center: new google.maps.LatLng(36.519015, -119.939135),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('gmap_marker'), options);

        var employeeId = $("#drpEmployee").val();
        $.ajax({
            type: "POST",
            url: "PendingService_List.aspx/GetEmployeeMapData",
            data: "{'EmployeeId':'" + employeeId + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                LoadMapData(response.d);
            },
            failure: function (response) {
                alert(response.d);
            }
        });
        //var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
        //var lat = $("#txtLatitude").val();
        //var lng = $("#txtLongitude").val();
        //// Making the Geocoder call 
        //var address = '';
        //if (lat != "" && address != "") {
        //    getCoordinates(address, lat, lng);
        //}
        //else if (lat != "") {
        //    getCoordinates('', lat, lng);
        //}
        //else if (address != "") {
        //    getCoordinates(address, 0, lng);
        //}
    }

    function LoadMapData(markers) {
        // Display multiple markers on a map
        var infoWindow = new google.maps.InfoWindow(), marker, i;
        var addedMarker = [];
        // Loop through our array of markers & place each one on the map  
        for (i = 0; i < markers.length; i++) {
            addedMarker.push([markers[i].EmpLat, markers[i].EmpLng]);
            var position = new google.maps.LatLng(markers[i].Lat, markers[i].Lng);
            bounds.extend(position);
            marker = new google.maps.Marker({
                position: position,
                map: map,
                title: markers[i].ServiceCaseNumber
            });

            // Allow each marker to have an info window    
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    infoWindow.setContent(infoWindowContent[i][0]);
                    infoWindow.open(map, marker);
                }


            })(marker, i));
            if (markers[i].EmpLat != 0 && markers[i].EmpLng != 0) {

                var position1 = new google.maps.LatLng(markers[i].EmpLat, markers[i].EmpLng);
                bounds.extend(position1);
                marker1 = new google.maps.Marker({
                    position: position1,
                    map: map,
                    icon: "http://maps.google.com/mapfiles/ms/icons/blue-dot.png",
                    title: markers[i].EmpName
                });

                // Allow each marker to have an info window    
                google.maps.event.addListener(marker1, 'click', (function (marker1, i) {
                    return function () {
                        infoWindow.setContent(infoWindowContent[i][0]);
                        infoWindow.open(map, marker1);
                    }


                })(marker1, i));
            }
            // Automatically center the map fitting all markers on the screen
            //map.fitBounds(bounds);
        }
        if (markers.length > 0) {
            //map.fitBounds(bounds);
        }
        google.maps.event.addDomListener(window, "resize", function () {
            var center = map.getCenter();
            google.maps.event.trigger(map, "resize");
            map.setCenter(center);
        });
        // Override our map zoom level once our fitBounds function runs (Make sure it only runs once)
        //var boundsListener = google.maps.event.addListener((map), 'bounds_changed', function (event) {
        //    //this.setZoom(8);
        //    google.maps.event.removeListener(boundsListener);
        //});
    }

    function ChangeMap() {
        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
        var lat = $("#txtLatitude").val();
        var lng = $("#txtLongitude").val();
        // Making the Geocoder call 
        if (lat != "") {
            getCoordinates(address, lat, lng);
        }
        else {
            getCoordinates(address, 0, lng);
        }

        // Preventing the form from doing a page submit
        return false;
    }

    var mapGeolocation = function () {

        var map = new GMaps({
            div: '#gmap_geo',
            lat: -12.043333,
            lng: -77.028333
        });

        GMaps.geolocate({
            success: function (position) {
                $("#maperror").attr("style", "display:none");
                $("#hdnLatitude").val(position.coords.latitude);
                $("#hdnLongitude").val(position.coords.longitude);
                map.setCenter(position.coords.latitude, position.coords.longitude);
            },
            error: function (error) {
                alert('Geolocation failed: ' + error.message);
            },
            not_supported: function () {
                alert("Your browser does not support geolocation");
            },
            always: function () {
                //alert("Geolocation Done!");
            }
        });
    }

    var mapGeocoding = function () {

        var map = new GMaps({
            div: '#gmap_geocoding',
            lat: -12.043333,
            lng: -77.028333
        });

        var handleAction = function () {
            var text = $.trim($('#gmap_geocoding_address').val());
            GMaps.geocode({
                address: text,
                callback: function (results, status) {
                    if (status == 'OK') {
                        $("#maperror").attr("style", "display:none");
                        var latlng = results[0].geometry.location;
                        map.setCenter(latlng.lat(), latlng.lng());
                        map.addMarker({
                            lat: latlng.lat(),
                            lng: latlng.lng()
                        });
                        App.scrollTo($('#gmap_geocoding'));
                    }
                }
            });
        }

        $('#gmap_geocoding_btn').click(function (e) {
            e.preventDefault();
            handleAction();
        });

        $("#gmap_geocoding_address").keypress(function (e) {
            var keycode = (e.keyCode ? e.keyCode : e.which);
            if (keycode == '13') {
                e.preventDefault();
                handleAction();
            }
        });

    }


    function getCoordinates(address, lat, lng) {

        lat = (typeof lat === "undefined") ? 0 : lat;
        lng = (typeof lng === "undefined") ? 0 : lng;

        // Check to see if we already have a geocoded object. If not we create one
        if (!geocoder) {
            geocoder = new google.maps.Geocoder();
        }
        var geocoderRequest;
        var latlng1;
        // Creating a GeocoderRequest object
        if (lat == 0 && lng == 0) {

            geocoderRequest = {
                address: address
            }
        }
        else {
            latlng1 = new google.maps.LatLng(lat, lng);
            geocoderRequest = {
                'latLng': latlng1
            }
        }
        // Making the Geocode request
        geocoder.geocode(geocoderRequest, function (results, status) {

            // Check if status is OK before proceeding
            if (status == google.maps.GeocoderStatus.OK) {
                // Center the map on the returned location

                // Check to see if we've already got a Marker object
                if (!marker) {

                    // Creating a new marker and adding it to the map
                    marker = new google.maps.Marker({
                        map: map,
                        //draggable: true
                        draggable: false
                    });
                }
                getZoomLevel();
                map.setZoom(zoom);
                if (lat == 0 && lng == 0) {
                    map.setCenter(results[0].geometry.location);
                    marker.setPosition(results[0].geometry.location);
                }
                else {
                    map.setCenter(latlng1);
                    marker.setPosition(latlng1);
                }

                marker = new google.maps.Marker({
                    position: new google.maps.LatLng(34.084178, -118.344894)
                });
                marker.setMap(map);

                marker = new google.maps.Marker({
                    position: new google.maps.LatLng(34.082404, -118.344893)
                });
                marker.setMap(map);
                return;
            }
            else {
                $("#maperror").attr("style", "display:block");
                $("#maperror").text("Invalid latitude and longitude");

                return;
            }
            $("#txtLatitude").val(marker.getPosition().lat().toFixed(10));
            $("#txtLongitude").val(marker.getPosition().lng().toFixed(10));
            return;
        }
        );
        //  }
    }


    return {
        //main function to initiate map samples
        init: function () {

            mapMarker();

        },
        mapchange: function () {
            ChangeMap();
        }
    };

}();