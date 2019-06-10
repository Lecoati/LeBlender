angular.module("umbraco").controller("LeBlender.Dialog.ParameterConfig.Prevalues.Controller",
    function ($scope, formHelper) {

        var vm = this;

        vm.promptIsVisible = false;

        vm.submit = submit;
        vm.close = close;

		var guidEmpty = "00000000-0000-0000-0000-000000000000";
		$scope.name = "Property settings";

        var parameter = $scope.model.dialogData.parameter;

        // Control if the property is custom 
        $scope.isCustom = function () {
            if ($scope.model.value.dataType === guidEmpty) {
                return true;
            }
            else {
                return false;
            }
        };

        // Stringify the current config
        $scope.init = function () {

            if (!$scope.model.value.propretyType) {
                $scope.model.value.propretyType = {};
            }

            if (!$scope.model.value.dataType) {
                // find "Textstring" datatype from guid and select on init - fallback to first element
                var datatypeToSelect = _.findWhere($scope.availableDataTypes, { guid: '0cc0eba1-9960-42c9-bf9b-60e150b429ae' }) || $scope.availableDataTypes[0];
                $scope.model.value.dataType = datatypeToSelect.guid;
            }

            if (!$scope.model.value.dataType && $scope.model.value.propretyType) {
                $scope.textAreaconfig = JSON.stringify($scope.model.value.propretyType.config, null, 4);
            }

            $scope.$watch('textAreaconfig', function () {
                try {
                    $scope.model.value.propretyType.config = JSON.parse($scope.textAreaconfig);
                } catch (exp) {
                    delete $scope.model.value.propretyType.config;
                }
            });

        };

        function submit() {
            if ($scope.model && $scope.model.submit && formHelper.submitForm({ scope: $scope })) {
                $scope.model.submit($scope.model);
            }
        }

        function close() {
            if ($scope.model && $scope.model.close) {
                $scope.model.close();
            }
        }

        /***************************************/
        /* autoPopulateAlias */
        /***************************************/

        // main method for autoPopulateAlias
        $scope.autoPopulateAlias = function (name) {
            var s = name.replace(/[^a-zA-Z0-9\s\.-]+/g, '');
            return s.toCamelCase();
        };

        // init autoPopulateAlias
        $scope.initAutoPopulateAlias = function () {
            if ($scope.model.value.name === "") {
                $scope.$watch("model.value.name", function () {
                    $scope.model.value.alias = $scope.autoPopulateAlias($scope.model.value.name);
                });
            }
        };

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

        // Init availableDataTypes
        $scope.availableDataTypes = angular.copy($scope.model.dialogData.availableDataTypes);
        $scope.availableDataTypes.unshift({
            guid: guidEmpty,
            name: "- - custom - -"
        });

		if (parameter)
            $scope.model.value = parameter;

        if (!$scope.model.value) {
            $scope.model.value = {
                name: "",
                alias: ""
            };
        }

        $scope.init();
        $scope.initAutoPopulateAlias();

    });