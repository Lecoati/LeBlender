angular.module("umbraco").
    directive('draggable', function () {
        return {
            restrict: 'A',
            scope: {
                layer: '=',
                handlerClick: '&ngClick',
                handlerMouseOver: '&ngMouseover',
                handlerMouseLeave: '&ngMouseleave',
                condition: '=',
                aspectratio: '=',
                resize: '=',
                parentwidth: '=',
                parentheight: '=',
            },
            link: function (scope, element, attrs) {
                scope.$watch(function () {
                    return scope.layer;
                }, function (modelValue) {

                    var percentage = function(position) {

                        var widthPer = 100 / scope.parentwidth;
                        var heightPer = 100 / scope.parentheight;

                        scope.layer.dataX = position.left;
                        scope.layer.dataY = position.top;
                        scope.layer.dataXPer = widthPer * scope.layer.dataX;
                        scope.layer.dataYPer = heightPer * scope.layer.dataY;

                    }

                    element.draggable({
                        snap: false,
                        revert: false,
                        scroll: false,
                        cursor: "move",
                        distance: 10,
                        cancel: ".text",
                        stop: function (event, ui) {
                            percentage(ui.position);
                        }
                    })

                    if (scope.resize) {
                        element.resizable({
                            aspectRatio: scope.aspectratio,
                            stop: function (event, ui) {

                                percentage(ui.position);
                                scope.layer.width =  ui.size.width;
                                scope.layer.height = ui.size.height;
                            }
                        });
                    }

                    element.css({ 'top': scope.layer.dataY, 'left': scope.layer.dataX, 'width': scope.layer.width + "px", 'height': scope.layer.height + "px" });
                });

            }
        };
    });