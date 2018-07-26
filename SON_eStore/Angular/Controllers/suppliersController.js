
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("suppliersController", function ($scope, $http, ngToast, stateService) {
    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/suppliers/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showItemUpdateForm = false;
    $scope.showItemEntryForm = false;
    $scope.showItemsSupplied = false
    $http.get(url, config).then(function successCallback(response) {
        $scope.suppliers = response.data;
        $scope.showItemEntryForm = true;
        $scope.showItemsSupplied = false
    })
    $scope.states = stateService.getState();
    $scope.CreateItemForm = function () {
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
            var model = {
                supplier_name: $scope.supplier_name,
                phone: $scope.s_phone,
                email: $scope.s_email,
                address: $scope.s_address,
                city: $scope.s_city,            
                state: ddlState_name
            }
            $http.post(url, model, config)
            .then(function successCallback(response) {
                $http.get(url, config).then(function successCallback(response) {
                    $scope.suppliers = response.data;
                })
                $scope.supplier_name = "";
                $scope.s_phone = "";
                $scope.s_email = "";
                $scope.s_address = "";
                $scope.s_city = "";
                $scope.s_city = ddlState;
                $scope.showItemEntryForm = true;
                $scope.showItemsSupplied = false
                myToastMsg = ngToast.info({
                    content: 'supplier has been successfully registered.'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'operation fail to register supplier. ' + response.data
                });
            })
            if (angular.isDefined($scope.ddlState)) {
                delete $scope.ddlState;
            }
        }
    $scope.editSupplier = function (input) {
        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.s_id = response.data.id
            $scope.supplier_name = response.data.supplier_name;
           // $scope.ddlState = response.data.state.name
            $scope.s_phone = response.data.phone_no
            $scope.s_email = response.data.email;
            $scope.s_address = response.data.address;
            $scope.s_city = response.data.city;
            var statename = jQuery.grep(stateService.getState(), function (state) {
                return (state.name == response.data.state);
            })
            $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
            $scope.showItemEntryForm = false;
            $scope.showItemUpdateForm = true;
            $scope.showItemsSupplied = false
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.UpdateSupplierForm = function () {
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
            var model = {
                id: $scope.s_id,
                supplier_name: $scope.supplier_name,
                phone: $scope.s_phone,
                email: $scope.s_email,
                address: $scope.s_address,
                city: $scope.s_city,
                state: ddlState_name
            }
            $http.put(url, model, config)
            .then(function successCallback(response) {
                $http.get(url, config).then(function successCallback(response) {
                    $scope.suppliers = response.data;
                })
                $scope.supplier_name = "";
                $scope.s_phone = "";
                $scope.s_email = "";
                $scope.s_address = "";
                $scope.s_city = "";
                $scope.ddlState = "";
                $scope.showItemEntryForm = true;
                $scope.showItemUpdateForm = false;
                $scope.showItemsSupplied = false
                myToastMsg = ngToast.info({
                    content: 'supplier details has been successfully updated.'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'item fields cannot be updated. ' + response.data.error
                });
            })
            if (angular.isDefined($scope.ddlState)) {
                delete $scope.ddlState;
            }
        }
    $scope.delsupplier = function (input) {

        swal({
            title: "Are you sure want to delete this supplier?",
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
                swal("Deleted!", "Your supplier has been deleted.", "success");
                $http.get(url, config).then(function successCallback(response) {
                    $scope.suppliers = response.data;
                    $scope.showItemUpdateForm = false;
                    $scope.showItemEntryForm = true;
                    $scope.showItemsSupplied = false
                    myToastMsg = ngToast.info({
                        content: 'supplier has been successfully deleted.'
                    });
                })
            },
            function errorCallback(response) {
                swal("Cancelled", "supplier cannot be deleted: " + response.data, "error");
                myToastMsg = ngToast.danger({
                    content: 'Delete operation fail:' + response.data
                });
            })
        })
    }
    $scope.itemsbySupplier = function (input) {
      
        $http.get(url+input+"/items",config).
        then(function successCallback(response) {
            $scope.showItemsSupplied = true
            if (response.data != null) {
                if (response.data.length > 0) {
                    $scope.items = response.data
                    $scope.supplierName = response.data[0].supplier_name
                    $scope.showItemUpdateForm = false;
                    $scope.showItemEntryForm = false;
                    $scope.showNoItem = false
                  
                }
                else {
                    $scope.showNoItem = true;
                    $scope.noItem = "No item supplied!"
                }
            } else {
                $scope.showNoItem = true;
                $scope.noItem = "No items supplied!"
            }
        }, function errorCallback(response) {
            $scope.showItemsSupplied = false
            myToastMsg = ngToast.danger({
                content: 'There occured an error:' + response.data
            });
        })
    }
    //// to clear a toast:
    //   ngToast.dismiss(myToastMsg);
    //var myToastMsg = ngToast.success({
    //    content: 'Login successfully'
    //});
    // clear all toasts:

    $scope.itemsByPage = 10;
    ngToast.dismiss();

})