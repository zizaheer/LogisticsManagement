

function GetListObject(actionUrl) {

    var returnObject = null;

    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'GET',
        dataType: 'json',
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}

function GetSingleObjectById(actionUrl, id) {

    var returnObject = null;

    $.ajax({
        'async': false,
        url: actionUrl + '/' + id,
        type: 'GET',
        dataType: 'json',
        id: id,
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}

function MaskPhoneNumber(element) {
    $(element).mask('000-000-0000');
}

function MaskDate(element) {
    $(element).mask('00/00/0000');
}