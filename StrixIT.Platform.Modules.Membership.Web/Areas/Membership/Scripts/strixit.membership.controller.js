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

    angular.module('strixAdmin').registerController('membershipcontroller', ['$scope', 'entityService', function ($scope, entityService) {
        $scope.hasPermissions = hasPermissions;
        $scope.hasRoles = hasRoles;
        $scope.getRoles = getRoles;
        $scope.getPermissions = getPermissions;
        $scope.submit = submit;
        $scope.changeDate = changeDate;

        $scope.delete = $scope.deleteEntity;
        $scope.confirmDelete = $scope.confirmDeleteEntity;

        function hasPermissions(item) {
            return item.permissions && item.permissions.length > 0;
        }

        function hasRoles(item) {
            return item.roles && item.roles.length > 0;
        }

        function getRoles() {
            var entity = entityService.getEntity();

            if (entity) {
                return entity.roles;
            }
        }

        function getPermissions() {
            var entity = entityService.getEntity();

            if (entity) {
                return entity.permissions;
            }
        }

        function submit(data) {
            if (!this.validator.validate()) {
                return;
            }

            $scope.saveEntity(data);
        }

        function changeDate(role, type) {
            if (type == 'start') {
                if (role.endDate && role.endDate < role.startDate) {
                    role.endDate = role.startDate;
                }
            }
            else {
                if (role.endDate && role.startDate > role.endDate) {
                    role.startDate = role.endDate;
                }
            }
        }
    }]);
})()