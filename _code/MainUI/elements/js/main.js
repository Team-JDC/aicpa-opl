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

$(document).ready(function () {

    //TOGGLE ADVANCED SEARCH
    $('#main_search .toggle_advanced').on('click', function (e) {
        e.preventDefault();

        $('#main_search').toggleClass('active');
        $('#advanced_search').toggleClass('active');
    });

    $('body').bind('touchstart', function () {
    });
	
	$(document).on('click','.touch', function () {
		var me = $(this);
		$('.touch .hover_on').each(function (index, val) {
			var t = me;
			if ($(val).parents('.primary_nav').length == 0) {
				$(this).removeClass('hover_on').addClass('hover_off');
			}
			// if (t.parents('.primary_nav').length == 0) {
				// $(this).removeClass('hover_on').addClass('hover_off');
			// }
		})
	});



    $('.primary_nav li .top_level').on('touchstart click', function (e) {
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

    $('#mobile_search_btn').on('touchstart', function (e) {
        e.preventDefault();
        var t = $(this),
            p = t.closest('.primary_nav > li');

        $('.primary_nav > li').not(p).removeClass('hover_on').addClass('hover_off');
        if (p.hasClass('hover_on')) {
            p.removeClass('hover_on').addClass('hover_off');
        } else {
            p.removeClass('hover_off').addClass('hover_on');
        }
    });

    $('#search_wrapper .toggle_advanced').bind('touchstart', function (e) {
        e.preventDefault();
        var t = $(this),
            p = t.closest('.primary_nav > li');

        $('.primary_nav > li').not(p).removeClass('hover_on').addClass('hover_off');
        if (p.hasClass('hover_on')) {
            p.removeClass('hover_on').addClass('hover_off');
        } else {
            p.removeClass('hover_off').addClass('hover_on');
        }
    });

    function hoverIn(me) {
        var t = $(me);
        if (t.has('.submenu')) {
            var s = t.children('.submenu');
            s.slideDown();
        }
    }

    function hoverOut(me) {
        var t = $(me);
        if (t.has('.submenu')) {
            var s = t.children('.submenu');
            s.slideUp();
        }
    }

    $('.no-touch')
        .on('mouseenter', '.primary_nav > li > .submenu li', function() {hoverIn(this);})
        .on('mouseleave', '.primary_nav > li > .submenu li', function () { hoverOut(this); });

    /*$('.touch #primary_nav > li > .submenu li').bind('touchstart', function(e) {
    e.preventDefault();
    var t = $(this);
        
    $('#primary_nav > li > .submenu li').not(t).removeClass('hover_on').addClass('hover_off');
    if( t.hasClass('hover_on') ) {
    t.removeClass('hover_on').addClass('hover_off');
    } else {
    t.removeClass('hover_off').addClass('hover_on');
    }
    });*/


    //    $('body').on('contentchange', '#toc.tools .acc_head', function () {
    //        acc_headClick(this);
    //    }
    //MAIN TOC ACCORDION, LVL 1

    $('body').on('click', '#toc.tools .acc_head', function () {
        acc_headClick(this);
    });

    function acc_contentClick(me, e) {
        e.preventDefault();
        var h = $(me),
	        p = $(me).closest('.acc_content'),
	        hc = h.next(),
	        not_hc = p.find('.acc_sub1').not(hc);

        not_hc.slideUp();
        hc.slideToggle();
        if (h.hasClass('active')) {
            h.removeClass('active');
        } else {
            h.addClass('active');
        }
        $('#toc.tools .acc_content > ul > li > div').not(h).removeClass('active');
    }

    //    //MAIN TOC ACCORDION, LVL 2    
    //    $('body').on('click', '#toc.tools .acc_content > ul > li > div', function (e) {
    //        acc_contentClick(this,e);
    //    });

    //    function acc_sub1Click(me,e) {
    //        e.preventDefault();
    //        var h = $(me),
    //	        p = $(me).closest('.acc_sub1'),
    //	        hc = h.next(),
    //	        not_hc = p.find('.acc_sub2').not(hc);

    //        not_hc.slideUp();
    //        hc.slideToggle();
    //        if (h.hasClass('active')) {
    //            h.removeClass('active');
    //        } else {
    //            h.addClass('active');
    //        }
    //        $('#toc.tools .acc_sub1 > li > a').not(h).removeClass('active');
    //    }

    //    //MAIN TOC ACCORDION, LVL 3
    //    $('body').on('click', '#toc.tools .acc_sub1 > li > a', function (e) {
    //        acc_sub1Click(this,e);
    //    });

    //    function acc_sub2Click(me, e) {
    //        e.preventDefault();
    //        var h = $(me),
    //	        p = $(me).closest('.acc_sub2'),
    //	        hc = h.next(),
    //	        not_hc = p.find('.acc_sub3').not(hc);

    //        not_hc.slideUp();
    //        hc.slideToggle();
    //        if (h.hasClass('active')) {
    //            h.removeClass('active');
    //        } else {
    //            h.addClass('active');
    //        }
    //        $('#toc.tools .acc_sub2 > li > a').not(h).removeClass('active');
    //    }

    //    //MAIN TOC ACCORDION, LVL 4
    //    $('#toc.tools .acc_sub2 > li > a').on('click', function (e) {
    //        acc_sub2Click(this,e)
    //    });

    //Truncate the next/prev titles
    //    var module = document.getElementById("prevTitle");
    //    $clamp(module, {clamp: 2});
    //    var module2 = document.getElementById("nextTitle");
    //    $clamp(module2, {clamp: 2});


    // Sticky header
    //http://codepen.io/senff/pen/ayGvD
    //https://github.com/senff/Sticky-Anything
    //$('#primary_nav_outer').addClass('original').clone().insertAfter('#primary_nav_outer').addClass('cloned').css('position', 'fixed').css('top', '0').css('margin-top', '0').css('z-index', '500').removeClass('original').hide();
    //stickMenu();
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

    //---FIXED RIGHT COLUMN---//


    //var top = $('.widget').offset().top;

    //$(document).scroll(function () {
    //    $('.widget').css('position', '');
    //    var top = $('.widget').offset().top;
    //    $('.widget').css('position', 'absolute'); $('.widget').css('top', Math.max(top, $(document).scrollTop()));
    //});
    $('#document-container-right').affix();
});