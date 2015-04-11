angular.module("umbraco").controller("LeBlender.Dialog.EditorConfig.Prevalues.Controller",
    function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        /***************************************/
        /* legacy adaptor 0.9.15 */
        /***************************************/

        if ($scope.dialogData.editor) {

            if ($scope.dialogData.editor.view == "/App_Plugins/Lecoati.LeBlender/core/LeBlendereditor.html") {
                $scope.dialogData.editor.view = "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html";
                $scope.dialogData.editor.render = "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/views/Base.cshtml"
            }

            if ($scope.dialogData.editor.view == "/App_Plugins/Lecoati.LeBlender/editors/leblendereditor/LeBlendereditor.html") {

                if ($scope.dialogData.editor.frontView) {
                    if (!$scope.dialogData.editor.config) {
                        $scope.dialogData.editor.config = {};
                    }
                    $scope.dialogData.editor.config.frontView = $scope.dialogData.editor.frontView;
                    delete $scope.dialogData.editor.frontView;
                }

                if ($scope.dialogData.editor.config) {

                    if ($scope.dialogData.editor.config.renderInGrid == true) {
                        $scope.dialogData.editor.config.renderInGrid = "1";
                    }

                    if ($scope.dialogData.editor.config.renderInGrid == false) {
                        $scope.dialogData.editor.config.renderInGrid = "0";
                    }

                    if ($scope.dialogData.editor.config.fixed != undefined &&
                        $scope.dialogData.editor.config.limit &&
                        !$scope.dialogData.editor.config.min &&
                        !$scope.dialogData.editor.config.max) {
                        if ($scope.dialogData.editor.config.fixed) {
                            $scope.dialogData.editor.config.min = $scope.dialogData.editor.config.limit;
                            $scope.dialogData.editor.config.max = $scope.dialogData.editor.config.limit;
                        }
                        else {
                            $scope.dialogData.editor.config.min = 1;
                            $scope.dialogData.editor.config.max = $scope.dialogData.editor.config.limit;
                        }
                        delete $scope.dialogData.editor.config.fixed;
                        delete $scope.dialogData.editor.config.limit;
                    }

                }
            }
        }

        /***************************************/
        /* grid editor */
        /***************************************/

        // init editor values
        $scope.initEditorFields = function () {
            delete $scope.model.value.config;
            $scope.model.value.render = "";
            $scope.textAreaconfig = "";
        }

        // save editor values
        $scope.save = function () {

            var submitPlease = true;
            if ($scope.model.value) {
                $scope.$broadcast('editorConfigSaving');
                if ($scope.dialogData.editor) {
                    angular.extend($scope.dialogData.editor, $scope.model.value);
                    submitPlease = false;
                }
            }

            if (submitPlease) {
                $scope.submit($scope.model.value);
            }
            else {
                $scope.close();
            }

        }

        // get config value 
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

        // open icon picker
        $scope.openIconPicker = function () {
            var dialog = dialogService.iconPicker({
                show: true,
                callback: function (data) {
                    $scope.model.value.icon = data;
                }
            });
        }



        /***************************************/
        /* property grid editor */
        /***************************************/

        // init pge
        $scope.propertyGridEditors = $scope.dialogData.propertyGridEditors;

        // search a pge by view
        $scope.searchPropertyGridEditor = function (view) {
            var sEditor = undefined;
            _.each($scope.propertyGridEditors, function (propertyGridEditor, editorIndex) {
                if (propertyGridEditor.editor && propertyGridEditor.editor.view === view) {
                    sEditor = propertyGridEditor
                }
            })
            return sEditor;
        }

        // set the selected pge
        $scope.setSelectedPropertyGridEditor = function () {
            $scope.selectedPropertyGridEditor = $scope.searchPropertyGridEditor($scope.model.value.view);
        }

        // init default Editor value for a new pge
        $scope.propertyGridEditorChanged = function () {
            $scope.setSelectedPropertyGridEditor();
            $scope.initEditorFields();
        }

        // get pge field view
        $scope.getFieldView = function (view) {
            if (view.indexOf('/') >= 0) {
                return view;
            }
            else {
                return '/umbraco/views/prevalueeditors/' + view + '.html';
            }
        }

        // check if current pge is custom 
        $scope.isCustom = function () {
            if ($scope.selectedPropertyGridEditor) {
                return false;
            }
            else {
                return true;
            }
        }



        /***************************************/
        /* autoPopulateAlias */
        /***************************************/

        // main method for autoPopulateAlias
        $scope.autoPopulateAlias = function (name) {
            var s = name.replace(/[^a-zA-Z0-9\s\.-]+/g, ''); 
            return s.toCamelCase();
        }

        // init autoPopulateAlias
        $scope.initAutoPopulateAlias = function () {
            if ($scope.model.value.name === "" && $scope.model.value.name === "") {
                $scope.$watch("model.value.name", function () {
                    $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
                })
            }
        }

        // toCamelCase
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

        // toPascalCase
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



        /***************************************/
        /* init */
        /***************************************/

        angular.extend($scope, {
            model: {
                value: angular.copy($scope.dialogData.editor)
            }    
        });

        if (!$scope.model.value) {
            $scope.model.value = {
                name: "",
                alias: "",
                view: "",
                icon: "",
            };
        }

        $scope.getConfigAsText();
        $scope.setSelectedPropertyGridEditor();
        $scope.initAutoPopulateAlias();

    });