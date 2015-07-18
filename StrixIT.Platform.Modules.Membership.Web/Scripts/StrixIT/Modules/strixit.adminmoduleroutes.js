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

    var routes = [];
    var adminModule = angular.module('strixAdmin');

    adminModule.config(['$routeProvider', '$locationProvider',
        function ($routeProvider, $locationProvider) {
            initRoutes($routeProvider, $locationProvider);
        }
    ]);

    adminModule.addRoute = addRoute;
    adminModule.addCrudRoute = addCrudRoute;

    function initRoutes($routeProvider, $locationProvider) {
        if (window.history && window.history.pushState) {
            $locationProvider.html5Mode({ enabled: true, requireBase: false });
        }

        if (routes.length) {
            for (var n in routes) {
                var route = routes[n];
                setRoute(route, $routeProvider);
            }
        }
    }

    function addRoute(routeParams) {
        var data = {
            module: routeParams.module ? routeParams.module.toLowerCase() : undefined,
            type: routeParams.type ? routeParams.type.toLowerCase() : undefined,
            baseRoute: routeParams.route,
            saveCallBack: routeParams.saveCallBack,
            deleteCallBack: routeParams.deleteCallBack,
        };

        routes.push({
            data: data,
            route: strixIT.config.rootUrl + strixIT.config.routePrefix + routeParams.route,
            templateUrl: strixIT.config.rootUrl + strixIT.config.routePrefix + routeParams.templateUrl,
            controller: routeParams.controller,
            dependencies: routeParams.dependencies
        });
    }

    function addCrudRoute(routeParams) {
        var baseRoute = routeParams.module && routeParams.module != routeParams.type ? routeParams.module + '/' + routeParams.type : routeParams.type;

        var data = {
            module: routeParams.module ? routeParams.module.toLowerCase() : undefined,
            type: routeParams.type.toLowerCase(),
            baseRoute: baseRoute,
            loadCallBack: routeParams.loadCallBack,
            saveCallBack: routeParams.saveCallBack,
            deleteCallBack: routeParams.deleteCallBack,
        };

        var rootRoute = strixIT.config.rootUrl + strixIT.config.routePrefix + baseRoute;

        routes.push({ data: data, route: rootRoute, templateUrl: rootRoute + '/Index', controller: routeParams.controller, dependencies: routeParams.dependencies });
        routes.push({ data: data, route: rootRoute + '/Edit', templateUrl: rootRoute + '/Edit', controller: routeParams.controller, dependencies: routeParams.dependencies });
        routes.push({ data: data, route: rootRoute + '/Edit/:id', templateUrl: rootRoute + '/Edit', controller: routeParams.controller, dependencies: routeParams.dependencies });
        routes.push({ data: data, route: rootRoute + '/Details/:id', templateUrl: rootRoute + '/Details', controller: routeParams.controller, dependencies: routeParams.dependencies });
    }

    function setRoute(route, $routeProvider) {
        return function () {
            $routeProvider.when(route.route, {
                controller: route.controller,
                templateUrl: route.templateUrl,
                caseInsensitiveMatch: true,
                resolve: { deps: ['$q', '$rootScope', 'dataService', function ($q, $rootScope, dataService) { return resolveRoute(route, $q, $rootScope, dataService, route.data.module); }] },
                data: route.data
            })
        }()
    }

    function resolveRoute(route, $q, $rootScope, dataService, module) {
        var deferred = $q.defer();
        var dependencyPromise = $q.defer();

        if (route.dependencies) {
            var dependencies = [];

            for (var n in route.dependencies) {
                dependencies.push(strixIT.config.resourceRootUrl + route.dependencies[n])
            }

            $script(dependencies, function () {
                $rootScope.$apply(function () {
                    dependencyPromise.resolve();
                });
            });
        }
        else {
            dependencyPromise.resolve();
        }

        if (strixIT.config.cmsActive == 'true') {
            $q.all([dataService.init('Web'), dataService.init('Cms'), dataService.init(module), dependencyPromise.promise])
                .then(function () { deferred.resolve(); });
        }
        else {
            $q.all([dataService.init('Web'), dataService.init(module), dependencyPromise.promise])
                .then(function () { deferred.resolve(); });
        }

        return deferred.promise;
    }
})();