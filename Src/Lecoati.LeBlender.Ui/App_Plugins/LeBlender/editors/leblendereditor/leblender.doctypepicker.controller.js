angular.module('umbraco').controller('leblender.doctypepicker.controller', [
        '$scope',
        'Umbraco.PropertyEditors.NestedContent.Resources',
        'overlayService',
        'localizationService',
        'iconHelper',
        'editorService',
        function ($scope, ncResources, overlayService, localizationService, iconHelper,editorService) {
            var selectElementTypeModalTitle = '';            
            init();
            $scope.currentType = null;
            function init() {
                localizationService.localize('content_nestedContentSelectElementTypeModalTitle').then(function (value) {
                    selectElementTypeModalTitle = value;
                });
                ncResources.getContentTypes().then(function (elemTypes) {
                    $scope.model.elemTypes = elemTypes;
                    // convert legacy icons
                    iconHelper.formatContentTypeIcons($scope.model.elemTypes);
                    $scope.currentType = placeholder();
                });
            }


            function placeholder() {
                return _.find($scope.model.elemTypes, function (elType) {
                    return elType.guid === $scope.model.value;
                });
            }

            $scope.openDocType = function (event) {
                event.preventDefault();
                var editor = {
                    id: $scope.currentType.id,
                    submit: function submit(model) {
                        editorService.close();
                    },
                    close: function close() {
                        editorService.close();
                    }
                };
                editorService.documentTypeEditor(editor);
            };

            $scope.openElemTypeModal = function (event) {
                event.preventDefault();
                var selectedItems = [];
                if ($scope.currentType)
                    selectedItems.push($scope.currentType);
                var elemTypeSelectorOverlay = {
                    view: 'itempicker',
                    title: selectElementTypeModalTitle,
                    availableItems: $scope.model.elemTypes,
                    selectedItems: selectedItems,
                    position: 'target',
                    event: event,
                    submit: function submit(model) {
                        $scope.model.value = model.selectedItem.guid;
                        $scope.currentType = model.selectedItem;
                        overlayService.close();
                    },
                    close: function close() {
                        overlayService.close();
                    }
                };
                overlayService.open(elemTypeSelectorOverlay);
            };
        }
    ]);
