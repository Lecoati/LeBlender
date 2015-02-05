angular.module("umbraco").controller("BlenderGrid.Dialog.LayersEditor.controller",
    function ($scope, assetsService, $http, dialogService, mediaHelper, imageHelper, $timeout) {

        // Partch: Init RTE with the grid RTE config
        $scope.rteConfig = angular.element($(".usky-grid")).scope().model.config.rte;

        /* oups, just need some more space :) */
        $('.umb-modal.fade.in').attr('style', 'width: 1100px !important; margin-left: -1100px');

        angular.extend($scope, {
            model: {
                value: []
            },
            slide: {
                image: "",
                color: ""
            }
        });

        angular.extend($scope.model, $scope.dialogData);

        if (!$scope.model.value) {
            $scope.model.value = [];
        }

        $scope.cancel = function () {
            $('.umb-modal.fade.in').attr('style', '');
            $scope.close();
        }

        $scope.save = function () {
            $('.umb-modal.fade.in').attr('style', '');
            $scope.submit($scope.model.value);
        }

        $scope.addTextLayer = function () {

            $scope.model.value.splice($scope.model.value.length + 1, 0, {
                name: "Text " + ($scope.model.value.length + 1),
                content: "<p>Lorem lipsum</p>",
                src: "",
                type: "text",
                icon: "icon-font",
                dataX: "0",
                dataY: "0",
                width: "",
                height: "",
                horizontalCentered: false
            });

            $scope.editLayer($scope.model.value[$scope.model.value.length - 1]);

        }

        $scope.updatePictureLayer = function (layer) {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    data = [data];
                    _.each(data, function (media, i) {
                        media.thumbnail = imageHelper.getThumbnailFromPath(media.image);
                        layer.src = media.image;
                    });
                }
            });
        }

        $scope.addPictureLayer = function () {
            dialogService.mediaPicker({
                multiPicker: false,
                callback: function (data) {
                    data = [data];
                    _.each(data, function (media, i) {
                        media.thumbnail = imageHelper.getThumbnailFromPath(media.image);
                        $scope.model.value.splice($scope.model.value.length + 1, 0, {
                            name: "Image " + ($scope.model.value.length + 1),
                            content: "",
                            src: media.image,
                            type: "image",
                            icon: "icon-picture",
                            dataX: "0",
                            dataY: "0",
                            width: 200,
                            height: "",
                            horizontalCentered: false
                        });
                        $scope.editLayer($scope.model.value[$scope.model.value.length - 1]);
                    });
                }
            });

        }

        $scope.initTinyMceContent = function (content) {
            if (tinyMCE) {
                tinyMCE.get('layerTinyMce').setContent(content);
            }
        }

        $scope.setLayerStyle = function (layer) {
            if (layer) {
                return {
                    'top': layer.dataY,
                    'left': layer.dataX,
                    'width': layer.width + 'px',
                    'height': layer.height + 'px'
                }
            }
        };

        $scope.setSlideStyle = function () {
            return {
                'background-image': 'url(' + $scope.model.slide.image + ')',
                'background-color': $scope.model.slide.color
            }
        };

        $scope.editLayer = function (layer) {
            if (layer.type == "text") {
                $scope.initTinyMceContent(layer.content);
            }
            $scope.currentLayer = layer;
        }

        $scope.setOverLayer = function (layer) {
            $scope.overLayer = layer;
        }

        $scope.initOverLayer = function (layer) {
            $scope.overLayer = undefined;
        }

        $scope.removeLayer = function (index) {
            $scope.model.value.splice(index, 1);
            $scope.currentLayer = undefined;
        };

        $timeout(function () {
            if ($scope.model.value.length > 0) {
                $scope.editLayer($scope.model.value[0]);
                $scope.$apply();
            }
        }, 800, false);

        // Load css asset
        assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/core/assets/layerseditor.css");
        assetsService.loadCss("/App_Plugins/Lecoati.BlenderGrid/lib/jquery-ui-1.10.4.custom/css/ui-lightness/jquery-ui-1.10.4.custom.min.css");

    });