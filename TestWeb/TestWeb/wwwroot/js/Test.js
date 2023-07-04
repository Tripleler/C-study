// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function rotate() {
    var form = document.getElementById("logo");
    form.setAttribute("style", "transform: scale(0.1) rotate(360deg); transition: all 1s linear;");
}
function rollback() {
    var form = document.getElementById("logo");
    form.setAttribute("style", "transform: scale(1) rotate(0deg); transition: all 1s linear;");
}