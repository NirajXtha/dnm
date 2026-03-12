// Code goes here
var app = angular.module('myApp', ['kendo.directives', 'ngMessages']);

app.controller('POSController', function ($scope, $http, $filter, PosService, $q) {
    $scope.food = [{
        pizza: { count: 1, id: 2, detail: "Brick Oven Pizza", price: 15 },
        donut: { count: 3, id: 3, detail: "Glazed Donut", price: 8 },
        tortilla: { count: 1, id: 4, detail: "Tortilla Chips", price: 3 },
        burger: { count: 1, id: 5, detail: "Burger", price: 3 },
        samosa: { count: 1, id: 6, detail: "Delicious Samosas", price: 3 },
        coldcoffee: { count: 1, id: 7, detail: "Cold Coffee", price: 2 },
        hotcoffee: { count: 1, id: 8, detail: "Hot Coffee", price: 2 },
        coke: { count: 1, id: 9, detail: "Coke", price: 2 },
        dietcoke: { count: 1, id: 10, detail: "Diet Coke", price: 2 },
        pepsi: { count: 1, id: 11, detail: "Pepsi", price: 2 }
    }];
    $scope.pager = [
        {
            Products: '',
            totalItems: '',
            currentPage: '',
            pageSize: '',
            totalPages: '',
            startPage: '',
            endPage: '',
            startIndex: '',
            endIndex: '',
            pages: ''
        }
    ];

  

    $scope.Products = [];
    //$scope.pager = [];
    $scope.snote = "";
    $scope.itemsCnt = 1;
    $scope.order = [];
    $scope.pagination = true;
    $scope.isDisabled = true;
    $scope.disountper = 0;
    $scope.taxpercentage = 0;
    $scope.discounttotalorder = "order";
    $scope.suspendReferenceNote = "";
    $scope.suspend = function () {
        alert($scope.suspendReferenceNote);
    };

    $scope.addcustomer = function (isValid) {

        if (isValid) {
            var loadAllProductsUrl = window.location.protocol + "//" + window.location.host + "/api/PosHome/CreateCustomer";

            var customerdetail = {
                CustomerName: $scope.name,
                Email: $scope.email,
                Phone: $scope.Phone,
                Mobile: $scope.Mobile,
                Address: $scope.address

            };
            var response = $http({
                method: "post",
                url: loadAllProductsUrl,
                data: JSON.stringify(customerdetail),
                dataType: "json",
                async: false
            });

            $('#customerModal').modal('hide');
            return response.then(function (data) {
                debugger;
                if (data != null) {
                    getCustomer(function (a) {
                        debugger;
                        setTimeout(function () {

                            $("#customers").data("kendoAutoComplete").value(data.data.CustomerName);
                        }, 100);

                    });
                    function getCustomer(a) {
                        debugger;
                        $("#customers").data("kendoAutoComplete").refresh();
                        return a();
                    }
                }
                else {
                    toastr.error("Customer Does Not Exists.");
                }

            });

        }
        else {
            toastr.error("Error");
        }

    };


   

    $scope.paymentmodel = function () {
        // alert("payment");
        $('#payModal').modal({ backdrop: 'static' });
    };
    $scope.updateTaxpercentage = function () {
        alert($scope.taxpercentage);
        $('#tsModal').modal('hide');
    };
    $scope.suspendHoldbtn = function () {
        $('#susModal').modal({ backdrop: 'static' });
    };
    $scope.taxLabel = function () {
        $('#tsModal').modal({ backdrop: 'static' });
        return false;
    };
    $scope.discountlable = function () {
        $('#dsModal').modal({ backdrop: 'static' });
        return false;
    };

    $scope.setPage = function (page) {
        if (page < 1 || page > $scope.pager.totalPages) {
            return;
        }
        if ($scope.Products.length > 0) {
            var filteredItem;
            if ($scope.searchText != "" || $scope.searchText != null) {
                filteredItem= $filter('filter')($scope.Products, { ProductName: $scope.searchText });
            } else {
                filteredItem =$scope.Products;
            }
           // var Product = $scope.Products;
            $scope.pager = GetPager(filteredItem, page, 18);
        }

    }
    function GetPager(Product, currentPage, pageSize) {
        totalItems = Product.length;
        // default to first page
        currentPage = currentPage || 1;

        // default page size is 10
        pageSize = pageSize || 18;

        // calculate total pages
        var totalPages = Math.ceil(totalItems / pageSize);

        var startPage, endPage;
        if (totalPages <= 10) {
            // less than 10 total pages so show all
            startPage = 1;
            endPage = totalPages;
        } else {
            // more than 10 total pages so calculate start and end pages
            if (currentPage <= 6) {
                startPage = 1;
                endPage = 10;
            } else if (currentPage + 4 >= totalPages) {
                startPage = totalPages - 9;
                endPage = totalPages;
            } else {
                startPage = currentPage - 5;
                endPage = currentPage + 4;
            }
        }

        // calculate start and end item indexes
        var startIndex = (currentPage - 1) * pageSize;
        var endIndex = Math.min(startIndex + pageSize - 1, totalItems - 1);

        // create an array of pages to ng-repeat in the pager control
        var pages = _.range(startPage, endPage + 1);

        var products = getFilterData(startIndex, endIndex, Product);

        function getFilterData(min, max, step) {
            if (max == undefined) {
                max = min;
                min = 0;
            }
            step = Math.abs(step) || 1;
            if (min > max) {
                step = -step;
            }
            // building the array
            var output = [];
            for (var value = min; value <= max; value += step) {
                output.push(Product[value]);
            }

            return output;
        }
        return {
            Products: products,
            totalItems: totalItems,
            currentPage: currentPage,
            pageSize: pageSize,
            totalPages: totalPages,
            startPage: startPage,
            endPage: endPage,
            startIndex: startIndex,
            endIndex: endIndex,
            pages: pages
        };
    }

    // ng-model="discounttotalorder"
    PosService.getAllProduct().then(function (d) {
        $scope.Products = d.data;
        initController();

        function initController() {
            // initialize to page 1
            $scope.setPage(1);
        }


    }, function () { alert("Error at: GetAllProducts") });
    function isEmpty(obj) {
        return Object.keys(obj).length === 0;
    }


    //PosService.getAllCustomer().then(function (d) {
    //    $scope.Customers = d.data;
    //}, function () { alert("Error at: GetAllCustomers") });
    //function isEmpty(obj) {
    //    return Object.keys(obj).length === 0;
    //}

    $scope.updateDiscount = function () {
        alert($scope.disountper);
        alert($scope.discounttotalorder);
        $('#dsModal').modal('hide');
    };

    //$scope.ChangePaginate = function () {
    //    debugger;
    //    GetPager($scope.filterProduct);
    //}
    $scope.searchByText = function (value) {
        if (event.keyCode === 13) {
            if (value.length === 1) {
                add(value[0]);
            }
        }
        var searchText = $scope.searchText;
        var filteredItem = $filter('filter')($scope.Products, { ProductName: searchText });
        $scope.pager = GetPager(filteredItem);
    }

    $scope.add = function (item) {
        add(item);
    }

    function add(item) {

        $scope.orderedItemCnt = 1;
        var foodItem = {
            orderedItemCnt: 1,
            totalPrice: item.Price,
            itemId: item.ProductCode,
            id: $scope.itemsCnt,
            discount: 0,
            item: item
        };

        // Find if the item is already in Cart
        var cartItems = $.grep($scope.order, function (e) { return e.itemId == item.ProductCode; });

        if (cartItems.length > 0 && !isEmpty($scope.order)) {
            cartItems[0].orderedItemCnt = ++cartItems[0].orderedItemCnt;
            cartItems[0].totalPrice = item.Price * cartItems[0].orderedItemCnt;
        }
        else {
            $scope.order.push(foodItem);
            $scope.itemsCnt = $scope.order.length;
        }
    };
    $scope.cancleOrder = function () {
        //}
        bootbox.confirm("Are You Sure", function (result) {
            alert(result);
            if (result) {
                $scope.snote = "";
                $scope.itemsCnt = 1;
                $scope.order = [];
                $scope.isDisabled = true;
            }
        });

    };
    $scope.getSum = function () {
        var i = 0,
          sum = 0;

        for (; i < $scope.order.length; i++) {
            sum += parseInt($scope.order[i].totalPrice, 10);
        }
        return sum;
    };
    $scope.productname = "";
    $scope.updateItem = function () {
        $('#proModal').modal('hide');
    };
    $scope.showdetail = function (item, index) {
        $scope.productname = item.item.ProductName;
        $scope.productindex = index;
        item.totalPrice = (item.item.Price * item.orderedItemCnt) - item.discount;
        $('#proModal').modal({ backdrop: 'static' });
    };
    $scope.addItem = function (item, index) {
        item.orderedItemCnt = ++item.orderedItemCnt;
        item.totalPrice = item.item.price * item.orderedItemCnt;
    };
    $scope.uploadnote = function () {
        alert($scope.snote);
        console.log($scope.snote);
        $('#noteModal').modal('hide');
    };
    $scope.subtractItem = function (item, $index) {
        if (item.orderedItemCnt > 1) {
            item.orderedItemCnt = --item.orderedItemCnt;
            item.totalPrice = item.item.price * item.orderedItemCnt;
        }
        else {
            $scope.isDisabled = true;
            // isDisabled = false;    
            // $("#SubstractItemBtn").prop("disabled", true);
        }
    }

    $scope.deleteItem = function (index) {
        $scope.order.splice(index, 1);
    };

    $scope.checkout = function (index) {
        alert("Order total: $" + $scope.getSum() + "\n\nPayment received. Thanks.");
    };

    $scope.clearOrder = function () {
        $scope.order = [];
    };
});


app.factory('PosService', function ($http) {
    //var fac = {};
    //fac.getAllProduct = function () {
    //    return $http.get('/api/PosHome/GetProducts');
    //}
    //fac.getItemByName = function (value) {
    //    return $http.get('/api/PosHome/GetProductsByValue', { value: value});
    //}
    //return fac;
    return {
        getAllProduct: function () {
            return $http.get('/api/PosHome/GetProducts');
        },
        getItemByName: function (value) {
            return $http.get({ method: 'GET', url: '/api/PosHome/GetProductsByValue', value: value });
        }



    };
    //fac.getAllCustomer = function () {
    //    return 

    //}

    //return fac;
    // return fac;
});



