//#region document.ready

$(document).ready(function () {


    $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(new Date));
    $('#txtDispatchDatetimeForNewOrders').val(ConvertDatetimeToUSDatetime(new Date));

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
var employeeId = 0;

// global tax amount
//var taxPercentage = 0.0;
//taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

var isCustomerTaxApplicable = false;

var selectedAdditionalServiceArray = [];
var selectedOrdersForDispatch = [];

$('#chkCheckAllOrders').prop('checked', true);
$('.chkDispatchToEmployee').prop('checked', true);
var wbArrayString = $('#hfWaybillArray').val();
selectedOrdersForDispatch = [];
var wbArray = wbArrayString.split(',');
$.each(wbArray, function (i, item) {
    if (item !== '') {
        selectedOrdersForDispatch.push(parseInt(item));
    }
});


//#endregion

//#region Events 

$('#btnNewOrder').unbind().on('click', function () {
    ClearForm();
    $('#frmOrderForm').trigger('reset');

    $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(new Date));

    var addressLinesForAutoComplete = GetList('Address/GetAddressForAutoComplete');

    if (addressLinesForAutoComplete !== null) {
        var addressLines = JSON.parse(addressLinesForAutoComplete);

        $.each(addressLines, function (i, item) {
            $('#dlShipperAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
            $('#dlConsigneeAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
        });
    }

    $('#newOrder').modal({
        backdrop: 'static',
        keyboard: false
    });

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
        var shipperCustomerId = $('#lblShipperAccountNo').text();
        if (shipperCustomerId !== '') {
            billerInfo = GetCustomerInfo(shipperCustomerId);
            if (billerInfo != null && billerInfo != '') {
                FillBillerInformation(billerInfo);
            }
        }
    }
    else if (paidByValue === '2') {
        var consigneeCustomerId = $('#lblConsigneeAccountNo').text();
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

$('#txtBillToCustomerName').keypress(function (event) {
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

$('#txtShipperCustomerName').keypress(function (event) {
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
            } else {
                $('#lblShipperAccountNo').text('');
                bootbox.alert('Customer information was not found for Id: ' + shipperCustomerId);
            }

        }

        CalculateOrderBaseCost();
    }
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
            $('#lblShipperAccountNo').text('');
        }

    }
});

$('#txtConsigneeCustomerName').keypress(function (event) {
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
                $('#lblConsigneeAccountNo').text('');
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
            $('#lblConsigneeAccountNo').text('');
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
        $('#lblShipperAccountNo').text(shipperCustomerInfo.Id);

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
        $('#lblConsigneeAccountNo').text(consigneeCustomerInfo.Id);
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
    }
});
$('#txtEmployeeName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();

    employeeId = $('#dlEmployees option').filter(function () {
        return this.value === valueSelected;
    }).data('employeeid');
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


$(document).on('input', '#service-list .txtAdditionalServiceName', function () {
    var valueSelected = $(this).val();
    var serviceId = '0';
    if (valueSelected !== '') {
        serviceId = $('.additionalServices option').filter(function () {
            return this.value === valueSelected;
        }).data('serviceid');
    }
    var currentRow = $(this).closest('tr');

    var serviceInfo = JSON.parse(GetSingleById('AdditionalService/GetAdditionalServiceInfoById', serviceId));
    currentRow.find('td:eq(1) .chkPayToDriver').prop('checked', serviceInfo.PayToDriver);

    currentRow.find('td:eq(4) .chkIsGstApplicableForService').prop('checked', serviceInfo.IsTaxApplicable);
    currentRow.find('td:eq(5) .btnAddAdditionalService').attr('data-serviceid', serviceId);
    currentRow.find('td:eq(5) .btnDeleteAdditionalService').attr('data-serviceid', serviceId);
});

$('#btnAddAddtionalServiceRow').on('click', function (event) {
    event.preventDefault();

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

    var selectedRow = $(this).closest('tr');
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

$('#service-list').on('click', '.btnAddAdditionalService', function (event) {
    event.preventDefault();
    var serviceId = event.currentTarget.dataset.serviceid;
    var currentRow = $(this).closest('tr');

    var payToDriver = currentRow.find('td:eq(1) .chkPayToDriver').is(':checked');
    var serviceFee = currentRow.find('td:eq(2) .txtServiceFee').val();
    var driverPercentage = currentRow.find('td:eq(3) .txtDriverPercentage').val();
    var isGstApplicable = currentRow.find('td:eq(4) .chkIsGstApplicableForService').is(':checked');
    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

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

    if (serviceId === undefined || serviceId === '' || serviceId === '0' || serviceId < 1) {
        bootbox.alert("Please select service before adding it to the order.");
        return;
    }

    if (serviceFee === "") {
        bootbox.alert("Please enter service charge before adding it to the order.");
        return;
    }

    if (payToDriver === true) {
        if (driverPercentage === "") {
            bootbox.alert("Please enter driver percentage before adding it to the order.");
            return;
        }
    }

    var serviceData = {
        orderId: $('#hfOrderId').val(),
        additionalServiceId: parseInt(serviceId),
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

    currentRow.find('td:eq(5) .btnAddAdditionalService').hide();
    currentRow.find('td:eq(5) .btnServiceAdded').show();
    currentRow.find('td:eq(5) .btnDeleteAdditionalService').removeAttr('disabled');

});

$('.btnDelete').unbind().on('click', function () {
    var waybillNumber = $(this).data('waybillnumber');
    bootbox.confirm("This waybill number will be deleted along with all relavant data. Are you sure to proceed?", function (result) {
        if (result === true) {
            PerformPostActionWithId('Order/Remove', waybillNumber);
            $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
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
    var parseData;

    if (dataArray[0].wayBillNumber > 0) {
        result = PerformPostActionWithObject('Order/Update', dataArray);
        if (result.length > 0) {
            bootbox.alert('The order has been updated successfully');
        }
    }
    else {
        result = PerformPostActionWithObject('Order/Add', dataArray);
        if (result !== null) {
            parseData = JSON.parse(result);
            $('#txtWayBillNo').val(parseData.waybillNumber);
            $('#hfOrderId').val(parseData.orderId);

            bootbox.alert('The order has been created successfully');
        }
    }
    //selectedAdditionalServiceArray = null;
    selectedAdditionalServiceArray = [];
    $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
    $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');

    ClearForm();
    $('#frmOrderForm').trigger('reset');

    $('#newOrder').modal('hide');
}

function ValidateOrderForm(formData) {
    if (formData.unitQuantity < 1 && formData.skidQuantity < 1 && formData.totalPieces < 1) {
        bootbox.alert('Quantity required either for Unit or Skid!');
        return false;
    }

    if (formData.shipperCustomerId < 1) {
        bootbox.alert('Please enter shipper information!');
        return false;
    }

    if (formData.consigneeCustomerId < 1) {
        bootbox.alert('Please enter consignee information!');
        return false;
    }

    if (formData.shipperCustomerId === formData.consigneeCustomerId && formData.shipperAddressId === formData.consigneeAddressId) {
        bootbox.alert('Shipper and consignee address must be different!');
        return false;
    }

    if (formData.shipperAddressline.length < 5) {
        bootbox.alert('Please enter a valid address line for shipper!');
        return false;
    }

    if (formData.consigneeAddressline.length < 5) {
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

    var calculateByUnit = $('#chkIsCalculateByUnit').is(':checked');
    if (calculateByUnit === false) {
        if (formData.skidQuantity < 1) {
            bootbox.alert('You selected the tariff to be calculated by skids; please enter Skid quantity in "Skids" field');
            return false;
        }
    }

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
    $('#dispatchOrder').modal('show');

    $('#txtEmployeeName').val('');
});

$('#btnDispatchToEmployee').unbind().on('click', function (event) {
    event.preventDefault();

    var selectedEmployeeId = employeeId;
    var dispatchDate = $('#txtDispatchDatetimeForNewOrders').val();
    var vehicleId = 0; //Get vehicle Id feature

    if (selectedEmployeeId < 1) {
        bootbox.alert('Please select an employee to dispatch the order/s');
        return;
    }

    if (dispatchDate === null || dispatchDate === "") {
        bootbox.alert('Please enter dispatch date');
        return;
    }

    var dataArray = [selectedOrdersForDispatch, selectedEmployeeId, dispatchDate, vehicleId];

    var result = PerformPostActionWithObject('Order/UpdateDispatchStatus', dataArray);
    if (result.length > 0) {
        $('#loadOrdersToBeDispatched').load('Order/LoadOrdersForDispatch');
        $('#loadDispatchedOrders').load('Order/LoadDispatchedOrdersForDispatchBoard');
        selectedOrdersForDispatch = [];

        $('#dispatchOrder').modal('hide');
    }

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
    if (wbNumber > 0) {
        GetAndFillOrderDetailsByWayBillNumber(wbNumber, 1);
    }

    $('#newOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newOrder').modal('show');

});


$('#btnExistingInvoicedOrder').unbind().on('click', function () {
    event.preventDefault();

    var orderId = $('#txtInvoicedOrderNumber').val();
    if (orderId !== '') {
        ClearForm();
        $('#frmOrderForm').trigger('reset');

        GetAndFillOrderDetailsByWayBillNumber(orderId, 1);

        $('#newOrder').modal({
            backdrop: 'static',
            keyboard: false
        });
        $('#newOrder').modal('show');
    }

});


$('#order-list').on('change', '#chkCheckAllOrders', function (event) {
    event.preventDefault();

    var isChecked = $(this).is(':checked');
    if (isChecked === true) {
        $('.chkDispatchToEmployee').prop('checked', true);
        var wbArrayString = $('#hfWaybillArray').val();
        selectedOrdersForDispatch = [];
        var wbArray = wbArrayString.split(',');
        $.each(wbArray, function (i, item) {
            if (item !== '') {
                selectedOrdersForDispatch.push(parseInt(item));
            }
        });
    } else {
        $('.chkDispatchToEmployee').prop('checked', false);
        selectedOrdersForDispatch = [];
    }
});
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

            var shippAccNo = $('#lblShipperAccountNo').text();
            var consigAccNo = $('#lblConsigneeAccountNo').text();
            $('#lblShipperAccountNo').text(consigAccNo);
            $('#lblConsigneeAccountNo').text(shippAccNo);
            var billerId = $('#hfBillerCustomerId').val();

            var shippName = $('#txtShipperCustomerName').val();
            var consigName = $('#txtConsigneeCustomerName').val();
            $('#txtShipperCustomerName').val(consigName);
            $('#txtConsigneeCustomerName').val(shippName);

            var shipperAddressId = $('#hfShipperAddressId').val();
            var consigneeAddressId = $('#hfConsigneeAddressId').val();
            FillShipperAddress(consigneeAddressId);
            FillConsigneeAddress(shipperAddressId);

            paidByValue = $('input[name=rdoPaidBy]:checked').val();
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
    var customerDefaultShippingAddressId = GetSingleById('Customer/GetCustomerDefaultShippingAddressById', customerId);
    return customerDefaultShippingAddressId;
}

function GetCustomerDefaultBillingAddress(customerId) {
    var customerDefaultBillingAddressId = GetSingleById('Customer/GetCustomerDefaultBillingAddressById', customerId);
    return customerDefaultBillingAddressId;
}

function FillShipperAddress(addressId) {
    var addressInfo = GetAddressInfo(addressId);
    if (addressInfo !== '') {
        var parsedAddress = JSON.parse(addressInfo);
        if (parsedAddress != null) {
            $('#hfShipperAddressId').val(parsedAddress.Id);
            $('#txtShipperAddressLine').val(parsedAddress.AddressLine);
            $('#txtShipperUnitNo').val(parsedAddress.UnitNumber);
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
    var vehicleTypeId = $('#ddlVehicleTypeId').val();
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
            if (selectedAdditionalServiceArray[i].additionalServiceFee > 0) {
                totalAdditionalServiceCost = totalAdditionalServiceCost + selectedAdditionalServiceArray[i].additionalServiceFee;
                if (selectedAdditionalServiceArray[i].isTaxAppliedOnAddionalService && selectedAdditionalServiceArray[i].taxAmountOnAdditionalService > 0) {
                    var addServiceTax = selectedAdditionalServiceArray[i].taxAmountOnAdditionalService * selectedAdditionalServiceArray[i].additionalServiceFee / 100;
                    totalAdditionalServiceCost = totalAdditionalServiceCost + addServiceTax;
                }

            }
        }
    }
    if (totalAdditionalServiceCost > 0) {
        $('#lblGrandAddServiceAmount').text(totalAdditionalServiceCost.toFixed(2));
    } else {
        $('#lblGrandAddServiceAmount').text('0.00');
    }

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

    if (totalAdditionalServiceCost > 0) {
        grandTotal = grandTotal + totalAdditionalServiceCost;
    }

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

        $('#lblShipperAccountNo').text(orderRelatedData.ShipperCustomerId);
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

        $('#lblConsigneeAccountNo').text(orderRelatedData.ConsigneeCustomerId);
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
            }
            else if (orderRelatedData.ConsigneeCustomerId === orderRelatedData.BillToCustomerId) {
                $('#rdoConsignee').prop('checked', true);
            }
            else {
                $('#rdoThirdParty').prop('checked', true);
            }
        }


        $('#txtSchedulePickupDate').val(ConvertDatetimeToUSDatetime(orderRelatedData.ScheduledPickupDate));

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
        $('#chkIsPrintOnInvoice').val('checked', orderRelatedData.IsPrintedOnInvoice);
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
        if (orderRelatedData.ApplicableGstPercent > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#txtBaseOrderGST').val((orderRelatedData.ApplicableGstPercent * orderRelatedData.OrderBasicCost / 100).toFixed(2));
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
        if (orderRelatedData.ApplicableGstPercent > 0 && orderRelatedData.BasicCostOverriden > 0) {
            $('#txtOverriddenOrderGST').val((orderRelatedData.ApplicableGstPercent * orderRelatedData.BasicCostOverriden / 100).toFixed(2));
        } else {
            $('#txtOverriddenOrderGST').val('');
        }



        if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.BasicCostOverriden > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * orderRelatedData.BasicCostOverriden / 100).toFixed(2));
        } else if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * orderRelatedData.OrderBasicCost / 100).toFixed(2));
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
                orderId: orderAdditionalServiceData[i].OrderId,
                additionalServiceId: orderAdditionalServiceData[i].AdditionalServiceId,
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


                var currentRow = $('#service-list tr:eq(' + i + ')');
                currentRow.find('td:eq(0) .txtAdditionalServiceName').val(additionalServiceInfo.ServiceName);
                currentRow.find('td:eq(0) .txtAdditionalServiceName').prop('disabled', true);
                currentRow.find('td:eq(1) .chkPayToDriver').prop('checked', additionalServiceInfo.PayToDriver);
                currentRow.find('td:eq(1) .chkPayToDriver').prop('disabled', true);
                currentRow.find('td:eq(2) .txtServiceFee').val(serviceData.additionalServiceFee);
                currentRow.find('td:eq(2) .txtServiceFee').prop('disabled', true);
                currentRow.find('td:eq(3) .txtDriverPercentage').val(serviceData.driverPercentageOnAddService);
                currentRow.find('td:eq(3) .txtDriverPercentage').prop('disabled', true);
                currentRow.find('td:eq(4) .chkIsGstApplicableForService').prop('checked', serviceData.isTaxAppliedOnAddionalService);
                currentRow.find('td:eq(4) .chkIsGstApplicableForService').prop('disabled', true);
                currentRow.find('td:eq(5) .btnAddAdditionalService').hide();
                currentRow.find('td:eq(5) .btnServiceAdded').show();
                currentRow.find('td:eq(5) .btnDeleteAdditionalService').attr('data-serviceid', serviceData.additionalServiceId);
            }
        }
    }
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

        shipperCustomerId: $('#lblShipperAccountNo').text() === "" ? 0 : parseInt($('#lblShipperAccountNo').text()),
        shipperAddressId: $('#hfShipperAddressId').val() === "" ? 0 : parseInt($('#hfShipperAddressId').val()),
        consigneeCustomerId: $('#lblConsigneeAccountNo').text() === "" ? 0 : parseInt($('#lblConsigneeAccountNo').text()),
        consigneeAddressId: $('#hfConsigneeAddressId').val() === "" ? 0 : parseInt($('#hfConsigneeAddressId').val()),
        billToCustomerId: $('#hfBillerCustomerId').val() === "" ? 0 : parseInt($('#hfBillerCustomerId').val()),

        shipperAddressline: $('#txtShipperAddressLine').val(),
        shipperUnitNo: $('#txtShipperUnitNo').val(),
        shipperCityId: $('#ddlShipperCityId').val() === "" ? 0 : parseInt($('#ddlShipperCityId').val()),
        shipperProvinceId: $('#ddlShipperProvinceId').val() === "" ? 0 : parseInt($('#ddlShipperProvinceId').val()),
        shipperPostcode: $('#txtShipperPostcode').val(),
        consigneeAddressline: $('#txtConsigneeAddressLine').val(),
        consigneeUnitNo: $('#txtConsigneeUnitNo').val(),
        consigneeCityId: $('#ddlConsigneeCityId').val() === "" ? 0 : parseInt($('#ddlConsigneeCityId').val()),
        consigneeProvinceId: $('#ddlConsigneeProvinceId').val() === "" ? 0 : parseInt($('#ddlConsigneeProvinceId').val()),
        consigneePostcode: $('#txtConsigneePostcode').val(),


        scheduledPickupDate: date,
        expectedDeliveryDate: $('#txtSchedulePickupDate').val(),
        cityId: $('#ddlConsigneeCityId').val() === "" ? 0 : parseInt($('#ddlConsigneeCityId').val()),
        deliveryOptionId: parseInt($('#ddlDeliveryOptionId').val()),
        vehicleTypeId: parseInt($("#ddlVehicleTypeId").val()),
        unitTypeId: parseInt($('#ddlUnitTypeId').val()),
        weightScaleId: parseInt($('#ddlWeightScaleId').val()),
        weightTotal: $('#txtWeightTotal').val() === "" ? null : $('#txtWeightTotal').val(),
        unitQuantity: $('#txtUnitQuantity').val() === "" ? 0 : parseInt($('#txtUnitQuantity').val()),
        skidQuantity: $('#txtSkidQuantity').val() === "" ? 0 : parseInt($('#txtSkidQuantity').val()),
        totalPieces: $('#txtTotalPieces').val() === "" ? 0 : parseInt($('#txtTotalPieces').val()),

        orderBasicCost: $('#txtBaseOrderCost').val() === "" ? null : parseFloat($('#txtBaseOrderCost').val()),
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

    $('#txtBillToCustomerName').val('');

    $('#txtWayBillNo').val('');
    $('#ddlDeliveryOptionId').val(1);

    $('#txtCustomerRefNo').val('');
    $('#txtCargoCtlNo').val('');
    $('#txtAwbCtnNo').val('');
    $('#txtPickupRefNo').val('');
    $('#txtOrderedBy').val('');

    $('#lblShipperAccountNo').text('');

    $('#txtShipperCustomerName').val('');
    $('#txtShipperAddressLine').val('');
    $('#txtShipperUnitNo').val('');
    $('#ddlShipperCityId').val(335);
    $('#ddlShipperProvinceId').val(7);
    $('#txtShipperPostcode').val('');

    $('#hfShipperAddressId').val('');
    $('#hfConsigneeAddressId').val('');

    $('#lblConsigneeAccountNo').text('');
    $('#txtConsigneeCustomerName').val('');
    $('#txtConsigneeAddressLine').val('');
    $('#txtConsigneeUnitNo').val('');
    $('#ddlConsigneeCityId').val(335);
    $('#ddlConsigneeProvinceId').val(7);
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
    $('#chkIsReturnOrder').is(':checked', false);

    $('#service-list').empty();

    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.off(true);

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

    appendString += '<tr style="height:32px; background-color:#dcf0ff">';
    appendString += '<td style="width:220px">';
    appendString += '<input class="form-control form-control-sm additionalServiceControl txtAdditionalServiceName" id="txtAdditionalServiceName" style="width:200px; margin-left:3px" placeholder="Service name" list="additionalServices" type="search" />';
    appendString += '<datalist id="additionalServices" class="additionalServices">';
    //appendString += '@foreach (var item in Model.AdditionalServices) {';
    //appendString += '<option data-serviceid="@item.Id" value="@item.ServiceName"></option> }';
    appendString += '</datalist>';
    appendString += '</td>';
    appendString += '<td style="width:100px; text-align:center">';
    appendString += '<input type="checkbox" class="chkPayToDriver" id="chkPayToDriver" name="chkPayToDriver" />';
    appendString += '</td>';
    appendString += '<td style="width:110px; padding-right:5px">';
    appendString += '<input type="number" class="form-control form-control-sm additionalServiceControl txtServiceFee " min="0" id="txtServiceFee" step=".01" name="txtServiceFee" placeholder="Fee" title="Applicable service fee amount" />';
    appendString += '</td>';
    appendString += '<td style="width:110px;">';
    appendString += '<input type="number" class="form-control form-control-sm additionalServiceControl txtDriverPercentage" min="0" id="txtDriverPercentage" step=".01" name="txtDriverPercentage" placeholder="Percent" title="Driver portion of the fee" />';
    appendString += '</td>';
    appendString += '<td style="width:100px; text-align:center">';
    appendString += '<input type="checkbox" class="chkIsGstApplicableForService" id="chkIsGstApplicableForService" name="chkIsGstApplicableForService" />';
    appendString += '</td>';
    appendString += '<td style="width:100px;text-align:center;">';
    appendString += '<button class="btn btn-sm btn-primary additionalServiceControl btnAddAdditionalService" id="btnAddAdditionalService" name="btnAddAdditionalService" title="Click to add to order"><i class="fa fa-plus-circle"></i> </button>';
    appendString += '<button class="btn btn-sm btn-success additionalServiceControl btnServiceAdded" style="display:none" title="Service added to order" disabled><i class="fa fa-check-circle"></i> </button>';
    appendString += '&nbsp;';
    appendString += '<button class="btn btn-sm btn-danger additionalServiceControl btnDeleteAdditionalService" id="btnDeleteAdditionalService" name="btnDeleteAdditionalService" title="Delete from order"><i class="fa fa-trash"></i> </button>';
    appendString += '</td>';
    appendString += '</tr>';

    return appendString;
}

//#endregion 





