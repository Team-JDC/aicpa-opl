function showLarge(id) {
    var largeId = id + '-large';
    if (routeTargetPtr == '')
    {
        var path = "/Handlers/GetDocument.ashx?id=" + routeTargetDoc + "&type=" + routeNodeType + "&table=" + largeId;
        window.open(path);
    } else if (routeTargetDoc != '' && routeTargetPtr != '') {
        var path = "/Handlers/GetDocument.ashx?targetdoc=" + routeTargetDoc + "&targetptr=" + routeTargetPtr + "&table=" + largeId;
        window.open(path);
    }
}
