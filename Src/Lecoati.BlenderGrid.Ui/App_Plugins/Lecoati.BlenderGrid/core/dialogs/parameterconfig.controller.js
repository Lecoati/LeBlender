angular.module("umbraco").controller("BlenderGrid.Dialog.Parameterconfig.Controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper, $timeout) {

        angular.extend($scope, {
            name: $scope.dialogData.name,
            model: {
                value: []
            },
            config:{
                limit: 1,
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

        $scope.remove = function (item, $index, $event) {

            if (item === $scope.selected) {
                if ($index === 0) {
                    $scope.selected = $scope.model.value[1];
                }
                else if ($index >= 0) {
                    $scope.selected = $scope.model.value[$index - 1];
                }
            }
            $scope.model.value.splice($index, 1);

        };

        $scope.initEditorPath = function (property) {
            if (property && property.$editor && property.$editor.propretyType) {
                if (property.$editor.propretyType.view && _.indexOf(property.$editor.propretyType.view, "/") >= 0) {
                    return property.$editor.propretyType.view;
                } else {
                    return "/App_Plugins/Lecoati.BlenderGrid/core/editors/" + property.$editor.propretyType.view + ".html";
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

    	$scope.searchEditor = function (alias) {
    	    var sEditor = undefined;
    	    if ($scope.config.editors) {
    	        _.each($scope.config.editors, function (editor, editorIndex) {
    	            if (editor.alias === alias) {
    	                sEditor = editor
    	            }
    	        })
    	    }
    	    return sEditor;
    	}

    	$scope.searchPropertyItem = function (item, alias) {
    	    var sProperty = undefined;
    	    _.each(item, function (property, propertyIndex) {
    	        if (property.editorAlias === alias) {
    	            sProperty = property
    	        }
    	    })
    	    return sProperty;
    	}

    	$scope.updateEditor = function () {
    	    if ($scope.model.value) {
    	        _.each($scope.model.value, function (item, itemIndex) {
    	            var order = 0;

    	            if ($scope.config.editors) {
    	                _.each($scope.config.editors, function (editor, editorIndex) {
    	                    var property = $scope.searchPropertyItem(item, editor.alias);
    	                    if (property) {
    	                        property.$editor = editor;
    	                        property.$order = order;
    	                    }
    	                    else {
    	                        var newProperty = {
    	                            value: null,
    	                            editorAlias: editor.alias,
    	                            editorName: editor.name,
    	                            $editor: editor,
    	                            $order: order
    	                        };
    	                        item[editor.alias] = newProperty;
    	                    }
    	                    order++;
    	                })
    	            }

    	            _.each(item, function (property, propertyIndex) {
    	                if (!$scope.searchEditor(property.editorAlias)) {
    	                    item[property.editorAlias] = undefined;
    	                }
    	            })

    	        })
    	    }
    	}

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

    	$scope.updateEditor();

    	// Load css asset
    	assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/core/assets/parameterconfig.css");

    });