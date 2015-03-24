angular.module("umbraco").controller("LeBlenderEditor.controller",
    function ($scope, assetsService, $http, dialogService, $routeParams, umbRequestHelper, LeBlenderRequestHelper) {

        $scope.preview = "";

        $scope.openListParameter = function () {
            if ($scope.control.editor.config && $scope.control.editor.config.editors ) {
        		var dialog = dialogService.open({
        		    template: '/App_Plugins/Lecoati.LeBlender/core/dialogs/parameterconfig.html',
        			show: true,
        			dialogData: {
        				name: $scope.control.editor.name,
        				value: angular.copy($scope.control.value),
        				config: $scope.control.editor.config
        			},
        			callback: function (data) {
        				$scope.control.value = data;
        				$scope.setPreview();
        				if (!$scope.control.guid)
        				    $scope.control.guid = guid()
        			}
        		});
        	}
        }

        var guid = function () {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

        if (!$scope.control.value || $scope.control.value.length == 0) {
            $scope.openListParameter();
        }
        else {
            if (!$scope.control.guid)
                $scope.control.guid = guid()
        }

        $scope.setPreview = function () {
            if ($scope.control.editor.config
                && ($scope.control.value || !$scope.control.editor.config.editors)
                && $scope.control.editor.config.renderInGrid) {
                LeBlenderRequestHelper.GetPartialViewResultAsHtmlForEditor($scope.control).success(function (htmlResult) {
                    $scope.preview = htmlResult;
                });
            }
        };

        $scope.setPreview();

    	// Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.LeBlender/core/assets/LeBlendereditor.css");

    });