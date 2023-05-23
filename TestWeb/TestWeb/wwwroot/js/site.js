// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function add() {
    var form = document.getElementById("loginForm");
    var id = form.USER_ID;
    var pw = form.USER_PWD;
    if (id.value.length < 3) {
        alert('길이가 3글자보다 커야합니다');
        return;
    }
    else {
        form.submit();
    }
}