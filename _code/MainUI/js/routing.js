//https://github.com/flatiron/director#api-documentation

var defaultRoute = function () {
    console.log('default route');
};

var notes = function () {
    console.log("author"); 
};
var bookmarks = function () {
    console.log("books"); 
};

var viewBook = function (bookId) {
    console.log("viewBook: bookId is populated: " + bookId);
};

var routes = {
    '/': defaultRoute,
    '/notes': notes,
    '/bookmarks': [bookmarks, function () {
        console.log("An inline route handler.");
    } ],
    '/books/view/:bookId': viewBook
};

var router = Router(routes);

router.init();