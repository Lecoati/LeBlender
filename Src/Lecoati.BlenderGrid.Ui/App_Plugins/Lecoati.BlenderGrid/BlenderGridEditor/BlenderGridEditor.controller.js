angular.module("umbraco").controller("BlenderGridEditor.controller",
    function ($scope, assetsService, $http, dialogService, $routeParams, umbRequestHelper, BlenderGridRequestHelper) {

        $scope.preview = "";

        $scope.openListParameter = function () {
        	if ($scope.control.editor.config) {
        		var dialog = dialogService.open({
        			template: '/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/BlenderGridEditorParameter.html',
        			show: true,
        			dialogData: {
        				name: $scope.control.editor.name,
        				value: angular.copy($scope.control.value),
        				config: $scope.control.editor.config
        			},
        			callback: function (data) {
        				$scope.control.value = data;
        				$scope.setPreview();
        			}
        		});
        	}
        }

        if (!$scope.control.value || $scope.control.value.length == 0) {
            $scope.openListParameter();
        }

        $scope.searchEditor = function (alias) {
            var sEditor = undefined;
            _.each($scope.control.editor.config.editors, function (editor, editorIndex) {
                if (editor.alias === alias) {
                    sEditor = editor
                }
            })
            return sEditor;
        }

        $scope.updateEditor = function () {
            if ($scope.control.value) {
                _.each($scope.control.value, function (item, itemIndex) {
                    var order = 0;
                    _.each(item, function (property, propertyIndex) {
                        var editor = $scope.searchEditor(property.editorAlias);
                        if (editor) {
                            property.$editor = editor;
                            property.$order = order;
                            order++;
                        }
                    })
                })
            }
        }

        $scope.setPreview = function () {
            if ($scope.control.value) {
                BlenderGridRequestHelper.GetPartialViewResultAsHtmlForEditor($scope.control).success(function (htmlResult) {
                    $scope.preview = htmlResult;
                });
            }
        };

        $scope.updateEditor();

        $scope.setPreview();

    	// Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/assets/BlenderGridEditor.css");

    });