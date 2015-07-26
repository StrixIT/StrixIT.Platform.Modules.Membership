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

(function () {
    'use strict';

    angular.module('strixDirectives').directive('strixShowHtml', ['$rootScope', '$sanitize', function ($rootScope, $sanitize) {
        return {
            require: 'ngModel',
            priority: 100,
            link: function (scope, elem, attrs, ngModel) {
                var globalMaxLengthElement = $('MaxTextLength');
                var globalMaxLength = $('MaxTextLength').val();
                globalMaxLength > 0 ? globalMaxLength : undefined;
                var maxLength = attrs['maxlength'] || globalMaxLength || undefined;
                var lengthElement = null;
                var lengthMessage = null;

                ngModel.$formatters.push(function (value) {
                    if (!value) {
                        return value;
                    }

                    value = $sanitize(value);
                    return strixIT.htmlDecode(value);
                });

                if (maxLength) {
                    lengthMessage = $rootScope.getResource('web', 'interface', 'maxLengthInfo');
                    lengthElement = angular.element('<span class="strix-max-length k-widget k-tooltip k-tooltip-validation"></span>');
                    elem.after(lengthElement);
                }

                scope.$watch(attrs.ngModel, function (newValue, oldValue) {
                    var value = null;

                    try {
                        value = $sanitize(newValue);
                    }
                    catch (ex) {
                        value = newValue.replace(/</g, "< ");
                        ngModel.$setViewValue(value);
                        elem.val(value);
                    }

                    if (maxLength) {
                        value = strixIT.htmlDecode(value);
                        value = strixIT.replaceDangerousCharacters(value);
                        var finalLength = value.length;
                        var message = lengthMessage.replace('{0}', finalLength).replace('{1}', maxLength);
                        lengthElement.text(message)

                        if (finalLength > maxLength) {
                            lengthElement.prepend(angular.element('<span class="k-icon k-warning"> </span>'));
                        }
                    }
                });
            }
        };
    }]);

})();