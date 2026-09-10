function showLarge(id) {
    var largeId = id + '-large';
    if (routeTargetPtr == '')
    {
        var path = appUrl("/Handlers/GetDocument.ashx?id=") + routeTargetDoc + "&type=" + routeNodeType + "&table=" + largeId;
        window.open(path);
    } else if (routeTargetDoc != '' && routeTargetPtr != '') {
        var path = appUrl("/Handlers/GetDocument.ashx?targetdoc=") + routeTargetDoc + "&targetptr=" + routeTargetPtr + "&table=" + largeId;
        window.open(path);
    }
}
