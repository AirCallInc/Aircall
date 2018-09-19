// Defining some global variables
var map, geocoder, marker, infowindow, zoom;
var icounter = 0;
var DemoGMaps = function () {

    function getZoomLevel() {
        if ($("#tb_game_answer").val() != "") {
            zoom = 15;
        }
        else
        {
            zoom = 15;
        }

    /*  if ($("#txtAddress").val() != "") {
            zoom = 19;
        }
        else if ($("#txtZipcode").val() != "") {
            zoom = 14;
        }
        else if ($("#ddlCity").find("option:selected").val() != "0") {
            zoom = 9;
        }
        else if ($("#ddlState").find("option:selected").val() != "0") {
            zoom = 7;
        }
        else if ($("#ddlCountry").find("option:selected").val() != "0") {
            zoom = 5;
        }
        else {
            zoom = 5;
        }
    */
    }

    var mapMarker = function () {
        // Creating a new map
        $("#hdnstatus").val("1");
        $("#txtLatitude").attr("disabled", "disabled");
        $("#txtLongitude").attr("disabled", "disabled");

        $("#hdnLatitude").val($("#txtLatitude").val());
        $("#hdnLongitude").val($("#txtLongitude").val());
        var options = {
            zoom: 16,
            center: new google.maps.LatLng(37.09, -95.71),
            mapTypeId: google.maps.MapTypeId.ROADMAP
        };
        map = new google.maps.Map(document.getElementById('gmap_marker'), options);

        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
        var lat = $("#txtLatitude").val();
        var lng = $("#txtLongitude").val();
        // Making the Geocoder call 

        if (lat != "") {
            getCoordinates(address, lat, lng);
        }
        else if (address != "") {
            getCoordinates(address, 0, lng);
        }
    }
    $("#ddlMerchantCompany").live("change", function () {
        var countryid;
        var urls = "";
        if (pathname.search("merchantstores_addedit")) {

            urls = "merchantstores_addedit.aspx/GetCountry";
        }
        else if (pathname.search("merchantcompetitors_addedit")) {
            urls = "merchantcompetitors_addedit.aspx/GetCountry";
        }
        $.ajax({

            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: urls,
            data: "{merchantId:'" + $("#ddlMerchantCompany").val() + "'}",
            dataType: "json",
            success: function (data) {

                countryid = data.d;
            },
            complete: function () {
                if (icounter == 0) {
                    var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + countryid;

                    // Making the Geocoder call 
                    getCoordinates(address);
                    icounter = 1;
                    // Preventing the form from doing a page submit
                    return false;
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert("Message: " + r.Message);
                alert("StackTrace: " + r.StackTrace);
                alert("ExceptionType: " + r.ExceptionType);
            }
        });

        

    });
    
/*    $("#ddlCountry").live("change", function () {

        // Getting the address from the text input
        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");

        // Making the Geocoder call 
        getCoordinates(address);

        // Preventing the form from doing a page submit
        return false;

    });
*/
     $("#tb_game_answer").live("change", function () {

        // Getting the address from the text input
    /*    var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
    */
        var address = $("#tb_game_answer").val() != "" ? $("#tb_game_answer").val():" ";

        // Making the Geocoder call 
        getCoordinates(address);

        // Preventing the form from doing a page submit
        return false;

    });
/*    $("#ddlState").live("change", function () {

        var pathname = window.location.pathname;
        var urls = "";
        if (pathname.search("merchantstores_addedit")) {

            urls = "merchantstores_addedit.aspx/BindDatatoDropdown";
        }
        else if (pathname.search("merchantcompetitors_addedit")) {
            urls = "merchantcompetitors_addedit.aspx/BindDatatoDropdown";
        }


        $("#ddlCity").empty();
        


            $.ajax({

                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: urls,
                data: "{state:'" + $("#ddlState").val() + "',languageId:'" + $("#hdLanguageId").val() + "'}",
                dataType: "json",
                success: function (data) {

                    $.each(data.d, function (key, value) {


                        $("#ddlCity").append($("<option></option>").val(value.CityId).html(value.CityName));

                    });
                },
                complete: function (data) {
                    var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");

                    // Making the Geocoder call 
                    getCoordinates(address);


                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert("Message: " + r.Message);
                    alert("StackTrace: " + r.StackTrace);
                    alert("ExceptionType: " + r.ExceptionType);
                }
            });

       

        // Getting the address from the text input


        // Preventing the form from doing a page submit
        //return false;

    });

    $("#ddlCity").live("change", function () {

        $("#hdnCityId").val($("#ddlCity").val());
        // Getting the address from the text input
        //var address = $(this).find("option:selected").text() + "," + $("#ddlState").find("option:selected").text() + "," + $("#ddlCountry").find("option:selected").text();
        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
        // Making the Geocoder call 
        getCoordinates(address);

        // Preventing the form from doing a page submit
        return false;

    });

    $("#txtZipcode").live("change", function () {

        // Getting the address from the text input
        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");

        // Making the Geocoder call
        getCoordinates(address);
        $("#hdnLatitude").val($("#txtLatitude").val());
        $("#hdnLongitude").val($("#txtLongitude").val());
        // Preventing the form from doing a page submit
        return false;

    });
    $("#txtAddress").live("change", function () {

        // Getting the address from the text input
        var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");

        // Making the Geocoder call 
        getCoordinates(address);

        // Preventing the form from doing a page submit
        return false;

    });
    */
    $('input:radio[name="optionsRadios1"]').change(
  function () {

      if ($(this).is(':checked') && $(this).val() == 'option2') {
          $("#txtLatitude").attr("disabled", "disabled");
          $("#txtLongitude").attr("disabled", "disabled");
          var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
          var lat = $("#txtLatitude").val();
          var lng = $("#txtLongitude").val();
          // Making the Geocoder call 
          if (lat != "") {
              getCoordinates(address, lat, lng);
          }
          else if (address != "") {
              getCoordinates(address, 0, lng);
          }
      }
      else {
          $("#txtLatitude").removeAttr("disabled");
          $("#txtLongitude").removeAttr("disabled");
      }
  });
    $('input:radio[name="ctl00$ContentPlaceHolder1$optionsRadios1"]').change(
   function () {

       if ($(this).is(':checked') && $(this).val() == 'option2') {
           $("#hdnstatus").val("1");
           $("#txtLatitude").attr("disabled", "disabled");
           $("#txtLongitude").attr("disabled", "disabled");
           var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
           var lat = $("#txtLatitude").val();
           var lng = $("#txtLongitude").val();
           // Making the Geocoder call 
           if (lat != "") {
               getCoordinates(address, lat, lng);
           }
           else if (address != "") {
               getCoordinates(address, 0, lng);
           }
       }
       else {
           $("#hdnstatus").val("0");
           $("#txtLatitude").removeAttr("disabled");
           $("#txtLongitude").removeAttr("disabled");
       }
       if ($("#hdnstatus").val() == "0") {
           if ($("#txtLatitude").val() == "" && $("#txtLongitude").val() == "") {
               $("#rfvLatitude").show();
               $("#rfvLongitude").show();
           }
       }
       else {
           $("#rfvLatitude").hide();
           $("#rfvLongitude").hide();
       }
   });
    $("#txtLatitude").live("change", function () {
        var pathname = window.location.pathname;
   
        if (pathname.search("merchantstores_addedit")) {


            if ($("#manualcheck").is(":checked")) {
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }
            }
            else {
                // Getting the address from the text input
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $(this).val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }

                // Preventing the form from doing a page submit
                return false;
            }
        }
        else if (pathname.search("merchantcompetitors_addedit"))
        {
            if ($("#uniform-ContentPlaceHolder1_manualcheck").is(":checked")) {
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }
            }
            else {
                // Getting the address from the text input
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $(this).val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }

                // Preventing the form from doing a page submit
                return false;
            }
            
        }
    });

    $("#txtLongitude").live("change", function () {
        var pathname = window.location.pathname;
   
        if (pathname.search("merchantstores_addedit")) {
            if ($("#manualcheck").is(":checked")) {
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }
            }
            else {
                // Getting the address from the text input
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $(this).val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }

                // Preventing the form from doing a page submit
                return false;
            }

        }
        else if (pathname.search("merchantcompetitors_addedit")) {
            if ($("##uniform-ContentPlaceHolder1_manualcheck").is(":checked")) {
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $("#txtLongitude").val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }
            }
            else {
                // Getting the address from the text input
                var address = ($("#txtAddress").val() != "" ? $("#txtAddress").val() + "," : "") + ($("#ddlCity").find("option:selected").val() != "0" ? $("#ddlCity").find("option:selected").text() + "," : "") + ($("#txtZipcode").val() != "" ? $("#txtZipcode").val() + "," : "") + ($("#ddlState").find("option:selected").val() != "0" ? $("#ddlState").find("option:selected").text() + "," : "") + ($("#ddlCountry").find("option:selected").val() != "0" ? $("#ddlCountry").find("option:selected").text() + "" : "");
                var lat = $("#txtLatitude").val();
                var lng = $(this).val();
                // Making the Geocoder call 
                if (lat != "") {
                    getCoordinates(address, lat, lng);
                }
                else if (address != "") {
                    getCoordinates(address, 0, lng);
                }

                // Preventing the form from doing a page submit
                return false;
            }
        }
        
    });

    function ChangeMap() {
        var address = ($("#tb_game_answer").val() != "" ? $("#txtAddress").val() : "");
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
        if ($("#manualcheck").is(":checked")) {
            geocoder.geocode(geocoderRequest, function (results, status) {

                // Check if status is OK before proceeding
                if (status == google.maps.GeocoderStatus.OK) {
                    $("#maperror").attr("style", "display:none");
                }
                else {
                    $("#maperror").attr("style", "display:block");
                    $("#maperror").text("Invalid latitude and longitude");
                
                    return;
                }
            });
        }
        else
        {
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
                            draggable: true
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

                    // Setting the position of the marker to the returned location

                    // Update current position info.
                    //alert(latlng);
                    //updateMarkerPosition(latlng);
                    //geocodePosition(latlng);

                    // Add dragging event listeners.
                    google.maps.event.addListener(marker, 'dragstart', function () {
                        //updateMarkerAddress('Dragging...');
                    });

                    google.maps.event.addListener(marker, 'drag', function () {
                        //updateMarkerStatus('Dragging...');
                        //updateMarkerPosition(marker.getPosition());
                        $("#txtLatitude").val(marker.getPosition().lat().toFixed(10));
                        $("#txtLongitude").val(marker.getPosition().lng().toFixed(10));
                        $("#hdnLatitude").val($("#txtLatitude").val());
                        $("#hdnLongitude").val($("#txtLongitude").val());
                        $("#rfvLatitude").hide();
                        $("#rfvLongitude").hide();
                        document.getElementById('txtLatitude').value = marker.getPosition().lat().toFixed(10);
                        document.getElementById('txtLongitude').value = marker.getPosition().lng().toFixed(10);
                    });

                    google.maps.event.addListener(marker, 'dragend', function () {
                        $("#maperror").attr("style", "display:none");
                        //updateMarkerStatus('Drag ended');
                        //geocodePosition(marker.getPosition());
                        //alert(marker.getPosition());
                        $("#txtLatitude").val(marker.getPosition().lat().toFixed(10));
                        $("#txtLongitude").val(marker.getPosition().lng().toFixed(10));
                        $("#hdnLatitude").val($("#txtLatitude").val());
                        $("#hdnLongitude").val($("#txtLongitude").val());
                        document.getElementById('txtLatitude').value = marker.getPosition().lat().toFixed(10);
                        document.getElementById('txtLongitude').value = marker.getPosition().lng().toFixed(10);
                    });
                    $("#maperror").attr("style", "display:none");
                    $("#txtLatitude").val(marker.getPosition().lat().toFixed(10));
                    $("#txtLongitude").val(marker.getPosition().lng().toFixed(10));
                    $("#hdnLatitude").val($("#txtLatitude").val());
                    $("#hdnLongitude").val($("#txtLongitude").val());
                    document.getElementById('txtLatitude').value = marker.getPosition().lat().toFixed(10);
                    document.getElementById('txtLongitude').value = marker.getPosition().lng().toFixed(10);
                    return;
                }
                else
                {
                    $("#maperror").attr("style", "display:block");
                    $("#maperror").text("Invalid latitude and longitude");
                   
                    return;
                }
                $("#txtLatitude").val(marker.getPosition().lat().toFixed(10));
                $("#txtLongitude").val(marker.getPosition().lng().toFixed(10));
                $("#hdnLatitude").val($("#txtLatitude").val());
                $("#hdnLongitude").val($("#txtLongitude").val());
                document.getElementById('txtLatitude').value = marker.getPosition().lat().toFixed(10);
                document.getElementById('txtLongitude').value = marker.getPosition().lng().toFixed(10);
                return;
            }
            );
        }
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