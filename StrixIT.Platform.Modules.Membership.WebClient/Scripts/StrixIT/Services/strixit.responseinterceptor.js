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

    angular.module('strixServices').factory('responseInterceptor', ['$q', '$exceptionHandler', '$rootScope', function ($q, $exceptionHandler, $rootScope, response) {
        return {
            responseError: responseError
        };

        function responseError(response) {
            if (response.status == 401) {
                document.location = strixIT.getLoginUrl();
            }
            else {
                var exception = new Error();
                exception.name = 'AjaxException';
                exception.level = 'Error';
                exception.message = "Http " + response.status + ' error for ' + response.config.method + ' to ' + response.config.url + '.';
                $exceptionHandler(exception);

                $rootScope.$broadcast('ajaxError', $rootScope.getResource('web', 'interface', 'listServerError'));

                return $q.reject(response);
            }
        }
    }]);
})();