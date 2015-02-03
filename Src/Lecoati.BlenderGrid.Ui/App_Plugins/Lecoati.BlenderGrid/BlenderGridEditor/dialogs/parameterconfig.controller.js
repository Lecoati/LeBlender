angular.module("umbraco").controller("Blendergrid.Parameterconfig.Controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper, $timeout) {

        angular.extend($scope, {
            name: $scope.dialogData.name,
            model: {
                value: []
            },
            config:{
                limit: 999,
                fixed: false,
                editors: []
            }     
        });

        angular.extend($scope.config,
            $scope.dialogData.config);

        angular.extend($scope.model.value,
            $scope.dialogData.value);

        if ($scope.model.value.length > 0) {
            $scope.selected = $scope.model.value[0];
        }

        $scope.select = function (index) {
            $scope.selected = index;
        };

        $scope.remove = function ($index) {
            if ($index > 0) {
                $scope.selected = $scope.model.value[$index - 1];
            }
        	$scope.model.value.splice($index, 1);
        };

        $scope.initEditorPath = function (property) {
            if (property.$editor) {
                if (property.$editor.view && _.indexOf(property.$editor.view, "/") >= 0) {
                    return property.$editor.view;
                } else {
                    return "/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/editors/" + property.$editor.view + ".html";
                }
            }
        };

        $scope.add = function () {
            newItem = {};
            _.each($scope.config.editors, function (editor, editorIndex) {
    	        var newProperty = {
    	            value: null,
    	            editorAlias: editor.alias,
    	            editorName: editor.name,
    	            $editor: editor,
    	            $order: editorIndex
    	        };
    	        newItem[editor.alias] = newProperty
    	    })
    		$scope.model.value.splice($scope.model.value.length + 1, 0, newItem);
    		$scope.selected = $scope.model.value[$scope.model.value.length - 1];
    	};

    	$scope.sortableOptions = {
    		handle: ".icon-navigation",
    		axis: "y",
    		delay: 150,
    		distance: 5
    	};

    	$scope.updateTemplate = function () {
    	    if ($scope.config.fixed) {
    	        while ($scope.model.value.length < $scope.config.limit) {
    	            $scope.add();
    	        }
    	        while ($scope.model.value.length > $scope.config.limit) {
    	            $scope.remove($scope.model.value.length -1);
    	        }
    	    }
    	}

    	$scope.updateTemplate();

    	// Load css asset
    	assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/BlenderGridEditor/assets/parameterconfig.css");

    });