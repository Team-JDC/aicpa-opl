// Modern listener utility for hash or HTML5 history routing
const listener = {
    mode: 'modern',
    hash: window.location.hash,
    history: false,

    /**
     * Initializes the routing listener
     * @param {Function} fn - callback to trigger on route change
     * @param {Boolean} useHistory - whether to use HTML5 history or hash
     */
    init(fn, useHistory = false) {
        this.history = useHistory;

        if (!Router.listeners) {
            Router.listeners = [];
        }

        // Internal handler that delegates route changes to all listeners
        const onChange = (event) => {
            Router.listeners.forEach(listenerFn => listenerFn(event));
        };

        // Use history API if supported and requested
        if (useHistory && 'onpopstate' in window) {
            setTimeout(() => {
                window.addEventListener('popstate', onChange);
            }, 0); // prevent duplicate trigger on load
        } else if ('onhashchange' in window) {
            window.addEventListener('hashchange', onChange);
        }

        this.mode = 'modern';
        Router.listeners.push(fn);
        return this.mode;
    },

    /**
     * Stops listening to hash/history changes for the given callback
     * @param {Function} fn - previously registered route handler
     */
    destroy(fn) {
        if (!Router.listeners) return;

        Router.listeners = Router.listeners.filter(l => l !== fn);
    },

    /**
     * Updates the browser's address bar (pushes history or changes hash)
     * @param {String} route - the new path or hash to set
     */
    setHash(route) {
        if (this.history && window.history && window.history.pushState) {
            window.history.pushState({}, document.title, route);
            this.fire(); // manually fire route handler
        } else {
            window.location.hash = route.startsWith('/') ? route : `/${route}`;
        }
    },

    /**
     * Manually invokes route change handler
     */
    fire() {
        if (this.history) {
            window.dispatchEvent(new PopStateEvent('popstate'));
        } else {
            window.dispatchEvent(new HashChangeEvent('hashchange'));
        }
    }
};
class Router {
    constructor(routes = {}) {
        this.routes = {};
        this.params = {};
        this.scope = [];
        this._methods = {};
        this.methods = ['on', 'once', 'after', 'before'];

        // Default routing mode
        this.historySupport = !!(window.history && window.history.pushState);
        this.history = false; // default to hash routing unless configured
        this._invoked = false;

        this.configure();  // Load initial settings
        this.mount(routes); // Attach routes
    }

    /**
     * Initialize router and bind listener for hash/history changes
     * @param {String} defaultRoute - fallback route if none is in URL
     */
    init(defaultRoute) {
        const self = this;

        // Main dispatch handler triggered by popstate or hashchange
        this.handler = function (event) {
            const current = self.history
                ? self.getPath()
                : window.location.hash.replace(/^#/, '');

            const route = current.startsWith('/') ? current : `/${current}`;
            self.dispatch('on', route);
        };

        // Start the listener
        listener.init(this.handler, this.history);

        // Determine initial route and dispatch
        if (!this.history) {
            if (window.location.hash === '' && defaultRoute) {
                window.location.hash = defaultRoute;
            } else if (window.location.hash !== '') {
                self.dispatch('on', '/' + window.location.hash.replace(/^(#\/|#|\/)/, ''));
            }
        } else {
            const initialPath = this.convert_hash_in_init
                ? (window.location.hash ? window.location.hash.replace(/^#/, '') : defaultRoute)
                : self.getPath();

            if (initialPath || this.run_in_init) {
                this.handler(); // trigger route handler on load
            }
        }

        return this;
    }

    /**
     * Returns the pathname part of the current URL (for history mode)
     * @returns {string}
     */
    getPath() {
        let path = window.location.pathname;
        return path.startsWith('/') ? path : `/${path}`;
    }
}
Router.prototype.configure = function (options = {}) {
    this.recurse = options.recurse || false;
    this.async = options.async || false;
    this.delimiter = options.delimiter || "/";
    this.strict = options.strict !== false; // default to true
    this.notfound = options.notfound || null;
    this.resource = options.resource || null;

    this.history = !!(options.html5history && this.historySupport);
    this.run_in_init = this.history && options.run_handler_in_init !== false;
    this.convert_hash_in_init = this.history && options.convert_hash_in_init !== false;

    // Global lifecycle hooks
    this.every = {
        before: options.before || null,
        after: options.after || null,
        on: options.on || null
    };

    // Register all supported HTTP-like methods
    this.methods.forEach(method => {
        this._methods[method] = true;
    });

    return this;
};

/**
 * Register a parameter transformer for named routes (e.g., ":id")
 * @param {string} token 
 * @param {RegExp|string} matcher 
 */
Router.prototype.param = function (token, matcher) {
    const key = token.startsWith(':') ? token : `:${token}`;
    const compiled = new RegExp(key, "g");

    this.params[key] = function (str) {
        return str.replace(compiled, matcher.source || matcher);
    };

    return this;
};

/**
 * Add route handlers for given path/method.
 * @param {string|string[]} method - e.g. "on", "after"
 * @param {string|string[]} path - route pattern(s)
 * @param {function} route - route handler
 */
Router.prototype.on = Router.prototype.route = function (method, path, route) {
    if (typeof path === 'function') {
        route = path;
        path = method;
        method = 'on';
    }

    if (Array.isArray(path)) {
        path.forEach(p => this.on(method, p, route));
        return;
    }

    if (path instanceof RegExp) {
        path = path.source.replace(/\\\//g, '/');
    }

    if (Array.isArray(method)) {
        method.forEach(m => this.on(m.toLowerCase(), path, route));
        return;
    }

    const splitPath = path.split(new RegExp(this.delimiter)).filter(Boolean);
    this.insert(method, splitPath, route);
};

/**
 * Extend the router with custom methods (e.g., GET, POST, etc.)
 * @param {string[]} methods 
 */
Router.prototype.extend = function (methods) {
    methods.forEach(method => {
        this._methods[method] = true;

        this[method] = function (...args) {
            const prefixArgs = args.length === 1 ? [method, ""] : [method];
            this.on.apply(this, prefixArgs.concat(args));
        };
    });
};

/**
 * Mount multiple routes under a common path prefix
 * @param {object} routes 
 * @param {string|string[]} path 
 */
Router.prototype.mount = function (routes, path = []) {
    if (!routes || typeof routes !== 'object' || Array.isArray(routes)) return;

    const basePath = Array.isArray(path) ? path : path.split(this.delimiter);

    Object.keys(routes).forEach(route => {
        let fullPath = basePath.slice();
        let target = routes[route];
        let parts = route.split(this.delimiter);

        if (route.startsWith(this.delimiter)) {
            parts.shift();
        }

        if (typeof target === 'object' && !Array.isArray(target)) {
            this.mount(target, fullPath.concat(parts));
        } else {
            const method = this._methods[parts[0]] ? parts.shift() : 'on';
            fullPath = fullPath.concat(parts);
            this.insert(method, fullPath, target);
        }
    });
};
/**
 * Dispatch a route based on method and path.
 * Resolves the handler list via `traverse` and executes via `invoke`.
 * @param {string} method - e.g. "on"
 * @param {string} path - full route path
 * @param {function} [callback] - optional callback for async handlers
 * @returns {boolean}
 */
Router.prototype.dispatch = function (method, path, callback) {
    const self = this;
    const cleanedPath = path.replace(/\?.*/, '');
    const fns = this.traverse(method, cleanedPath, this.routes, '');
    const invokedBefore = this._invoked;

    this._invoked = true;

    if (!fns || fns.length === 0) {
        this.last = [];

        if (typeof this.notfound === 'function') {
            return this.invoke([this.notfound], { method, path }, callback), false;
        }

        return false;
    }

    if (this.recurse === 'forward') {
        fns.reverse();
    }

    function proceedToInvoke() {
        self.last = fns.after;
        const runList = self.runlist(fns);
        self.invoke(runList, self, callback);
    }

    const afterFns = this.every?.after ? [this.every.after, ...this.last] : [...this.last];

    if (invokedBefore && afterFns.length) {
        if (this.async) {
            this.invoke(afterFns, self, proceedToInvoke);
        } else {
            this.invoke(afterFns, self);
            proceedToInvoke();
        }
    } else {
        proceedToInvoke();
    }

    return true;
};

/**
 * Invoke an array of route handlers (sync or async)
 * @param {Array} fns 
 * @param {Object} thisArg 
 * @param {Function} [callback]
 */
Router.prototype.invoke = function (fns, thisArg, callback) {
    const self = this;

    if (this.async) {
        const apply = (fn, next) => {
            if (Array.isArray(fn)) {
                return asyncEverySeries(fn, apply, next);
            } else if (typeof fn === 'function') {
                fn.apply(thisArg, (fns.captures || []).concat(next));
            }
        };

        asyncEverySeries(fns, apply, function () {
            if (callback) callback.apply(thisArg, arguments);
        });

    } else {
        const apply = (fn) => {
            if (Array.isArray(fn)) {
                return every(fn, apply);
            } else if (typeof fn === 'function') {
                return fn.apply(thisArg, fns.captures || []);
            } else if (typeof fn === 'string' && self.resource) {
                return self.resource[fn].apply(thisArg, fns.captures || []);
            }
        };

        every(fns, apply);
    }
};

/**
 * Combine route handlers into a flattened execution list,
 * including global `before`/`on` lifecycle hooks
 * @param {Array} fns 
 * @returns {Array} runlist
 */
Router.prototype.runlist = function (fns) {
    const base = this.every?.before ? [this.every.before, ...flatten(fns)] : flatten(fns);

    if (this.every?.on) {
        base.push(this.every.on);
    }

    base.captures = fns.captures;
    base.source = fns.source;

    return base;
};
Router.prototype.traverse = function (method, path, routes, regexp = '', filter) {
    const fns = [];

    if (path === this.delimiter && routes[method]) {
        const handlers = [[routes.before, routes[method]].filter(Boolean)];
        handlers.after = [routes.after].filter(Boolean);
        handlers.matched = true;
        handlers.captures = [];
        return this._applyFilter(handlers, filter);
    }

    for (const key in routes) {
        if (!Object.prototype.hasOwnProperty.call(routes, key)) continue;

        const isMethod = this._methods[key];
        const value = routes[key];
        const isObject = typeof value === 'object' && !Array.isArray(value);

        if (!isMethod && isObject) {
            const pattern = new RegExp(`^${regexp}${this.delimiter}${key}${this.strict ? '' : `[${this.delimiter}]?`}`);
            const match = path.match(pattern);

            if (match) {
                if (match[0] === path && value[method]) {
                    const exactHandlers = [[value.before, value[method]].filter(Boolean)];
                    exactHandlers.after = [value.after].filter(Boolean);
                    exactHandlers.matched = true;
                    exactHandlers.captures = match.slice(1);
                    return this._applyFilter(exactHandlers, filter);
                }

                const nextMatch = this.traverse(method, path, value, `${regexp}${this.delimiter}${key}`, filter);
                if (nextMatch && nextMatch.matched) {
                    if (this.recurse) {
                        fns.push([value.before, value.on].filter(Boolean));
                        nextMatch.after.push(...[value.after].filter(Boolean));
                    }
                    fns.push(...nextMatch);
                    fns.matched = true;
                    fns.captures = nextMatch.captures;
                    fns.after = nextMatch.after;
                    return this._applyFilter(fns, filter);
                }
            }
        }
    }

    return false;
};

Router.prototype._applyFilter = function (routes, filter) {
    if (!filter) return routes;

    const deepCopy = arr => arr.map(item => Array.isArray(item) ? deepCopy(item) : item);
    const newRoutes = deepCopy(routes);
    newRoutes.after = routes.after?.filter(filter) || [];

    return newRoutes.filter(group =>
        Array.isArray(group) ? group.some(fn => typeof fn === 'function' && filter(fn)) : true
    );
};
