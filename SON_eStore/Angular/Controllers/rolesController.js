/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("rolesController", function ($scope, $http, ngToast) {
    //   alert(sessionStorage.getItem('accessToken'))

    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')

    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/Roles/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }    
    $scope.editRoleFormss = false
    $scope.hideRoleID = true
    $http.get(url, config).then(function successCallback(response) {
        $scope.roles = response.data;
    })

    $scope.editRole = function (input) {

        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.r_name = response.data.name
            $scope.r_id = response.data.id
            $scope.editRoleForms = true
           
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.update = function () {
        var model = {
            id: $scope.r_id,
            Name: $scope.r_name
        }
        $http.put(url, model, config)
        .then(function successCallback(response, status) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.roles = response.data;
                $scope.editRoleForms = false              
                myToastMsg = ngToast.info({
                    content: 'Role has been successfully updated.'
                });
            })
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Update operation fail.' + response.data
            });
        })

    }
    $scope.addRole = function () {
        var model = {
            Name: $scope.RoleName
        }
        $http.post(url, model, config)
        .then(function successCallback(response) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.roles = response.data;
            })
            $scope.RoleName = "";
            $scope.editRoleForms = false;          
            myToastMsg = ngToast.info({
                content: 'New role has been successfully created.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Create role operation fail. ' + response.data
            });
        })
    }
    $scope.delRole = function (input) {
        swal({
            title: "Are you sure want to delete this role?",
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
                 swal("Deleted!", "role item has been deleted.", "success");
                 $http.get(url, config).then(function successCallback(response) {
                     $scope.roles = response.data;
                     $scope.editRoleForms = false
                     myToastMsg = ngToast.info({
                         content: 'role has been successfully deleted.'
                     });
                 })
             },
             function errorCallback(response) {
                 swal("Cancelled", "role cannot be deleted: " + response.data, "error");
                 myToastMsg = ngToast.danger({
                     content: 'Delete operation fail:' + response.data
                 });
             })
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