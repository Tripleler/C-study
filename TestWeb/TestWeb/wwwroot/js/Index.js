// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validateRegister() {
    var check = document.getElementById("btnCheck").innerText == "사용가능!";
    if (!check) {
        alert("아이디 중복확인을 진행해 주십시오.");
        return;
    };
    var form = document.getElementById("registerForm");
    var name = form.NAME;
    var id = form.USER_ID;
    var pw = form.USER_PWD;
    var pwCheck = form.USER_PWD_CHECK;
    var email = form.EMAIL_ADRESS;
    const regex = /^[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*.[a-zA-Z]{2,3}$/i;

    if (name.value.length < 1) {
        alert("이름을 입력해 주십시오");
        return;
    }
    else if (getTextLength(name.value) > 8) {
        alert("이름은 최대 8자(한글 4자)를 넘을 수 없습니다.")
        return;
    }
    else if (id.value.length < 1) {
        alert('아이디를 입력해 주십시오');
        return;
    }
    else if (getTextLength(id.value) > 10) {
        alert("아이디는 최대 10자(한글 5자)를 넘을 수 없습니다.")
        return;
    }
    else if (pw.value.length < 1) {
        alert("비밀번호를 입력해 주십시오");
        return;
    }
    else if (getTextLength(pw.value) > 10) {
        alert("비밀번호는 최대 10자(한글 5자)를 넘을 수 없습니다.")
        return;
    }
    else if (pwCheck.value != pw.value) {
        alert("작성하신 비밀번호와 확인문구가 일치하지 않습니다.");
        return;
    }
    else if (email.value.length < 1) {
        alert("이메일을 입력해 주십시오");
        return;
    }
    else if (getTextLength(email.value) > 50) {
        alert("이메일 길이가 너무 깁니다. 다른 메일을 사용해 주십시오.")
        return;
    }
    else if (!regex.test(email.value)) {
        alert("이메일형식에 맞게 입력해주십시오");
        return;
    }

    else {
        //form.submit();     
        $.ajax({
            url: "/Home/Register_temp",
            type: "post",
            data: {
                NAME: name.value,
                USER_ID: id.value,
                USER_PWD: pw.value,
                USER_PWD_CHECK: pwCheck.value,
                EMAIL: email.value,
            },
            success: function (result) {
                if (result == "Success") {
                    alert("회원가입 성공! 로그인 화면으로 돌아갑니다.")
                    toLogin();
                    clearRegister();
                }
                else {
                    alert("중복된 아이디입니다.")
                }
            },
            error: function (err) {
                alert(err);
            }
        })
    }
}

var getTextLength = function (str) {
    var len = 0;
    for (var i = 0; i < str.length; i++) {
        if (escape(str.charAt(i)).length == 6) {
            len++;
        }
        len++;
    }
    return len;
}

function clearRegister() {
    var form = document.getElementById("RegisterForm");
}

function validateLogin() {
    var form = document.getElementById("LoginForm");
    var id = form.USER_ID;
    var pw = form.USER_PWD;
    if (id.value.length < 1) {
        alert('아이디를 입력해 주십시오');
        return;
    }
    else if (pw.value.length < 1) {
        alert("비밀번호를 입력해 주십시오");
        return;
    }
    else {
        $.ajax({
            url: "/Home/Login",
            type: "post",
            data: {
                USER_ID: id.value,
                USER_PWD: pw.value,
            },
            success: function (result) {
                if (result == "Success") {
                    toBoard();
                }
                else {
                    alert("존재하지 않는 아이디이거나 비밀번호가 틀립니다.")
                }
            },
            error: function (err) {
                alert(err);
            }
        })
    }
}

function toBoard() {
    location.href = "/Board";
}

function toRegister() {
    var form = document.getElementById("RegisterLayout");
    form.setAttribute("style", "transform: rotateY(0deg); transform-origin: left; transition: all 1s linear; opacity: 1");
    var form2 = document.getElementById("LoginLayout");
    form2.setAttribute("style", "transition: all 1s linear; opacity:0");
}

function toLogin() {
    var form = document.getElementById("RegisterLayout");
    form.setAttribute("style", "transform: rotateY(180deg); transform-origin: left; transition: all 1s linear; opacity: 0;");
    var form2 = document.getElementById("LoginLayout");
    form2.setAttribute("style", "transition: all 1s linear; opacity:1");
}

function Login(e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        validateLogin();
    }
}

function Register(e) {
    if (e.keyCode === 13) {
        e.preventDefault();
        validateRegister();
    }
}

//$(function () {
//    $("#registerForm").on("submit", function (e){
//        e.preventDefault();
//        var formData = $(this).serialize();
//        $.ajax({
//            url: "/Home/Register_temp",
//            type: "post",
//            data: formData,
//            success: function (result) {
//                if (result == "Sucess") {
//                    Test();
//                }
//                else {
//                    alert("회원가입 실패")
//                }
//            },
//            error: function (err) {

//            }
//        })
//    })

//})

function buttonSucess() {
    var btn = document.getElementById("btnCheck");
    btn.classList.replace("btnOrigin", "btnSucess")
    btn.innerText = "사용가능!";
}

function toOrigin() {
    var btn = document.getElementById("btnCheck");
    btn.classList.replace("btnSucess", "btnOrigin");
    btn.innerText = "중복확인"
}

function validateID() {
    var id = document.getElementById("ID").value;
    if (id.length < 1) {
        alert('아이디를 입력해 주십시오');
        return;
    }
    else if (getTextLength(id) > 10) {
        alert("아이디는 최대 10자(한글 5자)를 넘을 수 없습니다.")
        return;
    }
    $.ajax({
        url: "/Home/EditUser",
        type: "post",
        data: {
            ID: document.getElementById("ID").value,
        },
        success: function (result) {
            if (result == "세션만료") {
                alert(result)
                Logout();
            }
            else if (result == "Sucess") {
                alert("사용할 수 있는 아이디입니다.")
                buttonSucess();
            }
            else {
                alert(result)
            }
        },
        error: function (err) {
            alert(err);
        }
    })
}