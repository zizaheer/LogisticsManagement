

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

function AddEntry(actionUrl, dataArray) {
    $.ajax({
        'async': false,
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 1) {
                SetResponseMessage('Success', 'Success! Data saved successfully. ');
            }
            else {
                SetResponseMessage('Failed', 'Failed! An error occurred while saving data. Please check your input and try again.');
            }

        },
        error: function (result) {

        }
    });
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
                SetResponseMessage('Success', 'Success! Data saved successfully. ');
            }
            else {
                SetResponseMessage('Failed', 'Failed! An error occurred while saving data. Please check your input and try again.');
            }

        },
        error: function (result) {

        }
    });
}



function RemoveEntry(actionUrl, dataArray) {
    $.ajax({
        url: actionUrl,
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            SetResponseMessage('Success', 'Success! Data has been removed successfully.');
        },
        error: function (result) {
            SetResponseMessage('Failed', 'Failed! An error occured during deleting the data.');
        }
    });
}


function MaskPhoneNumber(element) {
    $(element).mask('000-000-0000');
}

function MaskDate(element) {
    $(element).mask('00/00/0000');
}


function SetResponseMessage(responseType, messageContent) {
    $('#lblMsgContent').text(messageContent);
    $('#alertBox').css('style');
    $('#imgAlertType').removeAttr('src');
    $('#btnProceed').hide();
    $('#btnClose').show();
    $('#msgBox').show();

    if (responseType === 'Info') {
        $('#alertBox').css('background-color', '#daf2ff');
        $('#imgAlertType').attr('src', '/images/icon-archive/info.png');
    }
    else if (responseType === 'Warning') {
        $('#alertBox').css('background-color', '#f5f6d4');
        $('#imgAlertType').attr('src', '/images/icon-archive/warning.png');
        $('#btnProceed').show();
        $('#btnClose').hide();
    }
    else if (responseType === 'Success') {
        $('#alertBox').css('background-color', '#e1f2da');
        $('#imgAlertType').attr('src', '/images/icon-archive/tick.png');
    }
    else if (responseType === 'Failed') {
        $('#alertBox').css('background-color', '#fee5e5');
        $('#imgAlertType').attr('src', '/images/icon-archive/fail.png');
    }
    else if (responseType === 'Validation') {
        $('#alertBox').css('background-color', '#fee5e5');
        $('#imgAlertType').attr('src', '/images/icon-archive/stop.png');
    }

}