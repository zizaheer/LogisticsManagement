//#region document.ready

$(document).ready(function () {

    //if ($.fn.dataTable.isDataTable('#service-list')) {
    //    $('#service-list').DataTable().destroy();
    //}
    //$('#service-list').DataTable({
    //    'paging': false, // hinds pagination
    //    "bInfo": false, // hides the footer Showing Results n of n
    //    'bFilter': false //hides the searchbox
    //});

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });

    $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(new Date));
    $('#txtDispatchDatetimeForNewOrders').val(ConvertDatetimeToUSDatetime(new Date));

    $(function () {
        var canvas = document.querySelector('#signatureCanvas');
        var pad = new SignaturePad(canvas);

        $('#btnOk').click(function () {
            var data = pad.toDataURL();
            $('#imgSignature').val(data);
            pad.off();
        });

        $('#btnCancel').click(function () {
            pad.clear();
            pad.on();
            $('#imgSignature').val('');
        });
    });

});

//#endregion 

//#region Local Variables
var employeeData;
var paidByValue = '1';

var shipperCityId = 0;
var consigneeCityId = 0;
var deliveryOptionId = 0;
var vehicleTypeId = 0;
var unitTypeId = 0;
var unitQuantity = 0;
var weightScaleId = 0;
var weightQuantity = 0;
var fuelSurchargePercentage = 0.0;
var discountPercentage = 0.0;

// global tax amount
var taxPercentage = 0.0;
taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

var baseOrderCost = 0.0;
var baseFuelSurchargeAmount = 0.0;
var baseDiscountAmount = 0.0;
var baseTaxAmount = 0.0;

var overriddenOrderCost = 0.0;
var overriddenFuelSurchargeAmount = 0.0;
var overriddenDiscountAmount = 0.0;
var overriddenTaxAmount = 0.0;

var finalOrderCost = 0.0;
var finalFuelSurchargeAmount = 0.0;
var finalDiscountAmount = 0.0;
var finalTaxAmount = 0.0;

var selectedAdditionalServiceArray = [];
var totalAdditionalServiceCost = 0.0;

var selectedOrdersForDispatch = [];

//#endregion

//#region Events 

$('#btnNew').on('click', function () {
    $('#txtWayBillNo').removeAttr('readonly');
});

$('input[type=radio][name=rdoPaidBy]').change(function () {
    paidByValue = this.value;
    $('#ddlBillerId').change();
});

$('input[name=chkIsReturnOrder]').change(function () {

    var wayBillNo = $('#txtWayBillNo').val();
    var isChecked = $(this).is(':checked');

    var toggle = $(this).data('bs.toggle');

    if (isChecked) {
        if (wayBillNo === "" && wayBillNo < 1) {
            toggle.off(true);
            bootbox.alert("Return order can only be created for an existing order. Please enter way bill number.");
            return;
        }

        GetAndFillOrderDetailsByWayBillNumber(wayBillNo, 2);
    }
    else {
        if (wayBillNo !== "" && wayBillNo > 0) {
            GetAndFillOrderDetailsByWayBillNumber(wayBillNo, 1);
        }
    }

    $('#lblOrderTypeText').text('This is a return order.');
});

$('#txtBillerCustomerNo').keypress(function (event) {

    if (event.keyCode === 13) {
        event.preventDefault();

        var id = $('#txtBillerCustomerNo').val();
        $('#ddlBillerId').val(id);

        if ($('#ddlBillerId').val() === null) {
            $('#ddlBillerId').val(0);
            bootbox.alert('Customer not found');
            return;
        }
        $('#ddlBillerId').change();
    }

});

$('#txtShipperCustomerNo').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var id = $('#txtShipperCustomerNo').val();
        $('#ddlShipperId').val(id);

        if ($('#ddlShipperId').val() === null) {
            $('#ddlShipperId').val(0);
            bootbox.alert('Customer not found');
        }
        else {
            $('#ddlShipperId').change();
        }

    }
});

$('#txtConsigneeCustomerNo').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var id = $('#txtConsigneeCustomerNo').val();
        $('#ddlConsigneeId').val(id);

        if ($('#ddlConsigneeId').val() === null) {
            $('#ddlConsigneeId').val(0);
            bootbox.alert('Customer not found');
        }
        else {
            $('#ddlConsigneeId').change();
        }
    }
});

$('#ddlBillerId').on('change', function () {

    $('#txtBillerCustomerNo').val($('#ddlBillerId').val());

    if (paidByValue === '1') {
        $('#ddlShipperId').val($('#ddlBillerId').val());
        $('#ddlShipperId').change();
    }
    else if (paidByValue === '2') {
        $('#ddlConsigneeId').val($('#ddlBillerId').val());
        $('#ddlConsigneeId').change();
    }

    var customerInfo = JSON.parse(GetCustomerInfo($('#ddlBillerId').val()));

    if (customerInfo !== null) {
        if (customerInfo.FuelSurChargePercentage > 0) {
            $('#txtFuelSurchargePercent').val(customerInfo.FuelSurChargePercentage);
        }
        $('#txtDiscountPercent').val(customerInfo.DiscountPercentage);
        $('#chkIsGstApplicable').prop('checked', customerInfo.IsGstApplicable);
    }

});

$('#ddlShipperId').on('change', function () {

    var shipperAddress = null;
    var selectedValue = $('#ddlShipperId').val();

    var shipperInfo = JSON.parse(GetCustomerInfo(selectedValue));
    if (shipperInfo !== null || shipperInfo !== undefined) {
        var mailingAddId = shipperInfo.MailingAddressId;
        var billingAddId = shipperInfo.BillingAddressId;

        if (mailingAddId !== null) {
            shipperAddress = JSON.parse(GetAddressInfo(mailingAddId));
        }
        else if (billingAddId !== null) {
            shipperAddress = JSON.parse(GetAddressInfo(billingAddId));
        }

        if (shipperAddress !== null) {
            $('#txtShipperCustomerNo').val($('#ddlShipperId').val());
            $('#txtShipperAddressline').val(shipperAddress.AddressLine);
            $('#txtShipperUnitNo').val(shipperAddress.UnitNumber);
            $('#ddlShipperCityId').val(shipperAddress.CityId);
            $('#ddlShipperProvinceId').val(shipperAddress.ProvinceId);
            $('#txtShipperPostcode').val(shipperAddress.PostCode);

        }
    }

    if (paidByValue === '1') {
        $('#ddlBillerId').val(selectedValue);
    }
    else if (paidByValue === '2') {
        $('#ddlBillerId').val($('#ddlConsigneeId').val());
    }

    $('#txtUnitQuantity').change();
});

$('#ddlConsigneeId').on('change', function () {

    var consigneeAddress = null;
    var selectedValue = $('#ddlConsigneeId').val();

    var consigneeInfo = JSON.parse(GetCustomerInfo(selectedValue));
    if (consigneeInfo !== null || consigneeInfo !== undefined) {

        var mailingAddId = consigneeInfo.MailingAddressId;
        var billingAddId = consigneeInfo.BillingAddressId;

        if (mailingAddId !== null) {
            consigneeAddress = JSON.parse(GetAddressInfo(mailingAddId));
        }
        else if (billingAddId !== null) {
            consigneeAddress = JSON.parse(GetAddressInfo(billingAddId));
        }

        if (consigneeAddress !== null) {
            $('#txtConsigneeCustomerNo').val($('#ddlConsigneeId').val());
            $('#txtConsigneeAddressline').val(consigneeAddress.AddressLine);
            $('#txtConsigneeUnitNo').val(consigneeAddress.UnitNumber);
            $('#ddlConsigneeCityId').val(consigneeAddress.CityId);
            $('#ddlConsigneeProvinceId').val(consigneeAddress.ProvinceId);
            $('#txtConsigneePostcode').val(consigneeAddress.PostCode);
        }
    }

    if (paidByValue === '2') {
        $('#ddlBillerId').val(selectedValue);
    }
    else if (paidByValue === '1') {
        $('#ddlBillerId').val($('#ddlShipperId').val());
    }

    $('#txtUnitQuantity').change();

});

$('#txtUnitQuantity').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtUnitQuantity').change();
    }
});
$('#txtUnitQuantity').on('change', function (event) {
    CalculateOrderBaseCost();
});

$('#txtFuelSurchargePercent').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtFuelSurchargePercent').change();
    }
});
$('#txtFuelSurchargePercent').on('change', function (event) {
    $('#txtUnitQuantity').change();
});

$('#txtDiscountPercent').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtDiscountPercent').change();
    }
});
$('#txtDiscountPercent').on('change', function (event) {
    $('#txtUnitQuantity').change();
});

$('#txtOverriddenOrderCost').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtOverriddenOrderCost').change();
    }
});
$('#txtOverriddenOrderCost').on('change', function (event) {
    $('#txtUnitQuantity').change();
});

$('#service-list .chkAddService').change(function (event) {
    event.preventDefault();

    //$(this).prop('disabled', true);
    var isChecked = $(this).is(':checked');

    var serviceId = $(this).data('serviceid');
    var payToDriver = $(this).closest('tr').children('td:nth-child(2)').find('.chkPayToDriver').is(':checked');
    var serviceFee = $(this).closest('tr').children('td:nth-child(3)').find('.txtServiceFee').val();
    var driverPercentage = $(this).closest('tr').children('td:nth-child(4)').find('.txtDriverPercentage').val();
    var isGstApplicable = $(this).closest('tr').children('td:nth-child(5)').find('.chkIsGstApplicableForService').is(':checked');

    if (serviceFee === "") {
        bootbox.alert("Please enter service charge before adding it to order.");
        $(this).prop('checked', false);
        return;
    }


    if (payToDriver) {
        if (driverPercentage === "") {
            bootbox.alert("Please enter driver percentage before adding it to order.");
            $(this).prop('checked', false);
            return;
        }
    }


    var serviceData = {
        orderId: 0,
        additionalServiceId: serviceId,
        driverPercentageOnAddService: driverPercentage === "" ? 0 : parseFloat(driverPercentage),
        additionalServiceFee: parseFloat(serviceFee),
        isTaxAppliedOnAddionalService: isGstApplicable,
        taxAmountOnAdditionalService: taxPercentage
    };


    var index = selectedAdditionalServiceArray.findIndex(c => c.additionalServiceId === serviceData.additionalServiceId);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }

    if (isChecked) {
        selectedAdditionalServiceArray.push(serviceData);
    }

    CalculateAdditionalServiceCost();
    CalculateOrderBaseCost();

});

$('#chkIsGstApplicable').on('change', function () {
    $('#txtUnitQuantity').change();
});

$('#txtWayBillNo').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        $('#txtWayBillNo').change();
    }
});
$('#txtWayBillNo').on('change', function (event) {
    var wayBillNumber = $('#txtWayBillNo').val();
    GetAndFillOrderDetailsByWayBillNumber(wayBillNumber, 1);
});



$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This waybill number will be deleted along with all relavant data. Are you sure to proceed?", function (result) {
        if (result === true) {
            RemoveEntry('Order/Remove', waybillNumber);
            $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
        }
    });
});

$('#btnNewOrder').unbind().on('click', function () {
    ClearForm();
    $('#frmOrderForm').trigger('reset');
});

$('#frmOrderForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmOrderForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    var result;
    var parseData;

    if (dataArray[0].unitQuantity < 1 || dataArray[0].shipperCustomerId < 1 || dataArray[0].consigneeCustomerId < 1) {
        bootbox.alert('Shipper, consignee and unit quantity is required!');
        event.preventDefault();
        return;
    }

    if ($('input[name=chkIsReturnOrder]:checked').val() === 'on') {
        if (dataArray[0].wayBillNumber === "" || dataArray[0].wayBillNumber < 1) {
            bootbox.alert("Return order can only be created for an existing order. Please enter way bill number.");
            $('#txtWayBillNo').focus();
            event.preventDefault();
            return;
        }
    }

    if (dataArray[0].isPrintedOnWayBill === true) {
        if (dataArray[0].commentsForWayBill === null) {
            bootbox.alert('Waybill comments required when you choose to print them!');
            event.preventDefault();
            return;
        }
    }
    if (dataArray[0].isPrintedOnInvoice === true) {
        if (dataArray[0].commentsForInvoice === null) {
            bootbox.alert('Invoice comments required when you choose to print them!');
            event.preventDefault();
            return;
        }
    }


    if (dataArray[0].wayBillNumber > 0) {
        UpdateEntry('Order/Update', dataArray);
    }
    else {
        result = AddEntry('Order/Add', dataArray);
        if (result !== null) {
            parseData = JSON.parse(result);
            $('#txtWayBillNo').val(parseData.WayBillNumber);
            $('#hfOrderId').val(parseData.OrderId);
        }
    }
    event.preventDefault();
    $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
    selectedAdditionalServiceArray = [];
});

$('.btnPickup').unbind().on('click', function () {
    var orderId = $(this).data('orderid');
    $('#txtOrderIdForPickupModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForPickupModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForPickupModal').val(orderInfo.DispatchedDatetime);

        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForPickupModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        if (orderInfo.PickupDatetime !== null) {
            $('#txtPickupDateTime').val(orderInfo.PickupDatetime);
        }
        else
        {
            $('#txtPickupDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }

        $('#txtPickupWaitTime').val(orderInfo.PickupWaitTimeHour);
    }
});
$('#btnSavePickup').unbind().on('click', function () {
    var waitTime = $('#txtPickupWaitTime').val();
    var pickupDate = $('#txtPickupDateTime').val();
    var dispatchedDate = $('#txtDispatchDateTimeForPickupModal').val();
    var wayBillNumber = $('#txtWayBillNoForPickupModal').val();
    var orderId = $('#txtOrderIdForPickupModal').val();

    if (pickupDate <= dispatchedDate) {
        bootbox.alert('Pickup date must be greater than dispatch date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, pickupDate, orderId];

    UpdateEntry('Order/UpdatePickupStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemovePickup').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForPickupModal').val();
    RemoveEntry('Order/RemovePickupStatus', orderId);

    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});

$('.btnPasson').unbind().on('click', function () {

    var orderId = $(this).data('orderid');
    $('#txtOrderIdForPassOnModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForPassOnModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForPassOnModal').val(orderInfo.DispatchedDatetime);

        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForPassOnModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        if (orderInfo.PassOffDatetime !== null) {
            $('#txtPassOnDateTime').val(orderInfo.PassOffDatetime);
        }
        else
        {
            $('#txtPassOnDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }
        $('#txtPickupDateTimeForPassOnModal').val(orderInfo.PickupDatetime);
        $('#ddlPassOnEmployeeId').val(orderInfo.PassedOffToEmployeeId);
        $('#txtPassOnEmployeeNumber').val($('#ddlPassOnEmployeeId').val());
        $('#txtPassOnWaitTime').val(orderInfo.PassOffWaitTimeHour);
    }
});
$('#txtPassOnEmployeeNumber').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        $('#ddlPassOnEmployeeId').val($('#txtPassOnEmployeeNumber').val());
    }

});
$('#btnSavePassOn').unbind().on('click', function () {
    var waitTime = $('#txtPassOnWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForPassOnModal').val();
    var passOnDate = $('#txtPassOnDateTime').val();
    var passOnEmployeeId = $('#ddlPassOnEmployeeId').val();
    var orderId = $('#txtOrderIdForPassOnModal').val();
    var wayBillNumber = $('#txtWayBillNoForPassOnModal').val();

    if (passOnDate <= pickupDate) {
        bootbox.alert('Pass-on date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, passOnEmployeeId, passOnDate, orderId];

    UpdateEntry('Order/UpdatePassonStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemovePassOn').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForPassOnModal').val();
    RemoveEntry('Order/RemovePassonStatus', orderId);

    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});

$('.btnDeliver').unbind().on('click', function () {
    var orderId = $(this).data('orderid');
    $('#txtOrderIdForDeliverModal').val(orderId);
    var wayBillNumber = $(this).data('waybillnumber');
    $('#txtWayBillNoForDeliverModal').val(wayBillNumber);

    var orderInfo = JSON.parse(GetSingleObjectById('Order/GetOrderStatusByOrderId', orderId));

    if (orderInfo !== null) {
        $('#txtDispatchDateTimeForDeliverModal').val(orderInfo.DispatchedDatetime);


        $('#ddlEmployeeId').val(orderInfo.DispatchedToEmployeeId);
        $('#txtDispatchEmployeeNameForDeliverModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        $('#ddlEmployeeId').val(orderInfo.PassedOffToEmployeeId);
        $('#txtPassOnEmployeeNameForDeliverModal').val($("#ddlEmployeeId option:selected").text());
        $('#ddlEmployeeId').val(0); // reset <select>

        $('#txtPickupDateTimeForDeliverModal').val(orderInfo.PickupDatetime);
        if (orderInfo.DeliveredDatetime !== null) {
            $('#txtDeliveryDateTime').val(orderInfo.DeliveredDatetime);
        }
        else
        {
            $('#txtDeliveryDateTime').val(ConvertDatetimeToUSDatetime(new Date));
        }
        
        $('#txtDeliveryWaitTime').val(orderInfo.DeliveryWaitTimeHour);
        $('#txtReceivedByName').val(orderInfo.ReceivedByName);
        $('#txtDeliveryNote').val(orderInfo.ProofOfDeliveryNote);
        DrawSignatureImage(orderInfo.ReceivedBySignature);

    }
});
$('#btnSaveDeliver').unbind().on('click', function () {
    var waitTime = $('#txtDeliveryWaitTime').val();
    var pickupDate = $('#txtPickupDateTimeForDeliverModal').val();
    var deliveryDate = $('#txtDeliveryDateTime').val();
    var receivedByName = $('#txtReceivedByName').val();
    var deliveryNote = $('#txtDeliveryNote').val();
    var receivedBySign = $('#imgSignature').val();
    var orderId = $('#txtOrderIdForDeliverModal').val();

    var wayBillNumber = $('#txtWayBillNumber').val();

    if (deliveryDate <= pickupDate) {
        bootbox.alert('Delivery date must be greater than pickup date.');
        event.preventDefault();
        return;
    }

    var dataArray = [wayBillNumber, waitTime, deliveryDate, deliveryNote, receivedByName, receivedBySign, orderId];

    UpdateEntry('Order/UpdateDeliveryStatus', dataArray);

    event.preventDefault();
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});
$('#btnRemoveDelivery').unbind().on('click', function () {
    var orderId = $('#txtOrderIdForDeliverModal').val();
    RemoveEntry('Order/RemoveDeliveryStatus', orderId);

    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
});

$('.btnRemoveDispatch').unbind().on('click', function () {
    var wayBillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This will also remove the pickup and delivery status. Are you sure you want to remove?", function (result) {
        if (result === true) {
            RemoveEntry('Order/RemoveDispatchStatus', wayBillNumber);
            $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
            $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        }
    });
});

$('#btnDispatch').unbind().on('click', function (event) {
    event.preventDefault();
    var selectedEmployeeId = $('#ddlEmployeeId').val();
    var dispatchDate = $('#txtDispatchDatetimeForNewOrders').val();

    if (selectedEmployeeId < 1) {
        bootbox.alert('Please select an employee to dispatch the order/s');
        event.preventDefault();
        return;
    }

    if (dispatchDate === null || dispatchDate === "")
    {
        bootbox.alert('Please enter dispatch date');
        event.preventDefault();
        return;
    }


    if (selectedOrdersForDispatch.length < 1) {
        bootbox.alert('Please select order/s to be dispatched from the order list.');
        event.preventDefault();
        return;
    }

    var dataArray = [selectedOrdersForDispatch, selectedEmployeeId, dispatchDate];

    UpdateEntry('Order/UpdateDispatchStatus', dataArray);

    event.preventDefault();
    $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
    selectedOrdersForDispatch = [];

});
$('#order-list .chkDispatchToEmployee').change(function (event) {
    //event.preventDefault();

    var isChecked = $(this).is(':checked');
    var orderId = $(this).data('waybillnumber');

    var index = selectedOrdersForDispatch.indexOf(orderId);
    if (index >= 0) {
        selectedOrdersForDispatch.splice(index, 1);
    }

    if (isChecked) {
        selectedOrdersForDispatch.push(orderId);
    }
});
$('#order-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    ClearForm();
    $('#frmOrderForm').trigger('reset');
   

    var wbNumber = $(this).data('waybillnumber');

    GetAndFillOrderDetailsByWayBillNumber(wbNumber, 1);
    //$('#txtWayBillNo').attr('readonly', true);

});

//#endregion


//#region Private methods

function GetAndFillOrderDetailsByWayBillNumber(wayBillNumber, orderTypeId) {
    var orderData = null;
    var orderAdditionalServiceData = null;

    var orderInfo = GetSingleObjectById('Order/GetOrderDetailsByWayBillId', wayBillNumber);
    var parseData = JSON.parse(orderInfo);

    orderData = parseData.orderPocos.filter(function (item) {
        return item.OrderTypeId === orderTypeId;
    })[0];

    if (orderData !== null && orderData !== undefined) {
        orderAdditionalServiceData = parseData.orderAdditionalServices.filter(function (item) {
            return item.OrderId === orderData.Id;
        });

        FillOrderAdditionalServices(orderAdditionalServiceData);
        FillOrderDetails(orderData);

    }
    else {

        if (orderTypeId === 2) {

            var shipperId = $('#ddlShipperId').val();
            var consigneeId = $('#ddlConsigneeId').val();
            var billerId = $('#ddlBillerId').val();

            $('#ddlShipperId').val(consigneeId);
            $('#ddlConsigneeId').val(shipperId);

            paidByValue = $('input[name=rdoPaidBy]:checked').val();
            if (paidByValue === '1') {
                paidByValue = '2';
                $('#rdoShipper').prop('checked', false);
                $('#rdoConsignee').prop('checked', true);
                $('#ddlBillerId').val($('#ddlConsigneeId').val());
            }
            else if (paidByValue === '2') {
                paidByValue = '1';
                $('#rdoConsignee').prop('checked', false);
                $('#rdoShipper').prop('checked', true);
                $('#ddlBillerId').val($('#ddlShipperId').val());
            }

            $('#ddlShipperId').change();
            $('#ddlConsigneeId').change();
            $('#ddlBillerId').change();

            $('#txtDiscountPercent').val('');
            $('#txtUnitQuantity').val('');

            selectedAdditionalServiceArray = [];
            $('#txtUnitQuantity').change();
            $('#txtOverriddenOrderCost').val('');
            $('#txtOverriddenOrderCost').change();

        }

    }


}

function GetCustomerInfo(customerId) {
    var customerInfo = GetSingleObjectById('Customer/GetCustomerById', customerId);
    return customerInfo;
}

function GetAddressInfo(addressId) {

    var addressInfo = GetSingleObjectById('Address/GetAddressById', addressId);
    return addressInfo;
}

function GetTariffInfo() {
    shipperCityId = $('#ddlShipperCityId').val();
    consigneeCityId = $('#ddlConsigneeCityId').val();
    deliveryOptionId = $('#ddlDeliveryOptionId').val();
    vehicleTypeId = $('input[type=radio][name=rdoVehicleType]:checked').val();
    unitTypeId = $('#ddlUnitTypeId').val();
    unitQuantity = $('#txtUnitQuantity').val();
    weightScaleId = $('#ddlWeightScaleId').val();
    weightQuantity = $('#txtWeightTotal').val();

    var dataForTariff = {
        shipperCityId: shipperCityId,
        consigneeCityId: consigneeCityId,
        deliveryOptionId: deliveryOptionId === null ? 0 : deliveryOptionId,
        vehicleTypeId: vehicleTypeId === null ? 0 : vehicleTypeId,
        unitTypeId: unitTypeId === null ? 0 : unitTypeId,
        unitQuantity: unitQuantity === null || unitQuantity === "" || unitQuantity === undefined ? 0 : unitQuantity,
        weightScaleId: weightScaleId === null ? 0 : weightScaleId,
        weightQuantity: weightQuantity === null || weightQuantity === "" || weightQuantity === undefined ? 0 : weightQuantity
    };

    var tariffCost = GetListObjectByParam('Order/GetTariffCostByParam', dataForTariff);
    return tariffCost;
}

function CalculateAdditionalServiceCost() {
    totalAdditionalServiceCost = 0.0;
    if (selectedAdditionalServiceArray.length > 0) {
        for (var i = 0; i < selectedAdditionalServiceArray.length; i++) {
            if (selectedAdditionalServiceArray[i].additionalServiceFee > 0) {
                totalAdditionalServiceCost = totalAdditionalServiceCost + selectedAdditionalServiceArray[i].additionalServiceFee;
                if (selectedAdditionalServiceArray[i].isTaxAppliedOnAddionalService && taxPercentage > 0) {
                    var addServiceTax = taxPercentage * selectedAdditionalServiceArray[i].additionalServiceFee / 100;
                    totalAdditionalServiceCost = totalAdditionalServiceCost + addServiceTax;
                }

            }
        }
    }

    $('#lblGrandAddServiceAmount').text(totalAdditionalServiceCost.toFixed(2));
}

function CalculateOrderBaseCost() {

    fuelSurchargePercentage = $('#txtFuelSurchargePercent').val() !== "" ? parseFloat($('#txtFuelSurchargePercent').val()) : 0.0;
    discountPercentage = $('#txtDiscountPercent').val() !== "" ? parseFloat($('#txtDiscountPercent').val()) : 0.0;
    overriddenOrderCost = $('#txtOverriddenOrderCost').val() !== "" ? parseFloat($('#txtOverriddenOrderCost').val()) : 0.0;

    var isGstApplicable = $('#chkIsGstApplicable').is(':checked');

    overriddenDiscountAmount = 0.0;
    baseDiscountAmount = 0.0;

    baseOrderCost = parseFloat(GetTariffInfo());

    $('#txtBaseOrderCost').val(baseOrderCost.toFixed(2));
    if (fuelSurchargePercentage > 0) {
        baseFuelSurchargeAmount = fuelSurchargePercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost + baseFuelSurchargeAmount;
        $('#txtBaseOrderSurcharge').val(baseFuelSurchargeAmount.toFixed(2));
    }
    if (discountPercentage > 0) {
        baseDiscountAmount = discountPercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost - baseDiscountAmount;
    }
    if (taxPercentage > 0 && isGstApplicable) {
        baseTaxAmount = taxPercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost + baseTaxAmount;
        $('#txtBaseOrderGST').val(baseTaxAmount.toFixed(2));
    }
    else {
        baseTaxAmount = 0;
        $('#txtBaseOrderGST').val('');
    }

    if (overriddenOrderCost > 0) {

        $('#lblGrandBasicCost').text($('#txtOverriddenOrderCost').val());

        if (fuelSurchargePercentage > 0) {
            overriddenFuelSurchargeAmount = fuelSurchargePercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost + overriddenFuelSurchargeAmount;
            $('#txtOverriddenOrderSurcharge').val(overriddenFuelSurchargeAmount.toFixed(2));
        }
        if (discountPercentage > 0) {
            overriddenDiscountAmount = discountPercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost - overriddenDiscountAmount;
        }
        if (taxPercentage > 0 && baseTaxAmount > 0) {
            overriddenTaxAmount = taxPercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost + overriddenTaxAmount;
            $('#txtOverriddenOrderGST').val(overriddenTaxAmount.toFixed(2));
        }
        else {
            overriddenTaxAmount = 0;
            $('#txtOverriddenOrderGST').val('');
        }
    }
    else {
        overriddenOrderCost = baseOrderCost;
        $('#lblGrandBasicCost').text($('#txtBaseOrderCost').val());

        overriddenFuelSurchargeAmount = baseFuelSurchargeAmount;
        overriddenDiscountAmount = baseDiscountAmount;
        overriddenTaxAmount = baseTaxAmount;
    }

    $('#lblGrandDiscountAmount').text(overriddenDiscountAmount.toFixed(2));
    $('#lblGrandGstAmount').text(overriddenTaxAmount.toFixed(2));
    $('#lblGrandFuelSurchargeAmount').text(overriddenFuelSurchargeAmount.toFixed(2));

    $('#lblGrandTotalOrderCost').text(overriddenOrderCost.toFixed(2));

    var grandTotal = overriddenOrderCost;

    CalculateAdditionalServiceCost();

    if (totalAdditionalServiceCost > 0) {
        grandTotal = grandTotal + totalAdditionalServiceCost;
    }

    $('#lblGrandTotalAmount').text(grandTotal.toFixed(2));

}

function FillOrderDetails(orderRelatedData) {
    if (orderRelatedData !== null) {
        $('#hfOrderId').val(orderRelatedData.Id);

        var toggle = $('input[name=chkIsReturnOrder]').data('bs.toggle');
        if (orderRelatedData.OrderTypeId === 1) {
            toggle.off(true);
            $('#lblOrderTypeText').text('This is a single order.');
        } else if (orderRelatedData.OrderTypeId === 2) {
            toggle.on(true);
            $('#lblOrderTypeText').text('This is a return order.');
        }

        $('#txtWayBillNo').val(orderRelatedData.WayBillNumber);
        $('#txtCustomerRefNo').val(orderRelatedData.ReferenceNumber);
        $('#txtCargoCtlNo').val(orderRelatedData.CargoCtlNumber);
        $('#txtAwbCtnNo').val(orderRelatedData.AwbCtnNumber);
        $('#ddlShipperId').val(orderRelatedData.ShipperCustomerId);
        $('#ddlShipperId').change();
        $('#ddlConsigneeId').val(orderRelatedData.ConsigneeCustomerId);
        $('#ddlConsigneeId').change();

        $('#ddlBillerId').val(orderRelatedData.BillToCustomerId);

        if ($('#ddlBillerId').val() === $('#ddlShipperId').val()) {
            $('#rdoShipper').prop('checked', true);
        }
        else if ($('#ddlBillerId').val() === $('#ddlConsigneeId').val()) {
            $('#rdoConsignee').prop('checked', true);
        }
        else {
            $('#rdoThirdParty').prop('checked', true);
        }

        $('#txtBillerCustomerNo').val(orderRelatedData.BillToCustomerId);
        $('#txtShipperCustomerNo').val(orderRelatedData.ShipperCustomerId);
        $('#txtConsigneeCustomerNo').val(orderRelatedData.ConsigneeCustomerId);
        $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(orderRelatedData.ScheduledPickupDate));
        $('#txtSchedulePickupTime').val(GetTimeInHHmmFormat(orderRelatedData.ScheduledPickupDate));

        //$('#txtEstimatedDeliverypDate').val(orderRelatedData.OrderId);
        $('#ddlDeliveryOptionId').val(orderRelatedData.DeliveryOptionId);

        if (orderRelatedData.VehicleTypeId === 1) {
            $('#rdoTruck').prop('checked', true);
        }
        else if (orderRelatedData.VehicleTypeId === 2) {
            $('#rdoVan').prop('checked', true);
        }
        else if (orderRelatedData.VehicleTypeId === 3) {
            $('#rdoCar').prop('checked', true);
        }

        $('#ddlUnitTypeId').val(orderRelatedData.UnitTypeId);
        $('#ddlWeightScaleId').val(orderRelatedData.WeightScaleId);
        $('#txtWeightTotal').val(orderRelatedData.WeightTotal);
        $('#txtBaseOrderCost').val(orderRelatedData.OrderBasicCost);
        $('#txtFuelSurchargePercent').val(orderRelatedData.FuelSurchargePercentage);
        $('#txtDiscountPercent').val(orderRelatedData.DiscountPercentOnOrderCost);

        //$('#txtGrandTotalOrderCost').val(orderRelatedData.OrderId);
        //$('#txtGrandAddServiceAmount').val(orderRelatedData.TotalAdditionalServiceCost);
        $('#txtContactPerson').val(orderRelatedData.ContactName);

        $('#txtContactPerson').val(orderRelatedData.ContactName);
        $('#txtContactPhone').val(orderRelatedData.ContactPhoneNumber);
        $('#txtRemarks').val(orderRelatedData.Remarks);

        $('#chkIsPrintOnWayBill').prop(orderRelatedData.isPrintedOnWayBill);
        $('#txtCommentsForWayBill').val(orderRelatedData.CommentsForWayBill);
        $('#chkIsPrintOnInvoice').val(orderRelatedData.isPrintedOnInvoice);
        $('#txtCommentsForInvoice').val(orderRelatedData.commentsForInvoice);


        $('#txtOverriddenOrderCost').val(orderRelatedData.BasicCostOverriden);
        $('#txtUnitQuantity').val(orderRelatedData.UnitQuantity);
        $('#txtUnitQuantity').change();

    }
}

function FillOrderAdditionalServices(orderAdditionalServiceData) {

    selectedAdditionalServiceArray = [];

    if (orderAdditionalServiceData !== null) {
        for (var i = 0; i < orderAdditionalServiceData.length; i++) {
            var serviceData = {
                orderId: orderAdditionalServiceData[i].OrderId,
                additionalServiceId: orderAdditionalServiceData[i].AdditionalServiceId,
                driverPercentageOnAddService: orderAdditionalServiceData[i].DriverPercentageOnAddService === "" ? 0 : parseFloat(orderAdditionalServiceData[i].DriverPercentageOnAddService),
                additionalServiceFee: parseFloat(orderAdditionalServiceData[i].AdditionalServiceFee),
                isTaxAppliedOnAddionalService: orderAdditionalServiceData[i].IsTaxAppliedOnAddionalService,
                taxAmountOnAdditionalService: orderAdditionalServiceData[i].TaxAmountOnAdditionalService
            };

            selectedAdditionalServiceArray.push(serviceData);

            // Services table row selection and fill 
            //var serviceId = $(this).closest('tr').children('td:nth-child(7)').find('.chkAddService').data('serviceid');
            //var payToDriver = $(this).closest('tr').children('td:nth-child(3)').find('.chkPayToDriver').is(':checked');
            //var driverPercentage = $(this).closest('tr').children('td:nth-child(4)').find('.txtDriverPercentage').val();
            //var serviceFee = $(this).closest('tr').children('td:nth-child(5)').find('.txtServiceFee').val();
            //var isGstApplicable = $(this).closest('tr').children('td:nth-child(6)').find('.chkIsGstApplicableForService').is(':checked');


            // row selection and fill


        }

    }
}

function GetFormData() {

    var date = $('#txtSchedulePickupDate').val();

    var orderData = {
        id: $('#hfOrderId').val() === "" ? "0" : $('#hfOrderId').val(),
        orderTypeId: $('input[name=chkIsReturnOrder]:checked').val() === 'on' ? 2 : 1,

        wayBillNumber: $('#txtWayBillNo').val() === "" ? "0" : $('#txtWayBillNo').val(),
        referenceNumber: $('#txtCustomerRefNo').val() === "" ? null : $('#txtCustomerRefNo').val(),
        cargoCtlNumber: $('#txtCargoCtlNo').val() === "" ? null : $('#txtCargoCtlNo').val(),
        awbCtnNumber: $('#txtAwbCtnNo').val() === "" ? null : $('#txtAwbCtnNo').val(),
        shipperCustomerId: $('#ddlShipperId').val(),
        consigneeCustomerId: $('#ddlConsigneeId').val(),
        billToCustomerId: $('#ddlBillerId').val(),
        scheduledPickupDate: date,
        expectedDeliveryDate: $('#txtSchedulePickupDate').val(),
        cityId: $('#ddlConsigneeCityId').val(),
        deliveryOptionId: $('#ddlDeliveryOptionId').val() === "" ? "0" : $('#ddlDeliveryOptionId').val(),
        vehicleTypeId: $("input[name='rdoVehicleType']:checked").val(),
        unitTypeId: $('#ddlUnitTypeId').val(),
        weightScaleId: $('#ddlWeightScaleId').val(),
        weightTotal: $('#txtWeightTotal').val() === "" ? null : $('#txtWeightTotal').val(),
        unitQuantity: $('#txtUnitQuantity').val() === "" ? "0" : $('#txtUnitQuantity').val(),
        orderBasicCost: $('#txtBaseOrderCost').val(),
        basicCostOverriden: $('#txtOverriddenOrderCost').val() === "" ? null : $('#txtOverriddenOrderCost').val(),
        fuelSurchargePercentage: $('#txtFuelSurchargePercent').val() === "" ? null : $('#txtFuelSurchargePercent').val(),
        discountPercentOnOrderCost: $('#txtDiscountPercent').val() === "" ? null : $('#txtDiscountPercent').val(),
        applicableGstPercent: taxPercentage <= 0 ? null : taxPercentage,
        totalOrderCost: $('#txtGrandTotalOrderCost').val(),
        totalAdditionalServiceCost: $('#txtGrandAddServiceAmount').val(),
        orderedBy: $('#txtContactPerson').val() === "" ? null : $('#txtContactPerson').val(),
        departmentName: null,
        contactName: $('#txtContactPerson').val() === "" ? null : $('#txtContactPerson').val(),
        contactPhoneNumber: $('#txtContactPhone').val() === "" ? null : $('#txtContactPhone').val(),

        commentsForWayBill: $('#txtCommentsForWayBill').val() === "" ? null : $('#txtCommentsForWayBill').val(),
        isPrintedOnWayBill: $('#chkIsPrintOnWayBill').is(':checked') === true ? 1 : 0,
        commentsForInvoice: $('#txtCommentsForInvoice').val() === "" ? null : $('#txtCommentsForInvoice').val(),
        isPrintedOnInvoice: $('#chkIsPrintOnInvoice').is(':checked') === true ? 1 : 0,

        remarks: $('#txtRemarks').val() === "" ? null : $('#txtRemarks').val()

    };

    return [orderData, selectedAdditionalServiceArray];
}

function ClearForm() {

    $('#txtWayBillNo').val('');
    $('#ddlDeliveryOptionId').val(1);
    $('#txtCustomerRefNo').val('');
    $('#txtContactPerson').val('');
    $('#ddlBillerId').val(0);
    $('#txtBillerCustomerNo').val('');
    $('#txtCargoCtlNo').val('');
    $('#txtAwbCtnNo').val('');
    $('#lblShipperAccountNo').text('');
    $('#ddlShipperId').val(0);

    $('#txtShipperCustomerNo').val('');
    $('#txtShipperAddressline').val('');
    $('#txtShipperUnitNo').val('');
    $('#ddlShipperCityId').val(0);
    $('#ddlShipperProvinceId').val(7);
    $('#txtShipperPostcode').val('');
    $('#lblConsigneeAccountNo').text('');
    $('#ddlConsigneeId').val(0);
    $('#txtConsigneeCustomerNo').val('');
    $('#txtConsigneeAddressline').val('');
    $('#txtConsigneeUnitNo').val('');
    $('#ddlConsigneeCityId').val(0);
    $('#ddlConsigneeProvinceId').val(0);
    $('#txtConsigneePostcode').val('');
    $('#ddlUnitTypeId').val(0);
    $('#txtUnitQuantity').val('');
    $('#txtWeightTotal').val('');
    //$('#ddlWeightScaleId').val(0);
    $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(new Date));
    $('.chkAddService').prop('checked', false);
    $('.txtServiceFee').val('');
    $('.txtDriverPercentage').val('');

    $('#lblGrandBasicCost').text('0.00');
    $('#lblGrandDiscountAmount').text('0.00');
    $('#lblGrandGstAmount').text('0.00');
    $('#lblGrandFuelSurchargeAmount').text('0.00');
    $('#lblGrandTotalOrderCost').text('0.00');
    $('#lblGrandAddServiceAmount').text('0.00');
    $('#lblGrandTotalAmount').text('0.00');
    $('#rdoShipper').prop('checked', true);
    $('#rdoTruck').prop('checked', true);
    $('#txtFuelSurchargePercent').val('');
    $('#txtDiscountPercent').val('');
    $('#txtBaseOrderCost').val('');
    $('#txtBaseOrderSurcharge').val('');
    $('#txtBaseOrderGST').val('');
    $('#txtOverriddenOrderCost').val('');
    $('#txtOverriddenOrderSurcharge').val('');
    $('#txtOverriddenOrderGST').val('');
    $('#chkIsPrintOnWayBill').prop('checked', false);
    $('#txtCommentsForWayBill').val('');
    $('#chkIsPrintOnInvoice').prop('checked', false);
    $('#txtCommentsForInvoice').val('');
    $('#chkIsReturnOrder').is(':checked', false);
    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.off(true);

}

function DrawSignatureImage(base64String) {
    var canvas = document.getElementById('signatureCanvas');
    var canvasContext = canvas.getContext('2d');
    var image = new Image();
    image.onload = function () {
        canvasContext.drawImage(image, 0, 0);
    };
    image.src = "data:image/png;base64," + base64String;

}

//#endregion 





