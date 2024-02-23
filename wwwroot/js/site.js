// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.setTimeout(() => {
    $(".alert").fadeTo(500, 0).slideUp(500, () => {
        $(this).remove();
    });
}, 4000);