/**
 * Copyright (c) 2009, Nathan Bubna
 * Dual licensed under the MIT and GPL licenses:
 *   http://www.opensource.org/licenses/mit-license.php
 *   http://www.gnu.org/licenses/gpl.html
 *
 * Simple plugin to track and notify the user of session timeouts
 * due to inactivity on the client side.  If there is no ajax
 * activity (that would keep the server-side session alive) for the
 * specified amount of time (30 min by default), then it will notify
 * the user (locking the page in the meantime) and reload the page
 * when they click 'Ok'.
 *
 * @version 1.0
 * @name timeout
 * @cat Plugins/Timeout
 * @author Nathan Bubna
 */
(function ($) {

    var T = $.timeout = function(t, h) {
        if (t === false) {
            T.cancel();
        } else {
            if (typeof t == "number") T.time = t;
            if (h) T.handler = h;
            T.setup();
        }
        return $;
    };

    $.extend(T, {
        version: "1.0",
        time: 30*60*1000,//30 min default
        id: null,
        setup: function() {
            if (T.id) {
                T.cancel();
            }
            T.id = setTimeout(T.handler, T.time);
            $(document).ajaxSend(T.onAjaxSend);
        },
        cancel: function() {
            clearTimeout(T.id);
            T.id = null;
            $(document).unbind("ajaxSend", T.onAjaxSend);
        },
        handler: function() {
            if ($.txt) {
                var msg = 'timeout';
                if ($.txt(msg) == msg) msg = 'Session timed out.';
                $.txt.say(msg, T.reload);
            } else {
                alert('Session timed out.');
                T.reload();
            }
        },
        reload: function() {
            window.location += '';
        },
        onAjaxSend: function() {
            clearTimeout(T.id);
            T.id = setTimeout(T.handler, T.time);
        }
    });

    $.timeout();// run with default settings
})(jQuery);
