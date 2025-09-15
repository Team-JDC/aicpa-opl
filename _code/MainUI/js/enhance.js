/**
 * EnhanceJS - Cleaned & Documented Version
 * Version: 1.1 (Updated for modern browsers and jQuery 3.7.1 compatibility)
 * Source: http://enhancejs.googlecode.com/
 * Updated by: [Your Name or Team]
 */

(function (win, doc) {
    const docElem = doc.documentElement;
    let settings = {}, body, fakeBody, windowLoaded = false;
    let head = doc.querySelector('head') || docElem;
    let toggledMedia = [];
    let mediaCookieA, mediaCookieB;
    let testPass = false;

    win.enhance = function (options = {}) {
        settings = { ...enhance.defaultSettings, ...options };
        settings.tests = { ...enhance.defaultTests, ...options.addTests };

        if (!docElem.classList.contains(settings.testName)) {
            docElem.classList.add(settings.testName);
        }

        mediaCookieA = `${settings.testName}-toggledmediaA`;
        mediaCookieB = `${settings.testName}-toggledmediaB`;
        toggledMedia = [readCookie(mediaCookieA), readCookie(mediaCookieB)];

        setTimeout(() => {
            if (!testPass) removeHTMLClass();
        }, 3000);

        runTests();
        applyDocReadyHack();

        windowLoad(() => {
            windowLoaded = true;
        });
    };

    enhance.defaultSettings = {
        testName: 'enhanced',
        loadScripts: [],
        loadStyles: [],
        queueLoading: true,
        appendToggleLink: true,
        forcePassText: 'View high-bandwidth version',
        forceFailText: 'View low-bandwidth version',
        tests: {},
        addTests: {},
        alertOnFailure: false,
        onPass: () => { },
        onFail: () => { },
        onLoadError: () => docElem.classList.add(`${settings.testName}-incomplete`),
        onScriptsLoaded: () => { }
    };

    enhance.defaultTests = {
        getById: () => !!doc.getElementById,
        getByTagName: () => !!doc.getElementsByTagName,
        createEl: () => !!doc.createElement,
        boxmodel: () => {
            const div = doc.createElement('div');
            div.style.cssText = 'width:1px;padding:1px';
            body.appendChild(div);
            const width = div.offsetWidth;
            body.removeChild(div);
            return width === 3;
        },
        position: () => {
            const div = doc.createElement('div');
            div.style.cssText = 'position:absolute;left:10px';
            body.appendChild(div);
            const left = div.offsetLeft;
            body.removeChild(div);
            return left === 10;
        },
        floatClear: () => {
            const div = doc.createElement('div');
            div.innerHTML = `<div style='width:5px;height:5px;float:left;'></div><div style='width:5px;height:5px;float:left;'></div>`;
            body.appendChild(div);
            const [first, second] = div.childNodes;
            const topA = first.offsetTop;
            let topB = second.offsetTop;
            if (topA === topB) {
                second.style.clear = 'left';
                topB = second.offsetTop;
                if (topA !== topB) {
                    body.removeChild(div);
                    return true;
                }
            }
            body.removeChild(div);
            return false;
        },
        heightOverflow: () => {
            const div = doc.createElement('div');
            div.innerHTML = '<div style="height:10px;"></div>';
            div.style.cssText = 'overflow:hidden;height:0';
            body.appendChild(div);
            const height = div.offsetHeight;
            body.removeChild(div);
            return height === 0;
        },
        ajax: () => {
            try {
                return !!new XMLHttpRequest();
            } catch (e) {
                return false;
            }
        },
        resize: () => typeof win.onresize !== 'undefined',
        print: () => typeof win.print !== 'undefined'
    };

    function runTests() {
        const result = readCookie(settings.testName);

        if (result) {
            result === 'pass' ? handlePass() : handleFail();
        } else {
            addFakeBody();
            let pass = Object.values(settings.tests).every(test => test());
            removeFakeBody();

            createCookie(settings.testName, pass ? 'pass' : 'fail');
            pass ? handlePass() : handleFail();
        }

        if (settings.appendToggleLink) {
            windowLoad(() => appendToggleLinks(result));
        }
    }

    function handlePass() {
        testPass = true;
        enhancePage();
        settings.onPass();
    }

    function handleFail() {
        removeHTMLClass();
        settings.onFail();
    }

    function enhancePage() {
        if (settings.loadStyles.length) appendStyles();
        if (settings.loadScripts.length) appendScripts();
        else settings.onScriptsLoaded();
    }

    function appendStyles() {
        settings.loadStyles.forEach(item => {
            const link = doc.createElement('link');
            link.rel = 'stylesheet';
            link.type = 'text/css';
            link.href = typeof item === 'string' ? item : item.href;
            link.onerror = settings.onLoadError;
            head.appendChild(link);
        });
    }

    function appendScripts() {
        const queue = [...settings.loadScripts];

        function loadNext() {
            if (queue.length === 0) {
                settings.onScriptsLoaded();
                return;
            }
            const script = doc.createElement('script');
            script.type = 'text/javascript';
            script.src = queue.shift();
            script.onerror = settings.onLoadError;
            script.onload = loadNext;
            head.appendChild(script);
        }

        if (settings.queueLoading) loadNext();
        else queue.forEach(src => {
            const script = doc.createElement('script');
            script.type = 'text/javascript';
            script.src = src;
            script.onerror = settings.onLoadError;
            head.appendChild(script);
        });
    }

    function addFakeBody() {
        fakeBody = doc.createElement('body');
        docElem.insertBefore(fakeBody, doc.body);
        body = fakeBody;
    }

    function removeFakeBody() {
        docElem.removeChild(fakeBody);
        body = doc.body;
    }

    function removeHTMLClass() {
        docElem.classList.remove(settings.testName);
    }

    function createCookie(name, value, days = 90) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        doc.cookie = `${name}=${value};expires=${date.toUTCString()};path=/`;
    }

    function readCookie(name) {
        const match = doc.cookie.match(new RegExp(`(^| )${name}=([^;]+)`));
        return match ? match[2] : null;
    }

    function windowLoad(cb) {
        if (windowLoaded) return cb();
        const oldOnLoad = win.onload;
        win.onload = function () {
            if (typeof oldOnLoad === 'function') oldOnLoad();
            cb();
        };
    }

    function appendToggleLinks(result) {
        if (!result) return;
        const a = doc.createElement('a');
        a.href = '#';
        a.className = `${settings.testName}_toggleResult`;
        a.textContent = result === 'pass' ? settings.forceFailText : settings.forcePassText;
        a.onclick = result === 'pass' ? () => { createCookie(settings.testName, 'fail'); location.reload(); } : () => { createCookie(settings.testName, 'pass'); location.reload(); };
        doc.body.appendChild(a);
    }

})(window, document);
