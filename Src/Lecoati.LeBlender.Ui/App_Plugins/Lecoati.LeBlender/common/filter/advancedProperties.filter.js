
angular.module("umbraco").filter('advancedProperties', function () {
    return function (items, advanced) {
        var filtered = [];
        angular.forEach(items, function (item) {
            if (item.advanced === advanced) {
                filtered.push(item);
            }
        });
        return filtered;
    };
});
