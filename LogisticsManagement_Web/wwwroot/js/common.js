

function GetListObject(controllerName, actionName) {

    var actionUrl = controllerName + '/' + actionName;

    $.ajax({
        url: actionUrl,
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            return result;
        },
        error: function (result) {
            return result;
        }
    });




}