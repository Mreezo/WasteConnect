document.addEventListener("DOMContentLoaded", function () {
    const mapElement =
        document.getElementById("councillorReportMap");

    if (!mapElement) {
        console.error("Councillor map element was not found.");
        return;
    }

    const rawLatitude =
        mapElement.dataset.latitude;

    const rawLongitude =
        mapElement.dataset.longitude;

    const latitude =
        Number.parseFloat(
            rawLatitude.replace(",", ".")
        );

    const longitude =
        Number.parseFloat(
            rawLongitude.replace(",", ".")
        );

    console.log("Raw Latitude:", rawLatitude);
    console.log("Raw Longitude:", rawLongitude);
    console.log("Parsed Latitude:", latitude);
    console.log("Parsed Longitude:", longitude);

    if (
        !Number.isFinite(latitude) ||
        !Number.isFinite(longitude) ||
        latitude < -90 ||
        latitude > 90 ||
        longitude < -180 ||
        longitude > 180
    ) {
        console.error(
            "Invalid report coordinates:",
            rawLatitude,
            rawLongitude
        );

        mapElement.innerHTML =
            "<div class='map-unavailable-message'>" +
            "Invalid map coordinates." +
            "</div>";

        return;
    }

    if (!window.councillorMapKey) {
        console.error("Azure Maps key is missing.");

        mapElement.innerHTML =
            "<div class='map-unavailable-message'>" +
            "Map configuration is unavailable." +
            "</div>";

        return;
    }

    try {
        const map = new atlas.Map(
            "councillorReportMap",
            {
                center: [
                    longitude,
                    latitude
                ],
                zoom: 16,
                authOptions: {
                    authType: "subscriptionKey",
                    subscriptionKey:
                        window.councillorMapKey
                }
            }
        );

        map.events.add("ready", function () {
            const marker = new atlas.HtmlMarker({
                color: "red",
                text: "D",
                position: [
                    longitude,
                    latitude
                ]
            });

            map.markers.add(marker);
        });

        map.events.add("error", function (error) {
            console.error(
                "Azure Maps error:",
                error
            );
        });
    }
    catch (error) {
        console.error(
            "Failed to create Azure Map:",
            error
        );

        mapElement.innerHTML =
            "<div class='map-unavailable-message'>" +
            "The map could not be loaded." +
            "</div>";
    }
});