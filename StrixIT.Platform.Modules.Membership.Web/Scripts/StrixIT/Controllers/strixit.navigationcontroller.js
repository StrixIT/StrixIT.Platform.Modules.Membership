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

    angular.module('strixSite').controller('navigationcontroller', ['$scope', '$location', '$templateCache', '$window', 'dataService', function ($scope, $location, $templateCache, $window, dataService) {
        return navigationController($scope, $location, $templateCache, $window, dataService);
    }]);

    angular.module('strixAdmin').controller('navigationcontroller', ['$scope', '$location', '$templateCache', '$window', 'dataService', function ($scope, $location, $templateCache, $window, dataService) {
        return navigationController($scope, $location, $templateCache, $window, dataService);
    }]);

    var navigationController = function ($scope, $location, $templateCache, $window, dataService) {
        var tc = $templateCache;
        var baseUrl = null;
        var templateUrl = null;

        $scope.getUrl = getUrl;
        $scope.getTarget = getTarget;
        $scope.changeGroup = changeGroup;

        function getUrl(language) {
            if ($location.$$url != baseUrl && baseUrl != '/') {
                baseUrl = $location.$$url;

                if (!baseUrl) {
                    baseUrl = $location.$$absUrl.replace($location.$$protocol + '://' + $location.$$host, '');

                    var port = $location.$$port;

                    if (baseUrl.indexOf(port)) {
                        baseUrl = baseUrl.replace(':' + $location.$$port, '')
                    }

                    baseUrl = (baseUrl + '/').replace('//', '/');
                }

                var culture = strixIT.config.currentCulture;

                if (culture) {
                    templateUrl = baseUrl.replace('/' + culture + '/', '/{0}/').replace('/' + culture, '/{0}/');
                }
                else {
                    templateUrl = '/{0}' + baseUrl;
                }
            }

            var replaceValue = language.toLowerCase() != strixIT.config.defaultCulture.toLowerCase() ? '/' + language + '/' : '/';
            var result = templateUrl.toLowerCase().replace('/{0}/', replaceValue);

            if (result[result.length - 1] == '/' && result.length > 1) {
                result = result.substring(0, result.length - 1);
            }

            return result;
        }

        function getTarget() {
            if ($location.$$url && $location.$$url.toLowerCase().indexOf('/admin') > -1) {
                return '_self';
            }
        }

        function changeGroup(groupId) {
            dataService.callServer(strixIT.config.rootUrl + strixIT.config.routePrefix + "Membership/Group/ChangeGroup", { groupId: groupId }).then(function (result) {
                if (result) {
                    tc.removeAll();
                    $window.location.href = strixIT.config.rootUrl;
                }
                else {
                    throw new Error(dataService.getResource('membership', 'interface', 'errorChangingGroup'));
                }
            });
        }
    }
})()