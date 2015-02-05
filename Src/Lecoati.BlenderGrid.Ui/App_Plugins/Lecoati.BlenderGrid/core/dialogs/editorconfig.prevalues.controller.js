angular.module("umbraco").controller("BlenderGrid.Dialog.EditorConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, BlenderGridRequestHelper, dialogService) {

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
            	view: "/App_Plugins/Lecoati.BlenderGrid/core/blendergrideditor.html",
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
            if ("/App_Plugins/Lecoati.BlenderGrid/core/blendergrideditor.html" === $scope.model.value.view) {
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
            $scope.model.value.config = {}
        }

        $scope.getConfigAsText = function () {
            $scope.textAreaconfig = JSON.stringify($scope.model.value.config, null, 4);
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
                    view: ""
                });
                parameter = $scope.model.value.config.editors[$scope.model.value.config.editors.length - 1];
            }

            if (parameter) {
                var dialog = dialogService.open({
                    template: '/App_Plugins/Lecoati.BlenderGrid/core/Dialogs/parameterconfig.prevalues.html',
                    show: true,
                    dialogData: {
                        parameter: parameter
                    },
                    callback: function (data) {
                    }
                });
            }
        }

        $scope.save = function(){
            angular.extend($scope.dialogData.editor, $scope.model.value);
            $scope.close();
        }

        angular.extend($scope, {
            model: {
                value: angular.copy($scope.dialogData.editor)
            }    
        });

        $scope.getConfigAsText();

    });