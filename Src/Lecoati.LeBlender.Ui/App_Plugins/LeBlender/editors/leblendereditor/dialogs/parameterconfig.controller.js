angular.module("umbraco").controller("LeBlender.Dialog.Parameterconfig.Controller",
    function ($scope, assetsService, $timeout, leBlenderRequestHelper, umbPropEditorHelper) {

        var vm = this;

        vm.submit = submit;
        vm.close = close;

        vm.select = select;
        vm.remove = remove;
        vm.add = add;

        vm.showPrompt = showPrompt;
        vm.hidePrompt = hidePrompt;

        var dialogData = $scope.model.dialogData;

        angular.extend($scope, {
            name: dialogData.name,
            config:{
                min: 1,
                max: 1,
                editors: []
            }     
        });

        if (!$scope.model.value)
            $scope.model.value = [];

        angular.extend($scope.config,
            dialogData.config);

        angular.extend($scope.model.value,
            dialogData.value);

        if ($scope.model.value.length > 0) {
            $scope.selected = $scope.model.value[0];
        }

        if (!$scope.config.min)
            $scope.config.min = 1;

        if (!$scope.config.max)
			$scope.config.max = 1;

		$scope.icon = dialogData.icon;

        function select(item, index) {
            item.propertiesOpen = !item.propertiesOpen;
            $scope.selected = index;
        }

        function remove(item, event, index) {

            if (item === $scope.selected) {
                if (index === 0) {
                    $scope.selected = $scope.model.value[1];
                }
                else if (index >= 0) {
                    $scope.selected = $scope.model.value[index - 1];
                }
            }
            $scope.model.value.splice(index, 1);

            item.deletePrompt = false;
            event.stopPropagation();
        }

        function add() {
            var newItem = {};
            _.each($scope.config.editors, function (editor, editorIndex) {
                var newProperty = {
                    value: null,
                    dataTypeGuid: editor.dataType,
                    editorAlias: editor.alias,
                    editorName: editor.name,
                    $editor: editor,
                    $order: editorIndex,
                    $valid: false
                };
                newItem[editor.alias] = newProperty;
            });
    		$scope.model.value.splice($scope.model.value.length + 1, 0, newItem);
    		$scope.selected = $scope.model.value[$scope.model.value.length - 1];
    	}

        $scope.sortableOptions = {
            handle: ".handle",
            axis: "y",
            delay: 150,
            distance: 5,
            stop: function (e, ui) {
    	        ui.item.parents("#blender-grid-editor-parameter").find('.mceNoEditor').each(function () {
    		        tinyMCE.execCommand('mceRemoveEditor', false, $(this).attr('id'));
    		        tinyMCE.execCommand('mceAddEditor', false, $(this).attr('id'));
    	        });
            }
        };

        $scope.searchEditor = function (alias) {
            var sEditor = undefined;
            if ($scope.config.editors) {
                _.each($scope.config.editors, function (editor, editorIndex) {
                    if (editor.alias === alias) {
                        sEditor = editor;
                    }
                });
            }
            return sEditor;
        };

        $scope.searchPropertyItem = function (item, alias) {
            var sProperty = undefined;
            _.each(item, function (property, propertyIndex) {
                if (property.editorAlias === alias) {
                    sProperty = property;
                }
            });
            return sProperty;
        };

        var initEditor = function () {

            _.each($scope.model.value, function (item, itemIndex) {
                var order = 0;
                if ($scope.config.editors) {
                    _.each($scope.config.editors, function (editor, editorIndex) {
                        var property = $scope.searchPropertyItem(item, editor.alias);
                        if (property) {
                            property.$editor = editor;
                            property.$order = order;
                            if (!property.dataTypeGuid)
                                property.dataTypeGuid = editor.dataType;
                        }
                        else {
                            var newProperty = {
                                value: null,
                                dataTypeGuid: editor.dataType,
                                editorAlias: editor.alias,
                                editorName: editor.name,
                                $editor: editor,
                                $order: order,
                                $valid: false
                            };
                            item[editor.alias] = newProperty;
                        }
                        order++;
                    });
                }
                _.each(item, function (property, propertyIndex) {
                    if (!$scope.searchEditor(property.editorAlias)) {
                        delete item[property.editorAlias];
                    }
                });
            });
        };

        $scope.updateEditor = function () {
            if ($scope.model.value) {

                /***************************************/
                /* load dataType Info */
                /***************************************/
                var watchAppStart = $scope.$watch(function () {
                    var isLoadedCounter = 0;
                    _.each($scope.config.editors, function (editor, editorIndex) {
                        if (editor.$isLoaded) {
                            isLoadedCounter++;
                        }
                    });
                    return isLoadedCounter;
                }, function (newValue, oldValue) {
                    if (newValue === $scope.config.editors.length) {
                        initEditor();
                        watchAppStart();
                        $scope.configLoaded = true;
                    }
                }, true);
                /***************************************/

                /***************************************/
                /* load dataType Info */
                /***************************************/

                if ($scope.config.editors) {
                    _.each($scope.config.editors, function (editor, editorIndex) {

                        if (!$scope.model.value.propretyType) {
                            $scope.model.value.propretyType = {};
                        }

                        if (editor.dataType && !editor.$isLoaded) {
                            leBlenderRequestHelper.getDataType(editor.dataType).then(function (data) {

                                // Get config prevalues
                                var configObj = {};
                                _.each(data.preValues, function (p) {
                                    configObj[p.key] = p.value;
                                });

                                // Get config default prevalues
                                var defaultConfigObj = {};
                                if (data.defaultPreValues) {
                                    _.extend(defaultConfigObj, data.defaultPreValues);
                                }

                                // Merge prevalue and default prevalues
                                var mergedConfig = _.extend(defaultConfigObj, configObj);

                                editor.$isLoaded = true;
                                editor.propretyType.config = mergedConfig;
                                editor.propretyType.view = umbPropEditorHelper.getViewPath(data.view);
                            });
                        }
                        else {
                            editor.$isLoaded = true;
                        }
                    });
                }
                /***************************************/

            }

        };

        $scope.updateTemplate = function () {

            // Clean for fixed config
            if ($scope.model.value.length < $scope.config.min) {
                while ($scope.model.value.length < $scope.config.min) {
                    vm.add();
                }
            }
            if ($scope.model.value.length > $scope.config.max) {
                while ($scope.model.value.length > $scope.config.max) {
                    vm.remove($scope.model.value.length - 1);
                }
            }
            if ($scope.config.max == $scope.config.min) {
                $scope.fixed = true;
            }

        };

    	$scope.updateTemplate();

    	$scope.updateEditor();

    	$scope.isValid = function () {
			var isValid = true;
			
    		_.every($scope.model.value, function (item, itemIndex) {
    	         _.forEach(item, function (property, propertyIndex) {
    	            if (!property.$valid) {
						isValid = false;
    					return;
    	            }
    	        });
    	    });
    		
    		return isValid;
    	};
		
        function submit() {

            $scope.$broadcast("formSubmitting");
			
            if($scope.isValid()) {
                $timeout(function () {
                    if ($scope.model.submit) {
                        $scope.model.submit($scope.model);
                    }
                }, 250);	
            }

            //if ($scope.model.submit) {
            //    $scope.model.submit($scope.model);
            //}
    	}

        function close() {
            if ($scope.model.close) {
                $scope.model.close();
            }
        }

        function showPrompt(item, event) {
            item.deletePrompt = true;
            event.stopPropagation();
        }

        function hidePrompt(item, event) {
            item.deletePrompt = false;
            event.stopPropagation();
        }

        // Load css asset
        assetsService.loadCss("/App_Plugins/LeBlender/editors/leblendereditor/assets/parameterconfig.css");

    });
