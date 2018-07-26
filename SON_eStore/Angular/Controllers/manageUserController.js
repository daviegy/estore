
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("manageUserController", function ($scope, $http, ngToast, stateService) {
    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var user_url = "api/users/"
    var url_register = "api/Account/Register"
    var dept_url = "api/department/"
    var unit_url = "api/Units/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.editForm = false
    $http.get(dept_url, config).then(function successCallback(response) {
        $scope.depts = response.data;
    })
    $scope.getSubUnit = function () {        
        $http.get(dept_url + $scope.ddlDept.id + "/units", config).then(function successCallback(response) {
            if (response.data.length > 0) {
                $scope.units = response.data;
            }
            else {
                $scope.units = null
            }

        })
    }
    $http.get("api/roles", config).then(function successCallback(response) {
        $scope.roles = response.data;
    })
    $http.get("api/users/geteStoreUsers", config).then(function successCallback(response) {
        $scope.eStoreUsers = response.data;
    })
    $http.get("api/users/getStaff", config).then(function successCallback(response) {
        $scope.Staffs = response.data;
    })
    $scope.StaffType = function () {
        if ($scope.staffType == "stateStaff") {
            $scope.states = stateService.getState();            
            $scope.showState = true
            $scope.depts = null;
            $scope.units = null;
            $scope.ddlDept = null;
            $scope.ddlUnit = null;
            $scope.showDept = false
            $scope.rqState = true;
            $scope.rqDept = false;
            $scope.showRegions = false
            $scope.ddlRegion = null
            $scope.rqRegion = false;
            $scope.rqStaff = false;
            $scope.showStaffDiv = false;

        }
        else if ($scope.staffType == "internalStaff") {
            $http.get(dept_url, config).then(function successCallback(response) {
                $scope.depts = response.data;
                $scope.showDept = true
                $scope.states = "";
                $scope.showState = false
                $scope.rqState = false;
                $scope.rqDept = true;
                $scope.ddlStaff = "";
                $scope.Users = "";
                $scope.showRegions = false
                $scope.ddlRegion = null
                $scope.rqRegion = false
                $scope.rqStaff = true;
                $scope.showStaffDiv = true;
            })

        }
        else if ($scope.staffType == "regionalStaff") {
            $scope.showState = false
            $scope.depts = null;
            $scope.units = null;
            $scope.ddlDept = null;
            $scope.ddlUnit = null;
            $scope.showDept = false
            $scope.rqState = false;
            $scope.rqDept = false;
            $scope.showRegions = true
            //$scope.ddlRegion = null
            $scope.rqRegion = true
            $scope.rqStaff = false;
            $scope.showStaffDiv = false;

        }
        else {
            $scope.showState = false
            $scope.depts = null;
            $scope.units = null;
            $scope.Users = "";
            $scope.ddlDept = null;
            $scope.ddlUnit = null;
            $scope.ddlStaff = "";
            $scope.showDept = false
            $scope.rqState = false;
            $scope.rqDept = false;
            $scope.states = "";
            $scope.ddlState = "";
            $scope.showRegions = false
            $scope.ddlRegion = null
            $scope.rqRegion = false
            $scope.rqStaff = false;
            $scope.showStaffDiv = false;
        }

    }

    $scope.userRegForm = function () {
        var ddDept_id = $scope.ddlDept != null ? $scope.ddlDept.id : null
        var ddUnit_id = $scope.ddlUnit != null ? $scope.ddlUnit.id : null      
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
        var model = {
            //staff_id: $scope.staffid,
            Fname: $scope.fname,
            Lname: $scope.lname,
            dept_id: ddDept_id,
            unit_id: ddUnit_id,
            staffType: $scope.staffType,
            regionalOffice: $scope.ddlRegion,
            stateOffice : ddlState_name,
            //phone: $scope.phone,
            //email: $scope.email,
            //Username: $scope.Username,
            role_id: $scope.ddlRole,
            rank: $scope.rank
        }
        $http.post(url_register, model, config)
        .then(function successCallback(response) {
            $scope.lname = "";
            $scope.fname = "";
            $scope.staffid = "";
            $scope.phone = "";
            $scope.email = "";
            $scope.ddlDept = "";
            $scope.ddlUnit = "";
            $scope.ddlDept = "";
            $scope.ddlUnit = "";
            $scope.Username = "";
            $scope.ddlRole = "";
            $scope.showState = false
            $scope.depts = null;
            $scope.units = null;
            $scope.Users = "";
            $scope.ddlDept = null;
            $scope.ddlUnit = null;
            $scope.ddlStaff = "";
            $scope.showDept = false
            $scope.rqState = false;
            $scope.rqDept = false;
            $scope.states = "";
            $scope.ddlState = "";
            $scope.showRegions = false
            $scope.ddlRegion = null
            $scope.rqRegion = false
            $scope.rqStaff = false;
            $scope.showStaffDiv = false;
            myToastMsg = ngToast.info({
                content: 'New user registration is successfull.'
            });
            location.reload();
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'User registration fail. ' + response.data
            });
        })
    }
    $scope.editUsers = function (input) {
        $http.get(user_url + input, config).
        then(function successCallback(response) {
            $scope.u_id = response.data.id
            $scope.staffid = response.data.staff_id
            $scope.fname = response.data.fname;
            $scope.lname = response.data.lname
            $scope.email = response.data.email
            $scope.phone = response.data.phone;
            $scope.staffType = response.data.staffType
            if ($scope.staffType == "internalStaff") {
                $scope.ddlDept = { "id": response.data.dept_id, "dept_name": response.data.dept_name };
                $scope.ddlUnit = { "id": response.data.unit_id, "unit_name": response.data.unit_name };               
                $scope.showStaffDiv = true;
                $scope.showDept = true;
                $scope.showRegions = false
                $scope.showState = false;
            }
            else if ($scope.staffType == "stateStaff") {
                var statename = jQuery.grep(stateService.getState(), function (state) {
                    return (state.name == response.data.stateOffice);
                })
                $scope.states = stateService.getState();
                $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
                $scope.showState = true;
                $scope.showStaffDiv = false;
                $scope.showDept = false;
            }
            else if ($scope.staffType == "regionalStaff") {
                $scope.ddlRegion = response.data.regionalOffice;
                $scope.showRegions = true
                $scope.showState = false;
                $scope.showStaffDiv = false;
                $scope.showDept = false;
            }          
            $scope.ddlRole = response.data.role_id;            
            $scope.rank = response.data.rank;
            $scope.editForm = true;            
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.StaffEditForm = function () {
        var ddDept_id = $scope.ddlDept != null ? $scope.ddlDept.id : null
        var ddUnit_id = $scope.ddlUnit != null ? $scope.ddlUnit.id : null
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
       
        var model = {
            id: $scope.u_id,           
            Fname: $scope.fname,
            Lname: $scope.lname,
            dept_id: ddDept_id,
            unit_id: ddUnit_id,
            staffType: $scope.Stafftype,
            regionalOffice: $scope.ddlRegion,
            stateOffice: ddlState_name,            
            role_id: $scope.ddlRole,
            rank: $scope.rank
        }
        $http.put(user_url, model, config)
            .then(function successCallback(response) {
                $http.get("api/users/getStaff", config).then(function successCallback(response) {
                    $scope.Staffs = response.data;
                })
                $scope.l_name = "";
                $scope.f_name = "";
                $scope.staffid = "";
                $scope.phone = "";
                $scope.email = "";
                $scope.ddlDept = "";
                $scope.ddlUnit = "";
                $scope.ddlRegion = "";
                $scope.ddlState = "";
                $scope.Username = "";
                $scope.ddlRole = "";
                $scope.editForm = false
                myToastMsg = ngToast.info({
                    content: 'Item has been successfully updated.'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Update fail: ' + response.data
                });
            })
    }
    $scope.eStoreuserEditForm = function () {
        var ddDept_id = $scope.ddlDept != null ? $scope.ddlDept.id : null
        var ddUnit_id = $scope.ddlUnit != null ? $scope.ddlUnit.id : null
        var model = {
            id: $scope.u_id,
            //staff_id: $scope.staffid,
            Fname: $scope.fname,
            Lname: $scope.lname,
            dept_id: ddDept_id,
            unit_id: ddUnit_id,
            //phone: $scope.phone,
            //email: $scope.email,
            // Username: $scope.Username,
            role_id: $scope.ddlRole,
            rank: $scope.rank
        }
        $http.put(user_url, model, config)
            .then(function successCallback(response) {
                $http.get("api/users/geteStoreUsers", config).then(function successCallback(response) {
                    $scope.eStoreUsers = response.data;
                })
                $scope.lname = "";
                $scope.fname = "";
                $scope.staffid = "";
                $scope.phone = "";
                $scope.email = "";
                $scope.ddlDept = "";
                $scope.ddlUnit = "";
                $scope.Username = "";
                $scope.ddlRole = "";
                $scope.editForm = false
                myToastMsg = ngToast.info({
                    content: 'Item has been successfully updated.'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Update fail: ' + response.data
                });
            })
    }

    $scope.delUser = function (input) {
        swal({
            title: "Are you sure want to delete this user?",
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

            $http.delete(user_url + model.id, config)
            .then(function successCallback(response, status) {
                $http.get("api/users/geteStoreUsers", config).then(function successCallback(response) {
                    swal("Deleted!", "user has been deleted.", "success");                  
                    $scope.eStoreUsers = response.data;
                    //$scope.editForm = false
                   
                })
                $http.get("api/users/getStaff", config).then(function successCallback(response) {
                    $scope.Staffs = response.data;                    
                   
                })
                 $scope.editForm = false
                 myToastMsg = ngToast.info({
                        content: 'user has been successfully deleted.'
                    });
            },
            function errorCallback(response) {
                swal("Cancelled", "user cannot be deleted", "error");
                myToastMsg = ngToast.danger({
                    content: 'Delete operation fail:' + response.data
                });
            })
        })
    }
    $scope.checkStaffID = function () {
        $http.post(user_url + $scope.staffid + "/CheckStaffID", config)
         .then(function successCallback(response) {
             $scope.staffidexist = "StaffID: '" + response.data[0].staff_id + "' already exist, kindly supply a different staff id."
             $scope.staffid = "";
         }, function errorCallback(response) {
             $scope.staffidexist = "";
         })
    };
    $scope.checkEmail = function () {
        $http.post(user_url + $scope.email + "/checkEmail", config.headers)
         .then(function successCallback(response) {
             $scope.emailexist = "Email: '" + response.data[0].email + "' already exist, kindly supply a different email."
             $scope.email = "";
         }, function errorCallback(response) {
             $scope.emailexist = "";
         })
    };
    //// to clear a toast:
    //   ngToast.dismiss(myToastMsg);
    //var myToastMsg = ngToast.success({
    //    content: 'Login successfully'
    //});
    // clear all toasts:

    $scope.itemsByPage = 10;
    ngToast.dismiss();

})