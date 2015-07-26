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

// Most of the remote code taken from http://blogs.telerik.com/kendoui/posts/13-12-10/extending-the-kendo-ui-validator-with-custom-rules . Modified it slightly and adapted it to work with angular.
(function ($, kendo) {
    var resourceRetriever = null;
    var sanitizeService = null;

    $.extend(true, kendo.ui, {
        setResourceRetriever: setResourceRetriever,
        setSanitizeService: setSanitizeService,
        localizeWidgets: localizeWidgets
    });

    $.extend(true, kendo.ui.validator, {
        rules: {
            remote: remoteRule,
            maxLength: maxLengthRule,
            greaterThan: greaterThanRule
        },
        messages: {
            remote: remoteMessage,
            maxLength: maxLengthMessage,
            greaterThan: greaterThanMessage
        },
        clearRemoteCache: clearRemoteCache
    });

    var validatorScope = this;

    validatorScope.remote = {
        cache: {},
        check: checkRemote
    };

    function remoteRule (input) {
        var validate = input.data('remote');

        if (typeof validate !== 'undefined' && validate !== false) {
            // if the value is pristine but not empty, we're dealing with a previously entered and validated value.
            var formId = input[0].form.id;
            var form = input.scope()[formId];

            if (form[input.attr('name')].$pristine) {
                return true;
            }

            // Get or create a cache for the field.
            var id = input.attr('id');
            var cache = validatorScope.remote.cache[id] || {};

            if (cache.value === input.val()) {
                cache.checking = false;
                return cache.valid ? true : false;
            }

            var settings = {
                url: input.data('remoteUrl') || '',
                idProperty: input.data('remoteIdproperty') || 'id',
                message: kendo.template(input.data('remoteMsg')) || ''
            };

            cache.checking = true;
            validatorScope.remote.check(input, settings);

            // return false which goes into 'checking...' mode
            return false;
        }

        return true;
    }

    function maxLengthRule(input) {
        var validate = input.is('textarea') || input.is('input');

        if (typeof validate !== 'undefined' && validate !== false) {
            var maxLength = parseInt(input.attr('maxlength') || input.attr('data-maxlength'));

            if (maxLength) {
                value = sanitizeService(input.val());
                value = strixIT.htmlDecode(value);
                value = strixIT.replaceDangerousCharacters(value);

                if (value.length > maxLength) {
                    return false;
                }
            }
        }

        return true;
    }

    function greaterThanRule(input) {
        var greaterThanValue = input.data('greater-than');

        if (typeof greaterThanValue !== 'undefined' && greaterThanValue !== false) {
            var value = input.val().trim();

            if (parseFloat(value) <= greaterThanValue) {
                return false;
            }
        }

        return true;
    }

    function remoteMessage (input) {
        var id = input.attr('id');
        var msg = kendo.template(input.data('remoteMsg') || '');
        var cache = remote.cache[id];

        if (cache.checking) {
            return "Checking..."
        }
        else {
            return msg(input.val());
        }
    }

    function maxLengthMessage(input) {
        var maxLength = parseInt(input.attr('maxlength') || input.attr('data-maxlength'));
        var msg = kendo.template(input.data('maxlengthMsg') || '')(maxLength);
        return msg || resourceRetriever('web', 'interface', 'maxLengthMessage').replace('{0}', maxLength);
    }

    function greaterThanMessage(input) {
        var greaterThanValue = input.data('greater-than');
        var msg = kendo.template(input.data('greaterThanMsg') || '')(greaterThanValue);
        return msg || resourceRetriever('web', 'interface', 'greaterThanMessage').replace('{0}', greaterThanValue);
    }

    function checkRemote (element, settings) {
        var scope = element.scope();
        var form = element[0].form;
        var type = form.id.replace('form', '');
        var validator = $(form).data('kendoValidator');
        scope.$emit('remoteValidationInProgress');
        var id = element.attr('id');
        var cache = this.cache[id] = this.cache[id] || {};
        var dataService = angular.element('#body').scope().dataService;

        dataService.callServer(settings.url, { value: element.val(), id: scope[type][settings.idProperty] })
        .then(
        function(data) {
            cache.valid = data.success;
            cache.value = element.val();

            // trigger validation again
            validator.validateInput(element);
            scope.$emit('remoteValidationDone');
        },
        function() {
            // Todo: do something useful here.
            // the ajax call failed so just set the field
            // as valid since we don't know for sure that it's not
            cache.valid = true;
            cache.value = element.val();

            // trigger validation
            validator.validateInput(element);
            scope.$emit('remoteValidationDone');
        });
    }

    function clearRemoteCache() {
        validatorScope.remote.cache = {};
    }

    function localizeWidgets() {
        $.extend(true, kendo.ui.Pager.fn.options, {
            messages: {
                display: resourceRetriever('web', 'interface', 'pagerShowingItems'),
                empty: resourceRetriever('web', 'interface', 'pagerNoItems'),
                itemsPerPage: resourceRetriever('web', 'interface', 'pagerItemsPerPage'),
                first: resourceRetriever('web', 'interface', 'pagerFirstPage'),
                previous: resourceRetriever('web', 'interface', 'pagerPreviousPage'),
                next: resourceRetriever('web', 'interface', 'pagerNextPage'),
                last: resourceRetriever('web', 'interface', 'pagerLastPage')
            }
        });
    }

    function setResourceRetriever(retriever)
    {
        resourceRetriever = retriever;
    }

    function setSanitizeService(sanitize) {
        sanitizeService = sanitize;
    }
})(jQuery, kendo);