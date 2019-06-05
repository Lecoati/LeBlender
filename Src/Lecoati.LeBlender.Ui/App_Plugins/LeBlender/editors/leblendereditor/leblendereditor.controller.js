angular.module("umbraco").controller("LeBlenderEditor.controller",
    function ($scope, assetsService, $sce, editorService, leBlenderRequestHelper) {

        var vm = this;

        vm.openListParameter = openListParameter;

        $scope.preview = "";

        function openListParameter() {
            if ($scope.control.editor.config && $scope.control.editor.config.editors)
            {
                var dialog = {
                    view: '/App_Plugins/LeBlender/editors/leblendereditor/dialogs/parameterconfig.html',
                    dialogData: {
                        name: $scope.control.editor.name,
                        icon: $scope.control.editor.icon,
                        value: angular.copy($scope.control.value),
                        config: $scope.control.editor.config
                    },
                    submit: function (model) {
                        $scope.control.value = model.value;
                        $scope.setPreview();
                        if (!$scope.control.guid)
                            $scope.control.guid = guid();

                        editorService.close();
                    },
                    close: function (model) {
                        editorService.close();
                    }
                };

                editorService.open(dialog);
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
        };

        if ((!$scope.control.value || $scope.control.value.length === 0) &&
            ($scope.control.editor.config && $scope.control.editor.config.editors && $scope.control.editor.config.editors.length > 0)) {
            openListParameter();
        }
        else {
            if (!$scope.control.guid)
                $scope.control.guid = guid();
        }

        $scope.setPreview = function () {
            if ($scope.control.editor.config
                && ($scope.control.value || !$scope.control.editor.config.editors || $scope.control.editor.config.editors.length == 0)
                && $scope.control.editor.config.renderInGrid && $scope.control.editor.config.renderInGrid != "0") {
                leBlenderRequestHelper.GetPartialViewResultAsHtmlForEditor($scope.control).then(function (htmlResult) {
                    $scope.preview = htmlResult.data.trim();
                    $scope.allowedPreview = $sce.trustAsHtml($scope.preview);
                });
            }
        };

        $scope.setPreview();

    	// Load css asset
        assetsService.loadCss("/App_Plugins/LeBlender/views_samples/sample_styles.css");

    });