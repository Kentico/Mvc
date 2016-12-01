
(function (LOG_URL) {
    var xmlhttp = new XMLHttpRequest();
    // The actual response does not matter. If error occurred during request it should be logged on server side.
    // If everything is OK there is no action to be performed.
    xmlhttp.open("POST", LOG_URL, true);
    xmlhttp.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
    xmlhttp.send(
        "title=" + encodeURIComponent(document.title) +
        "&url=" + encodeURIComponent(window.location.href) +
        "&referrer=" + encodeURIComponent(document.referrer)
    );
})