
 'use strict'
 app.controller('loginController', function ($scope, $http, ngToast) {
        $scope.submit = function () {
            var data = $.param({
                username: $scope.username,
                password: $scope.password,
                grant_type: 'password'
            });
            var config = {
                header: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            }
            var url = "/token"
            $http.post(url, data, config)
                .then(function successCallback(response) {               
                    var credit= {
                        token: response.data.access_token,
                        userName:  response.data.userName
                    } 
                    sessionStorage.setItem("accessToken", credit.token);
                    sessionStorage.setItem("userName", credit.userName);
                    $http.post('/estore/authUsername',credit)
                    .then(function successCallback(result) {                        
                        window.location.href = '/estore/Dashboard/'
                    },
                    function errorCallback(result) {
                        $scope.ResponseErrors = result.data.error_description
                    });

                },
                function errorCallback(response) {
                    window.console.log("log", response.statusText)
                    $scope.ResponseErrors = response.data.error_description
                });
        }
        $scope.signout = function () {
            sessionStorage.removeItem("accessToken")
            
            var username = sessionStorage.getItem("userName");
            $http.post("/estore/LogOut?userName="+username).then(function successCallback() {
                window.location.href = "/estore/login"
            },
           function errorCallback() {

           })
        }
        $scope.changepassword = function () {
            var myToastMsg;
            var accessToken = sessionStorage.getItem('accessToken')
            var config = {
                headers: {
                    'Content-Type': 'application/json',
                    "Authorization": "Bearer " + accessToken
                }
            }
            var model = {
                OldPassword: $scope.o_pass,
                NewPassword: $scope.n_pass,
                ConfirmPassword: $scope.rt_pass
            }
            $http.post("api/account/ChangePassword", model, config)
            .then(function successCallback(response) {
                window.location.href = "/estore/dashboard"
                myToastMsg = ngToast.info({
                    content: 'password has been changed successfully'
                });
              
            }, function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Password not changed: '+ response.data
                });
            })

        }

        $scope.resetpassword = function () {
            var myToastMsg;
            var accessToken = sessionStorage.getItem('accessToken')
            var config = {
                headers: {
                    'Content-Type': 'application/json',
                    "Authorization": "Bearer " + accessToken
                }
            }
            var model = {
                Username: $scope.Username,
                NewPassword: $scope.n_pass,
                ConfirmPassword: $scope.rt_pass
            }
            $http.post("/estore/resetpassword/", model, config)
            .then(function successCallback(response) {
                window.location.href = "/estore/dashboard"
                myToastMsg = ngToast.info({
                    content: 'password has been reset successfully'
                });

            }, function errorCallback(response) {
                myToastMsg = ngToast.danger({
                    content: 'Password reset fail: ' + response.data
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

    })



