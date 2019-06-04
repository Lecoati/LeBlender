angular.module("umbraco").controller("leblender.editormanager.edit",
    function ($scope, assetsService, $http, leBlenderRequestHelper, editorService, $routeParams, notificationsService, navigationService, contentEditingHelper, editorState) {

        var vm = this;

        var create = $routeParams.create;
        //var infiniteMode = $scope.model && $scope.model.infiniteMode;

        vm.save = save;
        vm.close = close;

        vm.content = {
            name: "",
            alias: "",
            description: "",
            icon: "icon-settings-alt"
        };

        vm.labels = {};
        vm.submitButtonKey = "buttons_save";

        vm.page = {};
        vm.page.loading = false;
        vm.page.saveButtonState = "init";
        vm.page.navigation = [];


        /***************************************/
        /* Init editor data */
        /***************************************/

        $scope.getSetting = function (editorAlias) {

            vm.page.loading = true;

            leBlenderRequestHelper.getGridEditors().then(function (response) {

                // init model
                $scope.editors = response.data;

                // Init model value
                $scope.model = {
                    value: {
                        name: "",
                        alias: "",
                        view: "",
                        icon: "icon-settings-alt"
                    }
                };

                if (create) {
                    $scope.editors.push($scope.model.value);
                }
                else {
                    _.each($scope.editors, function (editor, editorIndex) {
                        if (editor.alias === editorAlias) {
							$scope.model.value = editor;
                            navigationService.syncTree({ tree: "GridEditorManager", path: [$scope.model.value.alias], forceReload: false });
                        }
                    });
                }

                $scope.getConfigAsText();
                $scope.setSelectedPropertyGridEditor();

                $scope.loaded = true;
                vm.page.loading = false;

                $scope.$broadcast('gridEditorLoaded');
            });
        };


        /***************************************/
        /* grid editor */
        /***************************************/

        // init editor values
        $scope.initEditorFields = function () {
            delete $scope.model.value.config;
            $scope.model.value.render = "";
            $scope.textAreaconfig = "";
        };

        function close() {
            if ($scope.model.close) {
                $scope.model.close($scope.model);
            }
        }

        // save editor values
        function save() {
            
            if ($scope.model.value) {
                $scope.$broadcast('gridEditorSaving');
            }

            _.each($scope.editors, function (editor, editorIndex) {
                if (editor.render === "") {
                    delete editor.render;
                }
            });

            leBlenderRequestHelper.setGridEditors($scope.editors).then(function (response) {
                notificationsService.success("Success", $scope.model.value.name + " has been saved");
                delete $scope.selectedPropertyGridEditor;
                $scope.getSetting($scope.model.value.alias);
                var editormanagerForm = angular.element('form[name=editormanagerForm]').scope().editormanagerForm;
                if ($scope.model.value) {
                    $scope.$broadcast('gridEditorSaved');
                    editormanagerForm.$dirty = false;
                }

                if (create) {         
                    editormanagerForm.$dirty = false;
                    contentEditingHelper.redirectToCreatedContent($scope.model.value.alias, true);
                }

            });

        }

        // get config value 
        $scope.getConfigAsText = function () {

            $scope.textAreaconfig = "";

            if ($scope.model.value.config) {

                var config = JSON.stringify($scope.model.value.config, null, 4);

                if (config && config != {}) {
                    $scope.textAreaconfig = config;
                }
                else {
                    $scope.textAreaconfig = "";
                }
            }
        };

        /***************************************/
        /* property grid editor */
        /***************************************/

        // search a pge by view
        $scope.searchPropertyGridEditor = function (view) {
            var sEditor = undefined;
            _.each($scope.propertyGridEditors, function (propertyGridEditor, editorIndex) {
                if (propertyGridEditor.editor && propertyGridEditor.editor.view === view) {
                    sEditor = propertyGridEditor;
                }
            });
            return sEditor;
        };

        // set the selected pge
        $scope.setSelectedPropertyGridEditor = function () {
            $scope.selectedPropertyGridEditor = $scope.searchPropertyGridEditor($scope.model.value.view);
        };

        // init default Editor value for a new pge
        $scope.propertyGridEditorChanged = function () {
            $scope.setSelectedPropertyGridEditor();
            $scope.initEditorFields();
        };

        // get pge field view
        $scope.getFieldView = function (view) {
            if (view.indexOf('/') >= 0) {
                return view;
            }
            else {
                return '/umbraco/views/prevalueeditors/' + view + '.html';
            }
        };

        // check if current pge is custom 
        $scope.isCustom = function () {
            if ($scope.selectedPropertyGridEditor) {
                return false;
            }
            else {
                return true;
            }
        };
        
        $scope.configChanged = function (textAreaconfig) {
            try {
                var configValue = JSON.parse(textAreaconfig);
                $scope.model.value.config = configValue;
            }
            catch (e)
            {
                console.error("Could not parse json", e);
            }
        };

        /***************************************/
        /* init */
        /***************************************/

        // Init
        $scope.loaded = false;

        leBlenderRequestHelper.getAllPropertyGridEditors().then(function (data) {
            $scope.propertyGridEditors = data;
            $scope.getSetting($routeParams.id);
        });
    });