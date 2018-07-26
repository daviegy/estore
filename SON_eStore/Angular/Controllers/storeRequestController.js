
/// <reference path="angular.min.js" />
/// <reference path="angular-growl.min.js" />

'use strict'
app.controller("storeRequestController", function ($scope, $http, ngToast, stateService, $cookies) {

    var myToastMsg;
    var accessToken = sessionStorage.getItem('accessToken')
    if (accessToken == null || accessToken == "") {
        window.location.href = "/estore/login"
    }
    var user_url = "api/users/"
    var url_register = "api/Account/Register"
    var dept_url = "api/department/"
    var unit_url = "api/Units/"
    var url_item = "api/items/"
    var url_item_instock = "api/items/in_stock"
    var url_cart = "api/cart/";
    var config = {
        headers: {
            'Content-Type': 'application/json',
            "Authorization": "Bearer " + accessToken
        }
    }
    $scope.showUpdateItemForm = false;
    $scope.showItemRequisitionForm = true;
    $scope.shw_convert_master_to_base = false
    $scope.conversionNotFound = false
    $scope.requestType = function () {
        if ($scope.rqtype == "stateRequest") {
            $scope.states = stateService.getState();
            //get staff
            $http.get(user_url, config).then(function successCallback(response) {
                if (response.data.length > 0) {
                    $scope.Users = response.data;
                }
                else {
                    $scope.Users = response.data;
                }
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
            })

        }
        else if ($scope.rqtype == "internalRequest") {
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
        else if ($scope.rqtype == "regionalRequest") {
            $http.get(user_url, config).then(function successCallback(response) {
                if (response.data.length > 0) {
                    $scope.Users = response.data;
                }
                else {
                    $scope.Users = response.data;
                }
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
            })

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
    //get sub-unit 
    $scope.getSubUnit = function () {
        $http.get(dept_url + $scope.ddlDept.id + "/units", config).then(function successCallback(response) {
            if (response.data.length > 0) {
                $scope.units = response.data;
            }
            else {
                $scope.units = ""
            }

        });
        $scope.Users = "";
        $scope.ddlStaff = "";
        $http.get(dept_url + $scope.ddlDept.id + "/staffs", config).then(function successCallback(response) {
            if (response.data.length > 0) {
                $scope.Users = response.data;
            }
           

        })

    }
    $scope.getStateStaff = function(){
        $scope.Users = "";
        $scope.ddlStaff = "";
        $http.get("api/users/" + $scope.ddlState.name + "/StateStaffs", config).then(function successCallback(response) {
            if (response.data.length > 0) {
                $scope.Users = response.data;
            }
            
        })
    }
    $scope.getRegionalStaff = function () {
        $scope.Users = "";
        $scope.ddlStaff = "";
        $http.get("api/users/" + $scope.ddlRegion + "/regionalStaffs", config).then(function successCallback(response) {
            if (response.data.length > 0) {
                $scope.Users = response.data;
            }            
        })
    }
    //get items in stock
    $http.get(url_item, config).then(function successCallback(response) {
        $scope.items = response.data;
        $scope.items_id = response.data[0].id;
        $scope.base_unit = response.data[0].item_base_unit
        $scope.showItemEntryForm = true;
        $scope.ddlDemonination = "";
        $scope.rq_qty_denom = ""
        $scope.qty_req = ""
        $scope.ddlItem = "";
        $scope.conversionNotFound = false
    })
    //get items details
    $scope.getItemsDetails = function () {
        $http.get(url_item + $scope.ddlItem.id, config).
       then(function successCallback(response) {
           $scope.qty_avail = response.data.current_stock_pending_approval + " " + response.data.item_base_unit
           $scope.qty_avail_without_unit = response.data.current_stock_pending_approval
           $scope.item_base_unit = response.data.item_base_unit
           $scope.ddlDemonination = "";
           $scope.rq_qty_denom = "";
           $scope.convert_master_to_base = "";
           $scope.qty_req = "";
           $scope.conversionNotFound = false
           $scope.base_unit = response.data.item_base_unit
           $scope.items_id = response.data.id;
           $scope.shw_convert_master_to_base = false
           $scope.rq_convert_master_to_base = false
           //  $scope.shw_alertconversionerror = false
           // $scope.stock_level_pending_approval = response.data.current_stock_pending_approval
       },
       function errorCallback(response) {
           return response.data
       })
    }
    $scope.getDenomination = function (input) {
        $scope.denom = $scope.ddlDemonination
        //  alert(input)
        if (input != $scope.denom) {
            if (input == "piece(s)" && ($scope.denom == "pack(s)" || $scope.denom == "carton(s)")) {
                $scope.shw_convert_master_to_base = true
                $scope.rq_convert_master_to_base = true;
                $scope.rq_qty_denom = "";
                $scope.qty_req = "";
                $scope.convert_master_to_base = "";
                // $scope.shw_alertconversionerror = false
            }
            else if (input == "pack(s)" && $scope.denom == "carton(s)") {
                $scope.shw_convert_master_to_base = true
                $scope.rq_convert_master_to_base = true;
                $scope.rq_qty_denom = "";
                $scope.qty_req = "";
                $scope.convert_master_to_base = "";
                //$scope.shw_alertconversionerror = false
            }
            else {
                $scope.shw_convert_master_to_base = false
                $scope.rq_convert_master_to_base = false;
                $scope.rq_qty_denom = "";
                $scope.ddlDemonination = "";
                $scope.qty_req = "";
                $scope.convert_master_to_base = "";
                myToastMsg = ngToast.danger({
                    content: "Conversion from '" + input + " to '" + $scope.denom + "' is not allowed. Reason: '" + input + "' is the smallest unit that can be requested for this item."
                });
            }

        }
        else {
            $scope.shw_convert_master_to_base = false
            $scope.rq_convert_master_to_base = false
            $scope.rq_qty_denom = "";
            $scope.qty_req = "";
            $scope.convert_master_to_base = "";
        }
    } 
    $scope.validate_gtyAllct_against_qtyAvail = function () {
        if ($scope.qty_allct > $scope.qty_avail_without_unit) {
            $scope.displayQtyError = "Qty. allocated can not be than Qty. available in store"
            $scope.qty_allct = "";
        }
        else if ($scope.qty_allct > $scope.qty_req) {
            $scope.displayQtyError = "You can not allocate more than the requested value"
            $scope.qty_allct = "";
        }
        else {
            $scope.displayQtyError = "";
        }
    };
    $scope.validateSRV = function () {
        $scope.displayQtyError = ""
        $http.get("api/storeRequest/validateSRV/" + $scope.srv, config)
        .then(function successCallback(response) {
            $scope.displayQtyError = ""
        }, function errorCallback(response) {
            $scope.displayQtyError = response.data
            $scope.srv = "";
        })
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
                    $scope.qty_req = $scope.rq_qty_denom * base_value
                    $scope.conversionNotFound = false
                },
                function errorCallback(response) {
                    $scope.conversionNotFound = true
                    $scope.ddlDemonination = "";
                    $scope.rq_qty_denom = ""
                    $scope.qty_req = ""
                    alert(" Cannot convert item from " + $scope.base_unit + " to " + $scope.denom + ". To setup conversion go to Conversion Table Menu.")
                })
            }
            else {
                $scope.qty_req = $scope.rq_qty_denom
                $scope.conversionNotFound = false
            }
        }
        else {
            //   $scope.conversionNotFound = true
            $scope.ddlDemonination = "";
            $scope.rq_qty_denom = ""
            $scope.qty_req = ""
            $scope.ddlItem = "";
        }
    }
    //get items in cart
    $http.get(url_cart, config).then(function successCallback(response) {
        if (response.data.length > 0) {
            if (response.data[0].request_status == "fresh") {
                $scope.RqCart = response.data;
                $scope.checkoutBTN = true;
                $scope.showCart = true;                
                $scope.depts = null               
                $scope.units = null;               
                $scope.Users = "";
                $scope.ddlStaff = { "id": response.data[0].staff_id, "name": response.data[0].staff_name };
                $scope.rqtype = response.data[0].request_type              
                if ($scope.rqtype == "internalRequest") {
                    $scope.ddlDept = { "id": response.data[0].dept_id, "dept_name": response.data[0].dept_name };
                    $scope.ddlUnit = { "id": response.data[0].unit_id, "unit_name": response.data[0].unit_name };                    
                    $scope.showDept = true
                    $scope.states = "";
                    $scope.showState = false
                    $scope.rqState = false;
                    $scope.rqDept = true;
                    $scope.showRegions = false
                    $scope.ddlRegion = null
                    $scope.rqRegion = false
                    $scope.rqStaff = true;
                    $scope.showStaffDiv = true;
                }
                else if ($scope.rqtype == "stateRequest") {
                    $scope.showDept = false
                    $scope.showState = true
                    $scope.showRegions = false
                    $scope.rqDept = false;
                    $scope.rqState = true;
                    $scope.rqRegion = false
                    $scope.labRequest = response.data[0].isLabRequest
                    //alert(response.data[0].isLabRequest)
                    var statename = jQuery.grep(stateService.getState(), function (state) {
                        return (state.name == response.data[0].state);
                    })
                    $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
                    $scope.ddlRegion = ""
                    $scope.ddlDept = null;
                    $scope.ddlUnit = null;
                   // $scope.ddlStaff = "";
                }
                else if ($scope.rqtype == "regionalRequest") {
                    $scope.showDept = false
                    $scope.showState = false
                    $scope.showRegions = true
                    $scope.rqDept = false;
                    $scope.rqState = false;
                    $scope.rqRegion = true
                    $scope.states = "";
                    $scope.ddlState = "";
                    $scope.ddlRegion = ""
                    $scope.ddlRegion = response.data[0].region;
                    $scope.ddlDept = null;
                    $scope.ddlUnit = null;
                   // $scope.ddlStaff = "";
                }
                else {
                    $scope.showDept = false
                    $scope.showState = false
                    $scope.showRegions = false
                    $scope.rqDept = false;
                    $scope.rqState = false;
                    $scope.rqRegion = false
                    $scope.states = "";
                    $scope.ddlState = "";
                    $scope.ddlRegion = ""
                    $scope.ddlRegion = null;
                    $scope.ddlDept = null;
                    $scope.ddlUnit = null;
                    $scope.ddlStaff = "";
                }
                $scope.rq_date = ""                           
                $scope.srv = response.data[0].s_r_v_no;
                $scope.rq_date = response.data[0].requested_date
            }
        }
        else {
            $scope.checkoutBTN = false;
            $scope.showCart = false;
        }

    });    
    $scope.AddItemToCart = function () {
       // alert($scope.ddlDept)
        var ddDept_id = $scope.ddlDept != null ? $scope.ddlDept.id : null
        var ddUnit_id = $scope.ddlUnit != null ? $scope.ddlUnit.id : null
        var ddlStaff_id = $scope.ddlStaff != null ? $scope.ddlStaff.id : null
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
        var labRequest = $scope.labRequest == null ? "No" : $scope.labRequest
        var region = $scope.ddlRegion == null || $scope.ddlRegion == "" ? null : $scope.ddlRegion
        var cartmodel = {
            dept_id: ddDept_id,
            //dept_name: $scope.ddlDept.dept_name,
            unit_id: ddUnit_id,
            //unit_name: $scope.ddlUnit.unit_name,
            staff_id: ddlStaff_id,
            // staff_name: $scope.ddlStaff.name,
            State: ddlState_name,
            isLabRequest: labRequest,
            Region: region,
            Requested_qty_unit: $scope.ddlDemonination,
            Requested_qty_unit_value: $scope.rq_qty_denom,
            conversion_value: $scope.convert_master_to_base,
            item_base_unit: $scope.item_base_unit,
            Qty_Allocated: $scope.qty_allct,
            Qty_Requested: $scope.qty_req,
            item_id: $scope.ddlItem.id,
            // item_name: $scope.ddlItem.product_name,
            requested_date: $scope.rq_date,
            Request_type: $scope.rqtype,
            s_r_v_no: $scope.srv,
            request_status: "fresh"
        };       
        $http.post(url_cart, cartmodel, config)
            .then(function successCallback(response) {
                $http.get(url_cart, config).then(function successCallback(response) {
                    if (response.data.length > 0) {
                        if (response.data[0].request_status == "fresh") {
                            $scope.RqCart = response.data;
                            $scope.srv = response.data[0].s_r_v_no;
                            $scope.checkoutBTN = true;
                            $scope.showCart = true;
                            $scope.ddlStaff = { "id": response.data[0].staff_id, "name": response.data[0].staff_name };
                            $scope.rqtype = response.data[0].request_type
                            if ($scope.rqtype == "internalRequest") {
                                $scope.ddlDept = { "id": response.data[0].dept_id, "dept_name": response.data[0].dept_name };
                                $scope.ddlUnit = { "id": response.data[0].unit_id, "unit_name": response.data[0].unit_name };                               
                                $scope.showDept = true
                                $scope.states = "";
                                $scope.showState = false
                                $scope.rqState = false;
                                $scope.rqDept = true;
                                $scope.showRegions = false
                                $scope.ddlRegion = null
                                $scope.rqRegion = false
                                $scope.rqStaff = true;
                                $scope.showStaffDiv = true;
                            }
                            else if ($scope.rqtype == "stateRequest") {
                                $scope.showDept = false
                                $scope.showState = true
                                $scope.showRegions = false
                                $scope.rqDept = false;
                                $scope.rqState = true;
                                $scope.rqRegion = false
                                $scope.labRequest = response.data[0].isLabRequest
                                var statename = jQuery.grep(stateService.getState(), function (state) {
                                    return (state.name ==  response.data[0].state);
                                })
                                $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
                                $scope.ddlRegion = ""
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;
                             //   $scope.ddlStaff = "";
                            }
                            else if ($scope.rqtype == "regionalRequest") {
                                $scope.showDept = false
                                $scope.showState = false
                                $scope.showRegions = true
                                $scope.rqDept = false;
                                $scope.rqState = false;
                                $scope.rqRegion = true
                                $scope.states = "";
                                $scope.ddlState = "";
                                $scope.ddlRegion = ""
                                $scope.ddlRegion = response.data[0].region;
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;
                             //   $scope.ddlStaff = "";
                            }
                            else {
                                $scope.showDept = false
                                $scope.showState = false
                                $scope.showRegions = false
                                $scope.rqDept = false;
                                $scope.rqState = false;
                                $scope.rqRegion = false
                                $scope.states = "";
                                $scope.ddlState = "";
                                $scope.ddlRegion = ""
                                $scope.ddlRegion = null;
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;
                                $scope.ddlStaff = "";
                            }
                            $scope.ddlItem = null;
                            $scope.ddlItem = "";
                            $scope.qty_avail = "";
                            $scope.qty_allct = "";
                            $scope.qty_req = "";
                            $scope.rq_qty_denom = ""
                            $scope.ddlDemonination = ""
                        }
                        else {
                            $scope.checkoutBTN = false;
                            $scope.showCart = false;
                            $scope.qty_avail = "";
                            $scope.qty_allct = "";
                            $scope.qty_req = "";
                            $scope.rq_qty_denom = ""
                            $scope.ddlDemonination = ""

                        }
                        myToastMsg = ngToast.info({
                            content: 'item has been added to cart, successfully'
                        });
                    }
                }, function errorCallback(response) {

                })
            }, function errorCallback(response) {
             myToastMsg = ngToast.danger({
                        content: 'Fail to add item to cart, REASON: ' + response.data
             });
          })
      //  $scope.srv = response.data[0].s_r_v_no;          
       
        if (angular.isDefined($scope.ddlDemonination)) {
            delete $scope.ddlDemonination;
        }       
        $scope.rqtype = response.data[0].request_type
        $scope.rq_date = response.data[0].requested_date
        $scope.showUpdateItemForm = false;
        $scope.showItemRequisitionForm = true;

    }
    $scope.editItemFromCart = function (input) {
       // alert(input)
        $http.get("api/cart/getItemRequestCart/" + input, config)
       .then(function successCallback(response) {
           $scope.ddlStaff = { "id": response.data.staff_id, "name": response.data.staff_name };
           if ($scope.rqtype == "internalRequest") {
               $scope.ddlDept = { "id": response.data.dept_id, "dept_name": response.data.dept_name };
               $scope.ddlUnit = { "id": response.data.unit_id, "unit_name": response.data.unit_name };            
               $scope.showStaffDiv = true;
               $scope.showDept = true;
           }
           else if ($scope.rqtype == "stateRequest") {             
               var statename = jQuery.grep(stateService.getState(), function (state) {
                   return (state.name == response.data.state);
               })
               $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
              // alert( response.data.isLabRequest)
               $scope.labRequest = response.data.isLabRequest
               $scope.showState = true;
           }
           else if ($scope.rqtype == "regionalRequest") {   
               $scope.ddlRegion = response.data.region;
               $scope.showRegions = true           
           }           
           $scope.c_id = response.data.id
           $http.get(url_item + response.data.item_id, config).then(function successCallback(response) {
               $scope.ddlItem = { "id": response.data.id, "product_name": response.data.product_name };
               $scope.qty_avail_without_unit = response.data.current_stock_pending_approval
               $scope.qty_avail = response.data.current_stock_pending_approval + " " + response.data.item_base_unit
           })        
           $scope.rqtype = response.data.request_type;
           $scope.ddlDemonination = response.data.requested_qty_unit        
           $scope.rq_qty_denom = response.data.requested_qty_unit_value
           $scope.item_base_unit = response.data.item_base_unit
           $scope.qty_req = response.data.qty_Requested
           $scope.qty_allct= response.data.qty_Allocated           
           $scope.rq_date = response.data.requested_date         
           $scope.srv=response.data.s_r_v_no
           $scope.showUpdateItemForm = true;
           $scope.showItemRequisitionForm = false;

       }, function errorCallback() {

       })
    }
    $scope.UpdateItemToCart = function () {
        var ddDept_id = $scope.ddlDept != null ? $scope.ddlDept.id : null
        var ddUnit_id = $scope.ddlUnit != null ? $scope.ddlUnit.id : null
        var ddlStaff_id = $scope.ddlStaff != null ? $scope.ddlStaff.id : null
        var ddlState_name = $scope.ddlState != null ? $scope.ddlState.name : null
        var cartmodel = {
            id: $scope.c_id,
            dept_id: ddDept_id,
            //dept_name: $scope.ddlDept.dept_name,
            unit_id: ddUnit_id,
            //unit_name: $scope.ddlUnit.unit_name,
            staff_id: ddlStaff_id,
            // staff_name: $scope.ddlStaff.name,
            State: ddlState_name,
            isLabRequest: $scope.labRequest,
            Region: $scope.ddlRegion,
            Requested_qty_unit: $scope.ddlDemonination,
            Requested_qty_unit_value: $scope.rq_qty_denom,
            conversion_value: $scope.convert_master_to_base,
            item_base_unit: $scope.item_base_unit,
            Qty_Allocated: $scope.qty_allct,
            Qty_Requested: $scope.qty_req,
            item_id: $scope.ddlItem.id,           
            requested_date: $scope.rq_date,
            Request_type: $scope.rqtype,
            s_r_v_no: $scope.srv,           
        };            
        $http.put("api/cart/updatecart", cartmodel, config)
            .then(function successCallback() {
                $http.get(url_cart, config).then(function successCallback(response) {
                    if (response.data.length > 0) {
                        if (response.data[0].request_status == "fresh") {
                            $scope.RqCart = response.data;
                            $scope.srv = response.data[0].s_r_v_no;
                            $scope.checkoutBTN = true;
                            $scope.showCart = true;
                            $scope.rqtype = response.data[0].request_type
                            $scope.ddlStaff = { "id": response.data[0].staff_id, "name": response.data[0].staff_name };
                            if ($scope.rqtype == "internalRequest") {
                                $scope.ddlDept = { "id": response.data[0].dept_id, "dept_name": response.data[0].dept_name };
                                $scope.ddlUnit = { "id": response.data[0].unit_id, "unit_name": response.data[0].unit_name };
                            //    $scope.ddlStaff = { "id": response.data[0].staff_id, "name": response.data[0].staff_name };
                                $scope.showDept = true
                                $scope.states = "";
                                $scope.showState = false
                                $scope.rqState = false;
                                $scope.rqDept = true;
                                $scope.showRegions = false
                                $scope.ddlRegion = null
                                $scope.rqRegion = false
                                $scope.rqStaff = true;
                                $scope.showStaffDiv = true;
                            }
                            else if ($scope.rqtype == "stateRequest") {
                                $scope.showDept = false
                                $scope.showState = true
                                $scope.showRegions = false
                                $scope.rqDept = false;
                                $scope.rqState = true;
                                $scope.rqRegion = false
                                $scope.labRequest = response.data[0].isLabRequest
                                var statename = jQuery.grep(stateService.getState(), function (state) {
                                    return (state.name == response.data[0].state);
                                })
                                $scope.ddlState = { "id": statename[0].id, "name": statename[0].name };
                                $scope.ddlRegion = ""
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;
                               // $scope.ddlStaff = "";
                            }
                            else if ($scope.rqtype == "regionalRequest") {
                                $scope.showDept = false
                                $scope.showState = false
                                $scope.showRegions = true
                                $scope.rqDept = false;
                                $scope.rqState = false;
                                $scope.rqRegion = true
                                $scope.states = "";
                                $scope.ddlState = "";
                               
                                $scope.ddlRegion = response.data[0].region;
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;
                               // $scope.ddlStaff = "";
                            }
                            else {
                                $scope.showDept = false
                                $scope.showState = false
                                $scope.showRegions = false
                                $scope.rqDept = false;
                                $scope.rqState = false;
                                $scope.rqRegion = false
                                $scope.states = "";
                                $scope.ddlState = "";
                                $scope.ddlRegion = ""
                                $scope.ddlRegion = null;
                                $scope.ddlDept = null;
                                $scope.ddlUnit = null;                                
                            }
                            $scope.ddlItem = null;
                            $scope.ddlItem = "";
                            $scope.qty_avail = "";
                            $scope.qty_allct = "";
                            $scope.qty_req = "";
                            $scope.rq_qty_denom = ""
                            $scope.ddlDemonination = ""
                        }
                        else {
                            $scope.checkoutBTN = false;
                            $scope.showCart = false;
                            $scope.qty_avail = "";
                            $scope.qty_allct = "";
                            $scope.qty_req = "";
                            $scope.rq_qty_denom = ""
                            $scope.ddlDemonination = ""

                        }
                        myToastMsg = ngToast.info({
                            content: 'item has been added to cart, successfully'
                        });
                    }
                })
            }, function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Fail to update item in cart, REASON: ' + response.data
                });
            });
        $scope.showUpdateItemForm = false;
        $scope.showItemRequisitionForm = true;
    }
    $scope.deleteItemFromCart = function (input) {
        $http.delete(url_cart + input, config).then(
            function successCallback(response) {
                $http.get(url_cart, config).then(function successCallback(response) {
                    if (response.data.length > 0) {
                        $scope.RqCart = response.data;
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
    $scope.checkout = function (input) {
        swal({
            title: "Are you sure want to submit this item(s) for request?",
            text: "ensure, that all items details are correctly entered. ",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, proceed!",
            closeOnConfirm: false
        },
      function (isConfirm) {
          if (isConfirm) {
              var model = { cart_id: input };
              $http.post("api/storeRequest/", model, config).then(
                  function successCallback(response) {
                      myToastMsg = ngToast.success({
                          content: 'Item request has been successful created.'
                      });
                      window.location.href = "/estore/create_item_request"
                  }, function errorCallback(response) {
                      myToastMsg = ngToast.danger({
                          content: 'item request fail ' + response.data
                      });
                  })
          }

      });
    }
    //get Fresh request
    $http.get("api/storeRequest/Fresh_request", config).
        then(function successCallback(response) {
            $scope.FreshRqList = response.data
        },
        function errorCallback(response) {

        })
    //get pending approval request
    $http.get("api/storeRequest/pending_approval", config).then(
        function successCallback(response) {
            //  console.log(response.data[0])
            $scope.PendingRqList = response.data
            // alert( $scope.PendingRqList )
        }, function errorCallback(response) {
        })

    $scope.editAllocation = function (input) {
        $http.get("api/storeRequest/" + input, config)
        .then(function successCallback(response) {
            $scope.qty_allct = response.data.qty_allocated
            $scope.item_id = response.data.product_id
            $scope.id = response.data.r_id
            $scope.showUpallct = true;

        }, function errorCallback() {

        })
    }
    $scope.updateAllocation = function () {
        var model = {
            R_ID: $scope.id,
            qty_allocated: $scope.qty_allct,
            product_id: $scope.item_id
        }
        $http.put("api/storeRequest/", model, config)
        .then(function successCallback(response) {
            var orderid = sessionStorage.getItem('orderid');
            $http.get("api/storeRequest/" + orderid + "/orderp", config)
   .then(function successCallback(response) {
       $scope.Model = response.data

   }, function errorCallback(response) { })
            var myToastMsg = ngToast.info({
                content: 'Quantity allocated has been updated successfully'
            });
        },
        function errorCallback(response) {
            var myToastMsg = ngToast.danger({
                content: 'An error occurred: ' + response.data
            });
        })
        $scope.showUpallct = false;

    }
    $scope.fupdateAllocation = function () {
        var model = {
            R_ID: $scope.id,
            qty_allocated: $scope.qty_allct,
            product_id: $scope.item_id
        }
        $http.put("api/storeRequest/", model, config)
        .then(function successCallback(response) {
            var orderid = sessionStorage.getItem('orderid');
            $http.get("api/storeRequest/" + orderid + "/orderf", config)
   .then(function successCallback(response) {
       $scope.fModel = response.data

   }, function errorCallback(response) { })
            var myToastMsg = ngToast.info({
                content: 'Quantity allocated has been updated successfully'
            });
        },
        function errorCallback(response) {
            var myToastMsg = ngToast.danger({
                content: 'An error occurred: '+response.data
            });
        })
        $scope.showUpallct = false;

    }
    $scope.deleteAllocation = function (input) {
        swal({
            title: "Are you sure want to delete this item from list of items requested?",
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

             $http.delete("api/storeRequest/" + model.id, config)
             .then(function successCallback(response, status) {
                 swal("Deleted!", "item has been deleted.", "success");
                 var orderid = sessionStorage.getItem('orderid');
                 $http.get("api/storeRequest/" + orderid + "/orderp", config)
                 .then(function successCallback(response) {
                     $scope.Model = response.data
                     $scope.orderid = orderid
                     $scope.showUpallct = false;
                 }, function errorCallback(response) { })
             },
             function errorCallback(response) {
                 swal("Cancelled", "item cannot be deleted: " + response.data, "error");
                 myToastMsg = ngToast.danger({
                     content: 'Delete operation fail:' + response.data
                 });
             })
         })
    }
    $scope.fdeleteAllocation = function (input) {
        swal({
            title: "Are you sure want to delete this item from list of items requested?",
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

             $http.delete("api/storeRequest/" + model.id, config)
             .then(function successCallback(response, status) {
                 swal("Deleted!", "item has been deleted.", "success");
                 var orderid = sessionStorage.getItem('orderid');
                 $http.get("api/storeRequest/" + orderid + "/orderf", config)
                 .then(function successCallback(response) {
                     $scope.fModel = response.data
                     $scope.orderid = orderid
                     $scope.showUpallct = false;
                 }, function errorCallback(response) { })
             },
             function errorCallback(response) {
                 swal("Cancelled", "item cannot be deleted: " + response.data, "error");
                 myToastMsg = ngToast.danger({
                     content: 'Delete operation fail:' + response.data
                 });
             })
         })
    }
    $scope.deleteAllBorrowbyOrder = function (input) {
        swal({
            title: "Are you sure want to delete this item from list of items requested?",
            text: "Once deleted you will not be able to recover it",
            type: "warning",
            showCancelButton: true,
            confirmButtonClass: "btn-danger",
            confirmButtonText: "Yes, delete it!",
            closeOnConfirm: false
        },
       function () {

           $http.delete("api/storeRequest/" + input + "/order", config)
           .then(function successCallback(response, status) {
               swal("Deleted!", "item has been deleted.", "success");
               window.location = document.referrer;
           },
           function errorCallback(response) {
               swal("Cancelled", "item cannot be deleted: " + response.data, "error");
               myToastMsg = ngToast.danger({
                   content: 'Delete operation fail:' + response.data
               });
           })
       })
    }

    $scope.viewMoref = function (input) {
        sessionStorage.setItem('orderid', input)
        //var orderid = sessionStorage.getItem('orderid');
        // alert(orderid)
        window.location.href = "/estore/get_Items_orderf";
    }
    var orderid = sessionStorage.getItem('orderid');
    $http.get("api/storeRequest/" + orderid + "/orderf", config)
    .then(function successCallback(response) {
        $scope.fModel = response.data
        $scope.orderid = orderid
    }, function errorCallback(response) { })

    $scope.viewMorep = function (input) {
        sessionStorage.setItem('orderid', input)
        //var orderid = sessionStorage.getItem('orderid');
        // alert(orderid)
        window.location.href = "/estore/get_Items_order";
    }
    var orderid = sessionStorage.getItem('orderid');
    $http.get("api/storeRequest/" + orderid + "/orderp", config)
    .then(function successCallback(response) {
        $scope.Model = response.data
        $scope.orderid = orderid
    }, function errorCallback(response) { })

    //get list of users that can approve item request
    $http.get(user_url + "approvers", config)
   .then(function successCallback(response) {
       $scope.approvers = response.data
   }, function errorCallback(response) {
       //var myToastMsg = ngToast.danger({
       //    content: 'error: ' + response.data
       //});
   })

    $scope.approve_request = function () {
        var model = {
            orderid: $scope.orderid,
            approverid: $scope.ddlApprover
        }
        //  alert(model)
        $http.post("api/storeRequest/approveRequest", model, config)
        .then(function successCallback(response) {
            window.location.href = "/estore/approval_receipt/?orderid=" + response.data
            var myToastMsg = ngToast.info({
                content: 'Store order request has been successfully approved!'
            });
        }, function errorCallback(response) {
            var myToastMsg = ngToast.danger({
                content: 'Request approval fail, reasons: ' + response.data
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