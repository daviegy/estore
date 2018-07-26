
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("ConversionController", function ($scope, $http, ngToast, $filter) {

    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/Conversion/"
    var url_item = "api/items/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showItemUpdateForm = false;
    $scope.showItemEntryForm = false;
    $scope.showCart = false;
    $scope.master_unit_val = 1
    $http.get(url, config).then(function successCallback(response) {
        $scope.conversionTable = response.data;
        $scope.showItemEntryForm = true;
    })
    $http.get("api/items/", config).then(function successCallback(response) {
        $scope.items = response.data;
        //  $scope.base_unit = response.data[0].item_base_unit
    })
    $scope.setUpConversion = function (input) {
        //  alert($scope.ddlMasterUnit)
        if (input == $scope.ddlMasterUnit) {
            if (angular.isDefined($scope.ddlMasterUnit)) {
                delete $scope.ddlMasterUnit;
            }
            myToastMsg = ngToast.danger({
                content: 'Selected Master Unit cannot be the same with the Base Unit. '
            });

        }
        else if ((input == "pack(s)" || input == "carton(s)") && ($scope.ddlMasterUnit == "piece(s)")) {
            if (angular.isDefined($scope.ddlMasterUnit)) {
                delete $scope.ddlMasterUnit;
            }
            myToastMsg = ngToast.danger({
                content: "Selected Master Unit is considered to be smaller than the base unit, Hence conversion is not allowed."
            });

            //$scope.ddlMasterUnit = "";
        }
        else if (input == "carton(s)" && ($scope.ddlMasterUnit == "piece(s)" || $scope.ddlMasterUnit == "pack(s)")) {
            if (angular.isDefined($scope.ddlMasterUnit)) {
                delete $scope.ddlMasterUnit;
            }
            myToastMsg = ngToast.danger({
                content: "Selected Master Unit is considered to be smaller than the base unit, Hence conversion is not allowed."
            });


        }

    }

    $scope.saveconversion = function () {
        var model = {
            item_id: $scope.ddlItem.id,
            master_unit: $scope.ddlMasterUnit,
            master_unit_value: $scope.master_unit_val,
            base_unit: $scope.base_unit,
            base_unit_value: $scope.base_unit_val
        }
        $http.post(url, model, config).then(function successCallback(response) {
            //   window.location.href = "/estore/converstionTable"
            $scope.ddlMasterUnit = ""
            $scope.ddlItem = ""
           // $scope.master_unit_val = ""
            $scope.base_unit = ""
            $scope.base_unit_val = ""           
            $http.get(url, config).then(function successCallback(response) {
                $scope.conversionTable = response.data;
                $scope.showItemEntryForm = true;               
            })
            myToastMsg = ngToast.success({
                content: 'Operation is successfull'
            });
        }, function errorCallback(response) {
            $scope.ddlMasterUnit = ""
            $scope.ddlItem = ""
            // $scope.master_unit_val = ""
            $scope.base_unit = ""
            $scope.base_unit_val = ""         
            myToastMsg = ngToast.danger({
                content: 'error: ' + response.data
            });
        })
       
    }
   
    //this delete item supplied from item supplies cart
   
    $scope.editItem = function (input) {
       // alert(input)
        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.c_id = response.data.id
            $scope.item_name = response.data.item_name;
            $scope.base_unit = response.data.base_unit
            $scope.base_unit_value = response.data.base_unit_value           
            $scope.showItemEntryForm = false;
            $scope.showItemUpdateForm = true;
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.updateconversion = function () {
        var model = {
            id: $scope.c_id,
            base_unit_value: $scope.base_unit_val,
            master_unit: $scope.ddlMasterUnit,          
        }
        $http.put(url, model, config)
        .then(function successCallback(response) {
            $scope.ddlMasterUnit = ""
            $scope.ddlItem = ""
            // $scope.master_unit_val = ""
            $scope.base_unit = ""
            $scope.base_unit_val = ""
            $http.get(url, config).then(function successCallback(response) {
                $scope.conversionTable = response.data;
                $scope.showItemEntryForm = true;
                $scope.showItemUpdateForm = false;
            })
            myToastMsg = ngToast.success({
                content: 'Operation is successfull'
            });     
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Update operation fail' + response.data.error
            });
        })
    }
    $scope.delitem = function (input) {

        swal({
            title: "Are you sure want to delete this conversion?",
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
                swal("Deleted!", "Your conversion has been deleted.", "success");
                $http.get(url, config).then(function successCallback(response) {
                    $scope.conversionTable = response.data;
                    $scope.showItemEntryForm = true;
                })
                myToastMsg = ngToast.success({
                    content: 'Operation is successfull'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Delete operation fail:' + response.data
                });
            })

        });
    }

    $scope.getItemUnit = function () {
        $http.get(url_item + $scope.ddlItem.id, config).
       then(function successCallback(response) {
           $scope.base_unit = response.data.item_base_unit
           $scope.ddlMasterUnit = "";
           // $scope.stock_level_pending_approval = response.data.current_stock_pending_approval
       },
       function errorCallback(response) {
           return response.data
       })

    }

  

    $scope.itemsByPage = 10;
    ngToast.dismiss();

})