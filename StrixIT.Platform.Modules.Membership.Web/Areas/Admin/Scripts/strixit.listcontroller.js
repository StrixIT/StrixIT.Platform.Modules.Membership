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

    angular.module('strixAdmin').controller('listcontroller', ['$scope', '$filter', '$route', 'entityService', function ($scope, $filter, $route, entityService) {
        $scope.getName = getName;
        $scope.getImage = getImage;
        $scope.hasImage = hasImage;
        $scope.getIcon = entityService.getImageIcon;
        $scope.getValue = getValue;
        $scope.delete = $scope.deleteEntity;
        $scope.confirmDelete = $scope.confirmDeleteEntity;
        $scope.pagerOptions = {
            //input: true,
            pageSizes: [10, 25, 50],
            //refresh: true,
            change: $scope.changePage
        }

        $scope.$on("$routeChangeSuccess", function (event, params) {
            var type = $route.current.data.type;
            var source = entityService.getOrCreateList();
            $scope[type + 'stemplate'] = $('#' + type + 'stemplate').html();

            if (!$scope[type + 's']) {
                $scope.storeData(type + 's', source);
            }
        });

        function getName(item) {
            return name = item.name ? item.name : item.title ? item.title : item.entityKey;
        }

        function hasImage(item, filePath) {
            var image = getImage(item, filePath);
            return image != null && image != undefined;
        }

        function getImage(item, filePath) {
            var image = item[filePath];

            if (image && image.substring(0, 1) != '/') {
                image = '/' + image;
            }

            return image;
        }

        function getValue(filterName, fieldName, item) {
            if (!filterName) {
                return item[fieldName];
            }

            return $filter(filterName)(item[fieldName]);
        }
    }]);

})();