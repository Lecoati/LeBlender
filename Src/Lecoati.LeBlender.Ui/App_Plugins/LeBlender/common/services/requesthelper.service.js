angular.module("umbraco").factory("leBlenderRequestHelper",
    function ($http, $routeParams, umbRequestHelper) {

        var configPath = "/config/grid.editors.config.js";

        return {

            getPartialViewResultAsHtmlForEditor: function (control) {

                var view = "grid/editors/base";
                var url = "/umbraco/backoffice/leblender/Helper/GetPartialViewResultAsHtmlForEditor";
                var resultParameters = { model: angular.toJson(control, false), view: view, id: $routeParams.id, doctype: $routeParams.doctype };

                //$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                return $http.post(url, resultParameters, {
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    transformRequest: function (result) {
                        return $.param(result);
                    }
                });
            },

            getGridEditors: function () {
                return $http.get(configPath + "?" + ((Math.random() * 100) + 1));
            },

            getAllPropertyGridEditors: function () {
                return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/PropertyGridEditor/GetAll"), 'Failed to retrieve datatypes from tree service');
            },

            setGridEditors: function (data) {

                var url = "/umbraco/backoffice/leblender/Helper/SaveEditorConfig";
                var resultParameters = { config: JSON.stringify(data, null, 4), configPath: configPath };

                //$http.defaults.headers.post["Content-Type"] = "application/x-www-form-urlencoded";
                return $http.post(url, resultParameters, {
                    headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
                    transformRequest: function (result) {
                        return $.param(result);
                    }
                });

            },

            getAllDataTypes: function () {
                return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetAll"), 'Failed to retrieve datatypes from tree service');
            },

            getDataType: function (guid) {
                return umbRequestHelper.resourcePromise($http.get("/umbraco/backoffice/LeBlenderApi/DataType/GetPropertyEditors?guid=" + guid, { cache: true }), 'Failed to retrieve datatype');
            }

        };

    });