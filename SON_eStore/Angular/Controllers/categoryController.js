/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("categoryController", function ($scope, $http, ngToast) {
    //   alert(sessionStorage.getItem('accessToken'))

    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')
  
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/category/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showCategoryItems = false
    $scope.catform = false
    $scope.showNoItem = false
    $http.get(url, config).then(function successCallback(response) {
        $scope.categories = response.data;
    })

    $scope.editcat = function (input) {

        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.cat_name = response.data.category_name
            $scope.catid = response.data.id
            $scope.catform = true
            $scope.showCategoryItems = false
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.update = function () {
        var model = {
            id: $scope.catid,
            category_name: $scope.cat_name
        }

        $http.put(url, model, config)
        .then(function successCallback(response, status) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.categories = response.data;
                $scope.catform = false
                $scope.showCategoryItems = false
                myToastMsg = ngToast.info({
                    content: 'Category has been successfully updated.'
                });
            })
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Update operation fail.' + response.data
            });
        })

    }
    $scope.CreateCategory = function () {

        var model = {
            category_name: $scope.CreateCatName
        }
        $http.post(url, model, config)
        .then(function successCallback(response) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.categories = response.data;
            })
            $scope.CreateCatName = "";
            $scope.catform = false;
            $scope.showCategoryItems = false
            myToastMsg = ngToast.info({
                content: 'New category has been successfully created.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Create category operation fail. ' + response.data
            });
        })
    }
    $scope.delCategory = function (input) {
        swal({
            title: "Are you sure want to delete this category?",
            text: "Once deleted you will not be able to recover it",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
         function () {
             var model = {
                 id: input,
             }

             $http.delete(url + model.id, config)
             .then(function successCallback(response, status) {
                 swal("Deleted!", "Category item has been deleted.", "success");
                 $http.get(url, config).then(function successCallback(response) {
                     $scope.categories = response.data;
                     $scope.catform = false
                     $scope.showCategoryItems = false
                     myToastMsg = ngToast.info({
                         content: 'Category has been successfully deleted.'
                     });
                 })
             },
             function errorCallback(response) {
                 swal("Cancelled", "Category cannot be deleted: " + response.data, "error");
                 myToastMsg = ngToast.danger({
                     content: 'Delete operation fail:' + response.data
                 });
             })
         })
    }
    $scope.getCategoryItems = function (input) {
        $http.get(url + input + "/items", config).
    then(function successCallback(response) {
        if (response.data != null) {
            if (response.data.length > 0) {
                $scope.items = response.data
                $scope.item_category_name = response.data[0].category_name
                $scope.showCategoryItems = true
                $scope.catform = false
                $scope.showNoItem = false
            }
            else {
                $scope.showNoItem = true;
                $scope.noItem = "No item found for the category selected."
            }
        } else {
            $scope.showNoItem = true;
            $scope.noItem = "No item found for the category selected."
        }
      

    },
    function errorCallback(response) {
        myToastMsg = ngToast.info({
            content: 'No item found for the category selected.'
        });
    })
    }
    //// to clear a toast:
    //   ngToast.dismiss(myToastMsg);
    //var myToastMsg = ngToast.success({
    //    content: 'Login successfully'
    //});
    // clear all toasts:
    ngToast.dismiss();
    $scope.itemsByPage = 10;

})