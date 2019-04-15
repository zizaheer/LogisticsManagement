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
var taxPercentage = 0.0;
var fuelSurchargeAmount = 0.0;
var discountAmount = 0.0;
var taxAmount = 0.0;

//#endregion

//#region Events 

$('#btnNew').on('click', function () {
    $('#txtWayBillNo').removeAttr('readonly');
});

$('input[type=radio][name=rdoPaidBy]').change(function () {
    paidByValue = this.value;
    $('#ddlBillerId').change();
});

$('#txtBillerCustomerNo').keypress(function (event) {

    if (event.keyCode === 13) {
        event.preventDefault();

        var id = $('#txtBillerCustomerNo').val();
        $('#ddlBillerId').val(id);

        if ($('#ddlBillerId').val() === null) {
            $('#ddlBillerId').val(0);
            alert('Customer not found');
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
            alert('Customer not found');
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
            alert('Customer not found');
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

    if (customerInfo != null) {
        if (customerInfo.FuelSurChargePercentage > 0) {
            $('#txtFuelSurchargeAmount').val(customerInfo.FuelSurChargePercentage);
        }
        $('#txtDiscountAmount').val(customerInfo.DiscountPercentage);
        $('#chkIsGstApplicable').prop('checked', customerInfo.IsGstApplicable);
    }

});

$('#ddlShipperId').on('change', function () {

    var shipperAddress = null;
    var selectedValue = $('#ddlShipperId').val();

    var shipperInfo = JSON.parse(GetCustomerInfo(selectedValue));
    if (shipperInfo != null || shipperInfo != undefined) {
        var mailingAddId = shipperInfo.MailingAddressId;
        var billingAddId = shipperInfo.BillingAddressId;

        if (mailingAddId != null) {
            shipperAddress = JSON.parse(GetAddressInfo(mailingAddId));
        }
        else if (billingAddId != null) {
            shipperAddress = JSON.parse(GetAddressInfo(billingAddId));
        }

        if (shipperAddress != null) {
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

});

$('#ddlConsigneeId').on('change', function () {

    var consigneeAddress = null;
    var selectedValue = $('#ddlConsigneeId').val();

    var consigneeInfo = JSON.parse(GetCustomerInfo(selectedValue));
    if (consigneeInfo != null || consigneeInfo != undefined) {

        var mailingAddId = consigneeInfo.MailingAddressId;
        var billingAddId = consigneeInfo.BillingAddressId;

        if (mailingAddId != null) {
            consigneeAddress = JSON.parse(GetAddressInfo(mailingAddId));
        }
        else if (billingAddId != null) {
            consigneeAddress = JSON.parse(GetAddressInfo(billingAddId));
        }

        if (consigneeAddress != null) {
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



//#endregion

//#region Private methods

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
        deliveryOptionId: deliveryOptionId == null ? 0 : deliveryOptionId,
        vehicleTypeId: vehicleTypeId == null ? 0 : vehicleTypeId,
        unitTypeId: unitTypeId == null ? 0 : unitTypeId,
        unitQuantity: (unitQuantity == null || unitQuantity == "" || unitQuantity == undefined) ? 0 : unitQuantity,
        weightScaleId: weightScaleId == null ? 0 : weightScaleId,
        weightQuantity: (weightQuantity == null || weightQuantity == "" || weightQuantity == undefined) ? 0 : weightQuantity
    };

    return GetListObjectByParam('Order/GetTariffCostByParam', dataForTariff);
}

function CalculateOrderBaseCost() {

    fuelSurchargePercentage = $('#txtFuelSurchargeAmount').val() != "" ? parseFloat($('#txtFuelSurchargeAmount').val()) : 0.0;
    discountPercentage = $('#txtDiscountAmount').val() != "" ? parseFloat($('#txtDiscountAmount').val()) : 0.0;
    taxPercentage = $('#lblGstAmount').text() != "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

    var orderCost = 0.0;

    var tariffCost = parseFloat(GetTariffInfo());
    if (tariffCost > 0) {
        if (fuelSurchargePercentage > 0) {
            fuelSurchargeAmount = fuelSurchargePercentage * tariffCost / 100;
        }

        orderCost = tariffCost + fuelSurchargeAmount;

        if (discountPercentage > 0) {
            discountAmount = discountPercentage * orderBasicCost / 100;
        }

        orderCost = orderCost - discountAmount;

        if (taxPercentage > 0) {
            taxAmount = taxPercentage * orderCost / 100;
        }

        orderCost = orderCost + taxAmount;

        $('#txtOrderCost').val(tariffCost.toFixed(2));
        $('#txtOrderSurcharge').val(fuelSurchargeAmount.toFixed(2));
        $('#txtOrderGST').val(taxAmount.toFixed(2));

        $('#txtOrderCost').val(parseFloat(tariffCost.toFixed(2)));
        $('#txtOrderCost').val(parseFloat(tariffCost.toFixed(2)));

    }


}

//#endregion 




















$('#order-list').on('click', '.btnEdit', function () {
    $('#txtWayBillNo').attr('readonly', true);
    var orderId = $(this).data('orderid');
    var data = GetSingleObjectById('Order/GetOrderDetails', orderid);

    $('#txtEmployeeId').val(data.id);
    $('#txtFirstName').val(data.firstName);
    $('#txtLastName').val(data.lastName);
    $('#txtDrivingLicenseNo').val(data.driverLicenseNo);
    $('#txtSocialInsuranceNo').val(data.socialInsuranceNo);
    $('#txtUnitNumber').val(data.unitNumber);
    $('#txtAddressLine').val(data.addressLine);
    $('#ddlCityId').val(data.cityId);
    $('#ddlProvinceId').val(data.provinceId);
    $('#ddlCountryId').val(data.countryId);
    $('#txtPostCode').val(data.postCode);
    $('#txtPhoneNumber').val(data.phoneNumber);
    $('#txtMobileNo').val(data.mobileNumber);
    $('#txtFaxNo').val(data.faxNumber);
    $('#txtEmailAddress').val(data.emailAddress);

    $('#ddlEmployeeTypeId').val(data.employeeTypeId);
    $('#chkIsHourlyPaid').val(data.employeeTypeId);
    $('#txtHourlyRate').val(data.id);
    $('#chkIsSalaryEmployee').val(data.isSalaried);
    $('#txtSalaryAmount').val(data.salaryAmount);
    $('#ddlSalaryTermId').val(data.salaryTerm);
    $('#chkIsCommissionProvided').val(data.isCommissionProvided);
    $('#txtCommissionAmount').val(data.commissionPercentage);
    $('#chkIsFuelProvided').val(data.isFuelChargeProvided);
    $('#txtFuelSurchargePercentage').val(data.fuelPercentage);
    $('#txtRadioInsuranceAmount').val(data.radioInsuranceAmount);
    $('#txtInsuranceAmount').val(data.insuranceAmount);

    $('#chkIsActive').val(data.isActive);


});

$('#btnDownloadData').unbind().on('click', function (event) {
    event.preventDefault();
    $('#loadDataTable').load('Order/PartialViewDataTable');

});



$('#frmOrderForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        UpdateEntry('Employee/Update', dataArray);
    }
    else {
        AddEntry('Employee/Add', dataArray);
    }
    event.preventDefault();
    $('#loadDataTable').load('Employee/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    employeeId = $(this).data('employeeid');
    RemoveEntry('Employee/Remove', employeeId);
    $('#loadDataTable').load('Employee/PartialViewDataTable');

});

function GetFormData() {

    var employeeData = {

        id: $('#txtEmployeeId').val() === "" ? "0" : $('#txtEmployeeId').val(),
        firstName: $('#txtFirstName').val(),
        lastName: $('#txtLastName').val(),
        driverLicenseNo: $('#txtDrivingLicenseNo').val(),
        socialInsuranceNo: $('#txtSocialInsuranceNo').val(),
        unitNumber: $('#txtUnitNumber').val(),
        addressLine: $('#txtAddressLine').val(),
        cityId: $('#ddlCityId').val(),
        provinceId: $('#ddlProvinceId').val(),
        countryId: $('#ddlCountryId').val(),
        postCode: $('#txtPostCode').val(),
        phoneNumber: $('#txtPhoneNumber').val(),
        mobileNumber: $('#txtMobileNo').val(),
        faxNumber: $('#txtFaxNo').val(),
        emailAddress: $('#txtEmailAddress').val(),

        employeeTypeId: $('#ddlEmployeeTypeId').val(),
        isHourlyPaid: $('#chkIsHourlyPaid').is(':checked') ? 1 : 0,
        hourlyRate: $('#txtHourlyRate').val(),
        isSalaried: $('#chkIsSalaryEmployee').is(':checked') ? 1 : 0,
        salaryAmount: $('#txtSalaryAmount').val(),
        salaryTerm: $('#ddlSalaryTermId').val(),
        isCommissionProvided: $('#chkIsCommissionProvided').is(':checked') ? 1 : 0,
        commissionPercentage: $('#txtCommissionAmount').val(),
        isFuelChargeProvided: $('#chkIsFuelProvided').is(':checked') ? 1 : 0,
        fuelPercentage: $('#txtFuelSurchargePercentage').val(),
        radioInsuranceAmount: $('#txtRadioInsuranceAmount').val(),
        insuranceAmount: $('#txtInsuranceAmount').val(),

        isActive: $('#chkIsActive').is(':checked') ? 1 : 0

    };

    return [employeeData];
}

