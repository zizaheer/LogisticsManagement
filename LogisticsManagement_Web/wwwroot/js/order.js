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

    if (customerInfo !== null) {
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


$('#txtFuelSurchargeAmount').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtFuelSurchargeAmount').change();
    }
});
$('#txtFuelSurchargeAmount').on('change', function (event) {
    CalculateOrderBaseCost();
});


$('#txtDiscountAmount').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtDiscountAmount').change();
    }
});
$('#txtDiscountAmount').on('change', function (event) {
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
        alert("Please enter service fee.");
        return;
    }

    if (payToDriver) {
        if (driverPercentage === "") {
            alert("Please enter driver percentage.");
            return;
        }
    }

    var serviceData = {
        serviceId: serviceId,
        payToDriver: payToDriver,
        driverPercentage: driverPercentage === "" ? 0 : parseFloat(driverPercentage),
        serviceFee: parseFloat(serviceFee),
        isGstApplicable: isGstApplicable
    };


    var index = selectedAdditionalServiceArray.findIndex(c => c.serviceId === serviceData.serviceId);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }

    selectedAdditionalServiceArray.push(serviceData);
    //console.log("New array : " + JSON.stringify(selectedAdditionalServiceArray));

    CalculateAdditionalServiceCost();
    CalculateOrderBaseCost();

});

$('#service-list .btnRemoveService').click(function (event) {
    event.preventDefault();

    var serviceId = $(this).data('serviceid');

    var index = selectedAdditionalServiceArray.findIndex(c => c.serviceId === serviceId);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }

    CalculateAdditionalServiceCost();
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
            if (selectedAdditionalServiceArray[i].serviceFee > 0) {
                totalAdditionalServiceCost = totalAdditionalServiceCost + selectedAdditionalServiceArray[i].serviceFee;
                if (selectedAdditionalServiceArray[i].isGstApplicable && taxPercentage > 0) {
                    var addServiceTax = taxPercentage * selectedAdditionalServiceArray[i].serviceFee / 100;
                    totalAdditionalServiceCost = totalAdditionalServiceCost + addServiceTax;
                }

            }
        }

    }

    $('#txtGrandAddServiceAmount').val(totalAdditionalServiceCost.toFixed(2));
    // console.log(totalAdditionalServiceCost);
    //console.log(selectedAdditionalServiceArray);

}

function CalculateOrderBaseCost() {

    fuelSurchargePercentage = $('#txtFuelSurchargeAmount').val() !== "" ? parseFloat($('#txtFuelSurchargeAmount').val()) : 0.0;
    discountPercentage = $('#txtDiscountAmount').val() !== "" ? parseFloat($('#txtDiscountAmount').val()) : 0.0;
    overriddenOrderCost = $('#txtOverriddenOrderCost').val() !== "" ? parseFloat($('#txtOverriddenOrderCost').val()) : 0.0;

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
    if (totalAdditionalServiceCost > 0) {
        grandTotal = grandTotal + totalAdditionalServiceCost;
    }

    $('#txtGrandTotalAmount').val(grandTotal.toFixed(2));

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


$('#frmOrderForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});

$('#frmOrderForm').unbind('submit').submit(function (event) {
    var dataArray = GetFormData();
    console.log(dataArray[0].id);
    if (dataArray[0].id > 0) {
        UpdateEntry('Order/Update', dataArray);
    }
    else {
        AddEntry('Order/Add', dataArray);
    }
    event.preventDefault();
    $('#loadDataTable').load('Order/PartialViewDataTable');
});

$('.btnDelete').unbind().on('click', function () {
    employeeId = $(this).data('orderid');
    RemoveEntry('Order/Remove', employeeId);
    $('#loadDataTable').load('Order/PartialViewDataTable');

});

function GetFormData() {

    var orderData = {
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),
        id: $('#txtOrderId').val() === "" ? "0" : $('#txtOrderId').val(),

        OrderId	int	Unchecked
OrderTypeId	int	Unchecked
WayBillNumber	nvarchar(30)	Checked
ReferenceNumber	nvarchar(50)	Checked
CargoCtlNumber	nvarchar(50)	Checked
AwbCtnNumber	nvarchar(50)	Checked
ShipperCustomerId	int	Checked
ConsigneeCustomerId	int	Checked
BillToCustomerId	int	Unchecked
ScheduledPickupDate	datetime	Checked
ExpectedDeliveryDate	datetime	Checked
CityId	int	Checked
DeliveryOptionId	int	Checked
VehicleTypeId	int	Checked
UnitTypeId	int	Unchecked
WeightScaleId	int	Checked
WeightTotal	decimal(18, 2)	Checked
UnitQuantity	int	Unchecked
OrderBasicCost	decimal(18, 2)	Unchecked
BasicCostOverriden	decimal(18, 2)	Checked
FuelSurchargePercentage	decimal(3, 2)	Checked
DiscountPercentOnOrderCost	decimal(3, 2)	Checked
ApplicableGstPercent	decimal(3, 2)	Checked
TotalOrderCost	decimal(18, 2)	Checked
TotalAdditionalServiceCost	decimal(18, 2)	Checked
OrderedBy	nvarchar(50)	Checked
DepartmentName	nvarchar(50)	Checked
ContactName	nvarchar(50)	Checked
ContactPhoneNumber	nvarchar(20)	Checked
Remarks	nvarchar(250)	Checked
CreateDate	datetime	Unchecked
CreatedBy	int	Unchecked
		Unchecked
































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

