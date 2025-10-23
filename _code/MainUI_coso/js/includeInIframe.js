function loadFafToc(pageName) {
    $('.navigation').treeview({
        collapsed: true,
        prerendered: true,
        unique: true
    });

}

$(document).on("mousemove", function (e) {
    if (top.drag && top.drag.state) {
        top.setBoxOffset((e.clientX + top.iframeOffset.left) - top.drag.indivx, (e.clientY + top.iframeOffset.top) - top.drag.indivy);
    }
});

$(document).on("mouseup", function (e) {
    if (top.drag && top.drag.state) {
        //drag.elem.style.backgroundColor = '#808';
        top.drag.state = false;
    }
});
