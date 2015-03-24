angular.module("umbraco").controller("LeBlender.Dialog.EditorConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        $scope.defaultEditorList = [
	        {
	            name: "Rich text editor",
	            view: "rte"
	        },
	        {
	            name: "Image",
	            view: "media"
	        },
	        {
	            name: "Macro",
	            view: "macro"
	        },
	        {
	            name: "Embed",
	            view: "embed"
	        },
	        {
	            name: "Textstring",
	            view: "textstring",
	        },
            {
            	name: "Blender Editor",
            	view: "/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html",
            }
        ]

        $scope.isCustom = function () {
            var isCustom = true;
            angular.forEach($scope.defaultEditorList, function (item, index) {
                if ($scope.model.value.view != "" && item.view === $scope.model.value.view) {
                    isCustom = false;
                }
            });
            return isCustom;
        }

        $scope.remove = function ($index) {
            $scope.model.value.config.editors.splice($index, 1);
        }

        $scope.isBlenderEditor = function () {

            /***************************************/
            /* legacy adaptor 0.9.15 */
            /***************************************/
            if ($scope.model.value.config.fixed != undefined &&
                $scope.model.value.config.limit &&
                !$scope.model.value.config.min &&
                !$scope.model.value.config.max) {
                if ($scope.model.value.config.fixed) {
                    $scope.model.value.config.min = $scope.model.value.config.limit;
                    $scope.model.value.config.max = $scope.model.value.config.limit;
                }
                else {
                    $scope.model.value.config.min = 1;
                    $scope.model.value.config.max = $scope.model.value.config.limit;
                }
                delete $scope.model.value.config.fixed;
                delete $scope.model.value.config.limit;
            }

            if ("/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html" === $scope.model.value.view) {
                return true;
            }
            return false;
        }

        $scope.openIconPicker = function () {
            var dialog = dialogService.iconPicker({
                show: true,
                callback: function (data) {
                    $scope.model.value.icon = data;
                }
            });
        }

        $scope.update = function () {
            $scope.model.value.config = "";
            $scope.textAreaconfig = "";
            if ($scope.isBlenderEditor()) {
                $scope.model.value.config = {
                    min: 1,
                    max: 1
                }
            }
        }

        $scope.getConfigAsText = function () {

            $scope.textAreaconfig = "";

            if ($scope.model.value.config) {

                var config = JSON.stringify($scope.model.value.config, null, 4)

                if (config && config != {}) {
                    $scope.textAreaconfig = config;
                }
                else {
                    $scope.textAreaconfig = "";
                }
                

            }
            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    //Exception handler
                };
            });
        };

        $scope.openParameterConfig = function (parameter) {

            if (!parameter) {

                if (!$scope.model.value.config.editors) {
                    $scope.model.value.config.editors = [];
                }

                $scope.model.value.config.editors.splice($scope.model.value.config.editors.length + 1, 0, {
                    name: "",
                    alias: "",
                });
                parameter = $scope.model.value.config.editors[$scope.model.value.config.editors.length - 1];
            }

            if (parameter) {
                var dialog = dialogService.open({
                    template: '/App_Plugins/Lecoati.LeBlender/core/Dialogs/parameterconfig.prevalues.html',
                    show: true,
                    dialogData: {
                        parameter: parameter,
                        availableDataTypes: $scope.dialogData.availableDataTypes
                    },
                    callback: function (data) {
                    }
                });
            }
        }

        $scope.autoPopulateAlias = function (name) {
            var s = name.replace(/[^a-zA-Z0-9\s\.-]+/g, ''); 
            return s.toCamelCase();
        }

        var toCamelCase = function (name) {
            var s = name.toPascalCase();
            if ($.trim(s) == "")
                return "";
            if (s.length > 1)
                s = s.substr(0, 1).toLowerCase() + s.substr(1);
            else
                s = s.toLowerCase();
            return s;
        };

        var toPascalCase = function (name) {
            var s = "";
            angular.each($.trim(name).split(/[\s\.-]+/g), function (val, idx) {
                if ($.trim(val) == "")
                    return;
                if (val.length > 1)
                    s += val.substr(0, 1).toUpperCase() + val.substr(1);
                else
                    s += val.toUpperCase();
            });
            return s;
        };

        $scope.save = function () {
            if ($scope.isBlenderEditor()) {
                $scope.dialogData.editor.render = "/App_Plugins/Lecoati.LeBlender/core/views/Base.cshtml";
            }
            angular.extend($scope.dialogData.editor, $scope.model.value);
            $scope.close();
        }

        angular.extend($scope, {
            model: {
                value: angular.copy($scope.dialogData.editor)
            }    
        });

        $scope.getConfigAsText();

        if ($scope.model.value.name === "" && $scope.model.value.name === "") {
            $scope.$watch("model.value.name", function () {
                $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
                $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
            })
        }

    });