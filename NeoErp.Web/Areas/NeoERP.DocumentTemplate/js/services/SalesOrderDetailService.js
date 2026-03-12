//templatesetupservice
// write all api get/post/put/delete code here
DTModule.factory('salesorderdetailfactory', function ($http) {
    var serv = {};
    serv.GetEntity = function () {
      
    }

    

    serv.getallSalesOrderdata = function () {
        return $http.get('/api/TemplateApi/GetAllSalesOrderDetails');
    }


    return serv;
});

// write all controller calculation part here
DTModule.service('salesorderdetailservice', function (salesorderdetailfactory) {

    this.GetAllSalesOrderData = function () {
        return salesorderdetailfactory.getallSalesOrderdata();
    }

});