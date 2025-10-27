// Utility to remove a specific selector from HTML string
(function ($) {
    $.strRemove = function (theTarget, theString) {
        return $("<div/>").append($(theTarget, theString).remove().end()).html();
    };
})(jQuery);

// (Optional) Not actively used — converts all CSS to inline (recursive)
(function ($) {
    $.fn.makeCssInline = function () {
        return this.each(function () {
            const properties = [];
            const style = this.style;
            for (let property in style) {
                const value = $(this).css(property);
                if (value) {
                    properties.push(`${property}:${value}`);
                }
            }
            this.style.cssText = properties.join(';');
            $(this).children().makeCssInline();
        });
    };
})(jQuery);

// Helpers
function getReturnUrl() {
    return typeof d_returnurl !== 'undefined' ? d_returnurl : '';
}

function getBaseUrl() {
    const [protocol, , host] = window.location.href.split('/');
    return `${protocol}//${host}/`;
}

function pad(num, size) {
    return num.toString().padStart(size, '0');
}

function getSelectionHtml() {
    let html = "";
    const sel = window.getSelection();
    if (sel?.rangeCount) {
        const container = document.createElement("div");
        for (let i = 0; i < sel.rangeCount; i++) {
            container.appendChild(sel.getRangeAt(i).cloneContents());
        }
        html = container.innerHTML;
    }
    return html;
}

function GetHilightedContent() {
    return sanitizeHtml(getSelectionHtml()).replace(/[\r\n]+/g, ' ');
}

function parseSectionInner(obj) {
    const paraNumber = $(obj).find(".titlepage .ps_para_number").html();
    let paraHTML = $(obj).children("p.ps_para_number").html() || '';
    return `${paraNumber}&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;${paraHTML}`;
}

// Main cleaning function to sanitize HTML before copy
function sanitizeHtml(htmlStr) {
    const $result = $('<div>').append(htmlStr);

    // Remove tooltip and notes
    $result.find("a.tooltip span.popupSpan, span.notesButtons, span.noindex, span.contentLink").remove();

    // Clean content links (locked vs unlocked)
    $result.find("span.contentLink a").each(function () {
        const onclickAttr = this.getAttribute("onclick");
        const params = onclickAttr?.match(/dolink\('([^']+)',\s*'([^']+)',\s*false/i);
        const targetDoc = params?.[1];
        const targetPtr = params?.[2];
        const img = $(this).parent().find("img[alt*='locked']").first();
        const isHidden = img.length && $(`.contentLink img.${img.attr("class")}`).is(":hidden");

        if (targetDoc && targetPtr) {
            if (isHidden) {
                const link = `<a href="${getReturnUrl()}&tdoc=${targetDoc}&tptr=${targetPtr}">${$(this).text()}</a>`;
                $(this).parent().before(link).remove();
            } else {
                $(this).parent().before($(this).text()).remove();
            }
        }
    });

    // Replace footnote links with clean URLs
    $result.find("a[onclick*='doFootnoteLink']").each(function () {
        const onclickAttr = this.getAttribute("onclick");
        const params = onclickAttr?.match(/doFootnoteLink\('([^']+)', ?'([^']+)'/i);
        if (params?.length >= 3) {
            $(this).removeAttr("onclick").attr("href", `${getReturnUrl()}&tdoc=${params[1]}&tptr=${params[2]}`);
        } else {
            $(this).replaceWith(this.innerHTML);
        }
    });

    // Replace GetResource links
    $result.find("a[href*='Handlers/GetResource.ashx?r_bn']").each(function () {
        const hrefAttr = this.getAttribute("href");
        const params = hrefAttr?.match(/.*r_bn=([^&]+).*?r_rn=([^&]+)/i);
        if (params?.length >= 3) {
            $(this).attr("href", `${getReturnUrl()}&r_bn=${params[1]}&r_rn=${params[2]}`);
        } else {
            $(this).replaceWith(this.innerHTML);
        }
    });

    // Remove join section icons
    $result.find("img[class*='joinSections']").remove();

    // Reconstruct section HTML if necessary
    if ($result.find(".section").length === 0) {
        if ($result.find(".ps_para_number").length && $result.find(".titlepage .ps_para_number").length) {
            const html = parseSectionInner($result);
            $result.find(".titlepage").remove();
            $result.children("p.ps_para_number").html(html);
        }
    }

    $result.find(".section").each(function () {
        if ($(this).children(".ps_para_number").length && $(this).find(".titlepage .ps_para_number").length) {
            const html = parseSectionInner(this);
            $(this).children(".titlepage").remove();
            $(this).children("p.ps_para_number").html(html);
        }
    });

    return $result.html();
}

// Clipboard operations
function selectElementContents(el) {
    const range = document.createRange();
    const sel = window.getSelection();

    try {
        range.selectNodeContents(el.firstChild || el);
        sel.removeAllRanges();
        sel.addRange(range);
    } catch (e) {
        console.warn("Selection error:", e);
    }

    setTimeout(() => {
        $('#ideal').html('');
        window.scrollTo(0, scrolltotop);
    }, 2);
}

// Set up copy/cut behavior
function doTextOperationsReady() {
    const getScrollTop = () => {
        const doc = document.documentElement;
        const body = document.body;
        return ((doc && doc.scrollTop) || (body && body.scrollTop) || 0) - (doc.clientTop || 0);
    };

    $('body').on('copy cut', function () {
        const html = GetHilightedContent();
        const container = document.createElement('div');
        container.setAttribute("style", "clear:both");
        container.innerHTML = html;

        const frag = document.createDocumentFragment();
        frag.appendChild(container);

        scrolltotop = getScrollTop();

        $('#ideal').css('top', scrolltotop).html(frag);

        try {
            $('#ideal').makeCssInline();
        } catch (e) {
            console.warn("Inline CSS conversion failed", e);
        }

        selectElementContents(document.getElementById('ideal'));
    });

    // Trigger hide on load and placeholder for future mouseup logic
    $("body").on("mouseup", function () {
        // Placeholder: Determine whether to show/hide copy buttons
    });

    parent?.hideTextOpButtons?.();
}

// Trigger copy setup on page ready
$(function () {
    doTextOperationsReady();
});
