(function () {
    'use strict';

    angular.module('strixServices').factory('sanitizeService', ['$sanitize', function ($sanitize, response) {
        return {
            sanitizeInput: sanitizeInput
        };

        function sanitizeInput(input) {
            if (typeof input !== "object") {
                return input;
            }

            for (var key in input) {
                if (!input.hasOwnProperty(key)) {
                    continue;
                }

                var value = input[key];

                if (typeof value === "string") {
                    input[key] = $sanitize(input[key]);
                }
                else if (typeof value === "object" && value) {
                    sanitizeInput(value);
                }
            }
        }
    }]);
})();