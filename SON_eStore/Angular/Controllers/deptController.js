/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("deptController", function ($scope, $http, ngToast) {
    //   alert(sessionStorage.getItem('accessToken'))

    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')

    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/department/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showDeptUnits = false
    $scope.deptForm = false
    $scope.showNoUnit = false
    $scope.showDeptStaffs = false
    $http.get(url, config).then(function successCallback(response) {
        $scope.dept = response.data;
        $scope.deptForm = false
    })

    $scope.editdept = function (input) {

        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.deptname = response.data.dept_name
            $scope.deptid = response.data.id
            $scope.deptForm = true
            $scope.showDeptUnits = false
            $scope.showDeptStaffs = false
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.update = function () {
        var model = {
            id: $scope.deptid,
            dept_name: $scope.deptname
        }
        $http.put(url, model, config)
        .then(function successCallback(response, status) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.dept = response.data;
                $scope.deptForm = false
                $scope.showDeptUnits = false
                $scope.showDeptStaffs = false
                myToastMsg = ngToast.info({
                    content: 'Department has been successfully updated.'
                });
            })
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Update operation fail.' + response.data
            });
        })

    }
    $scope.CreateDept = function () {

        var model = {
            dept_name: $scope.CreateDeptName
        }
        $http.post(url, model, config)
        .then(function successCallback(response) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.dept = response.data;
            })
            $scope.CreateDeptName = "";
            $scope.deptForm = false;
            $scope.showDeptUnits = false
            $scope.showDeptStaffs = false
            myToastMsg = ngToast.info({
                content: 'New Department has been successfully created.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Create Department operation fail. ' + response.data
            });
        })
    }
    $scope.delDept = function (input) {
        swal({
            title: "Are you sure want to delete this department?",
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
                 swal("Deleted!", "department has been deleted.", "success");
                 $http.get(url, config).then(function successCallback(response) {
                     $scope.dept = response.data;
                     $scope.deptForm = false
                     $scope.showDeptUnits = false
                     $scope.showDeptStaffs = false
                     myToastMsg = ngToast.info({
                         content: 'Department has been successfully deleted.'
                     });
                 })
             },
             function errorCallback(response) {
                 swal("Cancelled", "Department cannot be deleted: " + response.data, "error");
                 myToastMsg = ngToast.danger({
                     content: 'Delete operation fail:' + response.data
                 });
             })
         })
    }
    $scope.getDeptUnits = function (input) {
        $http.get(url + input + "/units", config).
    then(function successCallback(response) {
        if (response.data != null) {
            if (response.data.length > 0) {
                $scope.Units = response.data
                $scope.dept_name = response.data[0].dept_name
                $scope.showDeptUnits = true
                $scope.deptForm = false
                $scope.showNoUnit = false
                $scope.showNoStaff = false
                $scope.showDeptStaffs = false
            }
            else {
                $scope.showNoUnit = true;
                $scope.showDeptStaffs = false
                $scope.noItem = "No Unit found for the Department selected."
            }
        } else {
            $scope.showNoUnit = true;
            $scope.showDeptStaffs = false
            $scope.noItem = "No Unit found for the Department selected."
        }


    },
    function errorCallback(response) {
        $scope.showDeptStaffs = false
        $scope.showDeptUnits = false
        myToastMsg = ngToast.danger({
            content: 'No Unit found for the Department selected.'
        });
    })
    }
    $scope.getDeptStaffs = function (input) {
        $http.get(url + input + "/staffs", config).
    then(function successCallback(response) {
        if (response.data != null) {
            if (response.data.length > 0) {
                $scope.Staffs = response.data
                $scope.dept_name = response.data[0].dept_name
                $scope.showDeptUnits = false
                $scope.deptForm = false
                $scope.showNoUnit = false
                $scope.showDeptStaffs = true
                $scope.showNoStaff = false
            }
            else {
                $scope.showNoStaff = true;
                $scope.showDeptUnits = false
                $scope.noItem = "No staffs found for the Department selected."
            }
        } else {
            $scope.showNoStaff = true;
            $scope.showDeptUnits = false
            $scope.noItem = "No staffs found for the Department selected."
        }


    },
    function errorCallback(response) {
        $scope.showDeptStaffs = false
        $scope.showDeptUnits = false

        myToastMsg = ngToast.danger({
            content: 'No staffs found for the Department selected.'
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