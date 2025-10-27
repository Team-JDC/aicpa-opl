function htmlEncode(value) {
	//var retval = $('<div/>').text(value).html();
    var retval = value; 
    retval = retval.replace("\\", "\\\\");
	retval = retval.replace("\'", "\\\'");
	return retval;
}

function htmlDecode(value) {
	var retval = $('<div/>').html(value).text();
	return retval;
}

function escapeString(value) {
    value = value.replace(/\\/g, "\\\\");
    value = value.replace(/"/g, "\\\"");
    value = value.replace(/'/g, "\\\'");
    return value;
}

//HTML Encodes a single quote
function encodeQuotes(inputStr) {
    inputStr = inputStr.replace(/'/g, "&#39;");
    return inputStr;
}

//UnEncodes a single quote.
function decodeQuotes(inputStr) {
    inputStr = inputStr.replace(/&#39;/g, "'");
    return inputStr;
}

function ThemeReplace(label, id) {
    if (id != undefined) {
        $('.' + label).text(id);        
    }
}

function runThemeReplace() {
    if (typeof doThemeReplace == 'function') {
        doThemeReplace();
    }
}


//function doStringsReplace() {
//    StringsReplace('TrendTitle', TrendTitle);
//    StringsReplace('TrainingTitle', TrainingTitle);
//    StringsReplace('ResourcesTitle', ResourcesTitle);
//    StringsReplace('LibraryTitle', LibraryTitle);
//    console.log('doStringReplace');
//}