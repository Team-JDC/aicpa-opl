var app = app || {};

//global functions 
function acc_headClick(me) {
    var h = $(me),
	        hc = h.next(),
	        not_hc = $('.acc_head').not(h).next();

    not_hc.slideUp();
    hc.slideToggle();
    if (h.hasClass('active')) {
        h.removeClass('active');
    } else {
        h.addClass('active');
    }
    $('.acc_head').not(h).removeClass('active');
}

function restickMenu() {
    //if($('#primary_nav_outer.cloned').length > 0){
    //    $('#primary_nav_outer.cloned').remove();
    //}
    stickMenu();
	
    $('.touch .primary_nav li .top_level').off('click');

    $('.touch .primary_nav li .top_level').on('click', function (e) {
        e.preventDefault();
        var t = $(this);

        $('.primary_nav li.last').removeClass('hover_on').addClass('hover_off');
        $('.primary_nav li .top_level').not(t).parent().removeClass('hover_on').addClass('hover_off');
        if (t.parent().hasClass('hover_on')) {
            t.parent().removeClass('hover_on').addClass('hover_off');
        } else {
            t.parent().removeClass('hover_off').addClass('hover_on');
        }
    });


}

function stickMenu() {
    try {
        $('.primary_nav_outer').stickThis({ debugmode: true });
    } catch (e) {
        // nothing
    }
    // Sticky header
    //http://codepen.io/senff/pen/ayGvD
    //https://github.com/senff/Sticky-Anything
    //$('#primary_nav_outer').addClass('original').clone().insertAfter('#primary_nav_outer').addClass('cloned').css('position', 'fixed').css('top', '0').css('margin-top', '0').css('z-index', '500').removeClass('original').hide();
    //scrollIntervalID = setInterval(stickIt, 10);


    //function stickIt() {

    //    var orgElementPos = $('.original').offset();
    //    orgElementTop = orgElementPos.top;

    //    if ($(window).scrollTop() >= (orgElementTop)) {
    //        // scrolled past the original position; now only show the cloned, sticky element.

    //        // Cloned element should always have same left position and width as original element.     
    //        orgElement = $('.original');
    //        coordsOrgElement = orgElement.offset();
    //        leftOrgElement = coordsOrgElement.left;
    //        widthOrgElement = orgElement.css('width');

    //        $('.cloned').css('left', leftOrgElement + 'px').css('top', 0).css('width', widthOrgElement + 'px').css('right', '0px').show();
    //        $('.original').css('visibility', 'hidden');
    //    } else {
    //        // not scrolled past the menu; only show the original menu.
    //        $('.cloned').hide();
    //        $('.original').css('visibility', 'visible');
    //    }
    //}

}


window.onload = function() {
	
}

$(function () { // Modern shorthand for $(document).ready

    // Toggle Advanced Search
    $('#main_search .toggle_advanced').on('click', function (e) {
        e.preventDefault();
        $('#main_search').toggleClass('active');
        $('#advanced_search').toggleClass('active');
    });

    // Empty touchstart for iOS compatibility? Still fine.
    $('body').on('touchstart', function () { });

    // Touch handler for .touch elements
    $(document).on('click', '.touch', function () {
        var me = $(this);
        $('.touch .hover_on').each(function () {
            if ($(this).parents('.primary_nav').length === 0) {
                $(this).removeClass('hover_on').addClass('hover_off');
            }
        });
    });

    // Touch/click navigation toggles
    $('.primary_nav li .top_level').on('touchstart click', function (e) {
        e.preventDefault();
        var t = $(this);
        $('.primary_nav li.last').removeClass('hover_on').addClass('hover_off');
        $('.primary_nav li .top_level').not(t).parent().removeClass('hover_on').addClass('hover_off');

        t.parent().toggleClass('hover_on hover_off');
    });

    $('#mobile_search_btn').on('touchstart', function (e) {
        e.preventDefault();
        var p = $(this).closest('.primary_nav > li');
        $('.primary_nav > li').not(p).removeClass('hover_on').addClass('hover_off');
        p.toggleClass('hover_on hover_off');
    });

    $('#search_wrapper .toggle_advanced').on('touchstart', function (e) {
        e.preventDefault();
        var p = $(this).closest('.primary_nav > li');
        $('.primary_nav > li').not(p).removeClass('hover_on').addClass('hover_off');
        p.toggleClass('hover_on hover_off');
    });

    // Hover behavior (for non-touch devices)
    $('.no-touch')
        .on('mouseenter', '.primary_nav > li > .submenu li', function () {
            var s = $(this).children('.submenu');
            if (s.length) s.slideDown();
        })
        .on('mouseleave', '.primary_nav > li > .submenu li', function () {
            var s = $(this).children('.submenu');
            if (s.length) s.slideUp();
        });

    // Accordion behavior
    $('body').on('click', '#toc.tools .acc_head', function () {
        acc_headClick(this);
    });

    // Sticky right column
    $('#document-container-right').affix();
});
