var tariffData;





$('#btnEdit').on('click', function () {
    console.log('item id ' + data - bind);
});





$('.btnDelete').on('click', function () {
    SetAlertType('Warning', 'The data will be deleted. Are you sure you want ot continue?');
    tariffData = $(this).data('tariff');
});

$('#btnProceed').on('click', function () {
    if (tariffData !== null) {
        RemoveTariff(tariffData);
    }
});

function RemoveTariff(tariffData) {
    $.ajax({
        url: 'Tariff/Remove',
        type: 'POST',
        data: JSON.stringify([tariffData]),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (result) {
           // SetAlertType('Success', 'Data has been removed.');
        },
        error: function (result) {
           //// SetAlertType('Failed', 'An error occured during deleting the data.');
        }
    });
}