$(document).ready(function () {
    doProcessFeatures();
});

function doProcessFeatures(paramsForMsg) {
    if (d_features != null) {
        for (var i = 0; i < d_features.length; i++) {
            $(d_features[i]).remove();
        }
    }
}