// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

// Please refer to bundleconfig.json where the js files are being minified as site.min.js which is referenced on _Layout template.
$(document).ready(function () {

});

function SetAlertType(alertType, messageContent) {
    $('#lblModalTitle').text('');
    $('#modalDialogType').removeClass();
    $('#imgAlertType').removeAttr('src');
    $('#btnProceed').show();
    $('#modalContent').text(messageContent);

    if (alertType === 'Info') {
        $('#lblModalTitle').text('Information');
        $('#modalDialogType').addClass('modal-dialog modal-lg modal-info');
        $('#imgAlertType').attr('src', '/images/icon-archive/info.png');
    }
    else if (alertType === 'Warning') {
        $('#lblModalTitle').text('Warning!');
        $('#modalDialogType').addClass('modal-dialog modal-lg modal-warning');
        $('#imgAlertType').attr('src', '/images/icon-archive/warning.png');
    }
    else if (alertType === 'Success') {
        $('#lblModalTitle').text('Operation successful!');
        $('#modalDialogType').addClass('modal-dialog modal-lg modal-success');
        $('#imgAlertType').attr('src', '/images/icon-archive/tick.png');
        $('#btnProceed').hide();
    }
    else if (alertType === 'Failed') {
        $('#lblModalTitle').text('Operation failed!');
        $('#modalDialogType').addClass('modal-dialog modal-lg modal-danger');
        $('#imgAlertType').attr('src', '/images/icon-archive/fail.png');
        $('#btnProceed').hide();
    }
    else if (alertType === 'Validation') {
        $('#lblModalTitle').text('Validation failed!');
        $('#modalDialogType').addClass('modal-dialog modal-lg modal-danger');
        $('#imgAlertType').attr('src', '/images/icon-archive/stop.png');
        $('#btnProceed').hide();
    }
   
}