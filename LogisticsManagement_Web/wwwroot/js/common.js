/**
 * All common JavaScript functions are declared here
 * Please use them as required and 
 * try not to duplicate the same type of operations in different .js files; 
 * in stead create a common function here. Do necessary refactoring on the way.
 * Written by: Kazi on 25 Apr 2019
 * 
 */


function GetObject(actionUrl) {
    var returnObject = null;

    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'GET',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}
function GetList(actionUrl) {
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
function GetListByObject(actionUrl, objectData) {
    var returnObject = null;
    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'GET',
        data: { jsonStringParam: JSON.stringify(objectData) },
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
function GetListById(actionUrl, id) {
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
function GetNumberOfList(actionUrl, count) {
    var returnObject = null;
    $.ajax({
        'async': false,
        url: actionUrl + '/' + count,
        type: 'GET',
        dataType: 'json',
        count: count,
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}
function GetSingleById(actionUrl, id) {

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

function PerformPostAction(actionUrl) {
    var returnObject = null;
    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}
function PerformPostActionWithObject(actionUrl, dataArray) {
    var returnObject = null;
    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            returnObject = result;
        },
        error: function (result) {
            returnObject = result;
        }
    });

    return returnObject;
}
function PerformPostActionWithId(actionUrl, id) {
    var returnObject = null;
    $.ajax({
        'async': false,
        url: actionUrl + '/' + id,
        type: 'POST',
        data: id,
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
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


/**
 * Converts date to US format in yyyy-MM-dd HH:mm.
 * Returns the date and time 
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
function ConvertDateToUSFormat(date) {
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