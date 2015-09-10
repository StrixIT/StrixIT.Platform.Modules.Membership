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

    angular.module('strixServices').factory('entityService', ['$route', 'dataService', function ($route, dataService) {
        var dataStore = {};

        return {
            storeEntity: storeEntity,
            getEntity: getEntity,
            isNew: isNew,
            storeData: storeData,
            getData: getData,
            getOrCreateList: getOrCreateList,
            getImageIcon: getImageIcon
        };

        function storeEntity(entity) {
            dataStore[getType()] = entity;
        }

        function getEntity() {
            return dataStore[getType()];
        }

        function storeData(key, data) {
            dataStore[key] = data;
        }

        function getData(key) {
            return dataStore[key];
        }

        function getOrCreateList(params) {
            params = params || {};
            var url = params.url || '/List'
            var type = getType();
            var listName = params.key ? type + 's' + params.key : type + 's';
            var data = params.data || null;

            if (!dataStore[listName]) {
                dataStore[listName] = dataService.createDataSource({ url: strixIT.config.rootUrl + strixIT.config.routePrefix + $route.current.data.baseRoute + url, data: data, readCallBack: params.readCallBack });
            }

            return dataStore[listName];
        }

        function isNew() {
            var entity = getEntity();

            if (entity) {
                return entity.id == strixIT.config.emptyGuid || entity.id == 0;
            }

            return true;
        }

        function getType() {
            return $route.current.data.type;
        }

        function getImageIcon(item, filePath) {
            if (!item) {
                return;
            }

            var docTypes = dataService.getEnum('Cms', 'documentType');

            if (docTypes.length == 0) {
                return null;
            }

            if (!filePath) {
                filePath = 'filePath';
            }

            var path = item[filePath];
            var type = item.documentType ? item.documentType.toLowerCase() : null;
            var extension = item.extension ? item.extension.toLowerCase() : null;
            var iconClass = null;

            if (extension) {
                if (extension == 'pdf') {
                    iconClass = 'file-pdf-o';
                }
                else if (extension == 'doc' || extension == 'docx') {
                    iconClass = 'file-word-o';
                }
                else if (extension == 'xls' || extension == 'xlsx') {
                    iconClass = 'file-excel-o';
                }
            }

            if (!iconClass && type) {
                iconClass =
                  type == docTypes.image || docTypes[type] == docTypes.image ? 'file-image-o' :
                  type == docTypes.video || docTypes[type] == docTypes.video ? 'file-video-o' :
                  type == docTypes.audio || docTypes[type] == docTypes.audio ? 'file-audio-o' :
                  type == docTypes.document || docTypes[type] == docTypes.document ? 'file-text-o' : 'file-o';
            }

            return iconClass ? 'fa fa-' + iconClass : null;
        }
    }]);
})()