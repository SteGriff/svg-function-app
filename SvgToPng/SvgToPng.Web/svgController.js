var myApp = angular.module('myApp',[]);

myApp.controller('MainController', ['$scope', '$http', function($scope, $http) {

	$scope.rootUrl = "http://localhost:7071/api/";
	
	$scope.svgContent = "";
	$scope.response = {};
	
	$scope.sendSvg = function(svgContent){
		$http({
			method: 'POST',
			url: $scope.rootUrl + 'Convert',
            data: svgContent,
            headers: {
                'Content-Type': "text/plain"
            },
        }).then($scope.showResponse, $scope.showResponse);
	}
	
	$scope.showResponse = function(response)
	{
		console.log(response);
		$scope.response = response;
	}
	
}]);