//#region document.ready

$(document).ready(function () {

    $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(new Date));
    $('#txtDispatchDatetimeForNewOrders').val(ConvertDatetimeToUSDatetime(new Date));

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });


    //var routingOrderId = $('#txtInvoicedOrderNumber').val();
    //if (routingOrderId !== '') {
    //    var isTriggerModify = $('#txtInvoicedOrderNumber').data('triggermodify');
    //    if (isTriggerModify === "1" || isTriggerModify === 1) {
    //        $('#btnExistingInvoicedOrder').trigger('click');
    //    }
    //}

    waitLoading();
});

function waitLoading() {
    setTimeout(function () {
        var routingOrderId = $('#txtInvoicedOrderNumber').val();
        if (routingOrderId !== '') {
            var isTriggerModify = $('#txtInvoicedOrderNumber').data('triggermodify');
            if (isTriggerModify === "1" || isTriggerModify === 1) {
                $('#btnExistingInvoicedOrder').trigger('click');
            }
        }

    }, 500);
}

//#endregion 

//#region Local Variables
var employeeData;
var paidByValue = $('input[name=rdoPaidBy]').val();
//alert(paidByValue);
var isNewEntry = false;

// global tax amount
//var taxPercentage = 0.0;
//taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

var isCustomerTaxApplicable = false;

var selectedAdditionalServiceArray = [];
var selectedOrdersForDispatch = [];
var additionalServiceSerial = 0;

//$('#chkCheckAllOrders').prop('checked', true);
//$('.chkDispatchToEmployee').prop('checked', true);
//var wbArrayString = $('#hfWaybillArray').val();
//selectedOrdersForDispatch = [];
//var wbArray = wbArrayString.split(',');
//$.each(wbArray, function (i, item) {
//    if (item !== '') {
//        selectedOrdersForDispatch.push(parseInt(item));
//    }
//});


//#endregion

//#region Events 

$('#btnNewOrder').unbind().on('click', function () {
    ClearForm();
    $('#frmOrderForm').trigger('reset');

    $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(new Date));

    var maxWayBill = GetSingleById('Order/GetNextWaybillNumber', 0);

    var addressLinesForAutoComplete = GetList('Address/GetAddressForAutoComplete');
    if (addressLinesForAutoComplete !== null) {
        var addressLines = JSON.parse(addressLinesForAutoComplete);

        $.each(addressLines, function (i, item) {
            $('#dlShipperAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
            $('#dlConsigneeAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
        });
    }

    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            if (item.Alpha3CountryCode === 'CAN') {
                $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode).attr('selected', true));
                $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode).attr('selected', true));
            } else {
                $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
                $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
            }
        });
    }

    isNewEntry = true;
    //$('#txtWayBillNo').prop('disabled', false);
    $('#txtWayBillNo').val(maxWayBill);

    $('#newOrder').modal({
        backdrop: 'static',
        keyboard: false
    });

    $('#newOrder').draggable();
    $('#newOrder').modal('show');

});

$('#btnCloseModal').unbind().on('click', function (event) {
    event.preventDefault();
    var shipperCity = $('#ddlShipperCityId').val();
    var consigneeCity = $('#ddlConsigneeCityId').val();
    var unitQty = $('#txtUnitQuantity').val();
    var skidQty = $('#txtSkidQuantity').val();
    var wbNumber = $('#txtWayBillNo').val();

    var dataEntered = false;
    if (shipperCity > 0 || consigneeCity > 0 || unitQty.length > 0 || skidQty.length > 0) {
        dataEntered = true;
    }
    if (dataEntered === true && wbNumber.length < 1) {
        bootbox.confirm("There are some un-saved data. Are you sure you want to close the window?", function (result) {
            if (result === true) {
                $('#newOrder').modal('hide');
            }
        });
    } else {
        $('#newOrder').modal('hide');
    }
});

$('input[type=radio][name=rdoPaidBy]').change(function () {
    paidByValue = this.value;
    var billerInfo = '';
    if (paidByValue === '1') {
        var shipperCustomerId = $('#txtShipperAccountNo').val();
        if (shipperCustomerId !== '') {
            billerInfo = GetCustomerInfo(shipperCustomerId);
            if (billerInfo != null && billerInfo != '') {
                FillBillerInformation(billerInfo);
            }
        }
    }
    else if (paidByValue === '2') {
        var consigneeCustomerId = $('#txtConsigneeAccountNo').val();
        if (consigneeCustomerId !== '') {
            billerInfo = GetCustomerInfo(consigneeCustomerId);
            if (billerInfo != null && billerInfo != '') {
                FillBillerInformation(billerInfo);
            }
        }
    }

});

$('input[name=chkIsCalculateByUnit]').change(function () {

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('#txtUnitQuantity').css('background-color', '#b3ffc3');
        $('#txtSkidQuantity').css('background-color', '');
    } else {
        $('#txtUnitQuantity').css('background-color', '');
        $('#txtSkidQuantity').css('background-color', '#b3ffc3');
    }
});

$('input[name=chkIsReturnOrder]').on('change', function () {

    var wayBillNo = $('#txtWayBillNo').val();
    var isChecked = $(this).is(':checked');

    if (isChecked) {
        if (wayBillNo === "" && wayBillNo < 1) {
            MakeReturnToggleOff();
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

$('#txtBillToCustomerName').on('keypress', function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var billerCustomerId = $('#txtBillToCustomerName').val();
        if (billerCustomerId > 0) {
            var billerInfo = GetCustomerInfo(billerCustomerId);
            if (billerInfo != null && billerInfo != '') {
                FillBillerInformation(billerInfo);
                if (paidByValue === '1') {
                    FillShipperInformation(billerInfo);
                }
                else if (paidByValue === '2') {
                    FillConsigneeInformation(billerInfo);
                }
            } else {
                bootbox.alert('Customer information was not found for Id: ' + billerCustomerId + '. Please enter a valid number or select from the suggestion list.');
            }
        }
        CalculateOrderBaseCost();
    }
});
$('#txtBillToCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();
    var billerCustomerId = $('#dlBillers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');
    if (billerCustomerId > 0) {
        var billerInfo = GetCustomerInfo(billerCustomerId);
        if (billerInfo != null && billerInfo != '') {
            FillBillerInformation(billerInfo);
            if (paidByValue === '1') {
                FillShipperInformation(billerInfo);
            }
            else if (paidByValue === '2') {
                FillConsigneeInformation(billerInfo);
            }
            CalculateOrderBaseCost();
        }
    }
});

$("#btnAddShipperCustomer").on('click', function () {
    openCustomerForm(1);
});
$("#btnAddConsigneeCustomer").on('click', function () {
    openCustomerForm(2);
});
function openCustomerForm(customerType) {
    var customerNo = "";
    var customerName = "";
    var addressLine = "";
    var addressUnit = "";
    var addressCity = "335";
    var addressProvince = "7";
    var addressCountry = "41";
    var addressPostcode = "";

    if (customerType == 1) {
        customerNo = $("#txtShipperAccountNo").val();
        customerName = $("#txtShipperCustomerName").val();
        addressLine = $("#txtShipperAddressLine").val();
        addressUnit = $("#txtShipperUnitNo").val();
        addressCity = $("#ddlShipperCityId").val();
        addressProvince = $("#ddlShipperProvinceId").val();
        addressPostcode = $("#txtShipperPostcode").val();
        addressCountry = $("#ddlShipperCountries").val();
    } else if (customerType == 2) {
        customerNo = $("#txtConsigneeAccountNo").val();
        customerName = $("#txtConsigneeCustomerName").val();
        addressLine = $("#txtConsigneeAddressLine").val();
        addressUnit = $("#txtConsigneeUnitNo").val();
        addressCity = $("#ddlConsigneeCityId").val();
        addressProvince = $("#ddlConsigneeProvinceId").val();
        addressPostcode = $("#txtConsigneePostcode").val();
        addressCountry = $("#ddlConsigneeCountries").val();
    }

    $("#txtCustomerId").val(customerNo);
    $("#txtCustomerName").val(customerName);

    $("#txtAddressLineForMain").val(addressLine);
    $("#txtAddressUnitForMain").val(addressUnit);
    $("#ddlCityIdForMain").val(addressCity);
    $("#ddlProvinceIdForMain").val(addressProvince);
    $("#ddlCountryIdForMain").val(addressCountry);
    $("#txtPostCodeForMain").val(addressPostcode);
    $("#rdoShippingForMain").prop("checked", true);

    if (customerNo !== '') {
        var customerInfo = GetCustomerInfo(parseInt(customerNo));
        if (customerInfo != null && customerInfo != '') {
            customerInfo = JSON.parse(customerInfo);
            $("#txtFuelSurcharge").val(customerInfo.FuelSurChargePercentage);
            $("#txtSpecialDiscount").val(customerInfo.DiscountPercentage);
            $("#txtInvoiceDueDays").val(customerInfo.InvoiceDueDays);
            $("#isGstApplicable").prop("checked", customerInfo.IsGstApplicable);
        }
    }

    $("#btnSaveCustomer").data("source", customerType);

    $('#addNewCustomer').modal({
        backdrop: "static",
        keyboard: false
    });
    $('#addNewCustomer').draggable();
    $('#addNewCustomer').modal('show');
}
$(document).on('show.bs.modal', '.modal', function (event) {
    var zIndex = 1040 + (10 * $('.modal:visible').length);
    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});

$("#addNewCustomer #btnSaveCustomer").on("click", function (event) {
    event.preventDefault();

    var customerData = {
        id: $('#txtCustomerId').val() === "" ? "0" : $('#txtCustomerId').val(),
        customerName: $('#txtCustomerName').val(),
        fuelSurChargePercentage: $('#txtFuelSurcharge').val(),
        discountPercentage: $('#txtSpecialDiscount').val(),
        invoiceDueDays: $('#txtInvoiceDueDays').val(),
        isGstApplicable: $('#isGstApplicable').is(':checked') ? 1 : 0,
        isActive: $('#chkIsActive').is(':checked') === true ? 1 : 0
    };
    var addressData = {
        customerId: $('#txtCustomerId').val() === "" ? "0" : $('#txtCustomerId').val(),
        addressTypeId: $('input[name="rdoAddressTypeForMain"]:checked').val(),
        addressId: $('#hfAddressIdForMain').val() === '' ? 0 : parseInt($('#hfAddressIdForMain').val()),

        addressLine: $('#txtAddressLineForMain').val(),
        unitNumber: $('#txtAddressUnitForMain').val(),
        cityId: $('#ddlCityIdForMain').val(),
        provinceId: $('#ddlProvinceIdForMain').val(),
        countryId: $('#ddlCountryIdForMain').val(),
        postCode: $('#txtPostCodeForMain').val(),
        contactPersonName: $('#txtContactPersonForMain').val(),
        emailAddress1: $('#txtEmailAddressForMain').val(),
        primaryPhoneNumber: $('#txtPrimaryPhoneNumberForMain').val(),
        fax: $('#txtFaxNumberForMain').val(),
        isDefault: $('#chkMakeDefaultAddressForMain').is(':checked') === true ? 1 : 0,
        shippingAddressMappingId: $('#hfShippingAddressMappingId').val(),
        billingAddressMappingId: $('#hfBillingAddressMappingId').val()
    };

    var dataArray = [customerData, addressData];

    if (dataArray[0].customerName === '') {
        event.preventDefault();
        bootbox.alert('Please enter customer name.');
        return;
    }

    if (dataArray[1].addressLine === '') {
        event.preventDefault();
        bootbox.alert('Please enter address line.');
        return;
    }
    if (parseInt(dataArray[1].cityId) < 1) {
        event.preventDefault();
        bootbox.alert('Please enter city.');
        return;
    }
    if (parseInt(dataArray[1].provinceId) < 1) {
        event.preventDefault();
        bootbox.alert('Please enter province.');
        return;
    }
    if (parseInt(dataArray[1].countryId) < 1) {
        event.preventDefault();
        bootbox.alert('Please enter country.');
        return;
    }

    var result = '';

    var customerType = $("#btnSaveCustomer").data("source");

    if (dataArray[0].id > 0) {
        result = PerformPostActionWithObject('Customer/Update', dataArray);
    }
    else {
        result = PerformPostActionWithObject('Customer/Add', dataArray);
    }
    if (result.length > 0) {
        $('#txtCustomerId').val(result);
        var shipperConsigneeInfo = GetCustomerInfo(parseInt(result));
        if (shipperConsigneeInfo != null && shipperConsigneeInfo != '') {
            if (parseInt(customerType) == 1) {
                FillShipperInformation(shipperConsigneeInfo);
                if (paidByValue === '1') {
                    FillBillerInformation(shipperConsigneeInfo);
                }
            }
            else if (parseInt(customerType) == 2) {
                FillConsigneeInformation(shipperConsigneeInfo);
                if (paidByValue === '2') {
                    FillBillerInformation(shipperConsigneeInfo);
                }
            }
            CalculateOrderBaseCost();
        }

        //bootbox.alert('Success! Customer information was saved succefully. You may close this window.');

    } else {
        bootbox.alert('Failed! Something went wrong during adding the customer. Please check your data and try again.');
    }



});

$('#txtShipperCustomerName').on('keypress', function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var shipperCustomerId = $('#txtShipperCustomerName').val();
        if (shipperCustomerId > 0) {
            var shipperInfo = GetCustomerInfo(shipperCustomerId);
            if (shipperInfo != null && shipperInfo != '') {
                FillShipperInformation(shipperInfo);
                if (paidByValue === '1') {
                    FillBillerInformation(shipperInfo);
                }
                CalculateOrderBaseCost();
            } else {
                $('#txtShipperAccountNo').val('');
                bootbox.alert('Customer information was not found for Id: ' + shipperCustomerId);
            }

        }


    }
});

$('#ddlDeliveryOptionId').on('change', function () {
    CalculateOrderBaseCost();
});

$('#txtShipperCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $('#txtShipperCustomerName').val();
    var shipperCustomerId = $('#dlShipperCustomers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');

    if (shipperCustomerId > 0) {
        var shipperInfo = GetCustomerInfo(shipperCustomerId);
        if (shipperInfo != null && shipperInfo != '') {
            FillShipperInformation(shipperInfo);
            if (paidByValue === '1') {
                FillBillerInformation(shipperInfo);
            }
            CalculateOrderBaseCost();
        } else {
            $('#txtShipperAccountNo').val('');
        }

    }
});

$('#txtConsigneeCustomerName').on('keypress', function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var consigneeCustomerId = $('#txtConsigneeCustomerName').val();
        if (consigneeCustomerId > 0) {
            var consigneeInfo = GetCustomerInfo(consigneeCustomerId);
            if (consigneeInfo != null && consigneeInfo != '') {
                FillConsigneeInformation(consigneeInfo);
                if (paidByValue === '2') {
                    FillBillerInformation(consigneeInfo);
                }
                CalculateOrderBaseCost();
            } else {
                $('#txtConsigneeAccountNo').val('');
                bootbox.alert('Customer information was not found for Id: ' + consigneeCustomerId);
            }

        }
    }
});
$('#txtConsigneeCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $('#txtConsigneeCustomerName').val();
    var consigneeCustomerId = $('#dlConsigneeCustomers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');
    if (consigneeCustomerId > 0) {
        var consigneeInfo = GetCustomerInfo(consigneeCustomerId);
        if (consigneeInfo != null && consigneeInfo != '') {
            FillConsigneeInformation(consigneeInfo);
            if (paidByValue === '2') {
                FillBillerInformation(consigneeInfo);
            }
            CalculateOrderBaseCost();
        } else {
            $('#txtConsigneeAccountNo').val('');
        }

    }
});

function FillBillerInformation(billerInfo) {
    if (billerInfo != null && billerInfo != '') {
        var billerCustomerInfo = JSON.parse(billerInfo);
        if (billerCustomerInfo != null) {
            if (billerCustomerInfo.FuelSurChargePercentage > 0) {
                $('#txtFuelSurchargePercent').val(billerCustomerInfo.FuelSurChargePercentage);
            }
            $('#hfBillerCustomerId').val(billerCustomerInfo.Id);
            $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
            $('#txtDiscountPercent').val(billerCustomerInfo.DiscountPercentage);
            $('#chkIsGstApplicable').prop('checked', billerCustomerInfo.IsGstApplicable);

            isCustomerTaxApplicable = billerCustomerInfo.IsGstApplicable;
        }
    }
}
function FillShipperInformation(shipperInfo) {
    var addressId = 0;
    if (shipperInfo != null && shipperInfo != '') {
        var shipperCustomerInfo = JSON.parse(shipperInfo);
        $('#txtShipperCustomerName').val(shipperCustomerInfo.CustomerName);
        $('#txtShipperAccountNo').val(shipperCustomerInfo.Id);

        addressId = GetCustomerDefaultShippingAddress(shipperCustomerInfo.Id);
        if (addressId < 1 || addressId === '') {
            addressId = GetCustomerDefaultBillingAddress(shipperCustomerInfo.Id);
        }
        if (addressId > 0 && addressId !== '') {
            FillShipperAddress(addressId);
        }
        else {
            ClearShipperAddressArea();
        }
    }
}
function FillConsigneeInformation(consigneeInfo) {
    var addressId = 0;
    if (consigneeInfo != null && consigneeInfo != '') {
        var consigneeCustomerInfo = JSON.parse(consigneeInfo);
        $('#txtConsigneeCustomerName').val(consigneeCustomerInfo.CustomerName);
        $('#txtConsigneeAccountNo').val(consigneeCustomerInfo.Id);
        addressId = GetCustomerDefaultShippingAddress(consigneeCustomerInfo.Id);
        if (addressId < 1 && addressId == '') {
            addressId = GetCustomerDefaultBillingAddress(consigneeCustomerInfo.Id);
        }
        if (addressId > 0 && addressId != '') {
            FillConsigneeAddress(addressId);
        }
        else {
            ClearConsigneeAddressArea();
        }
    }
}

$('#txtEmployeeName').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var employeeId = $('#txtEmployeeName').val();

        $('#hfDispatchToEmployeeId').val('');

        var empInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
        if (empInfo !== '') {
            var emp = JSON.parse(empInfo);
            $('#txtEmployeeName').val(emp.FirstName);
            $('#hfDispatchToEmployeeId').val(employeeId);
            $('#txtEmailAddressForDispatch').val(emp.EmailAddress);

            //if ((emp.EmployeeTypeId === 4 | emp.EmployeeTypeId === 6) && selectedOrdersForDispatch.length === 1) {
            //    $('#ddlShareType').prop('disabled', false);
            //    $('#txtOrderPortionForNewOrders').prop('disabled', false);
            //}
        }
    }
});
$('#txtEmployeeName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();

    var employeeId = $('#dlEmployees option').filter(function () {
        return this.value === valueSelected;
    }).data('employeeid');

    $('#hfDispatchToEmployeeId').val('');

    var empInfo = GetSingleById('Employee/GetEmployeeById', employeeId);

    if (empInfo !== '') {
        var emp = JSON.parse(empInfo);
        $('#txtEmployeeName').val(emp.FirstName);
        $('#hfDispatchToEmployeeId').val(employeeId);
        $('#txtEmailAddressForDispatch').val(emp.EmailAddress);

        //if ((emp.EmployeeTypeId === 4 | emp.EmployeeTypeId === 6) && selectedOrdersForDispatch.length === 1) {
        //    $('#ddlShareType').prop('disabled', false);
        //    $('#txtOrderPortionForNewOrders').prop('disabled', false);
        //}
    }
});

$('#txtShipperAddressLine').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var addressId = $('#txtShipperAddressLine').val();
        if (addressId !== '' && addressId > 0 && addressId != null) {
            FillShipperAddress(addressId);
        }
    }
});
$('#txtShipperAddressLine').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();
    var addressId = $('#dlShipperAddressLines option').filter(function () {
        return this.value === valueSelected;
    }).data('addressid');
    if (addressId > 0 && addressId !== '' && addressId != null) {
        FillShipperAddress(addressId);
    } else {
        $('#hfShipperAddressId').val('');
    }
});

$('#txtConsigneeAddressLine').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        addressId = $('#txtConsigneeAddressLine').val();
        if (addressId != '' && addressId > 0 && addressId != null) {
            FillShipperAddress(addressId);
        }
    }
});
$('#txtConsigneeAddressLine').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();
    var addressId = $('#dlConsigneeAddressLines option').filter(function () {
        return this.value === valueSelected;
    }).data('addressid');
    if (addressId > 0 && addressId != '' && addressId != null) {
        FillConsigneeAddress(addressId);
    } else {
        $('#hfConsigneeAddressId').val('');
    }
});

$('#txtUnitQuantity').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtUnitQuantity').change();
    }
});
$('#txtUnitQuantity').on('change', function (event) {
    var qty = $('#txtUnitQuantity').val();
    if (qty !== '') {
        if (parseInt(qty) > 0) {
            $('#ddlUnitTypeId').prop('disabled', false);
            CalculateOrderBaseCost();
        } else {
            $('#ddlUnitTypeId').prop('disabled', true);
        }
    }
});

$('#txtSkidQuantity').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtSkidQuantity').change();
    }
});
$('#txtSkidQuantity').on('change', function (event) {

    var skidQty = $('#txtSkidQuantity').val();
    if (skidQty !== '' && skidQty > 0) {
        $('#txtTotalPieces').removeAttr('disabled');
    } else {
        $('#txtTotalPieces').attr('disabled', true);
    }

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

$('#chkIsGstApplicable').on('change', function () {
    var isChecked = $('#chkIsGstApplicable').is(':checked');
    if (isChecked === true) {
        if (isCustomerTaxApplicable === false) {
            bootbox.confirm("You are applying tax on non-taxable customer. Are you sure you want to apply tax?", function (result) {
                if (result === true) {
                    $('#chkIsGstApplicable').prop('checked', true);
                    $('#txtSkidQuantity').change();
                } else {
                    $('#chkIsGstApplicable').prop('checked', false);
                    $('#txtSkidQuantity').change();
                }
            });
        }
        else {
            $('#txtSkidQuantity').change();
        }
    } else {
        if (isCustomerTaxApplicable === true) {
            bootbox.confirm("This is a taxable customer. Are you sure you do not want to apply tax?", function (result) {
                if (result === true) {
                    $('#chkIsGstApplicable').prop('checked', false);
                    $('#txtSkidQuantity').change();
                } else {
                    $('#chkIsGstApplicable').prop('checked', true);
                    $('#txtSkidQuantity').change();
                }
            });
        }
        else {
            $('#txtSkidQuantity').change();
        }
    }

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

$(document).on('input', '#service-list .txtAdditionalServiceName', function (event) {
    var valueSelected = $(this).val();
    var serviceId = '0';
    if (valueSelected !== '') {
        serviceId = $('.additionalServices option').filter(function () {
            return this.value === valueSelected;
        }).data('serviceid');
        var currentRow = $(this).closest('div.row');

        var serviceInfo = JSON.parse(GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceId));
        currentRow.find('.hfServiceId').val(serviceId);

        if (serviceInfo.IsPriceApplicable == true) {
            currentRow.find('.txtUnitPrice').val(serviceInfo.UnitPrice);
            currentRow.find('.txtQuantity').val("1");
            currentRow.find('.txtUnitPrice').prop("disabled", false);
            currentRow.find('.txtQuantity').prop("disabled", false);
            currentRow.find('.txtServiceFee').val(serviceInfo.UnitPrice * 1.0);
            currentRow.find('.txtAdditionalServiceName').data("isunitpriceapplicable", "1");

        } else {
            currentRow.find('.txtUnitPrice').val("");
            currentRow.find('.txtQuantity').val("");
            currentRow.find('.txtUnitPrice').prop("disabled", true);
            currentRow.find('.txtQuantity').prop("disabled", true);
            currentRow.find('.txtServiceFee').val("");
            currentRow.find('.txtAdditionalServiceName').data("isunitpriceapplicable", "0");
        }

        currentRow.find('.chkIsGstApplicableForService').prop('checked', serviceInfo.IsTaxApplicable);
        currentRow.find('.chkPayToDriver').prop('checked', serviceInfo.PayToDriver);

        currentRow.find('.chkIsGstApplicableForService').prop('checked', serviceInfo.IsTaxApplicable);
        currentRow.find('.btnAddAdditionalService').attr('data-serviceid', serviceId);
        currentRow.find('.btnDeleteAdditionalService').attr('data-serviceid', serviceId);
    }

});

$('#btnAddAddtionalServiceRow').on('click', function (event) {
    event.preventDefault();

    var billingCustomerId = $('#hfBillerCustomerId').val();
    var billingCustomerName = $('#txtBillToCustomerName').val();
  
    if (billingCustomerId == "" || billingCustomerName == "") {
        bootbox.alert("Please select the billing customer before adding the additional service.");
        return;
    }

    $('#service-list').append(GenerateNewAdditionalServiceRow());

    var services = JSON.parse(GetList('AdditionalService/GetAdditionalServiceList'));

    $.each(services, function (i, item) {
        $('#service-list .additionalServices').append($('<option>').attr('data-serviceid', item.Id).val(item.ServiceName));
    });
    event.preventDefault();
});

$('#service-list').unbind().on('click', '.btnDeleteAdditionalService', function (event) {
    event.preventDefault();
    var serviceId = event.currentTarget.dataset.serviceid;

    var selectedRow = $(this).closest("div.row");
    bootbox.confirm("Are you sure you want to remove this additional service?", function (result) {
        if (result === true) {

            var index = selectedAdditionalServiceArray.findIndex(c => c.additionalServiceId === parseInt(serviceId));
            if (index >= 0) {
                selectedAdditionalServiceArray.splice(index, 1);
            }

            CalculateAdditionalServiceCost();
            CalculateOrderBaseCost();

            selectedRow.remove();
        }
    });
});

function addAdditionalServiceRowData(currentRow) {

    var billingCustomerId = $('#hfBillerCustomerId').val();
    var billingCustomerName = $('#txtBillToCustomerName').val();

    var serviceId = currentRow.find('.hfServiceId').val();
    var serviceName = currentRow.find('.txtAdditionalServiceName').val();
    var serialNo = parseInt(currentRow.find('.hfSerialNo').val());

    var payToDriver = currentRow.find('.chkPayToDriver').is(':checked');
    var serviceFee = currentRow.find('.txtServiceFee').val();
    var driverPercentage = currentRow.find('.txtDriverPercentage').val();
    var isGstApplicable = currentRow.find('.chkIsGstApplicableForService').is(':checked');
    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;
    var unitQty = currentRow.find('.txtQuantity').val();
    var unitPrice = currentRow.find('.txtUnitPrice').val();
    unitQty = unitQty !== '' ? parseInt(unitQty) : 0;
    unitPrice = unitPrice !== '' ? parseFloat(unitPrice) : 0.0;

    if (billingCustomerId == "" || billingCustomerName == "") {
        bootbox.alert("Please select the billing customer before adding the additional service.");
        return;
    }

    if (serviceName == "") {
        bootbox.alert("Please select the service from the additional service name.");
        return;
    }

    if (serviceId === undefined || serviceId === '' || serviceId === '0' || serviceId < 1) {
        bootbox.alert("Please select service before adding it to the order.");
        return;
    }

    if (serviceFee === "" || parseFloat(serviceFee) <= 0) {
        bootbox.alert("Please enter service fee before adding it to the order.");
        return;
    }

    if (isGstApplicable === true) {
        if (isCustomerTaxApplicable === false) {
            bootbox.alert('You cannot apply tax on a non taxable customer');
            return;
        }
    } else {
        if (isCustomerTaxApplicable === true) {
            bootbox.alert('You are not charging TAX for a taxable customer. If you made a mistake remove and add the service again.');
        }
    }

    

    if (unitQty !== "") {
        if (unitQty % 1 != 0) {
            bootbox.alert("Please enter valid quantity in whole number.");
            return;
        }
    }

    if (payToDriver === true) {
        if (driverPercentage === "") {
            bootbox.alert("Please enter driver percentage before adding it to the order.");
            return;
        }
    }

    var serviceData = {
        serialNo: serialNo,
        orderId: $('#hfOrderId').val(),
        additionalServiceId: parseInt(serviceId),
        unitPrice: unitPrice,
        quantity: unitQty,
        isPayToDriver: payToDriver,
        driverPercentageOnAddService: driverPercentage === "" ? 0 : parseFloat(driverPercentage),
        additionalServiceFee: parseFloat(serviceFee),
        isTaxAppliedOnAddionalService: isGstApplicable,
        taxAmountOnAdditionalService: taxPercentage
    };

    var index = selectedAdditionalServiceArray.findIndex(c => c.serialNo === serviceData.serialNo);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }
    selectedAdditionalServiceArray.push(serviceData);

    CalculateAdditionalServiceCost();
    CalculateOrderBaseCost();

    currentRow.find('.txtAdditionalServiceName').prop("disabled", true);
    currentRow.find('.txtUnitPrice').prop("disabled", true);
    currentRow.find('.txtQuantity').prop("disabled", true);
    currentRow.find('.txtServiceFee').prop("disabled", true);
    currentRow.find('.chkPayToDriver').prop("disabled", true);
    currentRow.find('.txtDriverPercentage').prop("disabled", true);
    currentRow.find('.chkIsGstApplicableForService').prop("disabled", true);
    currentRow.find('.txtAdditionalServiceName').prop("disabled", true);

    currentRow.find('.btnAddAdditionalService').prop("disabled", true);
    currentRow.find('.chkIsUpdate').prop("disabled", false);
    currentRow.find('.chkIsUpdate').prop("checked", false);

    currentRow.find('.btnEditService').prop("disabled", true);
    currentRow.find('.btnDeleteAdditionalService').removeAttr('disabled');

    //console.log("Additional service : " + JSON.stringify(selectedAdditionalServiceArray));

}

$('#service-list').on('click', '.btnAddAdditionalService', function (event) {
    event.preventDefault();
    var currentRow = $(this).closest('div.row');
    addAdditionalServiceRowData(currentRow);
});

$('#service-list').on('input', '.txtUnitPrice', function () {
    var selectedRow = $(this).closest("div.row");
    var unitPrice = $(this).val();
    var qty = selectedRow.find('.txtQuantity').val();

    unitPrice = unitPrice !== '' ? parseFloat(unitPrice) : 0.00;
    qty = qty !== '' ? parseInt(qty) : 0;

    selectedRow.find('.txtServiceFee').val(unitPrice * qty);
});

$('#service-list').on('input', '.txtQuantity', function () {
    var selectedRow = $(this).closest("div.row");
    var qty = $(this).val();
    var unitPrice = selectedRow.find('.txtUnitPrice').val();

    unitPrice = unitPrice !== '' ? parseFloat(unitPrice) : 0.00;
    qty = qty !== '' ? parseInt(qty) : 0;

    selectedRow.find('.txtServiceFee').val(unitPrice * qty);
});

$('#service-list').on('click', '.chkIsUpdate', function () {
    var selectedRow = $(this).closest("div.row");
    var isChecked = $(this).is(':checked');
    if (isChecked == true)
    {
        selectedRow.find('.btnEditService').prop("disabled", false);
        selectedRow.find('.txtAdditionalServiceName').prop("disabled", false);
        selectedRow.find('.txtServiceFee').prop("disabled", false);
        selectedRow.find('.chkPayToDriver').prop("disabled", false);
        selectedRow.find('.txtDriverPercentage').prop("disabled", false);
        selectedRow.find('.chkIsGstApplicableForService').prop("disabled", false);

        var isunitpriceapplicable = selectedRow.find('.txtAdditionalServiceName').data("isunitpriceapplicable");
        if (isunitpriceapplicable == "1")
        {
            selectedRow.find('.txtUnitPrice').prop("disabled", false);
            selectedRow.find('.txtQuantity').prop("disabled", false);
        }
        else
        {
            selectedRow.find('.txtUnitPrice').prop("disabled", true);
            selectedRow.find('.txtQuantity').prop("disabled", true);
        }
    }
    else
    {
        selectedRow.find('.btnEditService').prop("disabled", true);
        selectedRow.find('.txtAdditionalServiceName').prop("disabled", true);
        selectedRow.find('.txtUnitPrice').prop("disabled", true);
        selectedRow.find('.txtQuantity').prop("disabled", true);
        selectedRow.find('.txtServiceFee').prop("disabled", true);
        selectedRow.find('.chkPayToDriver').prop("disabled", true);
        selectedRow.find('.txtDriverPercentage').prop("disabled", true);
        selectedRow.find('.chkIsGstApplicableForService').prop("disabled", true);
    }

});

$('#service-list').on('click', '.btnEditService', function (event) {
    event.preventDefault();

    var currentRow = $(this).closest('div.row');

    addAdditionalServiceRowData(currentRow);
});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This waybill number will be deleted along with all relavant data. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Order/Remove', waybillNumber);
            location.reload();
        }
    });
});

$('#frmOrderForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmOrderForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();
    var isValid = ValidateOrderForm(dataArray[0]);
    if (isValid === false) {
        return;
    }

    var duplicateWaybill = GetObject('Order/FindDuplicateWayBillByOrderAndWaybillId?orderId=' + dataArray[0].id + '&orderTypeId=' + dataArray[0].orderTypeId + '&waybillNo=' + dataArray[0].wayBillNumber);

    if (duplicateWaybill !== '') {
        if (isNewEntry == true) {
            bootbox.alert('This waybill was already used. Cannot create duplicate waybill. Try a different number or keep it blank to create auto.');
            return;
        } else {
            bootbox.alert('This waybill was already used. Cannot create duplicate waybill. Try a different number.');
            return;
        }
    }

    var isNewEntryForPreprinted = GetObject('Order/IsPrePrintedWaybillForNewEntry?orderTypeId=' + dataArray[0].orderTypeId + '&waybillNo=' + dataArray[0].wayBillNumber);
    if (isNewEntryForPreprinted !== '') {
        PerformPostActionWithId('Order/RemoveByOrderId', dataArray[0].id);
        dataArray[0].id = isNewEntryForPreprinted;
        isNewEntry = false;
    }


    if (dataArray[0].cargoCtlNumber !== '' || dataArray[0].awbCtnNumber !== '' || dataArray[0].referenceNumber !== '') {
        var countCtl = PerformPostActionWithObject('Order/GetCargoCtlNumberCount', { cargoCtl: dataArray[0].cargoCtlNumber, orderId: dataArray[0].id });
        var countAwb = PerformPostActionWithObject('Order/GetAwbCtnNumberCount', { awbCtn: dataArray[0].awbCtnNumber, orderId: dataArray[0].id });
        var countRef = PerformPostActionWithObject('Order/GetCustomerReferenceNumberCount', { custRef: dataArray[0].referenceNumber, orderId: dataArray[0].id });

        if (countRef > 0 && countAwb > 0 && countCtl > 0) {
            bootbox.confirm("The Customer ref#, Cargo ctl# and Awb/ctn# are already exist. Do you want to use them for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countRef > 0 && countAwb > 0) {
            bootbox.confirm("The Customer ref# and Awb/ctn# are already exist. Do you want to use them for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countRef > 0 && countCtl > 0) {
            bootbox.confirm("The Customer ref# and Cargo ctl# are already exist. Do you want to use them for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countRef > 0) {
            bootbox.confirm("The Customer ref# already exist. Do you want to use the same for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countAwb > 0 && countCtl > 0) {
            bootbox.confirm("The Cargo ctl# and Awb/ctn# already exist. Do you want to use them for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countAwb > 0) {
            bootbox.confirm("The Awb/ctn# already exist. Do you want to use it for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else if (countCtl > 0) {
            bootbox.confirm("The Cargo ctl# already exist. Do you want to use it for this order too?", function (result) {
                if (result === true) {
                    SubmitOrderForm(dataArray);
                }
            });
        } else {
            SubmitOrderForm(dataArray);
        }
    }
});

function SubmitOrderForm(dataArray) {

    var result;
    var parseData;

    if (dataArray[0].wayBillNumber > 0 && isNewEntry === false) {
        result = PerformPostActionWithObject('Order/Update', dataArray);
        if (result.length > 0) {
            // bootbox.alert('The order has been updated successfully');
        }
    }
    else {
        if (dataArray[0].wayBillNumber >= 0 && isNewEntry === true) {
            result = PerformPostActionWithObject('Order/Add', dataArray);
            if (result !== null) {
                parseData = JSON.parse(result);
                $('#txtWayBillNo').val(parseData.waybillNumber);
                $('#hfOrderId').val(parseData.orderId);
                // bootbox.alert('The order has been created successfully');
            }
        }
    }
    //selectedAdditionalServiceArray = null;
    selectedAdditionalServiceArray = [];
    $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');

    ClearForm();
    $('#frmOrderForm').trigger('reset');
    $('#newOrder').modal('hide');

    location.reload();
}

function ValidateOrderForm(formData) {

    var maxWaybillNo = GetSingleById('Order/GetNextWaybillNumber', 0);
    if (parseInt(formData.wayBillNumber) > parseInt(maxWaybillNo)) {
        bootbox.alert('Waybill number cannot exceed the maximum WB# ' + maxWaybillNo + '.');
        return false;
    }

    if (formData.unitQuantity < 1 && formData.skidQuantity < 1 && formData.totalPieces < 1) {
        bootbox.alert('Quantity required either for Unit or Skid!');
        return false;
    }

    if (formData.shipperCustomerId < 1) {
        bootbox.alert('Shipper not found or invalid. Please verify and try again.');
        return false;
    }

    if (formData.consigneeCustomerId < 1) {
        bootbox.alert('Consignee not found or invalid. Please verify and try again.');
        return false;
    }

    if (formData.shipperCustomerId === formData.consigneeCustomerId && formData.shipperAddressId === formData.consigneeAddressId) {
        bootbox.alert('Shipper and consignee address must be different!');
        return false;
    }

    if (formData.shipperAddressline.length < 1) {
        bootbox.alert('Please enter a valid address line for shipper!');
        return false;
    }

    if (formData.consigneeAddressline.length < 1) {
        bootbox.alert('Please enter a valid address line for consignee!');
        return false;
    }

    //if (formData.shipperAddressId < 1) {
    //    bootbox.alert('No address found for shipper. Please select address for shipper!');
    //    return false;
    //}
    //if (formData.consigneeAddressId < 1) {
    //    bootbox.alert('No address found for consignee. Please select address for consignee!');
    //    return false;
    //}

    if (formData.billToCustomerId < 1) {
        bootbox.alert('Please select biller.');
        return false;
    }

    if ($('input[name=chkIsReturnOrder]:checked').val() === 'on') {
        if (formData.wayBillNumber === "" || formData.wayBillNumber < 1) {
            bootbox.alert("Return order can only be created for an existing order. Please enter way bill number.");
            $('#txtWayBillNo').focus();
            return false;
        }
    }

    if (formData.isPrintedOnWayBill === 1) {
        if (formData.commentsForWayBill === null) {
            bootbox.alert('Waybill comments required when you choose to print them!');
            return false;
        }
    }
    if (formData.isPrintedOnInvoice === 1) {
        if (formData.commentsForInvoice === null) {
            bootbox.alert('Invoice comments required when you choose to print them!');
            return false;
        }
    }

    //var calculateByUnit = $('#chkIsCalculateByUnit').is(':checked');
    //if (calculateByUnit === false) {
    //    if (formData.skidQuantity < 1) {
    //        bootbox.alert('You selected the tariff to be calculated by skids; please enter Skid quantity in "Skids" field');
    //        return false;
    //    }
    //}

    if (formData.totalOrderCost <= 0) {
        bootbox.alert('Total order cost must be greater than zero. Please check your entry and try again.');
        return false;
    }

    return true;
}

$('#btnDispatch').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedOrdersForDispatch.length < 1) {
        bootbox.alert('Please select order/s to be dispatched from the order list.');
        return;
    }

    $('#dispatchOrder').modal({
        backdrop: "static",
        keyboard: false
    });
    $('#dispatchOrder').draggable();
    $('#dispatchOrder').modal('show');

    $('#lblOrderNumbers').text(selectedOrdersForDispatch);

    $('#txtEmployeeName').val('');
    $('#hfDispatchToEmployeeId').val('');
    $('#txtOrderPortionForNewOrders').val('');
});

$('#btnDispatchToEmployee').unbind().on('click', function (event) {
    event.preventDefault();

    var selectedEmployeeId = $('#hfDispatchToEmployeeId').val();
    var dispatchDate = $('#txtDispatchDatetimeForNewOrders').val();
    var isPercent = parseInt($('#ddlShareType').val()) === 1 ? true : false;
    var sharePortion = $('#txtOrderPortionForNewOrders').val() === '' ? 0 : parseFloat($('#txtOrderPortionForNewOrders').val());
    var isSendEmail = $('#chkSendEmail').is(':checked') === true ? 1 : 0;
    var emailAddress = $('#txtEmailAddressForDispatch').val();


    var vehicleId = 0; //Get vehicle Id feature

    if (selectedEmployeeId < 1) {
        bootbox.alert('Please select an employee to dispatch the order/s');
        return;
    }

    if (dispatchDate === null || dispatchDate === "") {
        bootbox.alert('Please enter dispatch date');
        return;
    }

    if (isPercent === true) {
        if (sharePortion > 100) {
            bootbox.alert('Percent amount cannot be greater than 100.');
            return;
        }
    }

    if (isSendEmail === 1) {
        if (emailAddress === '') {
            bootbox.alert('Please enter email address.');
            return;
        }
    }

    var dataArray = [selectedOrdersForDispatch, selectedEmployeeId, dispatchDate, vehicleId, isPercent, sharePortion, isSendEmail, emailAddress];

    var result = PerformPostActionWithObject('Order/UpdateDispatchStatus', dataArray);
    if (result.length > 0) {
        $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
        $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        selectedOrdersForDispatch = [];

        $('#dispatchOrder').modal('hide');
        location.reload();
    }

});
$('#order-list .chkDispatchToEmployee').change(function (event) {
    //event.preventDefault();
    var isChecked = $(this).is(':checked');
    var orderId = $(this).data('waybillnumber');
    getSelectedOrder(isChecked, orderId);

});
$('#orderdelivered-list .chkOrderToPrint').change(function (event) {
    //event.preventDefault();
    var isChecked = $(this).is(':checked');
    var orderId = $(this).data('waybillnumber');
    getSelectedOrder(isChecked, orderId);
});
$('#orderdelivered-list').on('click', '.btnUndoDelivery', function (event) {
    event.preventDefault();
    var orderId = $(this).data('waybillnumber');
    if (orderId !== '') {
        bootbox.confirm("Delivery information related to this order will be deleted. Are you sure to proceed?", function (result) {
            if (result === true) {
                var status = PerformPostActionWithId('Order/RemoveDeliveryStatusByWaybill', orderId);
                if (status.length > 0) {
                    location.reload();
                }
            }
        });
    }

});
function getSelectedOrder(isChecked, wbNumber) {
    var index = selectedOrdersForDispatch.indexOf(wbNumber);
    if (index >= 0) {
        selectedOrdersForDispatch.splice(index, 1);
    }

    if (isChecked) {
        selectedOrdersForDispatch.push(wbNumber);
    }
}

$("#btnDeliveredOrder").click(function () {
    var btnText = $("#lblShowHide").text();
    if (btnText == "SHOW") {
        $("#loadOrdersToBeDispatched").css("display", "none");
        $("#loadOrdersDelivered").css("display", "block");
        $("#lblShowHide").text("HIDE");
        $("#btnDispatch").prop("disabled", true);
        $('#order-list, #orderdelivered-list .chkDispatchToEmployee, .chkOrderToPrint, .chkCheckAllOrders, .chkCheckAllDelOrders').prop('checked', false);
        selectedOrdersForDispatch = [];

        $("#chkIgnorePricingInformation").prop("checked", false);
        $("#ddlNumberOfCopies").val("1");
        $("#ddlCopyOnPage").val("1");

    }
    else {
        $("#loadOrdersToBeDispatched").css("display", "block");
        $("#loadOrdersDelivered").css("display", "none");
        $("#lblShowHide").text("SHOW");
        $("#btnDispatch").prop("disabled", false);
        $('#orderdelivered-list, #order-list .chkDispatchToEmployee, .chkOrderToPrint, .chkCheckAllOrders, .chkCheckAllDelOrders').prop('checked', false);
        selectedOrdersForDispatch = [];
        $("#chkIgnorePricingInformation").prop("checked", true);
        $("#ddlNumberOfCopies").val("2");
        $("#ddlCopyOnPage").val("2");
    }
});
$('#order-list, #orderdelivered-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    ClearForm();
    $('#frmOrderForm').trigger('reset');

    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
            $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
        });
    }

    var wbNumber = $(this).data('waybillnumber');
    if (wbNumber > 0) {
        GetAndFillOrderDetailsByWayBillNumber(wbNumber, 1);
    }

    //$('#txtWayBillNo').prop('disabled', true);
    isNewEntry = false;

    $('#newOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newOrder').draggable();
    $('#newOrder').modal('show');

});

$('#txtInvoicedOrderNumber').on('keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        var orderId = $('#txtInvoicedOrderNumber').val();
        ModifyReleasedOrder(orderId);
    }
});
$('#btnExistingInvoicedOrder').unbind().on('click', function (event) {
    event.preventDefault();
    var orderId = $('#txtInvoicedOrderNumber').val();
    ModifyReleasedOrder(orderId);
});
function ModifyReleasedOrder(orderId) {
    if (orderId !== '') {
        var orderInfo = GetSingleById('Order/GetReleaseStatusByOrderId', orderId);
        if (orderInfo !== '') {
            ClearForm();
            $('#frmOrderForm').trigger('reset');

            var countries = GetList('Country/GetAllCountries');
            if (countries !== '') {
                var parsedCountries = JSON.parse(countries);
                $.each(parsedCountries, function (i, item) {
                    $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
                    $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
                });
            }

            GetAndFillOrderDetailsByWayBillNumber(orderId, 1);

            //$('#txtWayBillNo').prop('disabled', true);
            isNewEntry = false;

            $('#newOrder').modal({
                backdrop: 'static',
                keyboard: false
            });
            $('#newOrder').draggable();
            $('#newOrder').modal('show');
        } else {
            bootbox.alert('Please enter a valid order. Release the invoice if invoice was generated for this order.');
            return;
        }
    }
}

$('#order-list').on('change', '.chkCheckAllOrders', function (event) {
    event.preventDefault();
    var isChecked = $(this).is(':checked');
    var wbArrayString = $('.hfWaybillArray').val();
    getSelectedOrders(isChecked, wbArrayString, false);
});

$('#orderdelivered-list').on('change', '.chkCheckAllDelOrders', function (event) {
    event.preventDefault();
    var isChecked = $(this).is(':checked');
    var wbDelArrayString = $('.hfDelWaybillArray').val();
    getSelectedOrders(isChecked, wbDelArrayString, true);
});
function getSelectedOrders(isChecked, wbArrayString, isDelivered) {
    selectedOrdersForDispatch = [];
    if (isChecked === true) {
        if (isDelivered) {
            $('.chkOrderToPrint').prop('checked', isChecked);
        } else {
            $('.chkDispatchToEmployee').prop('checked', isChecked);
        }

        var wbArray = wbArrayString.split(',');
        $.each(wbArray, function (i, item) {
            if (item !== '') {
                selectedOrdersForDispatch.push(parseInt(item));
            }
        });
    } else {
        $('.chkDispatchToEmployee').prop('checked', isChecked);
        $('.chkOrderToPrint').prop('checked', isChecked);
    }
}


$('#btnTrialPrintWaybill').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedOrdersForDispatch.length < 1) {
        bootbox.alert('Please select order/s to print.');
        event.preventDefault();
        return;
    }

    var printUrl = 'Order/PrintWaybillAsPdf';
    var printOption = {
        numberOfcopyOnEachPage: $('#ddlCopyOnPage').val(),
        numberOfcopyPerItem: $('#ddlNumberOfCopies').val(),
        ignorePrice: $('#chkIgnorePricingInformation').is(':checked') === true ? 1 : 0,
        orderTypeId: 3,
        isMiscellaneous: 0,
        viewName: 'PrintDeliveryWaybill',
        printUrl: printUrl
    };

    var dataArray = [selectedOrdersForDispatch, printOption];

    $.ajax({
        'async': false,
        url: "Order/PrintWaybillAsPdf",
        type: 'POST',
        data: JSON.stringify(dataArray),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
            if (result.length > 0) {
                window.open(result, "_blank");
            }
        },
        error: function (result) {
            bootbox.alert('Printing failed! There are some eror occurred while processing the printing.');
        }
    });

});

$('#ddlShipperCityId').on('change', function () {

});
$('#ddlShipperCountries').on('change', function () {
    var countryId = $('#ddlShipperCountries').val();
    var cities = GetListById('City/GetCitiesByCountry', countryId);
    $('#ddlShipperCityId').empty();
    $('#ddlShipperCityId').append($('<option></option>').val('0').html('City'));
    if (cities !== '') {
        var parsedCities = JSON.parse(cities);
        $.each(parsedCities, function (i, item) {
            $('#ddlShipperCityId').append($('<option></option>').val(item.Id).html(item.CityName));
        });
    }
});
$('#ddlConsigneeCountries').on('change', function () {
    var countryId = $('#ddlConsigneeCountries').val();
    var cities = GetListById('City/GetCitiesByCountry', countryId);
    $('#ddlConsigneeCityId').empty();
    $('#ddlConsigneeCityId').append($('<option></option>').val('0').html('City'));
    if (cities !== '') {
        var parsedCities = JSON.parse(cities);
        $.each(parsedCities, function (i, item) {
            $('#ddlConsigneeCityId').append($('<option></option>').val(item.Id).html(item.CityName));
        });
    }
});

$('#btnPrePrintedWaybill').on('click', function (event) {
    event.preventDefault();
    $('#txtFromNo').val('');
    $('#txtToNo').val('');

    var result = GetObject('Order/GetNextWaybillNumber');
    console.log(result);

    $('#txtFromNo').val(result);
    $('#txtToNo').val(parseInt(result) + 10);

    $('#prePrintedWaybill').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#prePrintedWaybill').draggable();
    $('#prePrintedWaybill').modal('show');

});

$('#btnPrintPrePrintedWaybill').unbind().on('click', function () {

    var fromNumber = $('#txtFromNo').val();
    var toNumber = $('#txtToNo').val();

    var result = PerformPostActionWithObject('Order/AddPrePrintedWaybill?fromNumber=' + fromNumber + '&&toNumber=' + toNumber);
    if (result !== "") {
        $('#prePrintedWaybill').modal('hide');
        bootbox.alert('Preprinted waybills pdf created.');
        window.open(result, "_blank");
    }
});

//#endregion


//#region Private methods

function GetAndFillOrderDetailsByWayBillNumber(wayBillNumber, orderTypeId) {
    var orderData = null;
    var orderAdditionalServiceData = null;

    var orderInfo = GetSingleById('Order/GetOrderDetailsByWayBillId', wayBillNumber);
    var parseData = JSON.parse(orderInfo);

    orderData = parseData.orderPocos.filter(function (item) {
        return item.OrderTypeId === orderTypeId;
    })[0];

    if (orderData !== null && orderData !== undefined && orderData !== '') {
        ClearForm();

        $('#hfOrderId').val(orderData.Id);
        $('#hfBillerCustomerId').val(orderData.BillToCustomerId);

        orderAdditionalServiceData = parseData.orderAdditionalServices.filter(function (item) {
            return item.OrderId === orderData.Id;
        });

        FillOrderAdditionalServices(orderAdditionalServiceData);
        FillOrderDetails(orderData);
    }
    else {

        if (orderTypeId === 2) {

            $('#hfOrderId').val('');
            selectedAdditionalServiceArray = [];
            $('#service-list').empty();

            var billerId = $('#hfBillerCustomerId').val();

            var shippAccNo = $('#txtShipperAccountNo').val();
            var consigAccNo = $('#txtConsigneeAccountNo').val();

            var shippName = $('#txtShipperCustomerName').val();
            var consigName = $('#txtConsigneeCustomerName').val();

            var shipperAddressId = $('#hfShipperAddressId').val();
            var consigneeAddressId = $('#hfConsigneeAddressId').val();

            if (shipperAddressId == '' || consigneeAddressId == '') {
                MakeReturnToggleOff();
                bootbox.alert('Shipper and/or consignee address was not found. Please make sure the waybill has valid order before placing a return order.');
                return;
            }

            $('#txtShipperAccountNo').val(consigAccNo);
            $('#txtConsigneeAccountNo').val(shippAccNo);

            $('#txtShipperCustomerName').val(consigName);
            $('#txtConsigneeCustomerName').val(shippName);


            FillShipperAddress(consigneeAddressId);
            FillConsigneeAddress(shipperAddressId);

            var paidByValue = $('input[name=rdoPaidBy]:checked').val();
            if (paidByValue === '1') {
                paidByValue = '2';
                $('#rdoShipper').prop('checked', false);
                $('#rdoConsignee').prop('checked', true);
                $('#txtBillToCustomerName').val(shippName);
            }
            else if (paidByValue === '2') {
                paidByValue = '1';
                $('#rdoConsignee').prop('checked', false);
                $('#rdoShipper').prop('checked', true);
                $('#txtBillToCustomerName').val(consigName);
            }

            $('#txtDiscountPercent').val('');
            $('#txtUnitQuantity').val('');
            $('#txtSkidQuantity').val('');
            $('#txtTotalPieces').val('');
            $('#txtWeightTotal').val('');

            $('#txtUnitQuantity').change();
            $('#txtOverriddenOrderCost').val('');
            $('#txtOverriddenOrderCost').change();
        }

    }
}

function GetCustomerInfo(customerId) {
    var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);
    return customerInfo;
}

function GetCustomerDefaultShippingAddress(customerId) {
    var shippingAddressId = '';
    var shippingAddress = GetDefaultShippingAddressByCustomer(customerId);
    if (shippingAddress != null) {
        shippingAddressId = shippingAddress.AddressId;
    }
    return shippingAddressId;
}
function GetCustomerDefaultBillingAddress(customerId) {
    var billingAddressId = '';
    var billingAddress = GetDefaultBillingAddressByCustomer(customerId);
    if (billingAddress != null) {
        billingAddressId = billingAddress.AddressId;
    }
    return billingAddressId;
}

function FillShipperAddress(addressId) {
    var addressInfo = GetAddressInfo(addressId);
    if (addressInfo !== '') {
        var parsedAddress = JSON.parse(addressInfo);
        if (parsedAddress != null) {
            $('#hfShipperAddressId').val(parsedAddress.Id);
            $('#txtShipperAddressLine').val(parsedAddress.AddressLine);
            $('#txtShipperUnitNo').val(parsedAddress.UnitNumber);
            $('#ddlShipperCountries').val(parsedAddress.CountryId);
            $('#ddlShipperCountries').trigger('change');
            $('#ddlShipperCityId').val(parsedAddress.CityId);
            $('#ddlShipperProvinceId').val(parsedAddress.ProvinceId);
            $('#txtShipperPostcode').val(parsedAddress.PostCode);
        }
    } else {
        $('#hfShipperAddressId').val('');
        $('#txtShipperUnitNo').val('');
        $('#ddlShipperCityId').val('335');
        $('#ddlShipperProvinceId').val('7');
        $('#txtShipperPostcode').val('');
        $('#ddlShipperCountries').val('41');
        bootbox.alert('Shipper address not found. Please try again.');
    }

}

function FillConsigneeAddress(addressId) {
    var addressInfo = GetAddressInfo(addressId);
    if (addressInfo !== '') {
        var parsedAddress = JSON.parse(addressInfo);
        if (parsedAddress != null) {
            $('#hfConsigneeAddressId').val(parsedAddress.Id);
            $('#txtConsigneeAddressLine').val(parsedAddress.AddressLine);
            $('#txtConsigneeUnitNo').val(parsedAddress.UnitNumber);
            $('#ddlConsigneeCountries').val(parsedAddress.CountryId);
            $('#ddlConsigneeCountries').trigger('change');
            $('#ddlConsigneeCityId').val(parsedAddress.CityId);
            $('#ddlConsigneeProvinceId').val(parsedAddress.ProvinceId);
            $('#txtConsigneePostcode').val(parsedAddress.PostCode);
        }
    } else {
        $('#hfConsigneeAddressId').val('');
        $('#txtConsigneeUnitNo').val('');
        $('#ddlConsigneeCityId').val('335');
        $('#ddlConsigneeProvinceId').val('7');
        $('#txtConsigneePostcode').val('');
        $('#ddlConsigneeCountries').val('41');
        bootbox.alert('Consignee address not found. Please try again.');
    }
}

function GetAddressInfo(addressId) {
    var addressInfo = GetSingleById('Address/GetAddressById', addressId);
    return addressInfo;
}

function GetTariffInfo() {
    var shipperCityId = $('#ddlShipperCityId').val();
    var consigneeCityId = $('#ddlConsigneeCityId').val();
    var deliveryOptionId = $('#ddlDeliveryOptionId').val();
    var vehicleTypeId = 1; // $('#ddlVehicleTypeId').val();
    var unitTypeId = 0;
    var unitQuantity = 0;

    var calculateByUnit = $('#chkIsCalculateByUnit').is(':checked');
    if (calculateByUnit === true) {
        if ($('#txtUnitQuantity').val() !== '') {
            unitTypeId = $('#ddlUnitTypeId').val();
            unitQuantity = $('#txtUnitQuantity').val();
        }

    } else {
        unitTypeId = 2;
        unitQuantity = $('#txtSkidQuantity').val();
        if (unitQuantity === '' || unitQuantity <= 0) {
            return;
        }
    }

    var weightScaleId = $('#ddlWeightScaleId').val();
    var weightQuantity = $('#txtWeightTotal').val();

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

    var tariffCost = GetListByObject('Order/GetTariffCostByParam', dataForTariff);
    return tariffCost;
}

function CalculateAdditionalServiceCost() {
    var totalAdditionalServiceCost = 0.0;
    if (selectedAdditionalServiceArray.length > 0) {
        for (var i = 0; i < selectedAdditionalServiceArray.length; i++) {
            //if (selectedAdditionalServiceArray[i].additionalServiceFee > 0) { //accept minus as per client to add discounts
            totalAdditionalServiceCost = totalAdditionalServiceCost + selectedAdditionalServiceArray[i].additionalServiceFee;
            if (selectedAdditionalServiceArray[i].isTaxAppliedOnAddionalService && selectedAdditionalServiceArray[i].taxAmountOnAdditionalService > 0) {
                var addServiceTax = selectedAdditionalServiceArray[i].taxAmountOnAdditionalService * selectedAdditionalServiceArray[i].additionalServiceFee / 100;
                totalAdditionalServiceCost = totalAdditionalServiceCost + addServiceTax;
            }

            //}
        }
    }
    //if (totalAdditionalServiceCost > 0) {
    $('#lblGrandAddServiceAmount').text(totalAdditionalServiceCost.toFixed(2));
    //} else {
    //    $('#lblGrandAddServiceAmount').text('0.00');
    //}


}

function CalculateOrderBaseCost() {

    var fuelSurchargePercentage = $('#txtFuelSurchargePercent').val() !== "" ? parseFloat($('#txtFuelSurchargePercent').val()) : 0.0;
    var discountPercentage = $('#txtDiscountPercent').val() !== "" ? parseFloat($('#txtDiscountPercent').val()) : 0.0;
    var overriddenOrderCost = $('#txtOverriddenOrderCost').val() !== "" ? parseFloat($('#txtOverriddenOrderCost').val()) : 0.0;

    var isGstApplicable = $('#chkIsGstApplicable').is(':checked');
    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

    var overriddenDiscountAmount = 0.0;
    var overriddenFuelSurchargeAmount = 0.0;
    var overriddenTaxAmount = 0.0;
    var baseDiscountAmount = 0.0;
    var baseTaxAmount = 0.0;

    var baseOrderCost = parseFloat(GetTariffInfo());

    $('#txtBaseOrderCost').val(baseOrderCost.toFixed(2));
    if (fuelSurchargePercentage > 0) {
        baseFuelSurchargeAmount = fuelSurchargePercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost + baseFuelSurchargeAmount;
        $('#txtBaseOrderSurcharge').val(baseFuelSurchargeAmount.toFixed(2));
    }
    else {
        baseFuelSurchargeAmount = 0;
        $('#txtBaseOrderSurcharge').val('');
    }
    if (discountPercentage > 0) {
        baseDiscountAmount = discountPercentage * baseOrderCost / 100;
        baseOrderCost = baseOrderCost - baseDiscountAmount;
    }
    if (taxPercentage > 0 && isGstApplicable === true) {
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
        else {
            overriddenFuelSurchargeAmount = 0;
            $('#txtOverriddenOrderSurcharge').val('');
        }

        if (discountPercentage > 0) {
            overriddenDiscountAmount = discountPercentage * overriddenOrderCost / 100;
            overriddenOrderCost = overriddenOrderCost - overriddenDiscountAmount;
        }
        else {
            overriddenDiscountAmount = 0;
        }

        if (taxPercentage > 0 && isGstApplicable === true) {
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
        $('#txtOverriddenOrderGST').val('');
        $('#txtOverriddenOrderSurcharge').val('');

        if ($('#txtBaseOrderCost').val() > 0) {
            $('#lblGrandBasicCost').text($('#txtBaseOrderCost').val());
        }
        else {
            $('#lblGrandBasicCost').text('0.00');
        }


        overriddenFuelSurchargeAmount = baseFuelSurchargeAmount;
        overriddenDiscountAmount = baseDiscountAmount;
        overriddenTaxAmount = baseTaxAmount;
    }

    if (overriddenDiscountAmount > 0) {
        $('#lblGrandDiscountAmount').text(overriddenDiscountAmount.toFixed(2));
    }
    else {
        $('#lblGrandDiscountAmount').text('0.00');
    }

    if (overriddenTaxAmount > 0) {
        $('#lblGrandGstAmount').text(overriddenTaxAmount.toFixed(2));
    } else {
        $('#lblGrandGstAmount').text('0.00');
    }

    if (overriddenFuelSurchargeAmount > 0) {
        $('#lblGrandFuelSurchargeAmount').text(overriddenFuelSurchargeAmount.toFixed(2));
    }
    else {
        $('#lblGrandFuelSurchargeAmount').text('0.00');
    }

    if (overriddenOrderCost > 0) {
        $('#lblGrandTotalOrderCost').text(overriddenOrderCost.toFixed(2));
    }
    else {
        $('#lblGrandTotalOrderCost').text('0.00');
    }



    var grandTotal = overriddenOrderCost;

    CalculateAdditionalServiceCost();

    var totalAdditionalServiceCost = $('#lblGrandAddServiceAmount').text() === "" ? 0.0 : parseFloat($('#lblGrandAddServiceAmount').text());

    //if (totalAdditionalServiceCost > 0) { // will accept minus figure as per client 
    grandTotal = grandTotal + totalAdditionalServiceCost;
    //}

    if (grandTotal > 0) {
        $('#lblGrandTotalAmount').text(grandTotal.toFixed(2));
    }
    else {
        $('#lblGrandTotalAmount').text('0.00');
    }

}

function FillOrderDetails(orderRelatedData) {
    if (orderRelatedData !== null) {

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
        $('#txtPickupRefNo').val(orderRelatedData.PickupReferenceNumber);
        $('#txtDeliveryRefNo').val(orderRelatedData.DeliveryReferenceNumber);

        $('#txtShipperAccountNo').val(orderRelatedData.ShipperCustomerId);
        if (orderRelatedData.ShipperCustomerId != '' && orderRelatedData.ShipperCustomerId != null) {
            var shipperInfo = GetCustomerInfo(orderRelatedData.ShipperCustomerId);
            if (shipperInfo != '' && shipperInfo != null) {
                var shipperCustomerInfo = JSON.parse(shipperInfo);
                $('#txtShipperCustomerName').val(shipperCustomerInfo.CustomerName);
            }
        }
        if (orderRelatedData.ShipperAddressId != '' && orderRelatedData.ShipperAddressId != null) {
            FillShipperAddress(orderRelatedData.ShipperAddressId);
        }

        $('#txtConsigneeAccountNo').val(orderRelatedData.ConsigneeCustomerId);
        if (orderRelatedData.ConsigneeCustomerId != '' && orderRelatedData.ConsigneeCustomerId != null) {
            var consigneeInfo = GetCustomerInfo(orderRelatedData.ConsigneeCustomerId);
            if (consigneeInfo != '' && consigneeInfo != null) {
                var consigneeCustomerInfo = JSON.parse(consigneeInfo);
                $('#txtConsigneeCustomerName').val(consigneeCustomerInfo.CustomerName);
            }
        }
        if (orderRelatedData.ConsigneeAddressId != '' && orderRelatedData.ConsigneeAddressId != null) {
            FillConsigneeAddress(orderRelatedData.ConsigneeAddressId);
        }

        if (orderRelatedData.BillToCustomerId != '' && orderRelatedData.BillToCustomerId != null) {
            var billerInfo = GetCustomerInfo(orderRelatedData.BillToCustomerId);
            if (billerInfo != '' && billerInfo != null) {
                var billerCustomerInfo = JSON.parse(billerInfo);
                $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
                $('#hfBillerCustomerId').val(orderRelatedData.BillToCustomerId);
            }
        }

        if (orderRelatedData.ShipperCustomerId != null && orderRelatedData.BillToCustomerId != null && orderRelatedData.ConsigneeCustomerId != null) {
            if (orderRelatedData.ShipperCustomerId === orderRelatedData.BillToCustomerId) {
                $('#rdoShipper').prop('checked', true);
                paidByValue = '1';
            }
            else if (orderRelatedData.ConsigneeCustomerId === orderRelatedData.BillToCustomerId) {
                $('#rdoConsignee').prop('checked', true);
                paidByValue = '2';
            }
            else {
                $('#rdoThirdParty').prop('checked', true);
                paidByValue = '3';
            }
        }

        if (orderRelatedData.ScheduledPickupDate !== null) {
            $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(orderRelatedData.ScheduledPickupDate));
        } else {
            $('#txtSchedulePickupDate').val(ConvertDateToUSFormat(new Date));
        }


        $('#ddlDeliveryOptionId').val(orderRelatedData.DeliveryOptionId);
        $('#ddlVehicleTypeId').val(orderRelatedData.VehicleTypeId);


        $('#txtUnitQuantity').val(orderRelatedData.UnitQuantity);
        if (orderRelatedData.UnitTypeId > 0) { $('#ddlUnitTypeId').val(orderRelatedData.UnitTypeId); }


        $('#txtSkidQuantity').val(orderRelatedData.SkidQuantity);
        $('#txtTotalPieces').val(orderRelatedData.TotalPieces);

        $('#ddlWeightScaleId').val(orderRelatedData.WeightScaleId);
        $('#txtWeightTotal').val(orderRelatedData.WeightTotal);
        $('#txtBaseOrderCost').val(orderRelatedData.OrderBasicCost);
        $('#txtFuelSurchargePercent').val(orderRelatedData.FuelSurchargePercentage);
        $('#txtDiscountPercent').val(orderRelatedData.DiscountPercentOnOrderCost);
        if (orderRelatedData.ApplicableGstPercent > 0) {
            $('#chkIsGstApplicable').prop('checked', true);
            isCustomerTaxApplicable = true;
        } else {
            $('#chkIsGstApplicable').prop('checked', false);
            isCustomerTaxApplicable = false;
        }
        //$('#lblGstAmount').text(orderRelatedData.applicableGstPercent);


        $('#txtOrderedBy').val(orderRelatedData.OrderedBy);

        $('#txtContactPerson').val(orderRelatedData.ContactName);
        $('#txtContactPhone').val(orderRelatedData.ContactPhoneNumber);
        $('#txtRemarks').val(orderRelatedData.Remarks);

        $('#chkIsPrintOnWayBill').prop('checked', orderRelatedData.IsPrintedOnWayBill);
        $('#txtCommentsForWayBill').val(orderRelatedData.CommentsForWayBill);
        $('#chkIsPrintOnInvoice').prop('checked', orderRelatedData.IsPrintedOnInvoice);
        $('#txtCommentsForInvoice').val(orderRelatedData.CommentsForInvoice);

        if (orderRelatedData.OrderBasicCost > 0) {
            $('#txtBaseOrderCost').val(orderRelatedData.OrderBasicCost.toFixed(2));
        }
        else {
            $('#txtBaseOrderCost').val('');
        }
        if (orderRelatedData.FuelSurchargePercentage > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#txtBaseOrderSurcharge').val((orderRelatedData.FuelSurchargePercentage * orderRelatedData.OrderBasicCost / 100).toFixed(2));
        } else {
            $('#txtBaseOrderSurcharge').val('');
        }


        var fuelSurChargeAmnt = $('#txtBaseOrderSurcharge').val() > 0 ? parseFloat($('#txtBaseOrderSurcharge').val()) : 0;

        if (orderRelatedData.ApplicableGstPercent > 0 && orderRelatedData.OrderBasicCost > 0) {
            var discountAmnt = 0;
            if (orderRelatedData.DiscountPercentOnOrderCost > 0) {
                discountAmnt = orderRelatedData.DiscountPercentOnOrderCost * (orderRelatedData.OrderBasicCost + fuelSurChargeAmnt) / 100;
            }
            $('#txtBaseOrderGST').val((orderRelatedData.ApplicableGstPercent * (orderRelatedData.OrderBasicCost + fuelSurChargeAmnt - discountAmnt) / 100).toFixed(2));
        } else {
            $('#txtBaseOrderGST').val('');
        }

        if (orderRelatedData.BasicCostOverriden > 0) {
            $('#txtOverriddenOrderCost').val(orderRelatedData.BasicCostOverriden.toFixed(2));

        } else {
            $('#txtOverriddenOrderCost').val('');
        }
        if (orderRelatedData.FuelSurchargePercentage > 0 && orderRelatedData.BasicCostOverriden > 0) {
            $('#txtOverriddenOrderSurcharge').val((orderRelatedData.FuelSurchargePercentage * orderRelatedData.BasicCostOverriden / 100).toFixed(2));
        } else {
            $('#txtOverriddenOrderSurcharge').val('');
        }


        var fuelSurChargeOverriddenAmnt = $('#txtOverriddenOrderSurcharge').val() > 0 ? parseFloat($('#txtOverriddenOrderSurcharge').val()) : 0;

        if (orderRelatedData.ApplicableGstPercent > 0 && orderRelatedData.BasicCostOverriden > 0) {
            var discountAmnt = 0;
            if (orderRelatedData.DiscountPercentOnOrderCost > 0) {
                discountAmnt = orderRelatedData.DiscountPercentOnOrderCost * (orderRelatedData.BasicCostOverriden + fuelSurChargeOverriddenAmnt) / 100;
            }
            $('#txtOverriddenOrderGST').val((orderRelatedData.ApplicableGstPercent * (orderRelatedData.BasicCostOverriden + fuelSurChargeOverriddenAmnt - discountAmnt) / 100).toFixed(2));
        } else {
            $('#txtOverriddenOrderGST').val('');
        }


        if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.BasicCostOverriden > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * (orderRelatedData.BasicCostOverriden + fuelSurChargeOverriddenAmnt) / 100).toFixed(2));
        } else if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * (orderRelatedData.OrderBasicCost + fuelSurChargeAmnt) / 100).toFixed(2));
        }
        else {
            $('#lblGrandDiscountAmount').text('0.00');
        }



        if (orderRelatedData.BasicCostOverriden > 0) {
            $('#lblGrandBasicCost').text(orderRelatedData.BasicCostOverriden.toFixed(2));
        }
        else if (orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandBasicCost').text(orderRelatedData.OrderBasicCost.toFixed(2));
        }
        else {
            $('#lblGrandBasicCost').text('0.00');
        }

        if ($('#txtOverriddenOrderGST').val() > 0) {
            $('#lblGrandGstAmount').text($('#txtOverriddenOrderGST').val());
        }
        else if ($('#txtBaseOrderGST').val() > 0) {
            $('#lblGrandGstAmount').text($('#txtBaseOrderGST').val());
        }
        else {
            $('#lblGrandGstAmount').text('0.00');
        }

        if ($('#txtOverriddenOrderSurcharge').val() > 0) {
            $('#lblGrandFuelSurchargeAmount').text($('#txtOverriddenOrderSurcharge').val());
        }
        else if ($('#txtBaseOrderSurcharge').val() > 0) {
            $('#lblGrandFuelSurchargeAmount').text($('#txtBaseOrderSurcharge').val());
        }
        else {
            $('#lblGrandFuelSurchargeAmount').text('0.00');
        }

        $('#lblGrandTotalOrderCost').text(orderRelatedData.TotalOrderCost.toFixed(2));
        $('#lblGrandAddServiceAmount').text(orderRelatedData.TotalAdditionalServiceCost.toFixed(2));
        $('#lblGrandTotalAmount').text((orderRelatedData.TotalOrderCost + orderRelatedData.TotalAdditionalServiceCost).toFixed(2));

    }
}

function FillOrderAdditionalServices(orderAdditionalServiceData) {

    selectedAdditionalServiceArray = [];
    $('#service-list').empty();

    if (orderAdditionalServiceData !== null) {
        for (var i = 0; i < orderAdditionalServiceData.length; i++) {
            var serviceData = {
                serialNo: i + 1,
                orderId: orderAdditionalServiceData[i].OrderId,
                additionalServiceId: orderAdditionalServiceData[i].AdditionalServiceId,
                unitPrice: orderAdditionalServiceData[i].UnitPrice,
                quantity: orderAdditionalServiceData[i].Quantity,
                isPayToDriver: orderAdditionalServiceData[i].IsPayToDriver,
                driverPercentageOnAddService: orderAdditionalServiceData[i].DriverPercentageOnAddService === "" ? 0 : parseFloat(orderAdditionalServiceData[i].DriverPercentageOnAddService),
                additionalServiceFee: parseFloat(orderAdditionalServiceData[i].AdditionalServiceFee),
                isTaxAppliedOnAddionalService: orderAdditionalServiceData[i].IsTaxAppliedOnAddionalService,
                taxAmountOnAdditionalService: orderAdditionalServiceData[i].TaxAmountOnAdditionalService
            };

            selectedAdditionalServiceArray.push(serviceData);

            var additionalServiceInfo = JSON.parse(GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceData.additionalServiceId));
            if (additionalServiceInfo !== null && additionalServiceInfo !== undefined && additionalServiceInfo !== '') {
                //$('#btnAddAddtionalServiceRow').trigger('click');
                $('#service-list').append(GenerateNewAdditionalServiceRow());
                var services = JSON.parse(GetList('AdditionalService/GetAdditionalServiceList'));
                $.each(services, function (i, item) {
                    $('#service-list .additionalServices').append($('<option>').attr('data-serviceid', item.Id).val(item.ServiceName));
                });

                var currentRow = $('#service-list').children('div.row').eq(i);
                currentRow.find('.hfServiceId').val(serviceData.additionalServiceId);
                currentRow.find('.txtAdditionalServiceName').val(additionalServiceInfo.ServiceName);
                currentRow.find('.hfSerialNo').val(serviceData.serialNo);
                currentRow.find('.chkPayToDriver').prop('checked', serviceData.isPayToDriver);

                
                if (serviceData.unitPrice > 0) {
                    currentRow.find('.txtUnitPrice').val(serviceData.unitPrice);
                } else {
                    currentRow.find('.txtUnitPrice').val("");
                }
                if (serviceData.quantity > 0) {
                    currentRow.find('.txtQuantity').val(serviceData.quantity);
                } else {
                    currentRow.find('.txtQuantity').val("");
                }

                if (additionalServiceInfo.IsPriceApplicable == true) {
                    currentRow.find('.txtAdditionalServiceName').data("isunitpriceapplicable", "1");
                } else {
                    currentRow.find('.txtAdditionalServiceName').data("isunitpriceapplicable", "0");
                }

                currentRow.find('.txtServiceFee').val(serviceData.additionalServiceFee);
                currentRow.find('.txtDriverPercentage').val(serviceData.driverPercentageOnAddService);
                currentRow.find('.chkIsGstApplicableForService').prop('checked', serviceData.isTaxAppliedOnAddionalService);
                currentRow.find('.txtAdditionalServiceName').prop("disabled", true);
                currentRow.find('.txtUnitPrice').prop("disabled", true);
                currentRow.find('.txtQuantity').prop("disabled", true);
                currentRow.find('.txtServiceFee').prop("disabled", true);
                currentRow.find('.chkPayToDriver').prop("disabled", true);
                currentRow.find('.txtDriverPercentage').prop("disabled", true);
                currentRow.find('.chkIsGstApplicableForService').prop("disabled", true);
                currentRow.find('.btnAddAdditionalService').prop('disabled', true);
                currentRow.find('.chkIsUpdate').prop('disabled', false);
                currentRow.find('.btnEditService').prop('disabled', true);
                currentRow.find('.btnDeleteAdditionalService').attr('data-serviceid', serviceData.additionalServiceId);

            }
        }

    }
}

function GetDefaultShippingAddressByCustomer(customerId) {
    var defaultShippingAddress = null;
    if (customerId !== '') {
        var shippingAddress = GetSingleById('Customer/GetCustomerDefaultShippingAddressById', customerId);
        if (shippingAddress !== '') {
            defaultShippingAddress = JSON.parse(shippingAddress);
        }
    }

    return defaultShippingAddress;
}
function GetDefaultBillingAddressByCustomer(customerId) {
    var defaultBillingAddress = null;
    if (customerId !== '') {
        var billingAddress = GetSingleById('Customer/GetCustomerDefaultBillingAddressById', customerId);
        if (billingAddress !== '') {
            defaultBillingAddress = JSON.parse(billingAddress);
        }
    }
    return defaultBillingAddress;
}

function GetFormData() {

    var date = $('#txtSchedulePickupDate').val();

    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : null;
    var isGstApplicable = $('#chkIsGstApplicable').is(':checked');
    if (isGstApplicable === false) {
        taxPercentage = null;
    }

    var orderData = {
        id: $('#hfOrderId').val() === "" ? 0 : parseInt($('#hfOrderId').val()),
        orderTypeId: $('#chkIsReturnOrder').is(':checked') === true ? 2 : 1,

        wayBillNumber: $('#txtWayBillNo').val() === "" ? 0 : parseInt($('#txtWayBillNo').val()),
        referenceNumber: $('#txtCustomerRefNo').val() === "" ? null : $('#txtCustomerRefNo').val(),
        cargoCtlNumber: $('#txtCargoCtlNo').val() === "" ? null : $('#txtCargoCtlNo').val(),
        awbCtnNumber: $('#txtAwbCtnNo').val() === "" ? null : $('#txtAwbCtnNo').val(),
        pickupReferenceNumber: $('#txtPickupRefNo').val() === "" ? null : $('#txtPickupRefNo').val(),
        deliveryReferenceNumber: $('#txtDeliveryRefNo').val() === "" ? null : $('#txtDeliveryRefNo').val(),

        shipperCustomerId: $('#txtShipperAccountNo').val() === "" ? 0 : parseInt($('#txtShipperAccountNo').val()),
        shipperAddressId: $('#hfShipperAddressId').val() === "" ? 0 : parseInt($('#hfShipperAddressId').val()),
        consigneeCustomerId: $('#txtConsigneeAccountNo').val() === "" ? 0 : parseInt($('#txtConsigneeAccountNo').val()),
        consigneeAddressId: $('#hfConsigneeAddressId').val() === "" ? 0 : parseInt($('#hfConsigneeAddressId').val()),
        billToCustomerId: $('#hfBillerCustomerId').val() === "" ? 0 : parseInt($('#hfBillerCustomerId').val()),

        shipperAddressline: $('#txtShipperAddressLine').val(),
        shipperUnitNo: $('#txtShipperUnitNo').val(),
        shipperCityId: $('#ddlShipperCityId').val() === "" ? 0 : parseInt($('#ddlShipperCityId').val()),
        shipperProvinceId: $('#ddlShipperProvinceId').val() === "" ? 0 : parseInt($('#ddlShipperProvinceId').val()),
        shipperPostcode: $('#txtShipperPostcode').val(),
        shipperCountryId: $('#ddlShipperCountries').val(),
        consigneeAddressline: $('#txtConsigneeAddressLine').val(),
        consigneeUnitNo: $('#txtConsigneeUnitNo').val(),
        consigneeCityId: $('#ddlConsigneeCityId').val() === "" ? 0 : parseInt($('#ddlConsigneeCityId').val()),
        consigneeProvinceId: $('#ddlConsigneeProvinceId').val() === "" ? 0 : parseInt($('#ddlConsigneeProvinceId').val()),
        consigneePostcode: $('#txtConsigneePostcode').val(),
        consigneeCountryId: $('#ddlConsigneeCountries').val(),


        scheduledPickupDate: date,
        expectedDeliveryDate: $('#txtSchedulePickupDate').val(),
        cityId: $('#ddlConsigneeCityId').val() === "" ? 0 : parseInt($('#ddlConsigneeCityId').val()),
        deliveryOptionId: parseInt($('#ddlDeliveryOptionId').val()),
        vehicleTypeId: 1, //parseInt($("#ddlVehicleTypeId").val()),
        unitTypeId: parseInt($('#ddlUnitTypeId').val()),
        weightScaleId: parseInt($('#ddlWeightScaleId').val()),
        weightTotal: $('#txtWeightTotal').val() === "" ? null : $('#txtWeightTotal').val(),
        unitQuantity: $('#txtUnitQuantity').val() === "" ? null : parseInt($('#txtUnitQuantity').val()),
        skidQuantity: $('#txtSkidQuantity').val() === "" ? null : parseInt($('#txtSkidQuantity').val()),
        totalPieces: $('#txtTotalPieces').val() === "" ? null : parseInt($('#txtTotalPieces').val()),

        orderBasicCost: $('#txtBaseOrderCost').val() === "" ? 0 : parseFloat($('#txtBaseOrderCost').val()),
        basicCostOverriden: $('#txtOverriddenOrderCost').val() === "" ? null : parseFloat($('#txtOverriddenOrderCost').val()),
        fuelSurchargePercentage: $('#txtFuelSurchargePercent').val() === "" ? null : parseFloat($('#txtFuelSurchargePercent').val()),
        discountPercentOnOrderCost: $('#txtDiscountPercent').val() === "" ? null : parseFloat($('#txtDiscountPercent').val()),
        applicableGstPercent: taxPercentage,
        totalOrderCost: $('#lblGrandTotalOrderCost').text() === "" ? null : parseFloat($('#lblGrandTotalOrderCost').text()),
        totalAdditionalServiceCost: $('#lblGrandAddServiceAmount').text() === "" ? null : parseFloat($('#lblGrandAddServiceAmount').text()),
        orderedBy: $('#txtOrderedBy').val() === "" ? null : $('#txtOrderedBy').val(),
        departmentName: null,
        contactName: $('#txtOrderedBy').val() === "" ? null : $('#txtOrderedBy').val(),
        contactPhoneNumber: null,

        commentsForWayBill: $('#txtCommentsForWayBill').val() === "" ? null : $('#txtCommentsForWayBill').val(),
        isPrintedOnWayBill: $('#chkIsPrintOnWayBill').is(':checked') === true ? 1 : 0,
        commentsForInvoice: $('#txtCommentsForInvoice').val() === "" ? null : $('#txtCommentsForInvoice').val(),
        isPrintedOnInvoice: $('#chkIsPrintOnInvoice').is(':checked') === true ? 1 : 0,

        remarks: $('#txtRemarks').val() === "" ? null : $('#txtRemarks').val()

    };

    return [orderData, selectedAdditionalServiceArray];
}

function ClearForm() {

    $('#hfOrderId').val('');
    paidByValue = '1';
    billerCustomerId = 0;
    shipperCustomerId = 0;
    consigneeCustomerId = 0;
    selectedAdditionalServiceArray = [];

    $('#txtBillToCustomerName').val('');

    $('#txtWayBillNo').val('');
    $('#ddlDeliveryOptionId').val(1);

    $('#txtCustomerRefNo').val('');
    $('#txtCargoCtlNo').val('');
    $('#txtAwbCtnNo').val('');
    $('#txtPickupRefNo').val('');
    $('#txtOrderedBy').val('');

    $('#txtShipperAccountNo').val('');

    $('#txtShipperCustomerName').val('');
    $('#txtShipperAddressLine').val('');
    $('#txtShipperUnitNo').val('');
    $('#ddlShipperCityId').val(335);
    $('#ddlShipperProvinceId').val(7);
    $('#ddlShipperCountries').val(41);
    $('#txtShipperPostcode').val('');

    $('#hfShipperAddressId').val('');
    $('#hfConsigneeAddressId').val('');

    $('#txtConsigneeAccountNo').val('');
    $('#txtConsigneeCustomerName').val('');
    $('#txtConsigneeAddressLine').val('');
    $('#txtConsigneeUnitNo').val('');
    $('#ddlConsigneeCityId').val(335);
    $('#ddlConsigneeProvinceId').val(7);
    $('#ddlConsigneeCountries').val(41);
    $('#txtConsigneePostcode').val('');

    $('#ddlUnitTypeId').val('1');
    $('#txtUnitQuantity').val('');
    $('#txtSkidQuantity').val('');
    $('#txtTotalPieces').val('');
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
    $('#service-list').empty();

    MakeReturnToggleOff();
}

function ClearShipperAddressArea() {
    $('#hfShipperAddressId').val('');
    $('#txtShipperAddressLine').val('');
    $('#txtShipperUnitNo').val('');
    $('#ddlShipperCityId').val('335');
    $('#ddlShipperProvinceId').val('7');
    $('#txtShipperPostcode').val('');
}

function ClearConsigneeAddressArea() {
    $('#hfConsigneeAddressId').val('');
    $('#txtConsigneeAddressLine').val('');
    $('#txtConsigneeUnitNo').val('');
    $('#ddlConsigneeCityId').val('335');
    $('#ddlConsigneeProvinceId').val('7');
    $('#txtConsigneePostcode').val('');
}

function GenerateNewAdditionalServiceRow() {
    var appendString = '';
    additionalServiceSerial += 1;

    appendString += '<div class="row ml-0 mr-0 additionalServiceBg">';
    appendString += '<div class="form-group pt-1 pl-1 pb-1 pr-0  mr-1">';
    appendString += '<input type="hidden" class="hfServiceId" name="hfServiceId">';
    appendString += '<input class="form-control form-control-sm additionalServiceControl txtAdditionalServiceName" style="width:156px;" data-isunitpriceapplicable="0" placeholder="Service name" list="additionalServices" type="search" />';
    appendString += '<datalist id="additionalServices" class="additionalServices">';
    appendString += '</datalist>';
    appendString += '</div>';

    appendString += '<div class="form-group ml-1 mr-1">';
    appendString += '<input type="hidden" class="hfSerialNo" name="hfSerialNo" value="' + additionalServiceSerial + '">';
    appendString += '<input type="number" style="width: 60px" class="form-control form-control-sm additionalServiceControl txtUnitPrice" step=".01" name="txtUnitPrice" placeholder="AMNT." title="Unit price" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-2 mr-1">';
    appendString += '<input type="number" style="width: 50px" class="form-control form-control-sm additionalServiceControl txtQuantity " name="txtQuantity" placeholder="QTY" title="Quantity" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-2 mr-2">';
    appendString += '<input type="number" style="width: 80px" class="form-control form-control-sm additionalServiceControl txtServiceFee " step=".01" name="txtServiceFee" placeholder="Fee" title="Applicable service charges" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-4 mr-5">';
    appendString += '<input type="checkbox" class="chkPayToDriver" id="chkPayToDriver" name="chkPayToDriver" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-1 mr-2">';
    appendString += '<input type="number" style="width: 66px" class="form-control form-control-sm additionalServiceControl txtDriverPercentage" min="0" step=".01" name="txtDriverPercentage" placeholder="Percent" title="Driver portion of the fee" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-3 mr-4">';
    appendString += '<input type="checkbox" class="chkIsGstApplicableForService" name="chkIsGstApplicableForService" />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-4 mr-4">';
    appendString += '<input type="checkbox" class="chkIsUpdate" name="chkIsUpdate" disabled />';
    appendString += '</div>';

    appendString += '<div class="form-group ml-5">';
    appendString += '<button type="button" class="btn btn-sm btn-primary additionalServiceControl btnAddAdditionalService" name="btnAddAdditionalService" title="Click to add to order"><i class="fa fa-plus-circle"></i> </button>';
    appendString += '&nbsp;' + '<button type="button" class="btn btn-sm btn-success additionalServiceControl btnEditService" title="Update & save" disabled><i class="fa fa-save"></i> </button>';
    appendString += '&nbsp;';
    appendString += '<button type="button" class="btn btn-sm btn-danger additionalServiceControl btnDeleteAdditionalService" name="btnDeleteAdditionalService" title="Delete from order"><i class="fa fa-trash"></i> </button>';
    appendString += '</div>';

    appendString += '</div>';


    return appendString;
}

function MakeReturnToggleOn() {
    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.on(true);
}
function MakeReturnToggleOff() {
    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.off(true);
}

//#endregion 





