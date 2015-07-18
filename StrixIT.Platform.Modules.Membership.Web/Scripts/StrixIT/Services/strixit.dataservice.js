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

    angular.module('strixServices').factory('dataService', ['$q', '$http', '$rootScope', '$sanitize', function ($q, $http, $rootScope, $sanitize) {
        var modules = {};
        $rootScope.getResource = getResource;

        return {
            init: init,
            createDataSource: createDataSource,
            callServer: callServer,
            getFromServer: getFromServer,
            saveEntity: saveEntity,
            deleteEntity: deleteEntity,
            getEnum: getEnum,
            getResource: getResource
        };

        function init(module) {
            if (!module) {
                module = 'Web';
            }

            var moduleName = module.toLowerCase();

            if (!modules[moduleName]) {
                modules[moduleName] = {};
                modules[moduleName].enumerations = {};
                modules[moduleName].resources = {};
                modules[moduleName].deferred = $q.defer();

                $q.all([callServer(strixIT.config.rootUrl + 'Home/GetResources', { moduleName: moduleName }),
                        callServer(strixIT.config.rootUrl + 'Home/GetEnumerations', { moduleName: moduleName })])
                  .then(function (data) {
                      for (var n in data[0]) {
                          modules[moduleName].resources[n.toLowerCase()] = data[0][n];
                      }

                      for (var n in data[1]) {
                          modules[moduleName].enumerations[n.toLowerCase()] = data[1][n];
                      }

                      if (moduleName == 'web') {
                          kendo.ui.setResourceRetriever(getResource);
                          kendo.ui.setSanitizeService($sanitize);
                          kendo.ui.localizeWidgets();
                      }

                      modules[moduleName].deferred.resolve();
                  });
            }

            return modules[moduleName].deferred.promise;
        }

        function createDataSource(settings) {
            var dataSource = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        {
                            callServer(settings.url, $.extend(true, options.data, settings.data, this.read.data))
                                .then(function (result) {
                                    options.success(result);

                                    if (settings.readCallBack) {
                                        settings.readCallBack(result);
                                    }
                                },
                                function (result) {
                                    options.error(result);
                                });
                        }
                    }
                },
                schema: {
                    data: 'data',
                    total: 'total'
                },
                change: settings.change,
                serverPaging: true,
                serverSorting: true,
                serverFiltering: true,
                filter: settings.filter || {
                    logic: settings.filterLogic || 'and'
                },
                pageSize: 10
            });

            return dataSource;
        }

        function getFromServer(path, data, url) {
            var deferred = $q.defer();

            if (!url) {
                url = 'get';
            }

            var correctedPath = path.replace('/edit', '/' + url).replace('/details', '/' + url);
            correctedPath = correctedPath.substring(0, correctedPath.indexOf('/' + url) + url.length + 1);

            return callServer(correctedPath, data);
        }

        function saveEntity(data, url) {
            return callServer(url, { model: data });
        }

        function deleteEntity(id, url) {
            return callServer(url, { id: id });
        }

        function getEnum(module, name) {
            // Todo: do I need to get the enum name as done with resources?
            var module = modules[module.toLowerCase()];

            if (module) {
                return module.enumerations[name.toLowerCase()];
            }

            return [];
        }

        function getResource(module, namespace, name) {
            if (modules[module.toLowerCase()]) {
                var resourceName = $.grep(Object.getOwnPropertyNames(modules[module.toLowerCase()].resources[namespace.toLowerCase()]), function (x) {
                    return x.toLowerCase() == name.toLowerCase();
                })[0];

                if (resourceName) {
                    return modules[module.toLowerCase()].resources[namespace.toLowerCase()][resourceName];
                }
            }

            return 'No resource for module ' + module + ' namespace ' + namespace + ' with key ' + name + '!';
        }

        function callServer(url, data) {
            transformData(data);

            var deferred = $q.defer();

            $http.post(url, data)
                .success(function (data, status, headers, config) {
                    deferred.resolve(data);
                }).
                error(function (data, status, headers, config) {
                    deferred.reject(data, status);
                });

            return deferred.promise;
        }

        function transformData(input) {
            if (typeof input !== "object") {
                return input;
            }

            for (var key in input) {
                if (!input.hasOwnProperty(key)) {
                    continue;
                }

                var value = input[key];

                // Todo
                // Exclude the body property for now as that always is a RTE field right now. Find a better way to handle this.
                if (typeof value === "string" && key != 'body') {
                    // Sanitize string properties because they may contain html markup.
                    try {
                        input[key] = $sanitize(value);
                    }
                    catch (ex) {
                        input[key] = $sanitize(value.replace(/</g, "< "));
                    }
                }
                else if (Object.prototype.toString.call(value) === "[object Date]") {
                    // Convert date properties to UTC to correct the time difference between the client and the server.
                    var offset = value.getTimezoneOffset(value) * 60 * 1000;
                    input[key] = new Date(value.getTime() - offset);
                }
                else if (typeof value === "object" && value) {
                    transformData(value);
                }
            }
        }
    }]);

})();