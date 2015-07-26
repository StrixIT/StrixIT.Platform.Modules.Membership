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

    angular.module('strixDirectives').directive('strixEnumDropDown', ['$route', '$timeout', 'dataService', function ($route, $timeout, dataService) {
        return {
            require: 'ngModel',
            templateUrl: strixIT.config.resourceRootUrl + 'Scripts/StrixIT/Directives/EnumDropDown/enum-drop-down.html',
            link: function (scope, element, attrs, ngModel) {
                var name, modelString, enumName, jsonData, dropDown;
                scope.name = '';
                checkSetup();
                initDropDown();
                scope.$on("kendoWidgetCreated", getDropDown);
                scope.$watch(modelString, updateDropDown);

                function checkSetup() {
                    name = attrs['name'];
                    modelString = attrs['ngModel'];
                    enumName = attrs['enumName'];
                    jsonData = attrs['enumData'] ? JSON.parse(attrs['enumData']) : null;

                    if (!name) {
                        throw new Error('No name specified for enum dropdown');
                    }

                    if (!enumName && !jsonData) {
                        throw new Error('No enum name or json data specified for enum dropdown with name ' + name);
                    }

                    if (!ngModel) {
                        throw new Error('ngModel not specified for enum dropdown with name ' + name);
                    }
                }

                function initDropDown() {
                    scope.name = name;
                    var data = jsonData ? jsonData : [];

                    if (data.length == 0) {
                        var enumeration = dataService.getEnum($route.current.data.module, enumName);

                        for (var n in enumeration) {
                            data.push({ name: n, value: enumeration[n] });
                        }
                    }

                    var source = new kendo.data.DataSource({ data: data });

                    scope.options = {
                        dataSource: source,
                        dataTextField: "value",
                        dataValueField: "name",
                        select: function (e) {
                            var selectedText = e.item.text();
                            var selectedItem = $.grep(e.sender.element.children(), function (l) { return l.text == selectedText })[0];
                            ngModel.$setViewValue(selectedItem.value);
                        }
                    };
                }

                function getDropDown(e, widget) {
                    if (widget.$angular_scope.$id == scope.$id) {
                        dropDown = widget;
                    }
                }

                function updateDropDown(newValue, oldValue) {
                    $timeout(function () {
                        if (newValue != oldValue) {
                            dropDown.select(function (item) {
                                return item.name.toLowerCase() === newValue.toLowerCase();
                            });
                        }
                    }, 50);
                }
            }
        }
    }]);
})()