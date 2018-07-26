
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("itemsController", function ($scope, $http, ngToast) {
   
    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/items/"
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showItemUpdateForm = false;
    $scope.showItemEntryForm = false;
    $scope.ConversionTab = false;
    $scope.shw_packs_to_carton = false;
    $scope.rdchange = function (input) {
      
        if (input == 'yes') {
            $scope.item_op_stock = true
            $scope.yes_item_exist_in_store = true;
            $scope.item_op_qty = ""
        }
        else {
            $scope.item_op_stock = false
            $scope.yes_item_exist_in_store = false;
            $scope.item_op_qty = ""
        }

    }
    $http.get(url, config).then(function successCallback(response) {
        $scope.items = response.data;
        $scope.showItemEntryForm = true;
    })
    $http.get("api/category/", config).then(function successCallback(response) {
        $scope.categories = response.data;            
    })
 
    $scope.CreateItemForm = function () {
   
        var model = {
            product_name: $scope.item_name,
            qtyAvailable: $scope.item_op_qty,
            desc: $scope.item_desc,
            serial_no: $scope.serial_no,
            qtyReorderAlertValue: $scope.item_ro_qty,
            unitPrice: $scope.unitPrice,
            catid: $scope.ddlCategory.id,
            is_Item_In_Store: $scope.item_exist_in_store,
            item_base_unit: $scope.ddlBaseUnit
        }
        if ($scope.item_op_qty != null && $scope.item_op_qty != "") {
            if (!($scope.item_op_qty < $scope.item_ro_qty)) {
                $http.post(url, model, config)
                .then(function successCallback(response) {
                    $http.get(url, config).then(function successCallback(response) {
                        $scope.items = response.data;
                    })

                },
                function errorCallback(response) {
                    myToastMsg = ngToast.danger({
                        content: 'Add item operation fail. ' + response.data
                    });
                })
            }
            else {
                myToastMsg = ngToast.danger({
                    content: 'Item opening quantity cannot be less than item re-order point value '
                });
                return
            }
        } else {
            $http.post(url, model, config)
               .then(function successCallback(response) {
                   $http.get(url, config).then(function successCallback(response) {
                       $scope.items = response.data;
                   })

               },
               function errorCallback(response) {
                   myToastMsg = ngToast.danger({
                       content: 'Add item operation fail. ' + response.data
                   });
               })
        }        
        $scope.item_name = "";
        $scope.item_op_qty = "";
        $scope.item_desc = "";
        $scope.serial_no = "";
        $scope.item_ro_qty = "";
        $scope.unitPrice = "";
        $scope.item_exist_in_store = "";
        $scope.ddlCategory = "";
        $scope.ddlBaseUnit = ""
        $scope.showItemEntryForm = true;      
        //$('#ddlCategory').last().val('').trigger('change.select2');
        if (angular.isDefined($scope.ddlCategory)) {
            delete $scope.ddlCategory;
        }
        if (angular.isDefined($scope.ddlBaseUnit)) {
            delete $scope.ddlBaseUnit;
        }
        myToastMsg = ngToast.info({
            content: 'Item has been successfully created.'
        });
      
    }
    $scope.editItem = function (input) {
        $http.get(url + input, config).
        then(function successCallback(response) {
            $scope.pid = response.data.id
            $scope.item_name = response.data.product_name;
            $http.get("api/category/"+ response.data.cat_id, config).
         then(function successCallback(response) {          
           $scope.ddlCategory = { "id": response.data.id, "category_name": response.data.category_name }
           })            
            $scope.item_op_qty = response.data.opening_stock_qty
            $scope.item_desc = response.data.p_descripition;
            $scope.serial_no = response.data.serial_no;
            $scope.item_ro_qty = response.data.stock_reorder_alert_qty;
            $scope.unitPrice = response.data.unitPrice;
            $scope.ddlBaseUnit = response.data.item_base_unit;
            $scope.showItemEntryForm = false;
            $scope.showItemUpdateForm = true;
            
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.UpdateItemForm = function () {       
        if (($scope.item_op_qty >= $scope.item_ro_qty)) {
            var model = {
                id:$scope.pid,
                product_name: $scope.item_name,
                qtyAvailable: $scope.item_op_qty,
                desc: $scope.item_desc,
                serial_no: $scope.serial_no,
                qtyReorderAlertValue: $scope.item_ro_qty,
                unitPrice: $scope.unitPrice,
                catid: $scope.ddlCategory.id,
                item_base_unit: $scope.ddlBaseUnit
            }
            $http.put(url, model, config)
            .then(function successCallback(response) {
            
                $http.get(url, config).then(function successCallback(response) {
                    $scope.items = response.data;
                })
                $scope.item_name = "";
                $scope.item_op_qty = "";
                $scope.item_desc = "";
                $scope.serial_no = "";
                $scope.item_ro_qty = "";
                $scope.unitPrice = "";
                $scope.showItemEntryForm = true;
                $scope.showItemUpdateForm = false;
                $scope.ddlCategory = "";
                $scope.ddlBaseUnit = ""
                $scope.showItemEntryForm = true;
                if (angular.isDefined($scope.ddlCategory)) {
                    delete $scope.ddlCategory;
                }
                if (angular.isDefined($scope.ddlBaseUnit)) {
                    delete $scope.ddlBaseUnit;
                }
                myToastMsg = ngToast.info({
                    content: 'Item has been successfully updated.'
                });
            },
            function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'item fields cannot be updated. ' + response.data.error
                });
            })
        }
        else {
            myToastMsg = ngToast.danger({
                content: 'Item opening quantity cannot be less than item re-order point value '
            });
            return
        }

    }
    $scope.delitem = function (input) {
        swal({
            title: "Are you sure want to delete this item?",
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
                $http.get(url, config).then(function successCallback(response) {
                    swal("Deleted!", "Item has been deleted.", "success");
                    $scope.items = response.data;
                    $scope.showItemUpdateForm = false;
                    $scope.showItemEntryForm = true;
                    myToastMsg = ngToast.info({
                        content: 'Item has been successfully deleted.'
                    });
                })
            },
            function errorCallback(response) {
                swal("Cancelled", "Item cannot be deleted", "error");
                myToastMsg = ngToast.danger({
                    content: 'Delete operation fail:' + response.data
                });
            })
        })
    }
    
    //$scope.setUpConversion = function () {
    //    if ($scope.ddlBaseUnit == "carton(s)") {
    //        $scope.base_unit = $scope.ddlBaseUnit
    //        $scope.ConversionTab = true;
    //    }
    //}
    //$scope.rdchange = function (input) {
    //    if ($scope.rdchange == "yes") {
    //        $scope.shw_packs_to_carton = true;
    //        $scope.rq_packs_to_carton = true;
    //    }
    //    else {
    //        $scope.shw_packs_to_carton = false;
    //        $scope.rq_packs_to_carton = false;
    //    }
    //}
  
    $scope.itemsByPage = 10;
    ngToast.dismiss();

})