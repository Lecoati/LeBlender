angular.module("umbraco").controller("leblender.editormanager.sort",
    function ($scope, assetsService, $http, leBlenderRequestHelper, $routeParams, navigationService, treeService) {

    $scope.save = function () {
        leBlenderRequestHelper.setGridEditors($scope.editors).then(function (response) {
            treeService.loadNodeChildren({ node: $scope.currentNode });
            navigationService.hideMenu();
        });
    };

    $scope.close = function () {
        navigationService.hideNavigation();
    };
    
    leBlenderRequestHelper.getGridEditors().then(function (response) {
        $scope.editors = response.data
    });

});