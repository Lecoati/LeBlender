
angular.module("umbraco").controller("LeBlender.Dialog.IconPickerEditor.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper) {

    $scope.config = $scope.dialogData.config;
    $scope.icons = [];

    assetsService.loadJs("/App_Plugins/Lecoati.LeBlender/lib/cssParser.js")
    .then(function () {

        $http.get($scope.config.cssPath)
        .success(function (data) {
            var parser = new CSSParser();
            angular.forEach(parser.parse(data, false, false).cssRules, function (nclass, key) {
                if (nclass && nclass.mSelectorText) {
                    if ($scope.config.prefix === "" || nclass.mSelectorText.indexOf($scope.config.prefix) === 0)
                    {
                        $scope.icons.push(nclass.mSelectorText.replace(/\./g, '').replace(/\:before/g, ''));
                    }
                }
            });
        })

    });
        
});