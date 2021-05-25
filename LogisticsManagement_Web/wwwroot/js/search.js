$(document).ready(function () {

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });
});

$('#ddlSearchItem').on('change', function () {
    itemname = $(this).val();

    if (itemname == 'order') {
        $('.fororder').show(); //prop('display', 'block');
        $('.forinvoice').hide(); //prop('display', 'none');
        $('.forcustomer').hide(); //prop('display', 'none');
    }
    if (itemname == 'invoice') {
        $('.fororder').hide();
        $('.forinvoice').show();
        $('.forcustomer').hide();
    }
    if (itemname == 'customer') {
        $('.fororder').hide();
        $('.forinvoice').hide();
        $('.forcustomer').show();
    }
});
$('#searchByWb').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForWayBill').prop('disabled', false);
        $('#txtWaybillNumber').prop('disabled', false);
    }
    else {
        $('#optionForWayBill').prop('disabled', true);
        $('#txtWaybillNumber').prop('disabled', true);
        $('#txtWaybillNumber').val('');
        $('#optionForWayBill').val('1');
    }
});
$('#searchByBillToCustomer').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForBillTo').prop('disabled', false);
        $('#ddlBillToCustomer').prop('disabled', false);
    }
    else {
        $('#optionForBillTo').prop('disabled', true);
        $('#ddlBillToCustomer').prop('disabled', true);
        $('#ddlBillToCustomer').val('');
        $('#optionForBillTo').val('1');
    }
});
$('#searchByShipper').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForShipper').prop('disabled', false);
        $('#ddlShipper').prop('disabled', false);
    }
    else {
        $('#optionForShipper').prop('disabled', true);
        $('#ddlShipper').prop('disabled', true);
        $('#ddlShipper').val('0');
        $('#optionForShipper').val('1');
    }
});
$('#searchByCctn').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForCctn').prop('disabled', false);
        $('#txtCctnRef').prop('disabled', false);
    }
    else {
        $('#optionForCctn').prop('disabled', true);
        $('#txtCctnRef').prop('disabled', true);
        $('#txtCctnRef').val('');
        $('#optionForCctn').val('1');
    }
});
$('#searchByActn').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForActn').prop('disabled', false);
        $('#txtActnRef').prop('disabled', false);
    }
    else {
        $('#optionForActn').prop('disabled', true);
        $('#txtActnRef').prop('disabled', true);
        $('#txtActnRef').val('');
        $('#optionForActn').val('1');
    }
});
$('#searchByConsignee').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForConsignee').prop('disabled', false);
        $('#ddlConsignee').prop('disabled', false);
    }
    else {
        $('#optionForConsignee').prop('disabled', true);
        $('#ddlConsignee').prop('disabled', true);
        $('#ddlConsignee').val('0');
        $('#optionForConsignee').val('1');
    }
});
$('#searchByDlRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForDelRef').prop('disabled', false);
        $('#txtDelRef').prop('disabled', false);
    }
    else {
        $('#optionForDelRef').prop('disabled', true);
        $('#txtDelRef').prop('disabled', true);
        $('#txtDelRef').val('');
        $('#optionForDelRef').val('1');
    }
});
$('#searchByDlRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForDelRef').prop('disabled', false);
        $('#txtDelRef').prop('disabled', false);
    }
    else {
        $('#optionForDelRef').prop('disabled', true);
        $('#txtDelRef').prop('disabled', true);
        $('#txtDelRef').val('');
        $('#optionForDelRef').val('1');
    }
});
$('#searchByPuRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForPuRef').prop('disabled', false);
        $('#txtPuRef').prop('disabled', false);
    }
    else {
        $('#optionForPuRef').prop('disabled', true);
        $('#txtPuRef').prop('disabled', true);
        $('#txtPuRef').val('');
        $('#optionForPuRef').val('1');
    }
});
$('#searchByOrderBy').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForOrderBy').prop('disabled', false);
        $('#txtOrderBy').prop('disabled', false);
    }
    else {
        $('#optionForOrderBy').prop('disabled', true);
        $('#txtOrderBy').prop('disabled', true);
        $('#txtOrderBy').val('');
        $('#optionForOrderBy').val('1');
    }
});
$('#searchByBolRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForBolRef').prop('disabled', false);
        $('#txtBolRef').prop('disabled', false);
    }
    else {
        $('#optionForBolRef').prop('disabled', true);
        $('#txtBolRef').prop('disabled', true);
        $('#txtBolRef').val('');
        $('#optionForBolRef').val('1');
    }
});
$('#searchByProRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForProRef').prop('disabled', false);
        $('#txtProRef').prop('disabled', false);
    }
    else {
        $('#optionForProRef').prop('disabled', true);
        $('#txtProRef').prop('disabled', true);
        $('#txtProRef').val('');
        $('#optionForProRef').val('1');
    }
});
$('#searchByCustomerRef').on('click', function () {
    var isChecked = $(this).is(':checked');
    if (isChecked == true) {
        $('#optionForCustomerRef').prop('disabled', false);
        $('#txtCustomerRef').prop('disabled', false);
    }
    else {
        $('#optionForCustomerRef').prop('disabled', true);
        $('#txtCustomerRef').prop('disabled', true);
        $('#txtCustomerRef').val('');
        $('#optionForCustomerRef').val('1');
    }
});
$('#btnSearch').on('click', function () {
    var fromDate = $('#fromDate').val();
    var toDate = $('#toDate').val();

    var optionForWaybill = $('#optionForWayBill').val();
    var waybillNumber = $('#txtWaybillNumber').val();
    var optionForBillTo = $('#optionForBillTo').val();
    var billToCustomerName = $('#ddlBillToCustomer').val();
    var optionForShipper = $('#optionForShipper').val();
    var shipperCustomerName = $('#ddlShipper').val();
    var optionForCctn = $('#optionForCctn').val();
    var cctnRef = $('#txtCctnRef').val();
    var optionForActn = $('#optionForActn').val();
    var actnRef = $('#txtActnRef').val();
    var optionForConsignee = $('#optionForConsignee').val();
    var consigneeCustomerName = $('#ddlConsignee').val();
    var optionForDelRef = $('#optionForDelRef').val();
    var delRef = $('#txtDelRef').val();
    var optionForPuRef = $('#optionForPuRef').val();
    var puRef = $('#txtPuRef').val();
    var optionForOrderBy = $('#optionForOrderBy').val();
    var orderBy = $('#txtOrderBy').val();
    var optionForBolRef = $('#optionForBolRef').val();
    var bolRef = $('#txtBolRef').val();
    var optionForProRef = $('#optionForProRef').val();
    var proRef = $('#txtProRef').val();
    var optionForCustomerRef = $('#optionForCustomerRef').val();
    var customerRef = $('#txtCustomerRef').val();

    var data = {
        fromDate: fromDate,
        toDate: toDate,
        optionForWaybill: optionForWaybill,
        waybillNumber: waybillNumber,
        optionForBillTo: optionForBillTo,
        billToCustomerName: billToCustomerName,
        optionForShipper: optionForShipper,
        shipperCustomerName: shipperCustomerName,
        optionForCctn: optionForCctn,
        cctnRef: cctnRef,
        optionForActn: optionForActn,
        actnRef: actnRef,
        optionForConsignee: optionForConsignee,
        consigneeCustomerName: consigneeCustomerName,
        optionForDelRef: optionForDelRef,
        delRef: delRef,
        optionForPuRef: optionForPuRef,
        puRef: puRef,
        optionForOrderBy: optionForOrderBy,
        orderBy: orderBy,
        optionForBolRef: optionForBolRef,
        bolRef: bolRef,
        optionForProRef: optionForProRef,
        proRef: proRef,
        optionForCustomerRef: optionForCustomerRef,
        customerRef: customerRef
    };

    var type = parseInt($('#ddlSearchItem').val());

    if (type == 0) {
        $('#loadSearchResult').load('Search/DeliveryOrderSearchResult?filteredData=' + encodeURI(JSON.stringify(data)));
    }
    else if (type == 1) {
        $('#loadSearchResult').load('Search/MiscOrderSearchResult?filteredData=' + encodeURI(JSON.stringify(data)));
    }

    



});

