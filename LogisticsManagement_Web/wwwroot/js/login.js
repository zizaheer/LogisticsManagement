var data;

$('#btnLogin').on('click', function (event) {

    var userName = $('#txtUserName').val();
    var userPassword = $('#txtPassword').val();

    if (userName === "" && userPassword === "") {
        event.preventDefault();
        $('#lblStatus').text('Err: Valid username and/or password are required.');
    }
});

