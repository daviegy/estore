/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("unitController", function ($scope, $http, ngToast) {
    //   alert(sessionStorage.getItem('accessToken'))

    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')

    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/units/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showDeptUnits = false
    $scope.unitForm = false
    $scope.showNoUnit = false
    $scope.showDeptStaffs = false
    $http.get(url, config).then(function successCallback(response) {
        $scope.units = response.data;
        $scope.unitForm = false
    })
    $http.get("api/department", config).then(function successCallback(response) {
        $scope.depts = response.data;
        $scope.unitForm = false
    })
    $scope.editunit = function (input) {
        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.unitid = response.data.id
            $scope.unit_name = response.data.unit_name
             $http.get("api/department/"+response.data.dept_id, config).then(function successCallback(response) {
                 $scope.ddlDepts = { "id": response.data.id, "dept_name": response.data.dept_name };
      
    })
          
            console.log($scope.ddlDepts)
            $scope.unitForm = true           
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.update = function () {
        var model = {
            dept_id: $scope.ddlDepts.id,
            unit_name: $scope.unit_name,
            id: $scope.unitid   
        }
        $http.put(url, model, config)
        .then(function successCallback(response, status) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.units = response.data;
                $scope.unitForm = false
                $scope.ddlDepts = { "id": response.data[0].dept_id, "dept_name": response.data[0].dept_name };
                myToastMsg = ngToast.info({
                    content: 'Unit detail has been successfully updated.'
                });
            })
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Update operation fail.' + response.data
            });
        })

    }
    $scope.CreateUnit = function () {
        var model = {
            dept_id: $scope.ddlDepts.id,
            unit_name: $scope.unitname
        }
        $http.post(url, model, config)
        .then(function successCallback(response) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.units = response.data;
            })
            $scope.unitname = "";
            $scope.ddlDepts = { "id": response.data[0].dept_id, "dept_name": response.data[0].dept_name };
            $scope.unitForm = false;
            if (angular.isDefined($scope.ddlDepts)) {
                delete $scope.ddlDepts;
            }
            myToastMsg = ngToast.info({
                content: 'Unit has been successfully added.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Create Unit operation fail. ' + response.data
            });
        })
    }
    $scope.delunit = function (input) {
        swal({
            title: "Are you sure want to delete this Unit?",
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
                 swal("Deleted!", "unit has been deleted.", "success");
                 $http.get(url, config).then(function successCallback(response) {
                     $scope.units = response.data;
                     $scope.unitForm = false                  
                     myToastMsg = ngToast.info({
                         content: 'Unit has been successfully deleted.'
                     });
                 })
             },
             function errorCallback(response) {
                 swal("Cancelled", "Unit cannot be deleted: " + response.data, "error");
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