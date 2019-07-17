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

});

//#endregion 

//#region Local Variables

var selectedAdditionalServiceArray = [];
var selectedOrdersForDispatch = [];

//#endregion

//#region Events 


$('#btnNewMiscOrder').unbind().on('click', function () {

    $('#frmMiscOrderForm').trigger('reset');
    ClearForm();

    var addressLinesForAutoComplete = GetList('Address/GetAddressForAutoComplete');

    if (addressLinesForAutoComplete !== null) {
        var addressLines = JSON.parse(addressLinesForAutoComplete);
        if (addressLines != '') {
            $.each(addressLines, function (i, item) {
                $('#dlCustomerAddressLines').append($('<option>').attr('data-addressid', item.AddressId).val(item.AddressLine));
            });
        }
    }

    $('#newMiscOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
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

        var billerCustomerId = $('#txtBillToCustomerName').val();
        if (billerCustomerId > 0) {
            FillBillerCustomerInfo(billerCustomerId);
        }
        else {
            bootbox.alert('Bill to customer was not found. Please try again.');
        }
    }
});
$('#txtBillToCustomerName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();

    var billerCustomerId = $('#dlBillers option').filter(function () {
        return this.value === valueSelected;
    }).data('customerid');

    if (billerCustomerId > 0) {
        FillBillerCustomerInfo(billerCustomerId);
    }
    else {
        $('#hfBillerCustomerId').val('');
    }

});

$('#txtCustomerName').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();

        var customerId = $('#txtCustomerName').val();
        if (customerId > 0) {
            FillCustomerInfoAndAddressInfo(customerId);
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
            var emp = GetEmployeeById(employeeId);
            $('#txtEmployeeName').val(emp.FirstName);
        }
    }
});
$('#txtEmployeeName').on('input', function (event) {
    event.preventDefault();
    var valueSelected = $(this).val();

    var employeeId = $('#dlEmployees option').filter(function () {
        return this.value === valueSelected;
    }).data('employeeid');

    if (employeeId > 0) {
        $('#hfEmployeeId').val(employeeId);
        var emp = GetEmployeeById(employeeId);
        $('#txtEmployeeName').val(emp.FirstName);
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
    }
    else {
        ClearCustomerAddressArea();
    }
});

function FillBillerCustomerInfo(customerId) {
    var billerCustomerInfo = null;

    if (customerId > 0) {

        var billerInfo = GetCustomerInfo(customerId);
        if (billerInfo != null && billerInfo != '') {
            billerCustomerInfo = JSON.parse(billerInfo);
            if (billerCustomerInfo !== '') {
                $('#hfBillerCustomerId').val(customerId);
                $('#txtBillToCustomerName').val(billerCustomerInfo.CustomerName);
            }
        }
    }
}

function FillCustomerInfoAndAddressInfo(customerId) {
    var customerInfo = null;

    if (customerId > 0) {
        var customer = GetCustomerInfo(customerId);
        if (customer != null && customer != '') {
            customerInfo = JSON.parse(customer);
            if (customerInfo !== '') {
                $('#txtCustomerName').val(customerInfo.CustomerName);
                $('#lblCustomerAccountNo').text(customerId);
            }
        }

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

$('#chkIsGstApplicable').on('change', function () {
    CalculateOrderCost();
});

$('#txtOrderTotal').keypress(function (event) {
    if (event.keyCode === 13) {
        event.preventDefault();
        $('#txtOrderTotal').change();
    }
});
$('#txtOrderTotal').on('change', function (event) {
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
            currentRow.find('td:eq(2) .chkIsGstApplicableForService').prop('checked', serviceInfo.IsTaxAppliedOnAddionalService);
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

            CalculateAdditionalServiceCost();
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

    CalculateAdditionalServiceCost();
    CalculateOrderCost();

    currentRow.find('td:eq(3) .btnAddAdditionalService').hide();
    currentRow.find('td:eq(3) .btnServiceAdded').show();
    currentRow.find('td:eq(3) .btnDeleteAdditionalService').removeAttr('disabled');

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

    var result;
    if (dataArray[0].wayBillNumber > 0) {
        result = PerformPostActionWithObject('MiscellaneousOrder/Update', dataArray);
        if (result.length !== '') {
            ClearForm();
            $('#newMiscOrder').modal('hide');
            bootbox.alert('Order updated successfully.');
        }
    }
    else {
        result = PerformPostActionWithObject('MiscellaneousOrder/Add', dataArray);
        if (result.length !== '') {
            ClearForm();
            $('#newMiscOrder').modal('hide');
            bootbox.alert('Order saved successfully.');
        }
    }
    selectedAdditionalServiceArray = null;
    $('#loadMiscellaneousOrders').load('MiscellaneousOrder/LoadMiscellaneousOrders');

});


$('#misc-order-list').on('click', '.btnEdit', function (event) {
    event.preventDefault();

    ClearForm();
    $('#frmMiscOrderForm').trigger('reset');

    var wbNumber = $(this).data('waybillnumber');
    if (wbNumber > 0) {
        GetAndFillOrderDetailsByWayBillNumber(wbNumber, 3);
    }

    $('#newMiscOrder').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#newMiscOrder').modal('show');

});

$('#btnPrintWaybill').unbind().on('click', function (event) {
    event.preventDefault();

    if (selectedOrdersForDispatch.length < 1) {
        bootbox.alert('Please select order/s to print.');
        event.preventDefault();
        return;
    }

    var dataArray = [selectedOrdersForDispatch];

    $.ajax({
        'async': false,
        url: "Order/PrintWaybill",
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

        }
    });

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
    if (employeeId != null && employeeId != '') {
        var empInfo = GetSingleById('Employee/GetEmployeeById', employeeId);
        if (empInfo != null) {
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
    var customerAddress = JSON.parse(GetAddressInfo(addressId));

    ClearCustomerAddressArea();

    if (customerAddress !== null) {
        $('#hfCustomerAddressId').val(customerAddress.Id);
        $('#txtCustomerAddressLine').val(customerAddress.AddressLine);
        $('#txtCustomerUnitNo').val(customerAddress.UnitNumber);
        $('#ddlCustomerCityId').val(customerAddress.CityId);
        $('#ddlCustomerProvinceId').val(customerAddress.ProvinceId);
        $('#txtCustomerPostcode').val(customerAddress.PostCode);
    }
}

function ValidateOrderForm(formData) {

    if (formData.billToCustomerId <= 0) {
        bootbox.alert('Please select biller');
        return false;
    }

    if (formData.customerId <= 0) {
        bootbox.alert('Please enter customer information');
        return false;
    }

    if (formData.customerAddressline === '') {
        bootbox.alert('Please enter a valid address or select from address list');
        return false;
    }

    if (formData.orderBasicCost <= 0) {
        bootbox.alert('Please enter order price.');
        return false;
    }

    return true;
}

function CalculateAdditionalServiceCost() {
    var totalAdditionalServiceCost = 0.0;
    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

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

    return totalAdditionalServiceCost;
}

function CalculateOrderCost() {

    var orderTotal = 0;
    var orderBasicTotal = $('#txtOrderTotal').val() !== "" ? parseFloat($('#txtOrderTotal').val()) : 0.0;
    var discountPercentage = $('#txtDiscountPercent').val() !== "" ? parseFloat($('#txtDiscountPercent').val()) : 0.0;
    var taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

    var isGstApplicable = $('#chkIsGstApplicable').is(':checked');

    if (orderBasicTotal > 0) {
        orderTotal = orderBasicTotal;
        $('#lblGrandBasicCost').text(orderBasicTotal.toFixed(2));
    }
    else {
        $('#lblGrandBasicCost').text('0.00');
    }

    if (discountPercentage > 0 && orderBasicTotal > 0) {
        var baseDiscountAmount = discountPercentage * orderBasicTotal / 100;
        $('#lblGrandDiscountAmount').text(baseDiscountAmount.toFixed(2));
        orderTotal = orderBasicTotal - baseDiscountAmount;
    } else {
        $('#lblGrandDiscountAmount').text('0.00');
    }

    if (taxPercentage > 0 && isGstApplicable) {
        var baseTaxAmount = taxPercentage * orderTotal / 100;
        orderTotal = orderTotal + baseTaxAmount;
        $('#lblGrandGstAmount').text(baseTaxAmount.toFixed(2));
    }
    else {
        $('#lblGrandGstAmount').text('0.00');
    }

    if (orderTotal > 0) {
        $('#lblGrandTotalOrderCost').text(orderTotal.toFixed(2));
    } else {
        $('#lblGrandTotalOrderCost').text('0.00');
    }


    var grandTotal = orderTotal;
    var totalServiceCost = CalculateAdditionalServiceCost();

    if (totalServiceCost > 0) {
        $('#lblGrandAddServiceAmount').text(totalServiceCost.toFixed(2));
        grandTotal = grandTotal + totalServiceCost;
    } else {
        $('#lblGrandAddServiceAmount').text('0.00');
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

        $('#hfOrderId').val(orderRelatedData.Id);
        $('#hfEmployeeId').val(orderRelatedData.ServiceProviderEmployeeId);

        $('#txtOrderTotal').val(orderRelatedData.OrderBasicCost);
        $('#lblGrandBasicCost').text('0.00');
        $('#lblGrandDiscountAmount').text('0.00');
        $('#lblGrandGstAmount').text('0.00');
        $('#lblGrandTotalOrderCost').text('0.00');
        $('#lblGrandAddServiceAmount').text('0.00');
        $('#lblGrandTotalAmount').text('0.00');

        $('#txtWayBillNo').val(orderRelatedData.WayBillNumber);
        $('#txtCustomerRefNo').val(orderRelatedData.ReferenceNumber);
        $('#txtCargoCtlNo').val(orderRelatedData.CargoCtlNumber);
        $('#txtAwbCtnNo').val(orderRelatedData.AwbCtnNumber);
        $('#txtOrderDate').val(ConvertDateToUSFormat(orderRelatedData.CreateDate));
        $('#txtOrderedBy').val(orderRelatedData.OrderedBy);
        $('#txtPhoneNo').val(orderRelatedData.ContactPhoneNumber);
        $('#txtDepartment').val(orderRelatedData.Department);

        $('#lblCustomerAccountNo').text(orderRelatedData.ShipperCustomerId);
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

        $('#ddlUnitTypeId').val(orderRelatedData.UnitTypeId);
        $('#txtUnitQuantity').val(orderRelatedData.UnitQuantity);
        $('#txtSkidQuantity').val(orderRelatedData.SkidQuantity);
        $('#txtTotalPieces').val(orderRelatedData.TotalPieces);
        $('#ddlWeightScaleId').val(orderRelatedData.WeightScaleId);
        $('#txtWeightTotal').val(orderRelatedData.WeightTotal);

        $('#txtDiscountPercent').val(orderRelatedData.DiscountPercentOnOrderCost);
        if (orderRelatedData.ApplicableGstPercent > 0) {
            $('#chkIsGstApplicable').prop('checked', true);
        } else {
            $('#chkIsGstApplicable').prop('checked', false);
        }

        $('#chkIsPrintOnWayBill').prop('checked', orderRelatedData.IsPrintedOnWayBill);
        $('#txtCommentsForWayBill').val(orderRelatedData.CommentsForWayBill);
        $('#chkIsPrintOnInvoice').prop('checked', orderRelatedData.IsPrintedOnInvoice);
        $('#txtCommentsForInvoice').val(orderRelatedData.CommentsForInvoice);

        if (orderRelatedData.OrderBasicCost > 0) {
            $('#txtOrderTotal').val(orderRelatedData.OrderBasicCost.toFixed(2));
            $('#lblGrandBasicCost').text(orderRelatedData.OrderBasicCost.toFixed(2));
        }
        else {
            $('#txtOrderTotal').val('');
            $('#lblGrandBasicCost').text('0.00');
        }

        if (orderRelatedData.ApplicableGstPercent != null && orderRelatedData.ApplicableGstPercent > 0) {
            $('#lblGstAmount').text(orderRelatedData.ApplicableGstPercent);
            $('#chkIsGstApplicable').prop('checked', true);
        } else {
            $('#lblGstAmount').text($('#hfTaxAmount').val());
            $('#chkIsGstApplicable').prop('checked', false);
        }

        if (orderRelatedData.ApplicableGstPercent > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandGstAmount').text((orderRelatedData.ApplicableGstPercent * orderRelatedData.OrderBasicCost / 100).toFixed(2));
        } else {
            $('#lblGrandGstAmount').text('0.00');
        }

        if (orderRelatedData.DiscountPercentOnOrderCost > 0 && orderRelatedData.OrderBasicCost > 0) {
            $('#lblGrandDiscountAmount').text((orderRelatedData.DiscountPercentOnOrderCost * orderRelatedData.OrderBasicCost / 100).toFixed(2));
        } else {
            $('#lblGrandDiscountAmount').text('0.00');
        }

        if (orderRelatedData.TotalOrderCost > 0) {
            $('#lblGrandTotalOrderCost').text(orderRelatedData.TotalOrderCost.toFixed(2));
        } else {
            $('#lblGrandTotalOrderCost').text('0.00');
        }

        if (orderRelatedData.TotalAdditionalServiceCost > 0) {
            $('#lblGrandAddServiceAmount').text(orderRelatedData.TotalAdditionalServiceCost.toFixed(2));
        } else {
            $('#lblGrandAddServiceAmount').text('0.00');
        }

        if (orderRelatedData.TotalOrderCost > 0) {
            if (orderRelatedData.TotalAdditionalServiceCost > 0) {
                $('#lblGrandTotalAmount').text((orderRelatedData.TotalOrderCost + orderRelatedData.TotalAdditionalServiceCost).toFixed(2));
            } else {
                $('#lblGrandTotalAmount').text(orderRelatedData.TotalOrderCost.toFixed(2));
            }

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

        customerId: $('#lblCustomerAccountNo').text() === "" ? 0 : parseInt($('#lblCustomerAccountNo').text()),
        customerAddressId: $('#hfCustomerAddressId').val() === "" ? 0 : parseInt($('#hfCustomerAddressId').val()),
        customerAddressline: $('#txtCustomerAddressLine').val(),
        customerUnitNo: $('#txtCustomerUnitNo').val(),
        customerCityId: $('#ddlCustomerCityId').val() === "" ? 0 : parseInt($('#ddlCustomerCityId').val()),
        customerProvinceId: $('#ddlCustomerProvinceId').val() === "" ? 0 : parseInt($('#ddlCustomerProvinceId').val()),
        customerPostcode: $('#txtCustomerPostcode').val(),
        cityId: $('#ddlCustomerCityId').val() === "" ? 0 : parseInt($('#ddlCustomerCityId').val()),
        serviceProviderEmployeeId: $('#hfEmployeeId').val() === "" ? 0 : parseInt($('#hfEmployeeId').val()),

        unitTypeId: $('#ddlUnitTypeId').val() === "" ? 0 : parseInt($('#ddlUnitTypeId').val()),
        weightScaleId: $('#ddlWeightScaleId').val() === "" ? null : parseInt($('#ddlWeightScaleId').val()),
        weightTotal: $('#txtWeightTotal').val() === "" ? null : parseFloat($('#txtWeightTotal').val()),
        unitQuantity: $('#txtUnitQuantity').val() === "" ? null : parseInt($('#txtUnitQuantity').val()),
        skidQuantity: $('#txtSkidQuantity').val() === "" ? null : parseInt($('#txtSkidQuantity').val()),
        totalPieces: $('#txtTotalPieces').val() === "" ? null : $('#txtTotalPieces').val(),

        discountPercentOnOrderCost: $('#txtDiscountPercent').val() === "" ? null : parseFloat($('#txtDiscountPercent').val()),
        applicableGstPercent: $('#chkIsGstApplicable').is(':checked') === true ? ($('#lblGstAmount').text() === "" ? null : parseFloat($('#lblGstAmount').text())) : null,

        orderBasicCost: $('#txtOrderTotal').val() === "" ? null : parseFloat($('#txtOrderTotal').val()),
        totalOrderCost: $('#lblGrandTotalOrderCost').text() === "" ? null : parseFloat($('#lblGrandTotalOrderCost').text()),
        totalAdditionalServiceCost: $('#lblGrandAddServiceAmount').text() === "" ? null : parseFloat($('#lblGrandAddServiceAmount').text()),

        commentsForWayBill: $('#txtCommentsForWayBill').val() === "" ? null : $('#txtCommentsForWayBill').val(),
        isPrintedOnWayBill: $('#chkIsPrintOnWayBill').is(':checked') === true ? 1 : 0,
        commentsForInvoice: $('#txtCommentsForInvoice').val() === "" ? null : $('#txtCommentsForInvoice').val(),
        isPrintedOnInvoice: $('#chkIsPrintOnInvoice').is(':checked') === true ? 1 : 0,
    };

    return [orderData, selectedAdditionalServiceArray];
}

function ClearForm() {

    $('#hfOrderId').val('');
    $('#txtWayBillNo').val('');
    $('#hfBillerCustomerId').val('');
    $('#txtCustomerRefNo').val('');
    $('#txtCargoCtlNo').val('');
    $('#txtAwbCtnNo').val('');
    $('#txtOrderDate').val(ConvertDateToUSFormat(new Date));
    $('#txtOrderedBy').val('');
    $('#txtPhoneNo').val('');
    $('#txtDepartment').val('');
    $('#lblCustomerAccountNo').text();
    $('#hfCustomerAddressId').val('');
    $('#txtCustomerAddressLine').val('');
    $('#txtCustomerUnitNo').val('');
    $('#ddlCustomerCityId').val('0');
    $('#ddlCustomerProvinceId').val('0');
    $('#txtCustomerPostcode').val('');
    $('#hfEmployeeId').val('');
    $('#txtEmployeeName').val('');
    $('#ddlUnitTypeId').val('0');
    $('#ddlWeightScaleId').val('1');
    $('#txtWeightTotal').val('');
    $('#txtUnitQuantity').val('');
    $('#txtSkidQuantity').val('');
    $('#txtTotalPieces').val('');

    $('#txtDiscountPercent').val('');
    $('#chkIsGstApplicable').prop('checked', false);
    //$('#lblGstAmount').text('');

    $('#txtCommentsForWayBill').val('');
    $('#chkIsPrintOnWayBill').prop('checked', false);
    $('#txtCommentsForInvoice').val('');
    $('#chkIsPrintOnInvoice').prop('checked', false);

    $('#txtOrderTotal').val('');
    $('#lblGrandBasicCost').text('0.00');
    $('#lblGrandDiscountAmount').text('0.00');
    $('#lblGrandGstAmount').text('0.00');
    $('#lblGrandTotalOrderCost').text('0.00');
    $('#lblGrandAddServiceAmount').text('0.00');
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





