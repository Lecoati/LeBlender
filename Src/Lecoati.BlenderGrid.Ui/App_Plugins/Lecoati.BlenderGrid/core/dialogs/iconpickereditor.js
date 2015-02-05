
angular.module("umbraco").controller("BlenderGrid.Dialog.IconPickerEditor.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

    $scope.config = $scope.dialogData.config;
    $scope.icons = [];

    assetsService.loadJs("/App_Plugins/Lecoati.BlenderGrid/lib/cssParser.js")
    .then(function () {

        $http.get($scope.config.cssPath)
        .success(function (data) {
            var parser = new CSSParser();
            angular.forEach(parser.parse(data, false, false).cssRules, function (nclass, key) {
                if (nclass && nclass.mSelectorText) {
                    if ($scope.config.prefixe === "" || nclass.mSelectorText.indexOf($scope.config.prefixe) === 0)
                    {
                        $scope.icons.push(nclass.mSelectorText.replace(/\./g, '').replace(/\:before/g, ''));
                    }
                }
            });
        })

    });
        
});