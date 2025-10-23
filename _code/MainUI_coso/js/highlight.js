// hit-highlighter.js (Modernized with Comments and jQuery 3.7.1 Compatibility)

let hitcount = 0;
let currentHit = 0;

/**
 * Scrolls smoothly to the given highlighted element by ID.
 * Adds highlight class to visually indicate focus.
 * @param {string} prop - The ID of the target hit element.
 */
function scrollToCurrentHitLocation(prop) {
    const $target = $('#' + prop);
    if ($target.length) {
        $('html, body').animate({ scrollTop: $target.offset().top - 50 }, 'slow');
        $target.addClass('highlight2');
    }
}

/**
 * Determines if there is a next hit to navigate to.
 * @returns {boolean}
 */
function isNextVisible() {
    return currentHit < hitcount - 1;
}

/**
 * Determines if there is a previous hit to navigate to.
 * @returns {boolean}
 */
function isPrevVisible() {
    return currentHit > 0;
}

/**
 * Navigates to the next highlighted hit.
 */
function goToNext() {
    $('#hitlocation' + currentHit).removeClass('highlight2');
    if (isNextVisible()) currentHit++;
    scrollToCurrentHitLocation('hitlocation' + currentHit);
}

/**
 * Navigates to the previous highlighted hit.
 */
function goToPrevious() {
    $('#hitlocation' + currentHit).removeClass('highlight2');
    if (isPrevVisible()) currentHit--;
    scrollToCurrentHitLocation('hitlocation' + currentHit);
}

$(function () {
    const $highlightInput = $('#hhighlight');
    const showButton = $highlightInput.length > 0;

    if (showButton) {
        parent.showInnerSearchButtons();

        const words = $highlightInput.val() || '';
        const anchor = $('#hitanchor').val() || '';

        const wordsArray = [];
        $.each(words.split(' '), function (_, val) {
            val = val.replace(/\+/g, '\\s+').trim().toLowerCase();
            if (val && !wordsArray.includes(val)) {
                wordsArray.push(val);
            }
        });

        // Remove previous highlights
        $('body').removeHighlight();

        // Highlight search terms
        wordsArray.forEach(val => {
            $('body').highlight(val);
        });

        // Assign IDs to each highlight and count them
        $('.highlight').each(function (i) {
            $(this).attr('id', 'hitlocation' + i);
            hitcount++;
        });

        // Determine current hit based on anchor position if available
        let anchorLoc = -1;
        if (anchor) {
            const html = $('body').html();
            const anchorText = `name="${anchor}"`;
            let anchorIndex = html.indexOf(anchorText);

            if (anchorIndex === -1) {
                anchorIndex = html.indexOf(anchorText.replace(/"/g, ''));
            }

            let index = 0;
            let pos = 0;
            while (index < hitcount && pos < anchorIndex) {
                pos = html.indexOf('hitlocation' + index);
                if (pos < anchorIndex) index++;
            }

            if (index < hitcount) anchorLoc = index;
        }

        if (anchorLoc > -1) currentHit = anchorLoc;
        scrollToCurrentHitLocation('hitlocation' + currentHit);
    } else {
        parent.hideInnerSearchButtons();
    }
});


// Hide highlight buttons if user clicks a link to download a document
$(document).on('click', function (event) {
    const element = event.target;
    const path = element?.pathname || element?.parentElement?.pathname || '';
    if (path.includes('GetDocument.ashx')) {
        parent.hideInnerSearchButtons();
    }
});
