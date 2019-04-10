

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
        $('#alertBox').css('background-color', '##f5f6d4');
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