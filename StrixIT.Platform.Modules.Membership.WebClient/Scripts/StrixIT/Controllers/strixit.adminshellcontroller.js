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

    angular.module('strixAdmin').controller(
        'adminshellcontroller',
        ['$scope', '$location', '$route', '$sanitize', 'dataService', 'entityService',
            function ($scope, $location, $route, $sanitize, dataService, entityService) {
                var deleteConfirmTitle = null;
                var deleteConfirmBody = null;

                // Expose the sanitize and data service on the scope so they can be used outside of angular code as well.
                $scope.dataService = dataService;
                $scope.sanitizeService = $sanitize;

                // Show the admin menu this way. ng-cloak does not work and there is no other angular context to use k-ng-delay.
                $('#adminmenu').css('display', 'inline-block');
                $scope.savingEntity = false;
                $scope.remoteValidationInProgress = false;
                $scope.templateLoaded = false;
                $scope.templateLoadError = false;
                $scope.dataLoaded = true;
                $scope.dataLoadError = false;

                $scope.getLoadingImage = strixIT.getLoadingImage;
                $scope.changePage = changePage;
                $scope.deleteEntity = deleteEntity;
                $scope.confirmDeleteEntity = confirmDeleteEntity;
                $scope.storeData = storeData;

                $scope.isNew = entityService.isNew;
                $scope.toTitleCase = toTitleCase;
                $scope.getResource = dataService.getResource;

                $scope.$on("$routeChangeStart", routeChangeStart);
                $scope.$on("$routeChangeError", routeChangeError);
                $scope.$on("$routeChangeSuccess", routeChangeSuccess);
                $scope.$on('ajaxError', ajaxError);

                function changePage(event) {
                    var table = event.sender.element.parent().find('table');

                    $('html, body').animate({
                        scrollTop: table.offset().top
                    }, 500);
                }

                function deleteEntity(data) {
                    var type = $route.current.data.type;
                    entityService.storeData(type + 'toDelete', data);
                    var window = $('#confirmdelete').data('kendoWindow');
                    var body = window.element.find('.confirmbody');

                    if (!deleteConfirmTitle) {
                        deleteConfirmTitle = window.title();
                    }

                    if (!deleteConfirmBody) {
                        deleteConfirmBody = body.html();
                    }

                    window.title(deleteConfirmTitle.replace('{0}', $scope.toTitleCase(type)));
                    body.html(deleteConfirmBody.replace('{0}', $scope.toTitleCase(type)).replace('{1}', data.name));
                    window.center().open();
                }

                function confirmDeleteEntity() {
                    var type = $route.current.data.type;
                    var entity = entityService.getData(type + 'toDelete');
                    var idProperty = entity.entityId ? 'entityId' : 'id';

                    dataService.deleteEntity(entity[idProperty], strixIT.config.rootUrl + strixIT.config.routePrefix + $route.current.data.baseRoute + '/Delete').then(
                        function (data) {
                            var typeText = $scope.toTitleCase(type) + " " + entity.name;

                            if (data.success) {
                                var message = $scope.getResource('web', 'interface', 'deleteSuccessful');
                                message = message.replace('{0}', typeText);
                                $scope.notify.show(message, "success");
                                var window = $('#confirmdelete').data('kendoWindow');
                                window.close();

                                if ($route.current.data.deleteCallBack) {
                                    $route.current.data.deleteCallBack(entity);
                                }
                                else {
                                    var path = $location.path().toLowerCase();
                                    var indexOfDetails = path.indexOf('/details')

                                    if (indexOfDetails > -1) {
                                        $location.path(strixIT.config.rootUrl + strixIT.config.routePrefix + $route.current.data.baseRoute)
                                    }
                                    else {
                                        $route.reload();
                                    }
                                }
                            }
                            else {
                                $scope.notify.show(data.message, "error");
                            }
                        }, function (status) {
                            var message = $scope.getResource('web', 'interface', 'deleteFailed');
                            message = message.replace('{0}', typeText);
                            $scope.notify.show(message, "error");
                        });
                }

                function storeData(key, data) {
                    $scope[key] = data;
                }

                function toTitleCase(text) {
                    if (text) {
                        return text.substring(0, 1).toUpperCase() + text.substring(1);
                    }
                }

                function routeChangeStart(event, params) {
                    $scope.templateLoadError = false;
                    $scope.templateLoaded = false;
                }

                function routeChangeError(event, params) {
                    $scope.templateLoaded = true;
                    $scope.templateLoadError = true;
                }

                function routeChangeSuccess(params) {
                    $scope.templateLoaded = true;
                }

                function ajaxError(event, data) {
                    $scope.notify.show(data, "error")
                }
            }]);
})();