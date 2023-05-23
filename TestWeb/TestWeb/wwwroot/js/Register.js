// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function validateRegister() {
    var form = document.getElementById("registerForm");
    var name = form.NAME;
    var id = form.USER_ID;
    var pw = form.USER_PWD;
    var pwCheck = form.USER_PWD_CHECK;
    var email = form.EMAIL_ADRESS;
    const regex = /^[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_.]?[0-9a-zA-Z])*.[a-zA-Z]{2,3}$/i;

    if (name.value.length < 1) {
        alert("이름을 입력해 주세요");
        return;
    }
    else if (id.value.length < 1) {
        alert('아이디를 입력해 주세요');
        return;
    }
    else if (pw.value.length < 1) {
        alert("비밀번호를 입력해 주세요");
        return;
    }
    else if (pwCheck.value != pw.value) {
        alert("작성하신 비밀번호와 확인문구가 일치하지 않습니다.");
        return;
    }
    else if (email.value.length < 1) {
        alert("이메일을 입력해 주세요");
        return;
    }
    else if (!regex.test(email.value)) {
        alert("이메일형식에 맞게 입력해주세요");
        return;
    }
    else
    {
        //form.submit();     
        $.ajax({
            url: "/Home/Register_temp",
            type: "post",
            data: {
                NAME = name.value,
                USER_ID: id.value,
                USER_PWD: pw.value,
                USER_PWD_CHECK: pwCheck.value,
                EMAIL = email.value,
            },
            success: function (result) {
                if (result == "Sucess") {

                }
                else {

                }
            },
            error: function (err) {

            }
        })
    }
}

function Test() {
    var form = document.getElementById("RegisterLayout");
    form.setAttribute("style", "position:absolute;left:50%;transform: rotateY(150deg); transform-origin: left;  transition: all 1s linear; opacity:0; ");

    //form.setAttribute("style", "display:none");
    setTimeout(() => form.setAttribute("style", "display:none;"), 1000);
    var form2 = document.getElementById("LoginLayout");
    form2.classList.add()
    form2.setAttribute("style", "position:absolute; left:50%;display: flex; align-items: center; justify-content: center; opacity:0;");
    form2.setAttribute("style", "position:absolute; left:50%;display: flex; align-items: center; justify-content: center;  transition: all 1s linear; opacity:1;");
}
