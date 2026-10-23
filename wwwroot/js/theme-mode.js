// Copyright (c) Heribert Gasparoli. SPDX-License-Identifier: AGPL-3.0-only

// Dark mode: the page follows the system setting (prefers-color-scheme); the header button overrides it for the
// current browser tab. Unlike Klacks.Marketing (no storage), the store keeps the override in sessionStorage so it
// survives the full reloads of language switch, login and logout. Loaded synchronously in <head> so the class is set
// before the first paint. The button works before and without the circuit (document-level click handler); sync()
// re-applies aria-pressed after Blazor re-rendered the header.
window.klacksTheme = (function () {
    var DARK = 'dark';
    var LIGHT = 'light';
    var TOGGLE_SELECTOR = '[data-theme-toggle]';
    var STORAGE_KEY = 'klacks-store-theme';
    var query = window.matchMedia ? window.matchMedia('(prefers-color-scheme: dark)') : null;
    var override = readOverride();

    function readOverride() {
        try {
            var stored = window.sessionStorage.getItem(STORAGE_KEY);
            return stored === DARK || stored === LIGHT ? stored : null;
        } catch (e) {
            return null;
        }
    }

    function writeOverride(value) {
        try {
            window.sessionStorage.setItem(STORAGE_KEY, value);
        } catch (e) {
        }
    }

    function isDark() {
        if (override !== null) {
            return override === DARK;
        }
        return query !== null && query.matches;
    }

    function sync() {
        var dark = isDark();
        var html = document.documentElement;
        html.classList.toggle(DARK, dark);
        html.classList.toggle(LIGHT, !dark);
        var toggles = document.querySelectorAll(TOGGLE_SELECTOR);
        for (var i = 0; i < toggles.length; i++) {
            toggles[i].setAttribute('aria-pressed', dark ? 'true' : 'false');
        }
    }

    function toggle() {
        override = isDark() ? LIGHT : DARK;
        writeOverride(override);
        sync();
    }

    if (query !== null && query.addEventListener) {
        query.addEventListener('change', sync);
    }

    document.addEventListener('click', function (ev) {
        if (ev.target.closest && ev.target.closest(TOGGLE_SELECTOR)) {
            toggle();
        }
    });

    document.addEventListener('DOMContentLoaded', sync);
    sync();

    return { sync: sync };
})();
