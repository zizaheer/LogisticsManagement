//#region document.ready

$(document).ready(function () {

    if ($.fn.dataTable.isDataTable('#service-list')) {
        $('#service-list').DataTable().destroy();
    }
    $('#service-list').DataTable({
        'paging': false, // hinds pagination
        "bInfo": false, // hides the footer Showing Results n of n
        'bFilter': false //hides the searchbox
    });

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });

    $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(new Date));
    $('#txtSchedulePickupTime').val(GetTimeInHHmmFormat(new Date));


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
    var toggle = $(this).data('bs.toggle');
    if (wayBillNo === "" && wayBillNo < 1) {
        toggle.off(true);
        bootbox.alert("Return order can only be created for an existing order. Please enter way bill number.");
        return;
    }

    var shipperId = $('#ddlShipperId').val();
    var consigneeId = $('#ddlConsigneeId').val();
    var billerId = $('#ddlBillerId').val();

    $('#ddlShipperId').val(consigneeId);
    $('#ddlConsigneeId').val(shipperId);

    paidByValue = $('input[name=rdoPaidBy]:checked').val();
    if (paidByValue === '1') {
        paidByValue = '2';
        $('#rdoConsignee').prop('checked', true);
        $('#ddlBillerId').val($('#ddlConsigneeId').val());
    }
    else if (paidByValue === '2') {
        paidByValue = '1';
        $('#rdoShipper').prop('checked', true);
        $('#ddlBillerId').val($('#ddlShipperId').val());
    }

    $('#ddlShipperId').change();
    $('#ddlConsigneeId').change();
    $('#ddlBillerId').change();

    //$('#txtBaseOrderCost').val('');
    // $('#txtGrandDiscountAmount').val('');

    $('#txtDiscountPercent').val('');
    $('#txtUnitQuantity').val('');

    selectedAdditionalServiceArray = [];
    $('#txtUnitQuantity').change();
    $('#txtOverriddenOrderCost').val('');
    $('#txtOverriddenOrderCost').change();

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
    if (consigneeInfo !== null || consigneeInfo != undefined) {

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
    CalculateOrderBaseCost();
});


$('#txtDiscountPercent').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtDiscountPercent').change();
    }
});
$('#txtDiscountPercent').on('change', function (event) {
    CalculateOrderBaseCost();
});


$('#txtOverriddenOrderCost').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtOverriddenOrderCost').change();
    }
});
$('#txtOverriddenOrderCost').on('change', function (event) {
    CalculateOrderBaseCost();
});


$('#service-list .btnAddService').click(function (event) {
    event.preventDefault();

    //$(this).prop('disabled', true);

    var serviceId = $(this).closest('tr').children('td:nth-child(7)').find('.btnAddService').data('serviceid');
    var payToDriver = $(this).closest('tr').children('td:nth-child(3)').find('.chkPayToDriver').is(':checked');
    var driverPercentage = $(this).closest('tr').children('td:nth-child(4)').find('.txtDriverPercentage').val();
    var serviceFee = $(this).closest('tr').children('td:nth-child(5)').find('.txtServiceFee').val();
    var isGstApplicable = $(this).closest('tr').children('td:nth-child(6)').find('.chkIsGstApplicableForService').is(':checked');

    if (serviceFee === "") {
        bootbox.alert("Please enter service fee.");
        return;
    }

    if (payToDriver) {
        if (driverPercentage === "") {
            bootbox.alert("Please enter driver percentage.");
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

    selectedAdditionalServiceArray.push(serviceData);

    CalculateAdditionalServiceCost();
    CalculateOrderBaseCost();

});

$('#service-list .btnRemoveService').click(function (event) {
    event.preventDefault();

    var serviceId = $(this).data('serviceid');

    var index = selectedAdditionalServiceArray.findIndex(c => c.additionalServiceId === serviceId);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }

    CalculateAdditionalServiceCost();
    CalculateOrderBaseCost();
});


$('#txtWayBillNo').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var wayBillNumber = $('#txtWayBillNo').val();
        GetAndFillOrderDetailsByWayBillNumber(wayBillNumber);

    }
});


$('#order-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    var wbNumber = $(this).data('waybillnumber');

    GetAndFillOrderDetailsByWayBillNumber(wbNumber);
    $('#txtWayBillNo').attr('readonly', true);

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    RemoveEntry('Order/Remove', waybillNumber);
    $('#loadDataTable').load('Order/PartialViewDataTable');

});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Order/PartialViewDataTable');

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
    $('#loadDataTable').load('Order/PartialViewDataTable');
    selectedAdditionalServiceArray = [];
});


//#endregion


//#region Private methods

function GetAndFillOrderDetailsByWayBillNumber(wayBillNumber) {
    var singleOrderData = null;
    var returnOrderData = null;
    var singleOrderAdditionalServiceData = null;
    var returnOrderAdditionalServiceData = null;

    var toggle = $('input[name=chkIsReturnOrder]').data('bs.toggle');
    toggle.off(true);

    var orderInfo = GetSingleObjectById('Order/GetOrderByWayBillId', wayBillNumber);
    var parseData = JSON.parse(orderInfo);

    console.log('parse data ' + orderInfo);

    singleOrderData = parseData.orderPocos.filter(function (item) {
        return item.OrderTypeId === 1;
    })[0];

    if (singleOrderData !== null) {
        singleOrderAdditionalServiceData = parseData.orderAdditionalServices.filter(function (item) {
            return item.OrderId === singleOrderData.Id;
        });
    }

    returnOrderData = parseData.orderPocos.filter(function (item) {
        return item.OrderTypeId === 2;
    })[0];

    //console.log('return data ' + returnOrderData);

    if (returnOrderData !== null) {
        returnOrderAdditionalServiceData = parseData.orderAdditionalServices.filter(function (item) {
            return item.OrderId === returnOrderData.Id;
        });
    }

    //console.log(returnOrderAdditionalServiceData);

    var isChecked = $('#chkShowReturnOrder').is(':checked');
    if (isChecked) {
        FillOrderAdditionalServices(returnOrderAdditionalServiceData);
        FillOrderDetails(returnOrderData);
    }
    else {
        FillOrderAdditionalServices(singleOrderAdditionalServiceData);
        FillOrderDetails(singleOrderData);
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
        unitQuantity: (unitQuantity === null || unitQuantity === "" || unitQuantity === undefined) ? 0 : unitQuantity,
        weightScaleId: weightScaleId === null ? 0 : weightScaleId,
        weightQuantity: (weightQuantity === null || weightQuantity === "" || weightQuantity === undefined) ? 0 : weightQuantity
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

    $('#txtGrandAddServiceAmount').val(totalAdditionalServiceCost.toFixed(2));


}

function CalculateOrderBaseCost() {

    fuelSurchargePercentage = $('#txtFuelSurchargePercent').val() !== "" ? parseFloat($('#txtFuelSurchargePercent').val()) : 0.0;
    discountPercentage = $('#txtDiscountPercent').val() !== "" ? parseFloat($('#txtDiscountPercent').val()) : 0.0;
    overriddenOrderCost = $('#txtOverriddenOrderCost').val() !== "" ? parseFloat($('#txtOverriddenOrderCost').val()) : 0.0;

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
    if (taxPercentage > 0) {
        baseTaxAmount = taxPercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost + baseTaxAmount;
        $('#txtBaseOrderGST').val(baseTaxAmount.toFixed(2));
    }

    if (overriddenOrderCost > 0) {

        $('#txtGrandBasicCost').val($('#txtOverriddenOrderCost').val());

        if (fuelSurchargePercentage > 0) {
            overriddenFuelSurchargeAmount = fuelSurchargePercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost + overriddenFuelSurchargeAmount;
            $('#txtOverriddenOrderSurcharge').val(overriddenFuelSurchargeAmount.toFixed(2));
        }
        if (discountPercentage > 0) {
            overriddenDiscountAmount = discountPercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost - overriddenDiscountAmount;
        }
        if (taxPercentage > 0) {
            overriddenTaxAmount = taxPercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost + overriddenTaxAmount;
            $('#txtOverriddenOrderGST').val(overriddenTaxAmount.toFixed(2));
        }
    }
    else {
        overriddenOrderCost = baseOrderCost;
        $('#txtGrandBasicCost').val($('#txtBaseOrderCost').val());

        overriddenFuelSurchargeAmount = baseFuelSurchargeAmount;
        overriddenDiscountAmount = baseDiscountAmount;
        overriddenTaxAmount = baseTaxAmount;
    }

    $('#txtGrandDiscountAmount').val(overriddenDiscountAmount.toFixed(2));
    $('#txtGrandGstAmount').val(overriddenTaxAmount.toFixed(2));
    $('#txtGrandFuelSurchargeAmount').val(overriddenFuelSurchargeAmount.toFixed(2));

    $('#txtGrandTotalOrderCost').val(overriddenOrderCost.toFixed(2));

    var grandTotal = overriddenOrderCost;

    CalculateAdditionalServiceCost();

    if (totalAdditionalServiceCost > 0) {
        grandTotal = grandTotal + totalAdditionalServiceCost;
    }

    $('#txtGrandTotalAmount').val(grandTotal.toFixed(2));

}

function FillOrderDetails(orderRelatedData) {
    if (orderRelatedData !== null) {
        $('#hfOrderId').val(orderRelatedData.Id);
        //$('input[name=chkIsReturnOrder]:checked').val() === true ? 2 : 1;

        $('#txtWayBillNo').val(orderRelatedData.WayBillNumber);
        $('#txtCustomerRefNo').val(orderRelatedData.ReferenceNumber);
        $('#txtCargoCtlNo').val(orderRelatedData.CargoCtlNumber);
        $('#txtAwbCtnNo').val(orderRelatedData.AwbCtnNumber);
        $('#ddlShipperId').val(orderRelatedData.ShipperCustomerId);
        $('#ddlShipperId').change();
        $('#ddlConsigneeId').val(orderRelatedData.ConsigneeCustomerId);
        $('#ddlConsigneeId').change();
        $('#ddlBillerId').val(orderRelatedData.BillToCustomerId);
        $('#txtBillerCustomerNo').val(orderRelatedData.BillToCustomerId);
        //$('#txtShipperCustomerNo').val(orderRelatedData.ShipperCustomerId);
        //$('#txtConsigneeCustomerNo').val(orderRelatedData.ConsigneeCustomerId);
        $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(orderRelatedData.ScheduledPickupDate));
        $('#txtSchedulePickupTime').val(GetTimeInHHmmFormat(orderRelatedData.ScheduledPickupDate));
        //console.log(GetTimeInHHmmFormat(orderRelatedData.ScheduledPickupDate));

        //$('#txtEstimatedDeliverypDate').val(orderRelatedData.OrderId);
        //$('#ddlConsigneeCityId').val(orderRelatedData.OrderId);
        $('#ddlDeliveryOptionId').val(orderRelatedData.DeliveryOptionId);
        $("input[name='rdoVehicleType']:checked").val();

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
            //var serviceId = $(this).closest('tr').children('td:nth-child(7)').find('.btnAddService').data('serviceid');
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
    var time = $('#txtSchedulePickupTime').val();
    var scheduleDatetime = date + 'T' + time;

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
        scheduledPickupDate: scheduleDatetime,
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
        remarks: $('#txtRemarks').val() === "" ? null : $('#txtRemarks').val()

    };

    return [orderData, selectedAdditionalServiceArray];
}


//#endregion 





