angular.module("umbraco").factory("LeBlenderRequestHelper",
    function ($rootScope, $q, $http, $parse, $routeParams) {

        var configPath = "/config/grid.editors.config.js";

        return {

            /*********************/
            /*********************/
            GetPartialViewResultAsHtmlForEditor: function (control) {

                var view = "grid/editors/base";
                var url = "/umbraco/surface/Helper/GetPartialViewResultAsHtmlForEditor";
                var resultParameters = { model: angular.toJson(control, false), view: view, id: $routeParams.id };

                //$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
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
            },

            /*********************/
            /*********************/
            getGridEditors: function () {
                return $http.get(configPath + "?" + ((Math.random() * 100) + 1));
            },

            /*********************/
            /*********************/
            setGridEditors: function (data) {



                var url = "/umbraco/surface/Helper/SaveEditorConfig";
                var resultParameters = { config: angular.toJson(data, false), configPath: configPath };

                //$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                var promise = $http.post(url, resultParameters, {
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    transformRequest: function (result) {
                        return $.param(result);
                    }
                })
                .success(function (Result) {

                    return Result;

                });
            }

        }

    });