(function () {
    'use strict';

    angular.module('strixDirectives').directive('strixDropDown', function () {
        return {
            restrict: 'AE',
            require: 'ngModel',
            priority: 100,
            scope: { model: '=ngModel', data: '=source', changeCallback: '=' },
            link: function (scope, elem, attr, ngModel) {
                var optionLabel = attr['optionLabel'] || 'Name';
                var optionValue = attr['optionValue'] || 'Id';
                var defaultFlag = attr['defaultFlag'];
                var select = null;

                // Save the model value on linking of the directive. This value needs to be stored to be able to set a
                // dropdown correctly when multiple changes of the datasource are triggered before its value is set.
                scope.oldModelValue = null;

                // Create the select element, if it is not present already.
                select = findOrCreateSelectElement(elem);

                // Watch the data source for changes and recreate the dropdown when changes occur.
                scope.$watchCollection('data', function (newValue, oldValue) {
                    dataChanged(select, scope, ngModel, optionLabel, optionValue, defaultFlag);
                });

                // Handle the selection of a dropdown option.
                select.change(function (e) {
                    onChange(e.currentTarget.value, scope, ngModel);
                });
            }
        };
    });

    function findOrCreateSelectElement(elem) {
        var select;

        if (elem.is('select')) {
            select = elem;
        }
        else {
            select = elem.find('select');
        }

        if (!select.length) {
            select = $('<select></select>');
            elem.append(select);
        }

        return select;
    }

    function dataChanged(select, scope, ngModel, optionLabel, optionValue, defaultFlag) {
        if (scope.data) {
            select.empty();
            var isInData = false;
            var isOldValueInData = false;

            // Check whether the current or old value of the model is present in the current
            // data of the source, then create a select option for the data entry.
            for (var n in scope.data) {
                var entry = scope.data[n];

                if (scope.model == entry[optionValue]) {
                    isInData = true;
                }

                if (scope.oldModelValue && scope.oldModelValue == entry[optionValue]) {
                    isOldValueInData = true;
                }

                addSelectOption(select, scope, entry, optionLabel, optionValue, defaultFlag);
            }

            // Set the active dropdown option when the list is populated.
            setSelectedValue(scope, ngModel, optionValue, isInData, isOldValueInData);
        }
    }

    function onChange(value, scope, ngModel) {
        // Do the callback before updating the view value. This order is important, which has something
        // to do with the angular digest cycle. I'm not sure what exactly.
        if (scope.changeCallback) {
            scope.changeCallback(value);
        }

        ngModel.$setViewValue(value);
    }

    function addSelectOption(select, scope, entry, optionLabel, optionValue, defaultFlag) {
        var selected;

        if (scope.model == entry[optionValue] || scope.oldModelValue == entry[optionValue] || entry[defaultFlag]) {
            selected = 'selected'
        }
        else {
            selected = '';
        }

        var option = $('<option value="' + entry[optionValue] + '" ' + selected + ' >' + entry[optionLabel] + '</option>');
        select.append(option);
    }

    function setSelectedValue(scope, ngModel, optionValue, isInData, isOldValueInData) {
        if (!scope.oldModelValue) {
            scope.oldModelValue = scope.model;
        }

        var value = scope.data[0] ? scope.data[0][optionValue] : null;

        if (isInData) {
            value = scope.model;
        }
        else if (isOldValueInData) {
            value = scope.oldModelValue;
        }

        // Call onchange to actually set the view value.
        onChange(value, scope, ngModel);
    }
})()