angular.module("umbraco").controller("BlenderGrid",
    function ($scope, assetsService, $http, $timeout) {

        // Here we can apply settings and styles on the current grid row element
        var updateRowSettingStyle = function (row, rowElement) {

            /* Native Grid Styles Config */

            if (rowElement.data("initStyles")) {
                angular.forEach(rowElement.data("initStyles").split(','), function (style, styleIndex) {
                    rowElement.css(style, "");
                });
            }

            var styles = [];
            angular.forEach(row.styles, function (style, styleIndex) {
                rowElement.css(styleIndex, style);
                styles.push(styleIndex);
            });

            rowElement.data("initStyles", styles.join(','));

            /* Enable / Disable row option */

            if (row.config && row.config.disable && row.config.disable == '1') {
                rowElement.addClass("disable");
            }
            else {
                rowElement.removeClass("disable");
            }

            /* Background classes view: BackgroundStyle.html */

            if (row.styles && row.styles.styling) {
                if (!rowElement.data("originalClasses")) {
                    rowElement.data("originalClasses", rowElement.attr("class"));
                }
                rowElement.attr("class", rowElement.data("originalClasses"))
                rowElement.addClass(row.styles.styling);
            }

            /* Full-size option */

            if (row.config && row.config.fullWidth && row.config.fullWidth == '1') {
                rowElement.addClass("full-size");
            }
            else {
                rowElement.removeClass("full-size");
            }

        }

        // Here we can apply settings and styles on the current grid area element
        var updateAreaSettingStyle = function (area, areaElement) {
        }

        // Watch the grid's model
        var watchGridSettings = function () {
            $timeout(function () {
                $scope.$watch(function () {
                    return angular.element($(".usky-grid")).scope().model.value;
                },
                    function (newValue, oldValue) {
                        if (newValue) {
                            if (newValue.sections) {
                                angular.forEach(newValue.sections, function (section, sectionIndex) {
                                    angular.forEach(section.rows, function (row, rowIndex) {
                                        updateRowSettingStyle(row, $(".usky-grid .usky-column:eq(" + sectionIndex + ") .usky-row:eq(" + rowIndex + ")"))
                                        angular.forEach(row.areas, function (area, areaIndex) {
                                            updateAreaSettingStyle(area, $(".usky-grid .usky-column:eq(" + sectionIndex + ") .usky-row:eq(" + rowIndex + ") .mainTd.usky-cell:eq(" + areaIndex + ")"));
                                        })
                                    });
                                });
                            }
                        }
                    }
                 , true);
            }, 500);
        }

        // Needed after save&published to start the grid's model watching again
        var unsubscribe = $scope.$on("formSubmitted", function () {
            watchGridSettings();
        });

        watchGridSettings();

        // Style needed to improve the grid user experience 
        if ($scope.model.config && $scope.model.config.cssBackendPath && $scope.model.config.cssBackendPath != "") {
            assetsService.loadCss($scope.model.config.cssBackendPath);
        }

    });