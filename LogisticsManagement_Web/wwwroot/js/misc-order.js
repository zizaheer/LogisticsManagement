//#region document.ready

$(document).ready(function () {

    $('#txtOrderDate').val(ConvertDateToUSFormat(new Date));

    MaskPhoneNumber('#txtMobileNo');
    MaskPhoneNumber('#txtPhoneNumber');

    $(document).ajaxStart(function () {
        $("#spinnerLoadingDataTable").css("display", "inline-block");
    });
    $(document).ajaxComplete(function () {
        $("#spinnerLoadingDataTable").css("display", "none");
    });

    waitLoading();

});

//#endregion 

//#region Local Variables

function waitLoading() {
    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            $('#ddlCustomerCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
        });
    }

    setTimeout(function () {
        var routingOrderId = $('#hfRoutingOrderId').val();
        if (routingOrderId !== '') {
            var isTriggerModify = $('#hfIsTriggerModify').val();
            if (isTriggerModify === "1" || isTriggerModify === 1) {
                if (routingOrderId > 0) {
                    GetAndFillOrderDetailsByWayBillNumber(routingOrderId, 3);

                    $('#newMiscOrder').modal({
                        backdrop: 'static',
                        keyboard: false
                    });
                    $('#newMiscOrder').draggable();
                    $('#newMiscOrder').modal('show');
                }
            }
        }

    }, 500);
}

var selectedAdditionalServiceArray = [];
var selectedOrdersForPrint = [];
var isNewEntry = false;

//$('#chkCheckAllOrders').prop('checked', true);
//$('.chkWaybillNumber').prop('checked', true);
//var wbArrayString = $('#hfWaybillArray').val();
//selectedOrdersForPrint = [];
//var wbArray = wbArrayString.split(',');
//$.each(wbArray, function (i, item) {
//    if (item !== '') {
//        selectedOrdersForPrint.push(parseInt(item));
//    }
//});


//#endregion

//#region Events 


$('#btnNewMiscOrder').unbind().on('click', function () {

    $('#frmMiscOrderForm').trigger('reset');
    ClearForm();

    var addressLinesForAutoComplete = GetList('Address/GetAddressForAutoComplete');
    var maxWayBill = GetSingleById('Order/GetNextWaybillNumber', 0);

    if (addressLinesForAutoComplete !== null) {
        var addressLines = JSON.parse(addressLinesForAutoComplete);
        if (addressLines != '') {
            $.each(addressLines, function (i, item) {
                $('#dlCustomerAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
            });
        }
    }

    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            if (item.Alpha3CountryCode === 'CAN') {
                $('#ddlCustomerCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode).attr('selected', true));
            } else {
                $('#ddlCustomerCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
            }
        });
    }

    isNewEntry = true;

    //$('#txtWayBillNo').prop('disabled', false);
    $('#txtWayBillNo').val(maxWayBill);

    $('#newMiscOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newMiscOrder').draggable();
    $('#newMiscOrder').modal('show');


});
$('#btnCloseModal').on('click', function () {

    var customerCity = $('#ddlCustomerCityId').val();
    var unitQty = $('#txtUnitQuantity').val();
    var skidQty = $('#txtSkidQuantity').val();
    var wbNumber = $('#txtWayBillNo').val();

    var dataEntered = false;
    if (customerCity > 0 || unitQty.length > 0 || skidQty.length > 0) {
        dataEntered = true;
    }
    if (dataEntered === true && wbNumber.length < 1) {
        bootbox.confirm("There are some un-saved data. Are you sure you want to close the window?", function (result) {
            if (result === true) {
                $('#newMiscOrder').modal('hide');
            }
        });
    } else {
        $('#newMiscOrder').modal('hide');
    }
});

$('#txtBillToCustomerName').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#hfBillerCustomerId').val('');
        var billerCustomerId = $('#txtBillToCustomerName').val();
        if (billerCustomerId > 0) {
            var billerInfo = GetCustomerInfo(billerCustomerId);
            if (billerInfo != null && billerInfo !== '') {
                billerCustomerInfo = JSON.parse(billerInfo);
                if (billerCustomerInfo !== '') {
                    $('#hfBillerCustomerId').val(billerCustomerId);
                    $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
                }
            } else {
                bootbox.alert('Customer not found with provided number.');
            }
        } else {
            bootbox.alert('Please enter a valid customer number.');
        }
    }
});
$('#txtBillToCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();
    $('#hfBillerCustomerId').val('');

    var billerCustomerId = $('#dlBillers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');

    if (billerCustomerId > 0) {
        var billerInfo = GetCustomerInfo(billerCustomerId);
        if (billerInfo != null && billerInfo !== '') {
            billerCustomerInfo = JSON.parse(billerInfo);
            if (billerCustomerInfo !== '') {
                $('#hfBillerCustomerId').val(billerCustomerId);
                $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
            }
        } else {
            bootbox.alert('Customer not found with provided number.');
        }
    }
});

$('#txtCustomerName').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var customerId = $('#txtCustomerName').val();
        if (customerId > 0) {
            FillCustomerInfoAndAddressInfo(customerId);
        } else {
            bootbox.alert('Please enter a valid customer number.');
        }
    }
});
$('#txtCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $('#txtCustomerName').val();
    var customerId = $('#dlCustomers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');

    if (customerId > 0) {
        FillCustomerInfoAndAddressInfo(customerId);
    }
});

$('#txtEmployeeName').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var employeeId = $('#txtEmployeeName').val();
        if (employeeId > 0) {
            var empInfo = GetEmployeeById(employeeId);
            if (empInfo !== '') {
                $('#txtEmployeeName').val(empInfo.FirstName);
                $('#hfEmployeeId').val(employeeId);

                if (empInfo.EmployeeTypeId === 4 | empInfo.EmployeeTypeId === 6) {
                    $('#ddlShareTypeId').prop('disabled', false);
                    $('#txtShareAmount').prop('disabled', false);
                }
            }
        }
    }
});
$('#txtEmployeeName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();

    var employeeId = $('#dlEmployees option').filter(function () {
        return this.value === valueSelected;
    }).data('employeeid');

    $('#ddlShareTypeId').prop('disabled', true);
    $('#txtShareAmount').prop('disabled', true);
    $('#txtShareAmount').val('');

    if (employeeId > 0) {
        var empInfo = GetEmployeeById(employeeId);
        if (empInfo !== '') {
            $('#txtEmployeeName').val(empInfo.FirstName);
            $('#hfEmployeeId').val(employeeId);

            if (empInfo.EmployeeTypeId === 4 | empInfo.EmployeeTypeId === 6) {
                $('#ddlShareTypeId').prop('disabled', false);
                $('#txtShareAmount').prop('disabled', false);
            }
        }
    }

});


$('#txtCustomerAddressLine').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        var addressId = $('#txtCustomerAddressLine').val();
        if (addressId > 0) {
            FillCustomerAddress(addressId);
        } else {
            bootbox.alert('Please enter a valid address id.');
        }
    }
});
$('#txtCustomerAddressLine').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();
    var addressId = $('#dlCustomerAddressLines option').filter(function () {
        return this.value === valueSelected;
    }).data('addressid');
    if (addressId > 0) {
        FillCustomerAddress(addressId);
    } else {
        bootbox.alert('Please enter a valid address id.');
    }
});


function FillCustomerInfoAndAddressInfo(customerId) {
    var customerInfo = null;

    if (customerId > 0) {
        var customer = GetCustomerInfo(customerId);
        if (customer != null && customer != '') {
            customerInfo = JSON.parse(customer);
            if (customerInfo !== null) {
                $('#txtCustomerName').val(customerInfo.CustomerName);
                $('#hfCustomerId').val(customerId);

                var addressId = 0;
                addressId = GetCustomerDefaultShippingAddress(customerId);
                if (addressId < 1) {
                    addressId = GetCustomerDefaultBillingAddress(customerId);
                }
                if (addressId > 0) {
                    FillCustomerAddress(addressId);
                }
                else {
                    ClearCustomerAddressArea();
                }
            }
        } else {
            bootbox.alert('Customer not found with provided number.');
            return;
        }
    }
}

$('#txtDiscountPercent').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtDiscountPercent').change();
    }
});
$('#txtDiscountPercent').on('change', function (event) {
    CalculateOrderCost();
});

$(document).on('input', '#service-list .txtAdditionalServiceName', function () {
    var valueSelected = $(this).val();
    var serviceId = '0'; // defined as string, since data-serviceid returns string
    if (valueSelected !== '' && valueSelected != null) {
        serviceId = $('.additionalServices option').filter(function () {
            return this.value === valueSelected;
        }).data('serviceid');
    }
    var currentRow = $(this).closest('tr');

    if (serviceId !== '0' && serviceId !== 'undefined') {
        var addServiceInfo = GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceId);
        if (addServiceInfo != null && addServiceInfo != '' && addServiceInfo !== 'null') {
            var serviceInfo = JSON.parse(addServiceInfo);
            currentRow.find('td:eq(2) .chkIsGstApplicableForService').prop('checked', serviceInfo.IsTaxApplicable);
            currentRow.find('td:eq(3) .btnAddAdditionalService').attr('data-serviceid', serviceId);
            currentRow.find('td:eq(3) .btnDeleteAdditionalService').attr('data-serviceid', serviceId);
        }
    }

});

$('#btnAddAddtionalServiceRow').on('click', function (event) {
    event.preventDefault();
    $('#service-list').append(GenerateNewAdditionalServiceRow());
    var services = JSON.parse(GetList('AdditionalService/GetAdditionalServiceList'));
    $.each(services, function (i, item) {
        $('#service-list .additionalServices').append($('<option>').attr('data-serviceid', item.Id).val(item.ServiceName));
    });
});
$('#service-list').unbind().on('click', '.btnDeleteAdditionalService', function (event) {
    event.preventDefault();
    var serviceId = event.currentTarget.dataset.serviceid;
    var selectedRow = $(this).closest('tr');
    bootbox.confirm("Are you sure you want to remove this additional service?", function (result) {
        if (result === true) {
            var index = selectedAdditionalServiceArray.findIndex(c => c.additionalServiceId === parseInt(serviceId));
            if (index >= 0) {
                selectedAdditionalServiceArray.splice(index, 1);
            }
            CalculateOrderCost();
            selectedRow.remove();
        }
    });
});
$('#service-list').on('click', '.btnAddAdditionalService', function (event) {
    event.preventDefault();
    var serviceId = event.currentTarget.dataset.serviceid;
    var currentRow = $(this).closest('tr');

    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;
    var serviceFee = currentRow.find('td:eq(1) .txtServiceFee').val();
    var isGstApplicable = currentRow.find('td:eq(2) .chkIsGstApplicableForService').is(':checked');


    if (serviceId === undefined || serviceId === '' || serviceId === '0' || serviceId < 1) {
        bootbox.alert("Please select service before adding it to the order or your service id is invalid.");
        return;
    }

    if (serviceFee === "") {
        bootbox.alert("Please enter service charge before adding it to the order.");
        return;
    }

    var serviceData = {
        orderId: $('#hfOrderId').val(),
        additionalServiceId: parseInt(serviceId),
        additionalServiceFee: parseFloat(serviceFee),
        isTaxAppliedOnAddionalService: isGstApplicable,
        taxAmountOnAdditionalService: taxPercentage
    };

    var index = selectedAdditionalServiceArray.findIndex(c => c.additionalServiceId === serviceData.additionalServiceId);
    if (index >= 0) {
        selectedAdditionalServiceArray.splice(index, 1);
    }
    selectedAdditionalServiceArray.push(serviceData);

    CalculateOrderCost();

    currentRow.find('td:eq(3) .btnAddAdditionalService').hide();
    currentRow.find('td:eq(3) .btnServiceAdded').show();
    currentRow.find('td:eq(3) .btnDeleteAdditionalService').removeAttr('disabled');
    currentRow.find('td:eq(1) .txtServiceFee').prop('disabled', true);
    currentRow.find('td:eq(0) .txtAdditionalServiceName').prop('disabled', true);
    currentRow.find('td:eq(2) .chkIsGstApplicableForService').prop('disabled', true);

});
$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This waybill number will be deleted along with all relavant data. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('MiscellaneousOrder/Remove', waybillNumber);
            $('#loadMiscellaneousOrders').load('MiscellaneousOrder/LoadMiscellaneousOrders');
        }
    });
});


$('#frmMiscOrderForm').on('keyup keypress', function (e) {
    var keyCode = e.keyCode || e.which;
    if (keyCode === 13) {
        e.preventDefault();
        return false;
    }
});
$('#frmMiscOrderForm').unbind('submit').submit(function (event) {
    event.preventDefault();
    var dataArray = GetFormData();
    var isValid = ValidateOrderForm(dataArray[0]);
    if (isValid === false) {
        return;
    }
    if (dataArray[0].cargoCtlNumber !== '' || dataArray[0].awbCtnNumber !== '' || dataArray[0].referenceNumber !== '') {
        var countCtl = PerformPostActionWithObject('Order/GetCargoCtlNumberCount', { cargoCtl: dataArray[0].cargoCtlNumber, wayBill: dataArray[0].wayBillNumber });
        var countAwb = PerformPostActionWithObject('Order/GetAwbCtnNumberCount', { awbCtn: dataArray[0].awbCtnNumber, wayBill: dataArray[0].wayBillNumber });
        var countRef = PerformPostActionWithObject('Order/GetCustomerReferenceNumberCount', { custRef: dataArray[0].referenceNumber, wayBill: dataArray[0].wayBillNumber });

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

    var duplicateWaybill = GetSingleById('Order/FindDuplicateWayBill', dataArray[0].wayBillNumber);
    if (duplicateWaybill !== '') {
        bootbox.alert('This waybill was already used. Cannot create duplicate waybill. Try a different number or keep it blank to create auto.');
        return;
    }

    if (dataArray[0].wayBillNumber > 0 && isNewEntry === false) {
        result = PerformPostActionWithObject('MiscellaneousOrder/Update', dataArray);
        if (result !== '') {
            //bootbox.alert('Order updated successfully.');
            location.reload();
        } else {
            bootbox.alert('Failed! An error occured updating order.');
            return;
        }
    }
    else {
        if (dataArray[0].wayBillNumber > 0 && isNewEntry === true) {
            result = PerformPostActionWithObject('MiscellaneousOrder/Add', dataArray);
            if (result !== '') {
                //bootbox.alert('Order saved successfully.');
                location.reload();
            } else {
                bootbox.alert('Failed! An error occured saving order.');
                return;
            }
        }
    }
    selectedAdditionalServiceArray = null;
    $('#loadMiscellaneousOrders').load('MiscellaneousOrder/LoadMiscellaneousOrders');

    ClearForm();
    $('#newMiscOrder').modal('hide');
}

$('#misc-order-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    ClearForm();
    $('#frmMiscOrderForm').trigger('reset');

    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            $('#ddlCustomerCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
        });
    }

    var wbNumber = $(this).data('waybillnumber');
    if (wbNumber > 0) {
        GetAndFillOrderDetailsByWayBillNumber(wbNumber, 3);
    }

    //$('#txtWayBillNo').prop('disabled', true);
    isNewEntry = false;

    $('#newMiscOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newMiscOrder').draggable();
    $('#newMiscOrder').modal('show');

});

$('#misc-order-list .chkWaybillNumber').change(function (event) {
    //event.preventDefault();

    var isChecked = $(this).is(':checked');
    var orderId = $(this).data('waybillnumber');

    var index = selectedOrdersForPrint.indexOf(orderId);
    if (index >= 0) {
        selectedOrdersForPrint.splice(index, 1);
    }

    if (isChecked) {
        selectedOrdersForPrint.push(orderId);
    }
});
$('#misc-order-list').on('change', '#chkCheckAllOrders', function (event) {
    event.preventDefault();

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkWaybillNumber').prop('checked', true);
        var wbArrayString = $('#hfWaybillArray').val();
        selectedOrdersForPrint = [];
        var wbArray = wbArrayString.split(',');
        $.each(wbArray, function (i, item) {
            if (item !== '') {
                selectedOrdersForPrint.push(parseInt(item));
            }
        });
    } else {
        $('.chkWaybillNumber').prop('checked', false);
        selectedOrdersForPrint = [];
    }
});

$('#btnTrialPrintWaybill').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedOrdersForPrint.length < 1) {
        bootbox.alert('Please select order/s to print.');
        event.preventDefault();
        return;
    }

    var printUrl = 'Order/PrintWaybillAsPdf';
    var printOption = {
        numberOfcopyOnEachPage: $('#ddlCopyOnPage').val(),
        numberOfcopyPerItem: $('#ddlNumberOfCopies').val(),
        ignorePrice: $('#chkIgnorePricingInformation').is(':checked') === true ? 1 : 0,
        isMiscellaneous: 1,
        viewName: 'PrintMiscellaneousWaybill',
        printUrl: printUrl
    };

    var dataArray = [selectedOrdersForPrint, printOption];

    $.ajax({
        'async': false,
        url: printOption.printUrl,
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

$('#ddlCustomerCountries').on('change', function () {
    var countryId = $('#ddlCustomerCountries').val();
    var cities = GetListById('City/GetCitiesByCountry', countryId);
    $('#ddlCustomerCityId').empty();
    $('#ddlCustomerCityId').append($('<option></option>').val('0').html('City'));
    if (cities !== '') {
        var parsedCities = JSON.parse(cities);
        $.each(parsedCities, function (i, item) {
            $('#ddlCustomerCityId').append($('<option></option>').val(item.Id).html(item.CityName));
        });
    }
});
//#endregion


//#region Private methods

function GetAndFillOrderDetailsByWayBillNumber(wayBillNumber, orderTypeId) {
    var orderData = null;
    var orderAdditionalServiceData = null;
    var parseData = null;

    var orderInfo = GetSingleById('MiscellaneousOrder/GetOrderDetailsByWayBillId', wayBillNumber);
    if (orderInfo != null) {
        parseData = JSON.parse(orderInfo);
        orderData = parseData.orderPocos.filter(function (item) {
            return item.OrderTypeId === orderTypeId;
        })[0];
    }

    if (orderData !== null && orderData !== undefined && orderData !== '') {
        $('#hfOrderId').val(orderData.Id);
        $('#hfBillerCustomerId').val(orderData.BillToCustomerId);

        orderAdditionalServiceData = parseData.orderAdditionalServices.filter(function (item) {
            return item.OrderId === orderData.Id;
        });

        FillOrderAdditionalServices(orderAdditionalServiceData);
        FillOrderDetails(orderData);

    }
}

function GetCustomerInfo(customerId) {
    var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);
    return customerInfo;
}
function GetAddressInfo(addressId) {
    var addressInfo = GetSingleById('Address/GetAddressById', addressId);
    return addressInfo;
}
function GetEmployeeById(employeeId) {
    if (employeeId != null && employeeId !== '') {
        var empInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
        if (empInfo != null && empInfo !== '') {
            var employee = JSON.parse(empInfo);
        }
        return employee;
    }
}
function GetCustomerDefaultShippingAddress(customerId) {
    var customerDefaultShippingAddressId = GetSingleById('Customer/GetCustomerDefaultShippingAddressById', customerId);
    return customerDefaultShippingAddressId;
}
function GetCustomerDefaultBillingAddress(customerId) {
    var customerDefaultBillingAddressId = GetSingleById('Customer/GetCustomerDefaultBillingAddressById', customerId);
    return customerDefaultBillingAddressId;
}
function FillCustomerAddress(addressId) {
    ClearCustomerAddressArea();
    var customerAddress = GetAddressInfo(addressId);
    if (customerAddress !== null && customerAddress !== '') {
        var customerAddressParsed = JSON.parse(GetAddressInfo(addressId));
        $('#hfCustomerAddressId').val(customerAddressParsed.Id);
        $('#txtCustomerAddressLine').val(customerAddressParsed.AddressLine);
        $('#txtCustomerUnitNo').val(customerAddressParsed.UnitNumber);
        $('#ddlCustomerCityId').val(customerAddressParsed.CityId);
        $('#ddlCustomerProvinceId').val(customerAddressParsed.ProvinceId);
        $('#txtCustomerPostcode').val(customerAddressParsed.PostCode);
    }
}

function ValidateOrderForm(formData) {

    if (formData.billToCustomerId <= 0) {
        bootbox.alert('Please select biller');
        return false;
    }

    if (formData.customerId <= 0) {
        bootbox.alert('Please enter or select service location customer');
        return false;
    }

    if (formData.customerAddressline === '') {
        bootbox.alert('Please enter a valid address for service location or select from address list');
        return false;
    }

    if (formData.totalAdditionalServiceCost <= 0) {
        bootbox.alert('Order price must be greater than 0.');
        return false;
    }

    return true;
}

function CalculateOrderCost() {

    var orderTotal = 0;
    var discountPercentage = $('#txtDiscountPercent').val() !== "" ? parseFloat($('#txtDiscountPercent').val()) : 0.0;

    var totalServiceCost = 0.0;
    var currentGstTotal = 0.0;
    if (selectedAdditionalServiceArray.length > 0) {
        for (var i = 0; i < selectedAdditionalServiceArray.length; i++) {
            if (selectedAdditionalServiceArray[i].additionalServiceFee > 0) {
                totalServiceCost = totalServiceCost + selectedAdditionalServiceArray[i].additionalServiceFee;
                if (selectedAdditionalServiceArray[i].isTaxAppliedOnAddionalService && selectedAdditionalServiceArray[i].taxAmountOnAdditionalService > 0) {
                    var discountedServiceFee = selectedAdditionalServiceArray[i].additionalServiceFee - (discountPercentage * selectedAdditionalServiceArray[i].additionalServiceFee / 100);
                    var addServiceTax = selectedAdditionalServiceArray[i].taxAmountOnAdditionalService * discountedServiceFee / 100;
                    currentGstTotal += addServiceTax;
                }
            }
        }
    }

    if (totalServiceCost > 0) {
        orderTotal = totalServiceCost;
        $('#lblGrandBasicCost').text(totalServiceCost.toFixed(2));
    }
    else {
        $('#lblGrandBasicCost').text('0.00');
    }

    if (discountPercentage > 0 && totalServiceCost > 0) {
        var baseDiscountAmount = discountPercentage * totalServiceCost / 100;
        $('#lblGrandDiscountAmount').text(baseDiscountAmount.toFixed(2));
        orderTotal = totalServiceCost - baseDiscountAmount;
    } else {
        $('#lblGrandDiscountAmount').text('0.00');
    }

    if (currentGstTotal > 0) {
        orderTotal = orderTotal + currentGstTotal;
        $('#lblGrandGstAmount').text(currentGstTotal.toFixed(2));
    } else {
        $('#lblGrandGstAmount').text('0.00');
    }

    if (orderTotal > 0) {
        $('#lblGrandTotalAmount').text(orderTotal.toFixed(2));
    } else {
        $('#lblGrandTotalAmount').text('0.00');
    }

}

function FillOrderDetails(orderRelatedData) {
    if (orderRelatedData !== null) {

        $('#hfOrderId').val(orderRelatedData.Id);
        $('#hfEmployeeId').val(orderRelatedData.ServiceProviderEmployeeId);

        $('#txtOrderTotal').val(orderRelatedData.OrderBasicCost);
        $('#lblGrandBasicCost').text('0.00');
        $('#lblGrandDiscountAmount').text('0.00');
        $('#lblGrandGstAmount').text('0.00');
        $('#lblGrandTotalAmount').text('0.00');

        $('#txtWayBillNo').val(orderRelatedData.WayBillNumber);
        $('#txtCustomerRefNo').val(orderRelatedData.ReferenceNumber);
        $('#txtCargoCtlNo').val(orderRelatedData.CargoCtlNumber);
        $('#txtAwbCtnNo').val(orderRelatedData.AwbCtnNumber);
        $('#txtOrderDate').val(ConvertDateToUSFormat(orderRelatedData.CreateDate));
        $('#txtOrderedBy').val(orderRelatedData.OrderedBy);
        $('#txtPhoneNo').val(orderRelatedData.ContactPhoneNumber);
        $('#txtDepartment').val(orderRelatedData.DepartmentName);

        $('#hfCustomerId').val(orderRelatedData.ShipperCustomerId);
        if (orderRelatedData.ShipperCustomerId != '' && orderRelatedData.ShipperCustomerId != null) {
            var customerInfo = GetCustomerInfo(orderRelatedData.ShipperCustomerId);
            if (customerInfo != '' && customerInfo != null) {
                var customerInfoParsed = JSON.parse(customerInfo);
                $('#txtCustomerName').val(customerInfoParsed.CustomerName);
            }
        }
        if (orderRelatedData.ShipperAddressId != '' && orderRelatedData.ShipperAddressId != null) {
            FillCustomerAddress(orderRelatedData.ShipperAddressId);
            $('#hfCustomerAddressId').val(orderRelatedData.ShipperAddressId);
        }

        if (orderRelatedData.BillToCustomerId != '' && orderRelatedData.BillToCustomerId != null) {
            var billerInfo = GetCustomerInfo(orderRelatedData.BillToCustomerId);
            if (billerInfo != '' && billerInfo != null) {
                var billerCustomerInfo = JSON.parse(billerInfo);
                $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
                $('#hfBillerCustomerId').val(orderRelatedData.BillToCustomerId);
            }
        }
        var employee = GetEmployeeById(orderRelatedData.ServiceProviderEmployeeId);
        if (employee != null) {
            $('#txtEmployeeName').val(employee.FirstName);
        }

        if (orderRelatedData.OrderShareAmount > 0) {
            if (orderRelatedData.IsSharingOnPercent === true) {
                $('#ddlShareTypeId').val('1');
            } else {
                $('#ddlShareTypeId').val('0');
            }

            $('#txtShareAmount').val(orderRelatedData.OrderShareAmount);
        }

        $('#txtDeliveredBy').val(orderRelatedData.DeliveredBy);
        $('#txtBolRefNumber').val(orderRelatedData.BolReferenceNumber);
        $('#txtProRefNumber').val(orderRelatedData.ProReferenceNumber);
        $('#txtShipperName').val(orderRelatedData.ShipperName);
        $('#txtShipperAddress').val(orderRelatedData.ShipperAddress);

        $('#ddlUnitTypeId').val(orderRelatedData.UnitTypeId);
        $('#txtUnitQuantity').val(orderRelatedData.UnitQuantity);
        $('#txtSkidQuantity').val(orderRelatedData.SkidQuantity);
        $('#txtTotalPieces').val(orderRelatedData.TotalPiece);
        $('#ddlWeightScaleId').val(orderRelatedData.WeightScaleId);
        $('#txtWeightTotal').val(orderRelatedData.WeightTotal);

        $('#txtDiscountPercent').val(orderRelatedData.DiscountPercentOnOrderCost);


        $('#chkIsPrintOnWayBill').prop('checked', orderRelatedData.IsPrintedOnWayBill);
        $('#txtCommentsForWayBill').val(orderRelatedData.CommentsForWayBill);
        $('#chkIsPrintOnInvoice').prop('checked', orderRelatedData.IsPrintedOnInvoice);
        $('#txtCommentsForInvoice').val(orderRelatedData.CommentsForInvoice);

        if (orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandBasicCost').text(orderRelatedData.OrderBasicCost.toFixed(2));
        }
        else {
            $('#lblGrandBasicCost').text('0.00');
        }


        if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * orderRelatedData.OrderBasicCost / 100).toFixed(2));
        } else {
            $('#lblGrandDiscountAmount').text('0.00');
        }

        var currentGstTotal = 0.0;
        var discountPercent = 0.0;
        if (orderRelatedData.DiscountPercentOnOrderCost != null && orderRelatedData.DiscountPercentOnOrderCost > 0) {
            discountPercent = orderRelatedData.DiscountPercentOnOrderCost;
        }

        if (selectedAdditionalServiceArray.length > 0) {
            for (var i = 0; i < selectedAdditionalServiceArray.length; i++) {
                if (selectedAdditionalServiceArray[i].additionalServiceFee > 0) {
                    if (selectedAdditionalServiceArray[i].isTaxAppliedOnAddionalService && selectedAdditionalServiceArray[i].taxAmountOnAdditionalService > 0) {
                        var discountedServiceFee = selectedAdditionalServiceArray[i].additionalServiceFee - (discountPercent * selectedAdditionalServiceArray[i].additionalServiceFee / 100);
                        var addServiceTax = selectedAdditionalServiceArray[i].taxAmountOnAdditionalService * discountedServiceFee / 100;
                        currentGstTotal += addServiceTax;
                    }
                }
            }
        }

        if (currentGstTotal > 0) {
            $('#lblGrandGstAmount').text(currentGstTotal.toFixed(2));
        } else {
            $('#lblGrandGstAmount').text('0.00');
        }


        if (orderRelatedData.TotalAdditionalServiceCost > 0) {
            $('#lblGrandTotalAmount').text(orderRelatedData.TotalAdditionalServiceCost.toFixed(2));
        } else {
            $('#lblGrandTotalAmount').text('0.00');
        }

    }
}

function FillOrderAdditionalServices(orderAdditionalServiceData) {

    selectedAdditionalServiceArray = [];
    $('#service-list').empty();

    if (orderAdditionalServiceData !== null) {
        for (var i = 0; i < orderAdditionalServiceData.length; i++) {
            var serviceData = {
                orderId: orderAdditionalServiceData[i].OrderId,
                additionalServiceId: orderAdditionalServiceData[i].AdditionalServiceId,
                additionalServiceFee: parseFloat(orderAdditionalServiceData[i].AdditionalServiceFee),
                isTaxAppliedOnAddionalService: orderAdditionalServiceData[i].IsTaxAppliedOnAddionalService,
                taxAmountOnAdditionalService: orderAdditionalServiceData[i].TaxAmountOnAdditionalService
            };

            selectedAdditionalServiceArray.push(serviceData);

            var serviceInfo = GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceData.additionalServiceId);
            var additionalServiceInfo = '';
            if (serviceInfo != null) {
                additionalServiceInfo = JSON.parse(serviceInfo);
            }

            if (additionalServiceInfo !== null && additionalServiceInfo !== undefined && additionalServiceInfo !== '') {
                $('#btnAddAddtionalServiceRow').trigger('click');
                var currentRow = $('#service-list tr:eq(' + i + ')');
                currentRow.find('td:eq(0) .txtAdditionalServiceName').val(additionalServiceInfo.ServiceName);
                currentRow.find('td:eq(0) .txtAdditionalServiceName').prop('disabled', true);
                currentRow.find('td:eq(1) .txtServiceFee').val(serviceData.additionalServiceFee);
                currentRow.find('td:eq(1) .txtServiceFee').prop('disabled', true);
                currentRow.find('td:eq(2) .chkIsGstApplicableForService').prop('checked', serviceData.isTaxAppliedOnAddionalService);
                currentRow.find('td:eq(2) .chkIsGstApplicableForService').prop('disabled', true);
                currentRow.find('td:eq(3) .btnAddAdditionalService').hide();
                currentRow.find('td:eq(3) .btnServiceAdded').show();
                currentRow.find('td:eq(3) .btnDeleteAdditionalService').attr('data-serviceid', serviceData.additionalServiceId);
            }
        }
    }
}

function GetFormData() {

    var orderData = {

        id: $('#hfOrderId').val() === "" ? "0" : $('#hfOrderId').val(),
        wayBillNumber: $('#txtWayBillNo').val() === "" ? 0 : parseInt($('#txtWayBillNo').val()),
        billToCustomerId: $('#hfBillerCustomerId').val() === "" ? 0 : parseInt($('#hfBillerCustomerId').val()),
        referenceNumber: $('#txtCustomerRefNo').val() === "" ? null : $('#txtCustomerRefNo').val(),
        cargoCtlNumber: $('#txtCargoCtlNo').val() === "" ? null : $('#txtCargoCtlNo').val(),
        awbCtnNumber: $('#txtAwbCtnNo').val() === "" ? null : $('#txtAwbCtnNo').val(),
        orderDate: $('#txtOrderDate').val() === "" ? null : $('#txtOrderDate').val(),
        orderedBy: $('#txtOrderedBy').val() === "" ? null : $('#txtOrderedBy').val(),
        phoneNumber: $('#txtPhoneNo').val() === "" ? null : $('#txtPhoneNo').val(),
        departmentName: $('#txtDepartment').val() === "" ? null : $('#txtDepartment').val(),

        customerId: $('#hfCustomerId').val() === "" ? 0 : parseInt($('#hfCustomerId').val()),
        customerAddressId: $('#hfCustomerAddressId').val() === "" ? 0 : parseInt($('#hfCustomerAddressId').val()),
        customerAddressline: $('#txtCustomerAddressLine').val(),
        customerUnitNo: $('#txtCustomerUnitNo').val(),
        customerCityId: $('#ddlCustomerCityId').val() === "" ? 0 : parseInt($('#ddlCustomerCityId').val()),
        customerProvinceId: $('#ddlCustomerProvinceId').val() === "" ? 0 : parseInt($('#ddlCustomerProvinceId').val()),
        customerPostcode: $('#txtCustomerPostcode').val(),
        customerCountry: $('#ddlCustomerCountries').val(),
        cityId: $('#ddlCustomerCityId').val() === "" ? 0 : parseInt($('#ddlCustomerCityId').val()),
        serviceProviderEmployeeId: $('#hfEmployeeId').val() === "" ? null : parseInt($('#hfEmployeeId').val()),

        deliveredBy: $('#txtDeliveredBy').val() === "" ? null : $('#txtDeliveredBy').val(),
        bolReferenceNumber: $('#txtBolRefNumber').val() === "" ? null : $('#txtBolRefNumber').val(),
        proReferenceNumber: $('#txtProRefNumber').val() === "" ? null : $('#txtProRefNumber').val(),
        shipperName: $('#txtShipperName').val() === "" ? null : $('#txtShipperName').val(),
        shipperAddress: $('#txtShipperAddress').val() === "" ? null : $('#txtShipperAddress').val(),

        unitTypeId: $('#ddlUnitTypeId').val() === "" ? 0 : parseInt($('#ddlUnitTypeId').val()),
        weightScaleId: $('#ddlWeightScaleId').val() === "" ? null : parseInt($('#ddlWeightScaleId').val()),
        weightTotal: $('#txtWeightTotal').val() === "" ? null : parseFloat($('#txtWeightTotal').val()),
        unitQuantity: $('#txtUnitQuantity').val() === "" ? null : parseInt($('#txtUnitQuantity').val()),
        skidQuantity: $('#txtSkidQuantity').val() === "" ? null : parseInt($('#txtSkidQuantity').val()),
        totalPieces: $('#txtTotalPieces').val() === "" ? null : $('#txtTotalPieces').val(),

        discountPercentOnOrderCost: $('#txtDiscountPercent').val() === "" ? null : parseFloat($('#txtDiscountPercent').val()),
        applicableGstPercent: 0.0,

        orderBasicCost: $('#lblGrandBasicCost').text() === "" ? null : parseFloat($('#lblGrandBasicCost').text()),
        totalOrderCost: 0,
        totalAdditionalServiceCost: $('#lblGrandTotalAmount').text() === "" ? 0 : parseFloat($('#lblGrandTotalAmount').text()),
        orderShareAmount: $('#txtShareAmount').val() === "" ? 0 : parseFloat($('#txtShareAmount').val()),
        isSharingOnPercent: parseInt($('#ddlShareTypeId').val()),

        commentsForWayBill: $('#txtCommentsForWayBill').val() === "" ? null : $('#txtCommentsForWayBill').val(),
        isPrintedOnWayBill: $('#chkIsPrintOnWayBill').is(':checked') === true ? 1 : 0,
        commentsForInvoice: $('#txtCommentsForInvoice').val() === "" ? null : $('#txtCommentsForInvoice').val(),
        isPrintedOnInvoice: $('#chkIsPrintOnInvoice').is(':checked') === true ? 1 : 0
    };

    return [orderData, selectedAdditionalServiceArray];
}

function ClearForm() {

    $('#hfOrderId').val('');
    $('#hfCustomerAddressId').val('');
    $('#hfEmployeeId').val('');
    $('#hfBillerCustomerId').val('');
    $('#hfCustomerId').val();

    $('#txtWayBillNo').val('');
    $('#txtCustomerRefNo').val('');
    $('#txtCargoCtlNo').val('');
    $('#txtAwbCtnNo').val('');
    $('#txtOrderDate').val(ConvertDateToUSFormat(new Date));
    $('#txtOrderedBy').val('');
    $('#txtPhoneNo').val('');
    $('#txtDepartment').val('');

    $('#txtCustomerAddressLine').val('');
    $('#txtCustomerUnitNo').val('');
    $('#ddlCustomerCityId').val('0');
    $('#ddlCustomerProvinceId').val('0');
    $('#txtCustomerPostcode').val('');

    $('#txtEmployeeName').val('');
    $('#txtDeliveredBy').val('');
    $('#txtBolRefNumber').val('');
    $('#txtProRefNumber').val('');
    $('#txtShipperName').val('');
    $('#txtShipperAddress').val('');

    $('#ddlUnitTypeId').val('1');
    $('#ddlWeightScaleId').val('1');
    $('#txtWeightTotal').val('');
    $('#txtUnitQuantity').val('');
    $('#txtSkidQuantity').val('');
    $('#txtTotalPieces').val('');

    $('#ddlShareTypeId').prop('disabled', true);
    $('#txtShareAmount').prop('disabled', true);
    $('#txtShareAmount').val('');

    $('#txtDiscountPercent').val('');

    $('#txtCommentsForWayBill').val('');
    $('#chkIsPrintOnWayBill').prop('checked', false);
    $('#txtCommentsForInvoice').val('');
    $('#chkIsPrintOnInvoice').prop('checked', false);

    $('#lblGrandBasicCost').text('0.00');
    $('#lblGrandDiscountAmount').text('0.00');
    $('#lblGrandGstAmount').text('0.00');
    $('#lblGrandTotalAmount').text('0.00');

    $('#service-list').empty();
}

function ClearCustomerAddressArea() {
    $('#hfCustomerAddressId').val('');
    $('#txtCustomerAddressLine').val('');
    $('#txtCustomerUnitNo').val('');
    $('#ddlCustomerCityId').val('335');
    $('#ddlCustomerProvinceId').val('7');
    $('#txtCustomerPostcode').val('');
}

function GenerateNewAdditionalServiceRow() {
    var appendString = '';

    appendString += '<tbody>';
    appendString += '<tr style="height:32px; background-color:#dcf0ff">';
    appendString += '<td style="width:300px">';
    appendString += '<input class="form-control form-control-sm additionalServiceControl txtAdditionalServiceName" id="txtAdditionalServiceName" style="width:200px; margin-left:3px" placeholder="Service name" list="additionalServices" type="search" />';
    appendString += '<datalist id="additionalServices" class="additionalServices">';
    appendString += '</datalist>';
    appendString += '</td>';
    appendString += '<td style="width:190px; padding-right:5px">';
    appendString += '<input type="number" class="form-control form-control-sm additionalServiceControl txtServiceFee " min="0" id="txtServiceFee" step=".01" name="txtServiceFee" placeholder="Fee" title="Applicable service fee amount" />';
    appendString += '</td>';
    appendString += '<td style="width:160px; text-align:center">';
    appendString += '<input type="checkbox" class="chkIsGstApplicableForService" id="chkIsGstApplicableForService" name="chkIsGstApplicableForService" />';
    appendString += '</td>';
    appendString += '<td style="width:100px;text-align:center;">';
    appendString += '<button class="btn btn-sm btn-primary additionalServiceControl btnAddAdditionalService" id="btnAddAdditionalService" name="btnAddAdditionalService" title="Click to add to order"><i class="fa fa-plus-circle"></i> </button>';
    appendString += '<button class="btn btn-sm btn-success additionalServiceControl btnServiceAdded" style="display:none" title="Service added to order" disabled><i class="fa fa-check-circle"></i> </button>';
    appendString += '&nbsp;';
    appendString += '<button class="btn btn-sm btn-danger additionalServiceControl btnDeleteAdditionalService" id="btnDeleteAdditionalService" name="btnDeleteAdditionalService" title="Delete from order"><i class="fa fa-trash"></i> </button>';
    appendString += '</td>';
    appendString += '</tr>';
    appendString += '</tbody>';

    return appendString;
}

//#endregion 





