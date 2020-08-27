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

var selectedOrdersForDispatch = [];

//#endregion

//#region Events 


$('#customer-list .chkCustomerId').change(function (event) {
    //event.preventDefault();

    var isChecked = $(this).is(':checked');
    var customerId = $(this).data('customerid');

    var index = selectedOrdersForDispatch.indexOf(customerId);
    if (index >= 0) {
        selectedOrdersForDispatch.splice(index, 1);
    }

    if (isChecked) {
        selectedOrdersForDispatch.push(customerId);
    }
});



$('#customer-list').on('change', '#chkCheckAllCustomers', function (event) {
    event.preventDefault();

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkCustomerId').prop('checked', true);
        var customerIdString = $('#hfcustomerArray').val();
        selectedOrdersForDispatch = [];
        var customerIdArray = customerIdString.split(',');
        $.each(customerIdArray, function (i, item) {
            if (item !== '') {
                selectedOrdersForDispatch.push(parseInt(item));
            }
        });
    } else {
        $('.chkCustomerId').prop('checked', false);
        selectedOrdersForDispatch = [];
    }
});
$('#btnShowReport').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedOrdersForDispatch.length < 1) {
        bootbox.alert('Please select customer/s to generate report.');
        event.preventDefault();
        return;
    }

    var startDate = $('#txtStartDate').val();
    var endDate = $('#txtEndDate').val();

    var reportParam = { "customers": selectedOrdersForDispatch, "startDate": startDate, "endDate": endDate };

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


//#endregion







