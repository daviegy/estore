
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("stockInController", function ($scope, $http, ngToast, $filter) {

    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var url = "api/storeSupplies/"
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
    $scope.conversionNotFound = false
    $http.get(url, config).then(function successCallback(response) {
        $scope.stockin = response.data;
        $scope.showItemEntryForm = true;
    })
    $http.get("api/suppliers/", config).then(function successCallback(response) {
        $scope.suppliers = response.data;
    })
    $http.get("api/items/", config).then(function successCallback(response) {
        $scope.items = response.data;
        $scope.items_id = response.data[0].id
        $scope.ddlDemonination = "";
        $scope.ds_denomination = true
        $scope.sp_qty_denom = ""
        $scope.item_op_qty = "";
        $scope.conversionNotFound = false
        //  $scope.base_unit = response.data[0].item_base_unit
    })

    $scope.getDenomination = function (input1, input2) {
        $scope.denom = $scope.ddlDemonination
        $scope.sp_qty_denom = ""
        $scope.item_op_qty = "";
        $scope.conversionNotFound = false
    }
    $scope.Convert = function (input1, input2, input3) {
        //alert(input1)
        //alert(input2)
        // alert(input3)
        if ((input1 != null || input1 != "") && (input2 != null || input2 != "") && (input3 != null || input3 != "")) {
            if (input1 != input2) {
                var model = {
                    itemid: input3,
                    master_unit: input2
                }
                $http.get("api/Conversion/conversion_by_items/" + model.itemid + "/" + model.master_unit, config).
                then(function successCallback(response) {
                    var base_value = response.data
                   // alert(base_value)
                    $scope.item_op_qty = $scope.sp_qty_denom * base_value
                    $scope.conversionNotFound = false
                },
                function errorCallback(response) {
                    $scope.conversionNotFound = true
                    $scope.ddlDemonination = "";
                    $scope.sp_qty_denom = ""
                    $scope.item_op_qty = ""
                    alert(" Cannot convert item from " + $scope.base_unit + " to " + $scope.denom + ". To setup conversion go to Conversion Table Menu.")
                })
            }
            else {
                $scope.item_op_qty = $scope.sp_qty_denom
                $scope.conversionNotFound = false
            }
        }
        else {
         //   $scope.conversionNotFound = true
            $scope.ddlDemonination = "";
            $scope.sp_qty_denom = ""
            $scope.item_op_qty = ""
            $scope.ddlItem = "";
        }
    }
    // get_item in cart
    $http.get("api/cart/getItem_Supplied_to_Cart", config).then(function successCallback(response) {
        if (response.data.length > 0) {
            $scope.stockincart = response.data;
            $scope.showCart = true
            $scope.checkoutBTN = true;          
            $scope.ddlSupplier = { "id": response.data[0].supplier_id, "supplier_name": response.data[0].supplier }
            $scope.item_op_qty = "";
            $scope.unitPrice = "";
            $scope.totalAmount = "";
            $scope.supply_date = response.data[0].supplied_date;
            $scope.base_unit = "";
            $scope.SRV = response.data[0].s_r_v_no;
            $scope.showItemEntryForm = true;          
        }
        else {
            $scope.checkoutBTN = false;
            $scope.showCart = false;
        }

    });
    $scope.addtocart = function () {
        var model = {
            s_r_v_no: $scope.SRV,
            supplier_id: $scope.ddlSupplier.id,
            item_id: $scope.ddlItem.id,
            qty_denomination: $scope.denom,
            qtySupplied: $scope.sp_qty_denom,
            qty_supplied_in_base_unit: $scope.item_op_qty,
            item_base_unit: $scope.base_unit,
            unit_price: $scope.unitPrice,
            total_amount_per_item: $scope.totalAmount,
            supplied_date: $filter('date')($scope.supply_date, "dd/MM/yyyy")
        }
        $http.post("api/cart/item_Supplied_to_Cart", model, config)
        .then(function successCallback(response) {
            $http.get("api/cart/getItem_Supplied_to_Cart", config).then(function successCallback(response) {
                $scope.stockincart = response.data;
                $scope.showCart = true
                $scope.checkoutBTN = true;
                //$scope.ddlItem = {"id": response.data[0].item_id, "product_name":response.data[0].item_name}
                $scope.ddlSupplier = { "id": response.data[0].supplier_id, "supplier_name": response.data[0].supplier }
                myToastMsg = ngToast.info({
                    content: 'Item has been added into store successfully.'
                });
            })          
            $scope.ddlDemonination = ""
            $scope.item_op_qty = "";
            $scope.unitPrice = "";
            $scope.totalAmount = "";
            $scope.sp_qty_denom = ""
            $scope.base_unit = "";
            $scope.SRV = response.data[0].s_r_v_no;
            $scope.showItemEntryForm = true;
            if (angular.isDefined($scope.ddlDemonination)) {
                delete $scope.ddlDemonination;
            }
        },
        function errorCallback(response) {          
            myToastMsg = ngToast.danger({
                content: 'Add item operation fail. ' + response.data
            });
        })
       
    }
    $scope.checkout = function (input) {
        swal({
            title: "Are you sure want to add this items supplied to store?",
            text: "ensure, that all items details are correctly entered. ",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, proceed!",
            closeOnConfirm: false
        },
       function (isConfirm) {
           if (isConfirm) {
               var model = { s_r_v_no: input };
               $http.post(url, model, config).then(
                   function successCallback(response) {
                       window.location.href = "/PrintRPT/Print_Fresh_Items_Supplied_by_srv/?srv=" + response.data
                       myToastMsg = ngToast.success({
                           content: 'Item has been successful added to store.'
                       });
                       //window.location.href = "/estore/stockSupplies"
                   }, function errorCallback(response) {
                       myToastMsg = ngToast.danger({
                           content: 'item request fail ' + response.data
                       });
                   })
           }

       });

    }
    //this delete item supplied from item supplies cart
    $scope.deleteItemFromCart = function (input) {
        $http.delete("api/cart/" + input + "/deleteItem_Supplied_to_Cart", config).then(
            function successCallback(response) {
                $http.get("api/cart/getItem_Supplied_to_Cart", config).then(function successCallback(response) {
                    if (response.data.length > 0) {
                        $scope.stockincart = response.data;
                        $scope.checkoutBTN = true;
                        $scope.showCart = true;
                    }
                    else {
                        $scope.checkoutBTN = false;
                        $scope.showCart = false;
                    }
                });
            }, function errorCallback() {

            })
    };
    $scope.CreateItemForm = function () {

        var model = {
            supplier_id: $scope.ddlSupplier,
            product_id: $scope.ddlItem,
            qty_supplied: $scope.item_op_qty,
            unitprice: $scope.unitPrice,
            supplied_date: $filter('date')($scope.supply_date, "dd/MM/yyyy")
        }
        $http.post(url, model, config)
        .then(function successCallback(response) {
            $http.get(url, config).then(function successCallback(response) {
                $scope.stockin = response.data;
            })
            $scope.ddlSupplier = null;
            $scope.ddlItem = null;
            $scope.item_op_qty = "";
            $scope.unitPrice = "";
            $scope.totalAmount = "";
            $scope.supply_date = "";
            $scope.showItemEntryForm = true;
            if (angular.isDefined($scope.ddlSupplier)) {
                delete $scope.ddlSupplier;
            }
            myToastMsg = ngToast.info({
                content: 'Item has been added into store successfully.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'Add item operation fail. ' + response.data
            });
        })
    }
    $scope.editItemFromCart = function (input) {
        // alert(input)
        $http.get("api/cart/item_Supplied_to_Cart/" + input, config).
        then(function successCallback(response) {
            $scope.s_id = response.data.id           
            $scope.ddlItem = {"id": response.data.item_id, "product_name":response.data.item_name}
            $scope.ddlSupplier = { "id": response.data.supplier_id, "supplier_name": response.data.supplier }
            $scope.item_op_qty = response.data.qty_supplied_in_base_unit
            $scope.denom = response.data.qty_denomination
            $scope.base_unit = response.data.item_base_unit
            $scope.unitPrice = response.data.unit_price;
            $scope.totalAmount = response.data.total_amount_per_item
            $scope.supply_date = response.data.supplied_date
            $scope.sp_qty_denom = response.data.qtySupplied;
            $scope.ddlDemonination = response.data.qty_denomination;
            $scope.showItemEntryForm = false;
            $scope.showItemUpdateForm = true;
            $scope.ds_denomination = false;
            $scope.SRV = response.data.s_r_v_no
        },
        function errorCallback(response) {
            return response.data
        })
    }
    $scope.updateItemForm = function () {
      //  alert("i am here")
        var model = {
            id: $scope.s_id,
            s_r_v_no: $scope.SRV,
            supplier_id: $scope.ddlSupplier.id,
            item_id: $scope.ddlItem.id,
            qty_denomination: $scope.denom,
            qtySupplied: $scope.sp_qty_denom,
            qty_supplied_in_base_unit: $scope.item_op_qty,
            item_base_unit: $scope.base_unit,
            unit_price: $scope.unitPrice,
            total_amount_per_item: $scope.totalAmount,
            supplied_date: $filter('date')($scope.supply_date, "dd/MM/yyyy")
        }
        $http.put("api/cart/updateSupplies_in_Cart", model, config)
        .then(function successCallback(response) {
            // get_item in cart
            $http.get("api/cart/getItem_Supplied_to_Cart", config).then(function successCallback(response) {
                if (response.data.length > 0) {
                    $scope.stockincart = response.data;
                    $scope.showCart = true
                    $scope.checkoutBTN = true;
                    $scope.ddlSupplier = { "id": response.data[0].supplier_id, "supplier_name": response.data[0].supplier }
                    $scope.ddlItem = null;
                    $scope.item_op_qty = "";
                    $scope.unitPrice = "";
                    $scope.totalAmount = "";
                     $scope.sp_qty_denom = ""
                    $scope.supply_date = response.data[0].supplied_date;
                    $scope.base_unit = "";
                    $scope.SRV = response.data[0].s_r_v_no;
                    $scope.showItemEntryForm = true;
                    $scope.showItemUpdateForm = false;                   
                }
                else {
                    $scope.checkoutBTN = false;
                    $scope.showCart = false;
                }

            });                    
            myToastMsg = ngToast.info({
                content: 'Supplied details Item has been successfully updated.'
            });
        },
        function errorCallback(response) {
            myToastMsg = ngToast.danger({
                content: 'item fields cannot be updated. ' + response.data.error
            });
        })
        if (angular.isDefined($scope.ddlDemonination)) {
            delete $scope.ddlDemonination;
        }
    }
    $scope.delitem = function (input) {

        swal({
            title: "Are you sure want to delete this supply?",
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
                swal("Deleted!", "Your supply item has been deleted.", "success");
                $http.get(url, config).then(function successCallback(response) {
                    $scope.stockin = response.data;
                    $scope.showItemUpdateForm = false;
                    $scope.showItemEntryForm = true;
                    myToastMsg = ngToast.info({
                        content: 'Item has been successfully deleted.'
                    });
                })
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
           $scope.items_id = response.data.id
           $scope.base_unit = response.data.item_base_unit
           $scope.ddlDemonination = "";
           $scope.ds_denomination = false
           $scope.sp_qty_denom = "";
           $scope.item_op_qty = "";
           $scope.conversionNotFound = false
           // $scope.stock_level_pending_approval = response.data.current_stock_pending_approval
       },
       function errorCallback(response) {
           return response.data
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