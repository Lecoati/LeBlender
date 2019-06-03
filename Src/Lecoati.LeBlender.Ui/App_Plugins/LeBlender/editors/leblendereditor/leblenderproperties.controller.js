angular.module("umbraco").controller("leblenderproperties.controller",
	function ($scope, leBlenderRequestHelper, editorService) {

		// Init render with the value of frontView
		// render have to be always = /App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml
		$scope.model.parentValue.render = $scope.model.parentValue.config.frontView ? $scope.model.parentValue.config.frontView : "";

        $scope.openPropertyConfig = function (parameter) {

            var dialog = {
                view: '/App_Plugins/LeBlender/editors/leblendereditor/Dialogs/parameterconfig.prevalues.html',
                size: "small",
                dialogData: {
                    parameter: parameter,
                    availableDataTypes: $scope.availableDataTypes
                },
                submit: function (data) {
                    if (!$scope.model.value) {
                        $scope.model.value = [];
                    }
                    $scope.model.value.splice($scope.model.value.length + 1, 0, data);

                    editorService.close();
                },
                close: function (model) {
                    editorService.close();
                }
            };

            editorService.open(dialog);
		};

		// remove a property
        $scope.remove = function ($index) {
            $scope.model.value.splice($index, 1);
        };

		// Init again the render and frontView value
		$scope.$on('gridEditorSaving', function () {
			$scope.model.parentValue.config.frontView = $scope.model.parentValue.render;
			$scope.model.parentValue.render = "/App_Plugins/LeBlender/editors/leblendereditor/views/Base.cshtml";
		});

		// Get a list of datatype
		leBlenderRequestHelper.getAllDataTypes().then(function (data) {
			$scope.availableDataTypes = data;
		});

	});