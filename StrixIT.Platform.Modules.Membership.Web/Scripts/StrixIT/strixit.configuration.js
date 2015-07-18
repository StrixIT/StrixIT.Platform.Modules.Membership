//#region Apache License
/**
 * Copyright 2015 StrixIT. Author R.G. Schurgers MA MSc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
//#endregion

(function (window) {
    'use strict';

    var config = $.parseXML($('#appconfig').html());
    var strix = window.strixIT = {};
    var strixConfig = window.strixIT.config = {};
    var parsingPre = document.createElement('pre');

    strixConfig.regexIso8601 = /^([\+-]?\d{4}(?!\d{2}\b))((-?)((0[1-9]|1[0-2])(\3([12]\d|0[1-9]|3[01]))?|W([0-4]\d|5[0-2])(-?[1-7])?|(00[1-9]|0[1-9]\d|[12]\d{2}|3([0-5]\d|6[1-6])))([T\s]((([01]\d|2[0-3])((:?)[0-5]\d)?|24\:?00)([\.,]\d+(?!:))?)?(\17[0-5]\d([\.,]\d+)?)?([zZ]|([\+-])([01]\d|2[0-3]):?([0-5]\d)?)?)?)?$/;
    strixConfig.ajaxHeaderKey = 'X-Requested-With';
    strixConfig.ajaxHeaderValue = 'XMLHttpRequest';
    strixConfig.csrfTokenName = '__RequestVerificationToken';
    strix.getLoginUrl = getLoginUrl;
    strix.getRequestVerificationToken = getRequestVerificationToken;
    strix.convertDateStringsToDates = convertDateStringsToDates;
    strix.getLoadingImage = getLoadingImage;
    strix.htmlDecode = htmlDecode;
    strix.replaceDangerousCharacters = replaceDangerousCharacters;
    strix.isIE = isIE;

    window.onerror = onError;

    // Read the application configuration and set some basic values
    $.each($(config).find('config')[0].attributes, function (elem, attr) {
        strixConfig[toCamelCase(attr.name)] = attr.value;
    });

    strixConfig.resourceRootUrl = strixConfig.rootUrl;
    strixConfig.rootUrl = strixConfig.currentCulture ? strixConfig.rootUrl + strixConfig.currentCulture + '/' : strixConfig.rootUrl;
    strixConfig.culturePrefix = strixConfig.currentCulture ? strixConfig.currentCulture : '';

    // Setup globalization
    kendo.culture(strixConfig.currentCulture ? strixConfig.currentCulture : strixConfig.defaultCulture);

    function getRequestVerificationToken() {
        return $('[name="' + strixConfig.csrfTokenName + '"]', $('form[name="requestvalidationform"]')).val();
    }

    function getLoginUrl() {
        var query = document.location.search ? '?' + document.location.search.substring(1) : null;
        var returnUrl = encodeURIComponent(document.location.pathname + (query ? query : ''));
        return strixIT.config.rootUrl + "Account/Login" + (returnUrl ? "?returnurl=" + returnUrl : '');
    }

    // Taken from http://aboutcode.net/2013/07/27/json-date-parsing-angularjs.html.
    function convertDateStringsToDates(input) {
        // Ignore things that aren't objects.
        if (typeof input !== "object") return input;

        for (var key in input) {
            if (!input.hasOwnProperty(key)) continue;

            var value = input[key];
            var match;
            // Check for string properties which look like dates.
            if (typeof value === "string" && (match = value.match(strixConfig.regexIso8601)) && !(/^\d+$/.test(value))) {
                var milliseconds = Date.parse(match[0])
                if (!isNaN(milliseconds)) {
                    input[key] = new Date(milliseconds);
                }
            } else if (typeof value === "object" && value) {
                // Recurse into object
                convertDateStringsToDates(value);
            }
        }

        return input;
    }

    function getLoadingImage() {
        var loadedStyleSheet = $.grep($('link'), function (x) { return x.href.indexOf('/Styles/Kendo/') > -1 && x.href.indexOf('common-bootstrap') == -1; })[0];
        var themePathPart = loadedStyleSheet.href.substring(loadedStyleSheet.href.indexOf('/Styles/Kendo/') + 14);
        var themeParts = themePathPart.substring(0, themePathPart.indexOf('.min.css')).split('.');
        var themeName = themeParts[themeParts.length - 1];
        return strixConfig.resourceRootUrl + 'Styles/Kendo/' + themeName + '/loading_2x.gif';
    }

    function htmlDecode(value) {
        parsingPre.innerHTML = value;
        return parsingPre.textContent;
    }

    function replaceDangerousCharacters(value) {
        return value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;").replace(/'/g, "&#39;").replace(/\//g, "&#47;");
    }

    function onError(message, url, line, column, ex, cause) {
        if (strixConfig.logErrorUrl) {
            if (ex == null) {
                return;
            }

            var output = ex.message != null ? ex.name + ': ' + ex.message : ex;
            var file = url ? (' file ' + url + '.') : '.';
            output += '\nAt url: ' + document.location + file;

            if (cause) {
                output += '\nCaused by: ' + cause;
            }

            if (ex.stack != null) {
                output += '\nStack trace: ' + ex.stack;
            }

            $.ajax({
                type: 'POST',
                url: strixConfig.rootUrl + strixConfig.routePrefix + strixConfig.logErrorUrl,
                data: { message: output }
            });
        }
    }

    function toCamelCase(str) {
        // Copied from Kendo Core
        return str.replace(/\-(\w)/g, function (strMatch, g1) {
            return g1.toUpperCase();
        });
    }

    function isIE() {
        // Adapted from http://stackoverflow.com/questions/19999388/jquery-check-if-user-is-using-ie.
            var userAgent = window.navigator.userAgent;
            return userAgent.indexOf('MSIE ') > 0 || userAgent.indexOf('Trident/') > 0 || userAgent.indexOf('Edge/') > 0;
    }

})(window);