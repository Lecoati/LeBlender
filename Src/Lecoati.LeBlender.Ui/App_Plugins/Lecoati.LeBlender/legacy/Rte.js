
angular.module("umbraco").controller("LeBlender.Editor.Rte.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

        var guid = function() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }
            return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
              s4() + '-' + s4() + s4() + s4();
        }

        // Partch: Init RTE with the grid RTE config
        $scope.rteConfig = angular.element($(".usky-grid")).scope().model.config.rte;
        $scope.guid = guid();

    });