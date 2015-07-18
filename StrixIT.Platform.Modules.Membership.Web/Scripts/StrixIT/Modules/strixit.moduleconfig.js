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

    var adminModule = angular.module('strixAdmin');
    var siteModule = angular.module('strixSite');

    configureModule(adminModule);
    configureModule(siteModule);

    siteModule.run(['$window', '$location', function ($window, $location) {
        addHashChangeListener($window, $location);
    }]);

    function configureModule(module) {
        module.config(['$httpProvider', function ($httpProvider) {
            $httpProvider.interceptors.push('responseInterceptor');
            $httpProvider.defaults.transformResponse.push(function (data) {
                return strixIT.convertDateStringsToDates(data);
            });
        }])
        .run(['$http', function ($http) {
            addCsrfToken($http);
        }]);
    }

    function addCsrfToken(http) {
        http.defaults.headers.common[strixIT.config.ajaxHeaderKey] = strixIT.config.ajaxHeaderValue;
        var token = strixIT.getRequestVerificationToken();

        if (token) {
            http.defaults.headers.common[strixIT.config.csrfTokenName] = token;
        }
    }

    function addHashChangeListener($window, $location) {
        var location = $location;

        angular.element($window).on('hashchange', function (e) {
            var hash = location.$$hash ? location.$$hash : location.$$url;

            if (hash) {
                if (hash.substring(0, 2) == '#/') {
                    hash = '#' + hash.substring(2)
                }

                if (hash.substring(0, 1) == '/') {
                    hash = '#' + hash.substring(1)
                }

                angular.element(hash)[0].scrollIntoView(true);
            }
        });
    }
})()