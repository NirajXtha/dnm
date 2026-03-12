jQuery.validator.addMethod("multipleEmail", function (value, element) {
    var emailArray = value.toString().split(',');
    var validate = true;
    $.each(emailArray, function (index) {
        var re = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
        validate = validate && re.test($.trim(emailArray[index].toString()));
    });
    return validate;
}, 'Please enter a valid email address.');