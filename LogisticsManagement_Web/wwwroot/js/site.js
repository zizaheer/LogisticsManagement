// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Write your JavaScript code.

// Please refer to bundleconfig.json where the js files are being minified as site.min.js which is referenced on _Layout template.
$(document).ready(function () {
    //displayClock();
});



$('#lnkEmployeeClockIn').on('click', function () {
    displayClock();
    setInterval(displayClock, 1000);

    $('#employeeClockIn').modal({
        backdrop: 'static',
        keyboard: false
    });
    $('#employeeClockIn').modal('show');
});
function displayClock() {
    var currentDate = new Date();
    document.getElementById('lblCurrentTime').innerHTML = currentDate.toLocaleString();
}

$('#btnClockIn').on('click', function () {

    var empId = $('#txtEmployeeId').val();

    var result = AddEntry('EmployeeTimesheet/Add', [empId]);


});
