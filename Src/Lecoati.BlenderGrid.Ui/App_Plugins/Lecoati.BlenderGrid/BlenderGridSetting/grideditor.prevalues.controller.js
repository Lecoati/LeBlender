angular.module("umbraco").controller("BlenderGrid.Prevalues.Controller",
    function ($scope, assetsService, $http, BlenderGridRequestHelper, dialogService) {

        $scope.model.value = [];

        $scope.getSetting = function () {
            BlenderGridRequestHelper.getGridEditors().then(function (response) {
                $scope.model.value = response.data;
            })
        };

        $scope.openGridConfig = function (editor) {

            if (!editor) {
                $scope.model.value.splice($scope.model.value.length + 1, 0, {
                    name: "",
                    alias: "",
                    view: "",
                    icon: "",
                    config: {}
                });
                editor = $scope.model.value[$scope.model.value.length -1];
            }

            var dialog = dialogService.open({
                template: '/App_Plugins/Lecoati.BlenderGrid/BlenderGridSetting/dialogs/editorconfig.html',
                show: true,
                dialogData: {
                    editor: editor
                },
                callback: function (data) {
                }
            });
        }

        $scope.remove = function ($index) {
            $scope.model.value.splice($index, 1);
        }

        $scope.$on("formSubmitting", function () {
            BlenderGridRequestHelper.setGridEditors($scope.model.value);
        });

        $scope.getSetting();

        // Load css asset
        // assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/BlenderGridSetting/assets/BlenderGridPrevalue.css");

    });