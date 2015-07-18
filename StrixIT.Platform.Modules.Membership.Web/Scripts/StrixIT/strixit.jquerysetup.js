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

    // Setup general JQuery Ajax handlers
    $.ajaxSetup({
        beforeSend: function (request) {
            request.setRequestHeader(strixIT.config.ajaxHeaderKey, strixIT.config.ajaxHeaderValue);
            var token = strixIT.getRequestVerificationToken();

            if (token) {
                request.setRequestHeader(strixIT.config.csrfTokenName, token);
            }
        }
    });

    $(document).bind("ajaxSuccess", function (event, result) {
        strixIT.convertDateStringsToDates(result);
    });

    $(document).bind("ajaxError", function (event, result) {
        if (result.status === 401) {
            document.location = strixIT.getLoginUrl();
        }
    });

    // Setup CSRF protection for forms
    $('input:submit').on('click', function (event) {
        var token = strixIT.getRequestVerificationToken();

        if (token) {
            var form = $(this).closest('form');
            var csrftoken = form.find('[name="' + strixIT.config.csrfTokenName + '"]');

            if (csrftoken.length == 0) {
                form.append('<input type="hidden" value="" name="' + strixIT.config.csrfTokenName + '">');
                csrftoken = form.find('[name="' + strixIT.config.csrfTokenName + '"]');
                csrftoken.val(token);
            }
        }
    });

})()