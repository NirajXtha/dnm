var Admin = (function (admin, $, ko) {
    "use strict"
    admin.UserDetail = function () {
        var config = {

        };
        var viewModel = {
            UserDetail: ko.observableArray(),
            id: ko.observable(),
            Name: ko.observable(),
            Address: ko.observable(),
            MobileNo: ko.observable(),
            users: ko.observableArray(),
            optionValue: ko.observableArray(["Admin", "User"]),
            selectedrolevalue:ko.observable(),
            userList:function(users)
            {
                viewModel.users(users);
            },
            AddUserArray: function () {
                viewModel.users.push({
                    Name: "",
                    Address: "",
                    MobileNo: "",
                    Id: 0,
                });
            },
            removeUserArray: function (users) {
                viewModel.users.remove(users);
            },
            addUser: function () {
                var test = $("#form1").valid();
               // alert(test);
                if (test == false)
                {
                   // alert("false");
                    return false;
                }
               
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddCustomer",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: ko.toJSON(viewModel.users),
                    success: function (data) {
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveCustomer",
                    type: "POST",
                    data: {
                        id: viewModel.id(),
                      

                    },
                    success: function (data) {
                        $(tr).remove();
                        //  viewModel.id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                        502: function (data) {
                            // alert(data);
                            var message = Messages.content.save.noaccess;
                            toastr[message.method](data, data.responseText);
                        },
                    }


                };
                $.ajax(setting);
            },
          
        };
        return {
            config: config,
            viewModel:viewModel,
            init: function (uservalue) {
                viewModel.UserDetail(uservalue);
                ko.applyBindings(viewModel);
            },
            render: function () { },

        }

    };
    admin.Debtors = function () {
        var config = {

        };
        var viewModel = {
            UserDetail: ko.observableArray(),
            Name: ko.observable(),
            Address: ko.observable(),
            MobileNo: ko.observable(),
            users: ko.observableArray(),
            optionValue: ko.observableArray(["Admin", "User"]),
            selectedrolevalue: ko.observable(),
            Id:ko.observable(),
            userList: function (users) {
                viewModel.users(users);
            },
            AddUserArray: function () {
                viewModel.users.push({
                    Name: "",
                    Address: "",
                    MobileNo: "",
                    Id: 0,
                });
            },
            removeUserArray: function (users) {
                viewModel.users.remove(users);
            },
            addUser: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddDebtors",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: ko.toJSON(viewModel.users),
                    success: function (data) {
                        viewModel.users.remove();
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            viewModel.users.remove();
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveDebtor",
                    type: "POST",
                    data: {
                        Id: viewModel.Id(),


                    },
                    success: function (data) {
                        $(tr).remove();
                        //  viewModel.id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                        502: function (data) {
                            // alert(data);
                            var message = Messages.content.save.noaccess;
                            toastr[message.method](data, data.responseText);
                        },
                    }


                };
                $.ajax(setting);
            },

        };
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                viewModel.UserDetail(uservalue);
                ko.applyBindings(viewModel);
            },
            render: function () { },

        }

    };
    admin.ItemSetup = function () {
        var config = {

        };
        var viewModel = {
            UserDetail: ko.observableArray(),
            Name: ko.observable(),
            price: ko.observable(),
            id:ko.observable(),
            Units: ko.observableArray(["Meter", "kg", "liter","Piece"]),
            unit:ko.observable(),
            Items: ko.observableArray(),

            userList: function (users) {
                viewModel.Items(users);
            },
            AddUserArray: function () {
                viewModel.Items.push({
                    Name: "",
                    price: "",
                    id:0,
                    Unit: "",
                    Units: ko.observableArray(["Piece", "Kg", "liter"]),
                });
            },
            removeUserArray: function (users) {
                viewModel.Items.remove(users);
            },
            addUser: function () {
                alert(viewModel.unit());
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddItem",
                    type: "POST",
                    contentType: 'application/json; charset=utf-8',
                    data: ko.toJSON(viewModel.Items),
                    success: function (data) {
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/Removeitem",
                    type: "POST",
                    data: {
                        id: viewModel.id(),
                        Name: "",
                        Price: "",
                        
                    },
                    success: function (data) {
                        $(tr).remove();
                      //  viewModel.id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                        502: function (data) {
                           // alert(data);
                            var message = Messages.content.save.noaccess;
                            toastr[message.method](data, data.responseText);
                        },
                    }


                };
                $.ajax(setting);
            },


        };
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                viewModel.UserDetail(uservalue);
                ko.applyBindings(viewModel);
            },
            render: function () { },

        }

    };

    admin.ExpTrans = function () {
        var config = {

        };
        var viewModel = {
            UserDetail: ko.observableArray(),
            ItemName: ko.observable(),
            Quantity: ko.observable(),
            Rate: ko.observable(),
            transTypeArray: ko.observableArray(["Cash"]),
            transtype: ko.observable("Cash"),
            DebtorName: ko.observable(),
            //TotalAmount: ko.computed(function () {
            //    return viewModel.Quantity();
            //}),
            TotalAmount: ko.observable(),
            TransDate:ko.observable(),
            Remarks: ko.observable(),
            Id:ko.observable(0),
            Items: ko.observableArray(),
          
            userList: function (users) {
                viewModel.Items(users);
            },
           
            addUser: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddExpSingleTrans",
                    type: "POST",
                 //   contentType: 'application/json; charset=utf-8',
                    data: {
                        ItemName: viewModel.ItemName(),
                        Quantity: viewModel.Quantity(),
                        Rate: viewModel.Rate(),
                        TotalAmount: viewModel.TotalAmount(),
                        TransDate: viewModel.TransDate(),
                        Remarks: viewModel.Remarks(),
                        TransType: viewModel.transtype(),
                        CreatedBy: viewModel.DebtorName(),
                        Id:viewModel.Id(),
                    },
                    success: function (data) {
                        viewModel.Id(0);
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveExpTrans",
                    type: "POST",
                    data: {
                        Id: viewModel.Id(),
                        //  CustomerId: viewModel.CustomerId(),
                        // Rate: viewModel.price(),
                        // Hour: viewModel.Quantity(),
                        // Remark: viewModel.Remarks(),
                        // TransDate: viewModel.TransDate(),
                        //  TransType: viewModel.transtype(),
                        // TotalAmount: viewModel.TotalAmount(),
                    },
                    success: function (data) {
                        $(tr).remove();
                        viewModel.Id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
        };
 
        viewModel.TotalAmount = ko.computed(function () {
            return viewModel.Rate() * viewModel.Quantity();
        }, viewModel);
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
              //  viewModel.UserDetail(uservalue);
                ko.applyBindings(viewModel);
            },
            render: function () {
                $("#transdate").datepicker();
            },

        }

    };

    admin.ExpFixTrans = function () {
        var config = {

        };
        var viewModel = {
            DebtorDetail: ko.observableArray(),
            itemStockAvalible: ko.observableArray(),
            ItemName: ko.observable(),
            Quantity: ko.observable(),
            Rate: ko.observable(),
            transTypeArray: ko.observableArray(["Cash", "Credit"]),
            transtype: ko.observable("Cash"),
            DebtorId: ko.observable(),
            ItemPrice: ko.observable(),
            //TotalAmount: ko.computed(function () {
            //    return viewModel.Quantity();
            //}),
            TotalAmount: ko.observable(),
            TransDate: ko.observable(),
            Remarks: ko.observable(),
            Id: ko.observable(0),
            Items: ko.observableArray(),
            UserDetail: ko.observableArray(),
            itemsDetail: ko.observableArray([]),
            userList: function (users) {
                viewModel.Items(users);
            },

            addUser: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddFixExpTransection",
                    type: "POST",
                    //   contentType: 'application/json; charset=utf-8',
                    data: {
                        ItemId: viewModel.ItemName(),
                        Quantity: viewModel.Quantity(),
                        Rate: viewModel.Rate(),
                        TotalAmount: viewModel.TotalAmount(),
                        TransDate: viewModel.TransDate(),
                        Remarks: viewModel.Remarks(),
                        TransType: viewModel.transtype(),
                        DebtorsId: viewModel.DebtorId(),
                        Id: viewModel.Id(),
                    },
                    success: function (data) {
                        viewModel.Id(0);
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveFixExpTransection",
                    type: "POST",
                    data: {
                        Id: viewModel.Id(),
                        //  CustomerId: viewModel.CustomerId(),
                        // Rate: viewModel.price(),
                        // Hour: viewModel.Quantity(),
                        // Remark: viewModel.Remarks(),
                        // TransDate: viewModel.TransDate(),
                        //  TransType: viewModel.transtype(),
                        // TotalAmount: viewModel.TotalAmount(),
                    },
                    success: function (data) {
                        $(tr).remove();
                        viewModel.Id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

        };

        viewModel.TotalAmount = ko.computed(function () {
            return viewModel.Rate() * viewModel.Quantity();
        }, viewModel);
        viewModel.ItemPrice.subscribe(function (newvalue) {
            var match = ko.utils.arrayFirst(viewModel.itemsDetail(), function (item) {
                return item.Price == newvalue;
            });
            viewModel.ItemName(match.Id);
            viewModel.Rate(newvalue);
           
            
        }, viewModel);
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                //  viewModel.UserDetail(uservalue);
                ko.applyBindings(viewModel);
                $.ajax({
                    type: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetDebtors",
                    data: {},
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        var i;
                        for (i = 0; i < data.itemList.length; i += 1) {
                            var datagroup = data.itemList[i];
                            var datagroupResult = {
                                Name: datagroup.Name,
                                Price: datagroup.Price,
                                Id: datagroup.Id,
                            }
                            viewModel.itemsDetail.push(datagroupResult);
                        };
                        for (i = 0; i < data.CreditorList.length; i += 1) {
                            var datagroup = data.CreditorList[i];
                            var datagroupResult = {
                                Name: datagroup.Name + " - " + datagroup.Address+"",
                                Id: datagroup.Id,
                            }
                            viewModel.DebtorDetail.push(datagroupResult);
                        }
                    },
                    error: function () {
                        alert('Error loading PatientID=' + id);
                    }
                });
            },
            render: function () {
                $("#transdate").datepicker();
            },

        }

    };


    admin.IncomeTrans = function () {
        var config = {

        };
        var viewModel = {
            CustomerDetail:ko.observableArray([]),
            itemsDetail: ko.observableArray([]),
            transTypeArray:ko.observableArray(["Cash","Credit"]),
            transtype:ko.observable(),
            ItemPrice: ko.observable(),
            CustomerId:ko.observable(),
            price:ko.observable(),
            UserDetail: ko.observableArray(),
            ItemName: ko.observable(),
            Quantity: ko.observable(),
            Rate: ko.observable(),
            TotalAmount: ko.observable(),
            TransDate: ko.observable(),
            Remarks: ko.observable(),
            Id: ko.observable(0),
            Items: ko.observableArray(),
            itemStockAvalible: ko.observableArray(),
            stockavail: ko.observable(0),
            stockArray:ko.observableArray(),
            addUser: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddIncomeTrack",
                    type: "POST",
                    data: {
                        CustomerId: viewModel.CustomerId(),
                        ItemId: viewModel.ItemName(),
                        price: viewModel.price(),
                        Quantity: viewModel.Quantity(),
                        Remarks: viewModel.Remarks(),
                        TranDate: viewModel.TransDate(),
                        TransType: viewModel.transtype(),
                        TotalAmount: viewModel.TotalAmount(),
                        Id: viewModel.Id(),

                    },
                    success: function (data) {
                        viewModel.Id(0);
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                        $.ajax({
                            type: 'GET',
                            url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetItemNew",
                            data: {},
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (data) {
                                var i;
                               
                                viewModel.itemStockAvalible.removeAll();
                                for (i = 0; i < data.itemavalible.length; i += 1) {
                                    var datagroup = data.itemavalible[i];
                                    var datagroupResult = {
                                        ItemId: datagroup.ItemId,
                                        AvaliableStock: datagroup.AvaliableStock,
                                        Id: datagroup.Id,
                                    }
                                    viewModel.itemStockAvalible.push(datagroupResult);
                                };

                            },
                            error: function () {
                                alert('something error so please reload ');
                            }
                        });
                    },
                    statusCode: {
                        202: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },
            RemoveUser: function (tr) {

                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveIncomeTrans",
                    type: "POST",
                    data: {
                        Id: viewModel.Id(),
                        //  CustomerId: viewModel.CustomerId(),
                        // Rate: viewModel.price(),
                        // Hour: viewModel.Quantity(),
                        // Remark: viewModel.Remarks(),
                        // TransDate: viewModel.TransDate(),
                        //  TransType: viewModel.transtype(),
                        // TotalAmount: viewModel.TotalAmount(),
                    },
                    success: function (data) {
                        $(tr).remove();
                        viewModel.Id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

        };
       
       
        viewModel.TotalAmount = ko.computed(function () {
            return viewModel.price() * viewModel.Quantity();
        }, viewModel);

        viewModel.ItemPrice.subscribe(function (newvalue) {
            var match = ko.utils.arrayFirst(viewModel.itemsDetail(), function (item) {
                return item.Price == newvalue;
            });
            viewModel.ItemName(match.Id);
            viewModel.price(newvalue);
            var matchtes = ko.utils.arrayFirst(viewModel.itemStockAvalible(), function (item) {
                return item.ItemId == match.Id;
            });
            viewModel.stockavail(matchtes.AvaliableStock);
            var i;
            viewModel.stockArray.removeAll();
            for (i = 1; i <= viewModel.stockavail(); i += 1) {
               
                var datagroupResult = {
                    number: i,
                   
                }
                viewModel.stockArray.push(datagroupResult);
            };
            
        }, viewModel);
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                //  viewModel.UserDetail(uservalue);
                $("#transdate").datepicker();
                ko.applyBindings(viewModel);
                $.ajax({
                    type: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetItem",
                    data: { },
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        var i;
                             //   viewModel.itemsDetail([]);
                        for (i = 0; i < data.itemList.length; i += 1) {
                            var datagroup = data.itemList[i];
                            var datagroupResult = {
                                Name: datagroup.Name,
                                Price: datagroup.Price,
                                Id: datagroup.Id,
                            }
                            viewModel.itemsDetail.push(datagroupResult);
                        };
                        for (i = 0; i < data.CustomerList.length; i += 1) {
                            var datagroup = data.CustomerList[i];
                            var datagroupResult = {
                                Name: datagroup.Name + "               - " + datagroup.Address + "",
                                Id: datagroup.Id,
                            }
                            viewModel.CustomerDetail.push(datagroupResult);
                        }
                        for (i = 0; i < data.itemavalible.length; i += 1) {
                            var datagroup = data.itemavalible[i];
                            var datagroupResult = {
                                ItemId: datagroup.ItemId,
                                AvaliableStock: datagroup.AvaliableStock,
                                Id: datagroup.Id,
                            }
                            viewModel.itemStockAvalible.push(datagroupResult);
                        };
                      
                    },
                    error: function () {
                        alert('Error loading PatientID=' + id);
                    }
                });

            },
            render: function () {

            },

        }

    };
    admin.IncomeHourTrans = function () {
        var config = {

        };
        var viewModel = {
            CustomerDetail: ko.observableArray([]),
            itemsDetail: ko.observableArray([]),
            transTypeArray: ko.observableArray(["Cash", "Credit"]),
            transtype: ko.observable(),
            ItemPrice: ko.observable(),
            CustomerId: ko.observable(),
            price: ko.observable(0),
            UserDetail: ko.observableArray(),
            ItemName: ko.observable(),
            Quantity: ko.observable(0),
            Rate: ko.observable(),
            TotalAmount: ko.observable(0),
            TransDate: ko.observable(),
            Remarks: ko.observable(),
            Min:ko.observable(0),
            Items: ko.observableArray(),
            Id:ko.observable(0),

            addUser: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/AddIncomeHour",
                    type: "POST",
                    data: {
                        CustomerId: viewModel.CustomerId(),
                        Id:viewModel.Id(),
                        Rate: viewModel.price(),
                        Hour: viewModel.Quantity(),
                        Remark: viewModel.Remarks(),
                        TransDate: viewModel.TransDate(),
                        TransType: viewModel.transtype(),
                        TotalAmount: viewModel.TotalAmount(),
                        Minval: viewModel.Min(),
                    },
                    success: function (data) {
                        viewModel.Id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

            RemoveUser: function (tr) {
               
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/RemoveIncomeHour",
                    type: "POST",
                    data: {
                        Id: viewModel.Id(),
                      //  CustomerId: viewModel.CustomerId(),
                       // Rate: viewModel.price(),
                       // Hour: viewModel.Quantity(),
                       // Remark: viewModel.Remarks(),
                       // TransDate: viewModel.TransDate(),
                      //  TransType: viewModel.transtype(),
                       // TotalAmount: viewModel.TotalAmount(),
                    },
                    success: function (data) {
                        $(tr).remove();
                        viewModel.Id(0)
                        var message = Messages.content.save.success;
                        toastr[message.method](data, data);
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

        };

        viewModel.TotalAmount = ko.computed(function () {
            var min = (viewModel.Min() / 60) * viewModel.price();
            var totalamount = (viewModel.price() * viewModel.Quantity());
           // alert(min);
            return Number(totalamount) +Number(min);
        }, viewModel);

       
        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                //  viewModel.UserDetail(uservalue);
                $("#transdate").datepicker();
                ko.applyBindings(viewModel);
                $.ajax({
                    type: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetItem",
                    data: {},
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        var i;
                        //   viewModel.itemsDetail([]);
                        for (i = 0; i < data.CustomerList.length; i += 1) {
                            var datagroup = data.CustomerList[i];
                            var datagroupResult = {
                                Name: datagroup.Name + "               - " + datagroup.Address+"",
                                Id: datagroup.Id,
                            }
                            viewModel.CustomerDetail.push(datagroupResult);
                        }

                    },
                    error: function () {
                        alert('Error loading PatientID=' + id);
                    }
                });

                
            },
            render: function () {

            },

        }

    };

    admin.TakeFromCustomer = function () {
        var config = {

        };
        var viewModel = {
            CustomerDetail: ko.observableArray([]),
            
            transTypeArray: ko.observableArray(["Cash", "Credit"]),
            transtype: ko.observable(),
            CustomerId: ko.observable(),
            totalCredit: ko.observable(),
            totalCash: ko.observable(),
            cashRecived: ko.observable(),
            Remarks:ko.observable(),
            searchCustomer: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetTotalRecord",
                    type: "GET",
                    data: {
                        CustomerId: viewModel.CustomerId(),
                    },
                    success: function (data) {
                        viewModel.totalCash(data.totalCashReceived);
                        viewModel.totalCredit(data.totalCashCredit);
                      
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

            ReceivedCash: function () {
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/PaidCashReceived",
                    type: "POST",
                    data: {
                        CustomerId: viewModel.CustomerId(),
                        Remarks: viewModel.Remarks(),
                        TotalPaid:viewModel.cashRecived(),
                    },
                    success: function () {
             
                        var totalPaid = Number(viewModel.totalCash()) + Number(viewModel.cashRecived());
                      var  totalCredit = viewModel.totalCredit() - viewModel.cashRecived();
                        viewModel.totalCash(totalPaid);
                        viewModel.totalCredit(totalCredit);
                        var message = Messages.content.save.success;
                        toastr[message.method](data, "Cash Received.");
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

        };

       


        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                //  viewModel.UserDetail(uservalue);
               
                ko.applyBindings(viewModel);
                $.ajax({
                    type: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetItem",
                    data: {},
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        var i;
                        //   viewModel.itemsDetail([]);
                        for (i = 0; i < data.CustomerList.length; i += 1) {
                            var datagroup = data.CustomerList[i];
                            var datagroupResult = {
                                Name: datagroup.Name + "               - " + datagroup.Address + "",
                                Id: datagroup.Id,
                            }
                            viewModel.CustomerDetail.push(datagroupResult);
                        }

                    },
                    error: function () {
                        alert('Error loading PatientID=' + id);
                    }
                });

             
            },
            render: function () {

            },

        }

    };
    admin.TakeFromDebtors = function () {
        var config = {

        };
        var viewModel = {
            CustomerDetail: ko.observableArray([]),
            transtype: ko.observable(),
            CustomerId: ko.observable(),
            totalCredit: ko.observable(),
            totalCash: ko.observable(),
            cashRecived: ko.observable(),
            Remarks: ko.observable(),
            searchCustomer: function () {
                var test = $("#form1").valid();
                // alert(test);
                if (test == false) {
                    // alert("false");
                    return false;
                }
                $("#search").button("loading...");
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetDebtorsRecord",
                    type: "GET",
                    data: {

                        CustomerId: viewModel.CustomerId(),
                    },
                    success: function (data) {
                        $("#search").button("reset");
                        viewModel.totalCash(data.totalCashReceived);
                        viewModel.totalCredit(data.totalCashCredit);

                    },
                    statusCode: {
                        200: function (data) {
                            $("#search").button("reset");
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

            ReceivedCash: function () {
                var setting = {
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/PaidCashDebtors",
                    type: "POST",
                    data: {
                        DebtorId: viewModel.CustomerId(),
                        Remarks: viewModel.Remarks(),
                        TotalPaid: viewModel.cashRecived(),
                    },
                    success: function () {

                        var totalPaid = Number(viewModel.totalCash()) + Number(viewModel.cashRecived());
                        var totalCredit = viewModel.totalCredit() - viewModel.cashRecived();
                        viewModel.totalCash(totalPaid);
                        viewModel.totalCredit(totalCredit);
                        var message = Messages.content.save.success;
                        toastr[message.method](data, "Cash Received.");
                    },
                    statusCode: {
                        200: function (data) {
                            var message = Messages.content.save.noaccess;
                            toastr[message](data, data);
                        },
                    }


                };
                $.ajax(setting);
            },

        };




        return {
            config: config,
            viewModel: viewModel,
            init: function (uservalue) {
                //  viewModel.UserDetail(uservalue);

                ko.applyBindings(viewModel);
                $.ajax({
                    type: 'GET',
                    url: window.location.protocol + "//" + window.location.host + "/Api/admin/GetDebtors",
                    data: {},
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        var i;
                        //   viewModel.itemsDetail([]);
                        for (i = 0; i < data.CreditorList.length; i += 1) {
                            var datagroup = data.CreditorList[i];
                            var datagroupResult = {
                                Name: datagroup.Name + " - " + datagroup.Address + "",
                                Id: datagroup.Id,
                            }
                            viewModel.CustomerDetail.push(datagroupResult);
                        }
                       
                    },
                    error: function () {
                        alert('Error loading PatientID=' + id);
                    }
                });


            },
            render: function () {

            },

        }

    };
    return admin;
}(Admin || {},jQuery,ko));