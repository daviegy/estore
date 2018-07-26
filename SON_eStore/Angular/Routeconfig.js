app.config(function ($routeProvider, $locationProvider) {
    $routeProvider
    .when("/Category",
        {
            templateUrl: "View/Category/CategoryList",
            controller: 'categoryController'

        })
    
    $locationProvider.html5Mode(true);
})