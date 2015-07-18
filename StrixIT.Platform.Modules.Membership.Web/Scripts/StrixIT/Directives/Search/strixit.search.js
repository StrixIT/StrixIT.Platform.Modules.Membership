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

    angular.module('strixDirectives').directive('strixSearch', ['$window', 'dataService', function ($window, dataService) {
        return {
            restrict: 'A',
            link: function (scope, element, attributes) {
                // Make sure a data source is assigned.
                var dataSourceName = attributes['kDataSource'];
                var resultUrl = attributes['strixResultUrl'];

                if (!dataSourceName && !resultUrl) {
                    console.log('No kendo data source or result url specified for strixIT search directive.');
                    return;
                }

                if (resultUrl && strixIT.config.cmsActive == 'false') {
                    console.log('A result url is specified for the strixIT search directive, but the Cms module is not loaded.');
                    return;
                }

                // Remove the display: none style attribute from the element.
                element.show();

                if (!dataSourceName) {
                    var dataSource = dataService.createDataSource({});
                    dataSource.options.transport.read = function (options) {
                        var query = encodeURIComponent(JSON.stringify(options.data));
                        $window.location.href = strixIT.config.rootUrl + 'search/index?' + query;
                    };

                    scope['searchDataSource'] = dataSource;
                    dataSourceName = 'searchDataSource';
                }

                // Bind the filter and, when the element is not an input, the sort events.
                bindEvents(scope, $(element), dataSourceName);

                // Restore any previous filter that might exist.
                restoreFilter(scope, $(element), dataSourceName);
            }
        }
    }]);

    function bindEvents(scope, element, dataSourceName) {
        if (element.is('input')) {
            // For single inputs, use 'or' filtering logic.
            bindFilterEvents(scope, element, dataSourceName, 'or');
        }
        else {
            $('.filter input', element).each(function () {
                bindFilterEvents(scope, $(this), dataSourceName);
            });

            bindSortEvent(scope, element, dataSourceName);
        }
    }

    function bindFilterEvents(scope, element, dataSourceName, logic) {
        element.bind('keyup', function (event) {
            if (event.keyCode == 13) {
                var dataSource = scope[dataSourceName];
                var filter = getFilter(scope, dataSourceName);
                setFilter($(event.currentTarget), filter);
                dataSource.filter({ logic: logic || dataSource.options.filter.logic, filters: filter });
            }
        }).bind('blur', function (event) {
            setFilter($(event.currentTarget), getFilter(scope, dataSourceName));
        });
    }

    function bindSortEvent(scope, element, dataSourceName) {
        $('.sort', element).children().each(function () {
            $(this).bind('click', function (event) {
                var element = $(event.currentTarget);
                var field = element.data('field');
                var sort = getSort(scope, dataSourceName);
                var exists = false;

                if (sort.length > 0) {
                    for (var n in sort) {
                        if (sort[n].field == field) {
                            if (sort[n].dir == 'desc') {
                                sort.splice(n, 1);
                                removeSortClass(element, 'down');
                                addSortClass(element, 'left');
                            }
                            else if (sort[n].dir == 'asc') {
                                sort[n].dir = 'desc';
                                removeSortClass(element, 'up');
                                addSortClass(element, 'down');
                            }

                            exists = true;
                        }
                    }
                }
                else {
                    removeSortClass(element, 'left');
                    addSortClass(element, 'up');
                }

                if (!exists) {
                    sort.push({ field: field, dir: 'asc' });
                    removeSortClass(element, 'left');
                    addSortClass(element, 'up');
                }

                scope[dataSourceName].sort(sort);
            });
        });
    }

    function getFilter(scope, dataSourceName) {
        scope[dataSourceName].options.filter.filters = scope[dataSourceName].options.filter.filters || [];
        return scope[dataSourceName].options.filter.filters;
    }

    function getSort(scope, dataSourceName) {
        scope[dataSourceName].options.sort = scope[dataSourceName].options.sort || [];
        return scope[dataSourceName].options.sort;
    }

    function restoreFilter(scope, element, dataSourceName) {
        // Check whether the datasource exist. If so, try to restore the previous filter and sort settings.
        var dataSource = scope[dataSourceName];

        if (!dataSource) {
            return;
        }

        var filterElements = $('.filter input', element);
        var sortElements = $('.sort', element).children();
        var filter = getFilter(scope, dataSourceName);
        var sort = getSort(scope, dataSourceName);

        for (var n in filter) {
            var theFilter = filter[n];

            if (theFilter.field) {
                filter.push(theFilter);
                var filterElement = $($.grep(filterElements, function (x) { return $(x).data('field') == theFilter.field; }));
                filterElement.val(theFilter.value);
            }
        }

        for (var n in sort) {
            var theSort = sort[n];

            if (theSort.field) {
                sort.push(theSort);
                var sortElement = $($.grep(sortElements, function (x) { return $(x).data('field') == theSort.field; }));

                if (theSort.dir == 'asc') {
                    removeSortClass(sortElement, 'left');
                    addSortClass(sortElement, 'up');
                }
                else if (theSort.dir == 'desc') {
                    removeSortClass(sortElement, 'left');
                    addSortClass(sortElement, 'down');
                }
            }
        }
    }

    function setFilter(element, filter) {
        var fields = element.data('field').split(',');
        var operator = element.data('operator') || 'contains';
        var value = element.val();

        var exists = false;

        for (var n in fields) {
            var field = fields[n].trim();

            for (var n in filter) {
                if (filter[n].field == field) {
                    filter[n].value = value;
                    exists = true;
                }
            }

            if (!exists) {
                filter.push({ field: field, operator: operator, value: value });
            }
        }
    }

    function addSortClass(element, dir) {
        var icon = element.children('i').first();
        icon.addClass('fa');
        icon.addClass('fa-caret-' + dir);
    }

    function removeSortClass(element, dir) {
        var icon = element.children('i').first();
        icon.removeClass('fa');
        icon.removeClass('fa-caret-' + dir);
    }

})();