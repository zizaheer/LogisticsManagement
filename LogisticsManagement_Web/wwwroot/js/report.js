//#region document.ready

$(document).ready(function () {

    var currentDate = new Date();
    currentDate.setDate(currentDate.getDate() - 14);

    $('#txtStartDate').val(ConvertDateToUSFormat(currentDate));
    $('#txtEndDate').val(ConvertDateToUSFormat(new Date));

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });


});


//#endregion 

//#region Local Variables

var selectedCustomersForReport = [];

//#endregion

//#region Events 


$('#customer-list .chkCustomerId').change(function (event) {
    //event.preventDefault();

    var isChecked = $(this).is(':checked');
    var customerId = $(this).data('customerid');

    var index = selectedCustomersForReport.indexOf(customerId);
    if (index >= 0) {
        selectedCustomersForReport.splice(index, 1);
    }

    if (isChecked) {
        selectedCustomersForReport.push(customerId);
    }
});



$('#customer-list').on('change', '#chkCheckAllCustomers', function (event) {
    event.preventDefault();

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkCustomerId').prop('checked', true);
        var customerIdString = $('#hfcustomerArray').val();
        selectedCustomersForReport = [];
        var customerIdArray = customerIdString.split(',');
        $.each(customerIdArray, function (i, item) {
            if (item !== '') {
                selectedCustomersForReport.push(parseInt(item));
            }
        });
    } else {
        $('.chkCustomerId').prop('checked', false);
        selectedCustomersForReport = [];
    }
});
$('#btnShowReport').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedCustomersForReport.length < 1) {
        bootbox.alert('Please select customer/s to generate report.');
        event.preventDefault();
        return;
    }

    var startDate = $('#txtStartDate').val();
    var endDate = $('#txtEndDate').val();

    var reportParam = { "customers": selectedCustomersForReport, "startDate": startDate, "endDate": endDate };

    $.ajax({
        'async': false,
        url: 'SalesReportGenerated',
        type: 'POST',
        data: JSON.stringify(reportParam),
        dataType: 'html',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 0) {
                var w = window.open();
                $(w.document.body).html(result);
            }
        },
        error: function (result) {
            bootbox.alert('Report generation failed or there is no data found. Please check your report criteria.');
        }
    });

});


$('#btnShowCustomerWiseDueReport').unbind().on('click', function (event) {
    event.preventDefault();

    var customerId = $('.ddlcustomers').val();
    var isPaidIncluded = $('#chkIsPaidIncluded').is(':checked') ? true : false;
    if (customerId == "" || parseInt(customerId) < 1) {
        bootbox.alert('Please select a customer to generate the report.');
        event.preventDefault();
        return;
    }

    var startDate = $('#txtStartDate').val();
    var endDate = $('#txtEndDate').val();
    var reportParam = { "customers": customerId, "startDate": startDate, "endDate": endDate, "isPaidIncluded": isPaidIncluded };

    $.ajax({
        'async': false,
        url: "PrintCustomerDueReportAsPdf",
        type: 'POST',
        data: JSON.stringify(reportParam),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 0) {
                window.open(result, "_blank");
            }
        },
        error: function (result) {
            bootbox.alert('Printing failed! There might not be any data to print. Please check and try again.');
        }
    });

});



//#endregion







