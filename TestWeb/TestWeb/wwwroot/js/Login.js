// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validateLogin() {
    var form = document.getElementById("loginForm");
    var id = form.USER_ID;
    var pw = form.USER_PWD;
    if (id.value.length < 1) {
        //var test = document.getElementById("Test");
        //test.style.transform = 'rotate(180deg)';
        //test.style.transitionDuration = "1s";
        alert('아이디를 입력해 주세요');
        return;
    }
    else if (pw.value.length < 1) {
        alert("비밀번호를 입력해 주세요");
        return;
    }
    else {
        form.submit();     
    }
}