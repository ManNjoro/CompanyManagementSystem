// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.setTimeout(() => {
    $(".alert").fadeTo(500, 0).slideUp(500, () => {
        $(this).remove();
    });
}, 4000);

$(document).ready(function () {
    $('#EmpId, #ClientId').change(function () {
        var empId = $('#EmpId').val();
        console.log(empId);
        var clientId = $('#ClientId').val();
        console.log(clientId);
        var url = '/WorksWiths/GetTotalSales';

        $.ajax({
            url: url,
            type: 'GET',
            data: { empId: empId, clientId: clientId },
            success: function (data) {
                console.log("data:",data);
                var totalSales = $('#TotalSales')
                totalSales.val(data); // Update TotalSales field with the returned value
                console.log(totalSales);
            },
            error: function () {
                alert('Error occurred while retrieving total sales.');
            }
        });
    });
});