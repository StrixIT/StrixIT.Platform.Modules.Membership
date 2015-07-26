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

    angular.module('strixAdmin').controller('itemcontroller', ['$rootScope', '$scope', '$location', '$window', '$route', 'dataService', 'entityService', function ($rootScope, $scope, $location, $window, $route, dataService, entityService) {
        var type = $route.current.data.type;
        $scope[type] = {};
        $scope.mode = $location.path().toLowerCase().indexOf('edit') == -1 ? 'details' : 'edit';

        $scope.saveEntity = saveEntity;
        $scope.showCancel = showCancel;
        $scope.showDelete = showDelete;
        $scope.showEdit = showEdit;
        $scope.delete = $scope.deleteEntity;
        $scope.confirmDelete = $scope.confirmDeleteEntity;
        $scope.getLink = getLink;

        $scope.$on("$routeChangeSuccess", routeChanged);
        $scope.$on('remoteValidationInProgress', remoteValidationStart);
        $scope.$on('remoteValidationDone', remoteValidationDone);
        $scope.$on('entityLoaded', entityLoaded);
        $scope.$on('versionLoaded', entityLoaded);

        function showCancel() {
            return $scope.mode == 'edit' && !$scope.isNew();
        }

        function showEdit() {
            var entity = entityService.getEntity();

            if (entity) {
                return $scope.mode == 'details' && entity.canEdit;
            }
        }

        function showDelete() {
            var entity = entityService.getEntity();

            if (entity) {
                return $scope.mode == 'details' && entity.canDelete;
            }
        }

        function getLink(baseUrl, urlProperty) {
            var entity = entityService.getEntity();

            if (entity) {
                return (baseUrl + '/').replace('//', '/') + entity[urlProperty];
            }
        }

        function saveEntity(data) {
            $scope.savingEntity = true;
            dataService.saveEntity(data, $location.path()).then(function (result) {
                $scope.savingEntity = false;

                if (result.success) {
                    $scope.notify.show($scope.getResource('web', 'interface', 'saveSuccessful'), "success");
                    kendo.ui.validator.clearRemoteCache();
                    $scope.storeData('refresh', true);

                    if ($route.current.data.saveCallBack) {
                        $route.current.data.saveCallBack(result.data);
                    }

                    if (data.returnUrl) {
                        $window.location.href = data.returnUrl;
                    }
                    else {
                        $location.path(strixIT.config.rootUrl + strixIT.config.routePrefix + $route.current.data.baseRoute);
                    }
                }
                else {
                    $scope.notify.show(result.message, "error");
                }
            }, function (status) {
                $scope.savingEntity = false;
                $scope.notify.show($scope.getResource('web', 'interface', 'saveFailed'), "error");
            });
        }

        function routeChanged(event, params) {
            if ($scope.notify) {
                $scope.notify.hide();
            }
            $scope.dataLoaded = false;
            $scope.dataLoadError = false;

            var path = $location.path().toLowerCase();
            var id = getId(params.pathParams);

            var promise = dataService.getFromServer(path, { id: id });

            promise.then(function (data) {
                dataLoaded(data);
                $scope.dataLoaded = true;
            },
            function (result) {
                $scope.dataLoaded = true;
                $scope.dataLoadError = true;
            });
        }

        function remoteValidationStart() {
            $scope.remoteValidationInProgress = true;
        }

        function remoteValidationDone() {
            $scope.remoteValidationInProgress = false;
        }

        function entityLoaded(event, entity) {
            $scope[type] = entity;
        }

        function dataLoaded(data) {
            entityService.storeEntity(data);

            data.returnUrl = $('#returnUrl').val();
            $('#returnUrl').val('');

            if ($route.current.data.loadCallBack) {
                $route.current.data.loadCallBack(data);
            }

            $rootScope.$broadcast('entityLoaded', data);
        }

        function getId(params) {
            var id = null;

            for (var n in params) {
                if (id) {
                    id = id + "/";
                }

                var param = params[n];

                if (param) {
                    if (id) {
                        id = id + param;
                    }
                    else {
                        id = param;
                    }
                }
            }

            return id;
        }

    }]);

})()