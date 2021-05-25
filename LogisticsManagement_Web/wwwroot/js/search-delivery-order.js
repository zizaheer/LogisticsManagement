

//#region Local Variables
var employeeData;
var paidByValue = $('input[name=rdoPaidBy]').val();
//alert(paidByValue);
var isNewEntry = false;

// global tax amount
// var taxPercentage = 0.0;
// taxPercentage = $('#lblGstAmount').text() !== "" ? parseFloat($('#lblGstAmount').text()) : 0.0;

var isCustomerTaxApplicable = false;
var selectedAdditionalServiceArray = [];
var additionalServiceSerial = 0;

//#endregion

//#region Events 

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
$('#delivery-order-list .chkOrderToPrint').change(function (event) {
    //event.preventDefault();
    var isChecked = $(this).is(':checked');
    var orderId = $(this).data('waybillnumber');
    getSelectedOrder(isChecked, orderId);
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
$('#delivery-order-list').on('click', '.btnEdit', function (event) {
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
    var countries = GetList('Country/GetAllCountries');
    if (countries !== '') {
        var parsedCountries = JSON.parse(countries);
        $.each(parsedCountries, function (i, item) {
            $('#ddlShipperCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
            $('#ddlConsigneeCountries').append($('<option></option>').val(item.Id).html(item.Alpha3CountryCode));
        });
    }
    var deliveryOptions = GetList('DeliveryOption/GetAllOptions');
    if (deliveryOptions !== '') {
        var parsedOptions = JSON.parse(deliveryOptions);
        $.each(parsedOptions, function (i, item) {
            $('#ddlDeliveryOptionId').append($('<option></option>').val(item.Id).html(item.OptionName + ' - (' + item.ShortCode + ')'));
        });
    }
    var unitTypes = GetList('UnitType/GetAllTypes');
    if (unitTypes !== '') {
        var parsedTypes = JSON.parse(unitTypes);
        $.each(parsedTypes, function (i, item) {
            $('#ddlUnitTypeId').append($('<option></option>').val(item.Id).html(item.ShortCode));
        });
    }

    var weightScales = GetList('WeightScale/GetAllScales');
    if (weightScales !== '') {
        var parsedScales = JSON.parse(weightScales);
        $.each(parsedScales, function (i, item) {
            $('#ddlWeightScaleId').append($('<option></option>').val(item.Id).html(item.ShortCode));
        });
    }

    var config = GetSingleById('Configuration/GetConfiguration');
    if (config !== '') {
        var configuration = JSON.parse(config);
        $('#lblGstAmount').text(configuration.TaxToCall + '-' + configuration.TaxAmount + '%');
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
    appendString += '<button type="button" class="btn btn-sm btn-primary additionalServiceControl btnAddAdditionalService" name="btnAddAdditionalService" title="Click to add to order" disabled><i class="fa fa-plus-circle"></i> </button>';
    appendString += '&nbsp;' + '<button type="button" class="btn btn-sm btn-success additionalServiceControl btnEditService" title="Update & save" disabled><i class="fa fa-save"></i> </button>';
    appendString += '&nbsp;';
    appendString += '<button type="button" class="btn btn-sm btn-danger additionalServiceControl btnDeleteAdditionalService" name="btnDeleteAdditionalService" title="Delete from order" disabled><i class="fa fa-trash"></i> </button>';
    appendString += '</div>';

    appendString += '</div>';


    return appendString;
}
function GetCustomerInfo(customerId) {
    var customerInfo = GetSingleById('Customer/GetCustomerById', customerId);
    return customerInfo;
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

$('#order-list').on('change', '.chkCheckAllOrders', function (event) {
    event.preventDefault();
    var isChecked = $(this).is(':checked');
    var wbArrayString = $('.hfWaybillArray').val();
    getSelectedOrders(isChecked, wbArrayString, false);
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

    var provinces = GetListById('Province/GetProvincesByCountry', countryId);
    $('#ddlShipperProvinceId').empty();
    $('#ddlShipperProvinceId').append($('<option></option>').val('0').html('Prov'));
    if (provinces !== '') {
        var parsedProvinces = JSON.parse(provinces);
        $.each(parsedProvinces, function (i, item) {
            $('#ddlShipperProvinceId').append($('<option></option>').val(item.Id).html(item.ShortCode));
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
    var provinces = GetListById('Province/GetProvincesByCountry', countryId);
    $('#ddlConsigneeProvinceId').empty();
    $('#ddlConsigneeProvinceId').append($('<option></option>').val('0').html('Prov'));
    if (provinces !== '') {
        var parsedProvinces = JSON.parse(provinces);
        $.each(parsedProvinces, function (i, item) {
            $('#ddlConsigneeProvinceId').append($('<option></option>').val(item.Id).html(item.ShortCode));
        });
    }
});

//#endregion

//#region Private methods
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
function MakeReturnToggleOn() {
    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.on(true);
}
function MakeReturnToggleOff() {
    var toggle = $('#chkIsReturnOrder').data('bs.toggle');
    toggle.off(true);
}
//#endregion 



