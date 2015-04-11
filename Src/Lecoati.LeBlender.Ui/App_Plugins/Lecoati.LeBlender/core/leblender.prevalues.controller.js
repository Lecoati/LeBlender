angular.module("umbraco").controller("LeBlender.Prevalues.Controller",
    function ($scope, assetsService, $http, LeBlenderRequestHelper, dialogService) {

        $scope.model.value = [];

        $scope.getSetting = function () {
            LeBlenderRequestHelper.getGridEditors().then(function (response) {
                $scope.model.value = response.data;
            })
        };

        $scope.openGridConfig = function (editor) {
            dialogService.closeAll();
            dialogService.open({
                template: '/App_Plugins/Lecoati.LeBlender/core/dialogs/editorconfig.prevalues.html',
                show: true,
                dialogData: {
                    editor: editor,
                    propertyGridEditors: $scope.propertyGridEditors
                },
                callback: function (data) {
                    if (data) {
                        $scope.model.value.splice($scope.model.value.length + 1, 0, data);
                    }
                }
            });
        }

        $scope.remove = function ($index) {
            $scope.model.value.splice($index, 1);
        }

        $scope.$on("formSubmitting", function () {
            LeBlenderRequestHelper.setGridEditors($scope.model.value);
        });

        $scope.getSetting();

        // Get a list of propertyGridEditors
        LeBlenderRequestHelper.getAllPropertyGridEditors().then(function (data) {
            $scope.propertyGridEditors = data;
        });

    });