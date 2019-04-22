

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

function GetListObjectByParam(actionUrl, paramData) {

    var returnObject = null;

    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'GET',
        data: { jsonStringParam: JSON.stringify(paramData) },
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

function AddEntry(actionUrl, dataArray) {
    var returnObject = null;

    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 0) {
                returnObject = result;
                bootbox.alert('Success! Data saved successfully. ');
            }
            else {
                bootbox.alert('Failed! An error occurred while saving data. Please check your input and try again.');
            }

        },
        error: function (result) {

        }
    });

    return returnObject;
}

function UpdateEntry(actionUrl, dataArray) {
    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 1) {
                bootbox.alert('Success! Data updated successfully. ');
            }
            else {
                bootbox.alert('Failed! An error occurred while saving data. Please check your input and try again.');
            }

        },
        error: function (result) {

        }
    });
}

function RemoveEntry(actionUrl, id) {
    $.ajax({
        'async': false,
        url: actionUrl + '/' + id,
        type: 'POST',
        data: id,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            bootbox.alert('Success! Data has been removed successfully.');
        },
        error: function (result) {
            bootbox.alert('Failed! An error occured during deleting the data.');
        }
    });
}

function MaskPhoneNumber(element) {
    $(element).mask('000-000-0000');
}

function MaskDate(element) {
    $(element).mask('00/00/0000');
}

//function SetResponseMessage(responseType, messageContent) {
//    $('#lblMsgContent').text(messageContent);
//    $('#imgAlertType').removeAttr('src');
//    $('#btnProceed').hide();
//    $('#btnClose').show();
    
//    $('#msgBox').slideDown('slow', function () {
    
//    });

//    if (responseType === 'Info') {
//        $('#alertBox').css('background-color', '#daf2ff');
//        $('#imgAlertType').attr('src', '/images/icon-archive/info.png');
//    }
//    else if (responseType === 'Warning') {
//        $('#alertBox').css('background-color', '#f5f6d4');
//        $('#imgAlertType').attr('src', '/images/icon-archive/warning.png');
//        $('#btnProceed').show();
//        $('#btnClose').hide();
//    }
//    else if (responseType === 'Success') {
//        $('#alertBox').css('background-color', '#e1f2da');
//        $('#imgAlertType').attr('src', '/images/icon-archive/tick.png');
//    }
//    else if (responseType === 'Failed') {
//        $('#alertBox').css('background-color', '#fee5e5');
//        $('#imgAlertType').attr('src', '/images/icon-archive/fail.png');
//    }
//    else if (responseType === 'Validation') {
//        $('#alertBox').css('background-color', '#fee5e5');
//        $('#imgAlertType').attr('src', '/images/icon-archive/stop.png');
//    }

//}



/**
 * Converts date to US format in yyyy-MM-dd HH:mm.
 * Returns only the date part
 * @param {any} date pass the date or datetime
 * @returns {string} - Returns the date part in 'yyyy-MM-dd  HH:mm' format
 */
function ConvertDatetimeToUSDatetime(date) {
    var dt = new Date(date);
    var day = dt.getDate();
    var month = dt.getMonth() + 1; // month starts from 0; where 0 = January
    var year = dt.getFullYear();

    var hour = dt.getHours();
    var minute = dt.getMinutes();
    hour = hour < 10 ? '0' + hour : hour;
    minute = minute < 10 ? '0' + minute : minute;


    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;


    var yyyymmddHHmm = year + '-' + month + '-' + day + "T" + hour + ":" + minute;
    return yyyymmddHHmm;
}

/**
 * ConvertDateToUSFormat converts date to US format in yyyy-MM-dd.
 * Returns only the date part
 * @param {any} date pass the date or datetime
 * @returns {string} - Returns the date part in yyyy-MM-dd format
 */
function ConvertDateToUSFormat(date)
{
    var dt = new Date(date);
    var day = dt.getDate();
    var month = dt.getMonth() + 1; // month starts from 0; where 0 = January
    var year = dt.getFullYear();

    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;


    var yyyymmdd = year + '-' + month + '-' + day; 
    return yyyymmdd;
}

/**
 * Returns only the Time part in HH:mm format // like 15:30 which is 03:30 PM
 * @param {any} date pass the date or datetime; 
 * @returns {string} - returns the time part in 'HH:mm' format
 */
function GetTimeInHHmmFormat(date) {
    var dt = new Date(date);

    var hour = dt.getHours();
    var minute = dt.getMinutes();
    hour = hour < 10 ? '0' + hour : hour;
    minute = minute < 10 ? '0' + minute : minute;

    var timeAmPm = hour + ':' + minute; 

    return timeAmPm;
}