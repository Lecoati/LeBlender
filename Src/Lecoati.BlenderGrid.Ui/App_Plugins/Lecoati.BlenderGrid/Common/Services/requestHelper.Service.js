angular.module("umbraco").factory("BlenderGridRequestHelper",
    function ($rootScope, $q, $http, $parse) {

        return {

            GetPartialViewResultAsHtmlForEditor: function (control) {

                var view = "grid/editors/base";
                var url = "/umbraco/surface/Helper/GetPartialViewResultAsHtmlForEditor";
                var resultParameters = { model: angular.toJson(control, false), view: view };

                $http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                var promise = $http.post(url, resultParameters, {
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    transformRequest: function (result) {
                        return $.param(result);
                    }
                })
                .success(function (htmlResult) {
                    if (htmlResult.trim().length > 0) {
                        return htmlResult;
                    }
                });

                return promise;
            }

        }

    });