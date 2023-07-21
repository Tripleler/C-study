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

function Logout() {
    //$.ajax({
    //    url: "/Home/Logout",
    //    type: "post"
    //})
    location.href = "/Home/Logout";
}

function toList(n) {
    if (n != null) {
        location.href = "/Board?page=" + n.component.option("text");
    }
    else {
        location.href = "/Board";
    }
}

function toEdit(n) {
    location.href = "/Board/Edit/" + n;
}

function CreateBoard() {
    var form = document.getElementById("createForm");
    var title = form.BoardTitle;
    if (title.value.length < 1) {
        alert("제목을 입력해주십시오");
        return;
    }
    var content = form.BoardContent;
    if (content.value.length < 1) {
        alert("내용을 입력해주십시오");
        return;
    }
    if (getTextLength(content.value) > 4000) {
        alert("게시글의 내용은 4천자를 넘을 수 없습니다");
        return;
    }
    _FormData.append('BoardTitle', title.value)
    _FormData.append("BoardContent", content.value)
    $.ajax({
        url: "/Board/AddBoard",
        type: "post",
        data: _FormData,
        contentType: false,
        processData: false,
        success: function (result) {
            if (result == "세션만료") {
                alert(result)
                Logout();
            }
            else if (result == "Sucess") {
                alert("게시글 등록 성공!")
                toList();
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

function UploadFiles() {
    $.ajax({
        url: "/File/FileUpload",
        type: "post",
        data: {
            title: document.getElementById("createForm").BoardTitle,
            files: _FormData,
        },
        contentType: false,
        processData: false,
        success: function (result) {
            if (result == "세션만료") {
                alert(result)
                Logout();
            }
            else if (result == "0") {
                alert("파일 첨부 성공!")
                toList();
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

function EditBoard() {
    var form = document.getElementById("editForm");
    var title = form.BoardTitle;
    if (title.value.length < 1) {
        alert("제목을 입력해주십시오");
        return;
    }
    var content = form.BoardContent;
    if (content.value.length < 1) {
        alert("내용을 입력해주십시오");
        return;
    }
    if (getTextLength(content.value) > 4000) {
        alert("게시글의 내용은 4천자를 넘을 수 없습니다");
        return;
    }
    $.ajax({
        url: "/Board/EditBoard",
        type: "post",
        data: {
            BoardNo: document.getElementById("BoardNo").textContent,
            BoardTitle: title.value,
            BoardContent: content.value,
        },
        success: function (result) {
            if (result == "세션만료") {
                alert(result)
                Logout();
            }
            else if (result == "Sucess") {
                alert("게시글 편집 성공!")
                toList();
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

function DeleteBoard() {
    $.ajax({
        url: "/Board/Delete",
        type: "post",
        data: {
            BoardNo: Number(document.getElementById("BoardNo").textContent),
        },
        success: function (result) {
            if (result == "세션만료") {
                alert(result)
                Logout();
            }
            else if (result == "Sucess") {
                toList();
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


function onRowClick(e) {
    location.href = "/Board/Details/" + e.data.BoardNo;
}

function urlCopy() {
    navigator.clipboard.writeText(window.location.href);
}

//function FileDownload(BoardNo, idx, fname) {
//    var startPosition = 0;
//    var endPosition = 0;
//    var chunkSize = 1024 * 1024; // 1MB 단위로 청크 다운로드
//    var fileSize = 0;

//    function downloadChunk(start, end) {
//        var xhr = new XMLHttpRequest();
//        xhr.open('POST', '/Board/FileDownload', true);
//        xhr.responseType = 'blob';

//        xhr.setRequestHeader('Content-Type', 'application/json');

//        xhr.onload = function () {
//            if (xhr.status === 200) {
//                var blob = xhr.response;
//                var link = document.createElement('a');
//                link.href = window.URL.createObjectURL(blob);
//                link.download = fname; // 다운로드될 파일의 이름 설정
//                link.click();
//                window.URL.revokeObjectURL(link.href);

//                if (end < fileSize - 1) {
//                    startPosition = end + 1;
//                    endPosition = Math.min(endPosition + chunkSize, fileSize - 1);
//                    downloadChunk(startPosition, endPosition);
//                }
//            }
//        };

//        xhr.onerror = function () {
//            console.error('An error occurred while downloading the file.');
//        };

//        var data = JSON.stringify({
//            BoardNo: BoardNo,
//            idx: idx,
//            startPosition: start,
//            endPosition: end
//        });

//        xhr.send(data);
//    }

//    // 파일 크기를 먼저 가져오는 요청
//    var sizeXhr = new XMLHttpRequest();
//    sizeXhr.open('GET', '/Board/GetFileSize?BoardNo=' + BoardNo + '&idx=' + idx, true);
//    sizeXhr.onload = function () {
//        if (sizeXhr.status === 200) {
//            fileSize = parseInt(sizeXhr.responseText);

//            if (fileSize > 0) {
//                startPosition = 0;
//                endPosition = Math.min(chunkSize - 1, fileSize - 1);
//                downloadChunk(startPosition, endPosition);
//            }
//        }
//    };
//    sizeXhr.send();
//}

function FileDownload(BoardNo, idx, fname) {
    var xhr = new XMLHttpRequest();
    xhr.open('POST', '/Board/FileDownload', true);
    xhr.responseType = 'blob';
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var blob = new Blob([xhr.response], { type: 'application/octet-stream' });
                var url = URL.createObjectURL(blob);
                var a = document.createElement('a');
                a.href = url;
                a.download = fname;
                a.click();
                URL.revokeObjectURL(url);
            }
        }
    };

    var data = JSON.stringify({
        BoardNo: BoardNo,
        idx: idx
    });

    xhr.send(data);
}

function handleChunk(chunk) {
    // 청크 처리 로직
    // 예: 파일에 쓰기, 다운로드 등
}

function toBoard() {
    location.href = "/Board";
}



function validateMyPage() {
    //var check = document.getElementById("btnCheck").innerText == "사용가능!";
    //if (!check) {
    //    alert("아이디 중복확인을 진행해 주십시오.");
    //    return;
    //};
    var form = document.getElementById("MyPageForm");
    var name = form.NAME;
    //var id = form.USER_ID;
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
    //else if (id.value.length < 1) {
    //    alert('아이디를 입력해 주십시오');
    //    return;
    //}
    //else if (getTextLength(id.value) > 10) {
    //    alert("아이디는 최대 10자(한글 5자)를 넘을 수 없습니다.")
    //    return;
    //}
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
        $.ajax({
            url: "/Home/EditUser_temp",
            type: "post",
            data: {
                NAME: name.value,
                //USER_ID: id.value,
                USER_PWD: pw.value,
                USER_PWD_CHECK: pwCheck.value,
                EMAIL_ADRESS: email.value,
            },
            success: function (result) {
                if (result == "Success") {
                    alert("회원정보 수정 성공! 로그인 화면으로 돌아갑니다.")
                    Logout();
                }
                else {
                    alert("수정에 실패했습니다. 잠시 후 다시 시도해주십시오.")
                }
            },
            error: function (err) {
                alert(err);
            }
        })
    }
}


var _FormData = new FormData(); // 서버 전송 폼 변수
var _FILE_MAX_SIZE = 10;        // 업로드 파일 최대 사이즈

window.onload = function () {
    var isCreate = document.getElementById("Create");
    if (isCreate) {
        // 드래그 앤드 드롭 이벤트가 일어나는 태그
        var fileArea = document.getElementById('dragDropArea');
        // 첨부파일  Input태그
        var fileInput = document.getElementById('fileInput');
        // 파일을 드래그 해서 범위 안에 두었을 때 발생하는 이벤트
        fileArea.addEventListener('dragover', function (evt) {
            evt.preventDefault();
            fileArea.classList.add('dragover');
        });
        // 드래그 한 파일을 범위 밖으로 가져 갈 때 발생하는 이벤트
        fileArea.addEventListener('dragleave', function (evt) {
            evt.preventDefault();
            fileArea.classList.remove('dragover');
        });

        // 드래그 한 파일을 범위 안에 두고 마우스를 떼었을 때 발생하는 이벤트
        fileArea.addEventListener('drop', function (evt) {
            evt.preventDefault();
            fileArea.classList.remove('dragenter');
            // 드래그 한 파일 정보를 가져오는 부분
            var files = evt.dataTransfer.files;
            // 첨부파일  Input태그에 드래그 한 파일을 적용
            fileInput.files = files;
            // 파일 선택 확인 함수 호출
            fnSelectFile();
        });
    }
}

// 파일 선택 확인
function fnSelectFile() {
    // 파일 사이즈 확인
    var maxsize = _FILE_MAX_SIZE * 1024 * 1024;
    // 첨부파일 Input태그 호출
    var input = document.getElementById('fileInput');
    // Input 태그에서 선택된 파일 개수 만큼 반복
    for (var i = 0; i < input.files.length; i++) {
        var name = input.files.item(i).name; // 파일명
        var size = input.files.item(i).size; // 파일 사이즈

        // 파일 사이즈 체크
        if (size > maxsize) {
            alert("파일은 " + _FILE_MAX_SIZE + "MB 이하만 가능합니다.");
        }
        else {
            // 폼 데이터에 파일 정보 보존
            var check = true;
            var filelist = _FormData.getAll("files");
            for (var file of filelist) {
                if (file.lastModified == input.files[i].lastModified) {  // test
                    check = false;
                }
            }
            if (check) {
                _FormData.append("files", input.files[i]);
            }
        }
    }

    // 선택 파일을 화면에 표시
    fnDisplayFileList();
}

// 선택한 파일을 삭제
function fnCancelFile(idx) {
    var filelist = _FormData.getAll("files");
    _FormData.delete("files");
    filelist.splice(idx, 1);
    for (var i = 0; i < filelist.length; ++i) {
        _FormData.append("files", filelist[i]);
    }

    // 선택 파일을 화면에 표시
    fnDisplayFileList();
}

// 선택 파일을 화면에 표시
function fnDisplayFileList() {
    var children = "";
    var output = document.getElementById('fileList');
    filelist = _FormData.getAll("files");
    var i = 0;
    for (var file of filelist) {
        var name = file.name;
        //var size = file.size;
        children += '📎&nbsp;&nbsp;&nbsp;' + name + ' ';
        children += "<span style='cursor:pointer;color:red;' onclick='fnCancelFile(" + i + ");'>✖</span><Br />";
        i++;
    }
    // 추가되는 파일 목록 태그를 HTML에 적용
    output.innerHTML = children;
}

// 파일 업로드 작업
function fnSendFile() {
    $.ajax({
        url: '/File/FileUpload',
        type: "POST",
        contentType: false,
        processData: false,
        data: _FormData,
        async: false,
        success: function (data) {
            if (data.result == 0) {
                // 파일 업로드 후 리스트 초기화
                alert("파일 업로드 완료!");
            }
            else {
                alert("파일 업로드 실패!");
            }
        },
        error: function (err) {
            // 에러일 경우 에러 메세지 출력
            alert(err.statusText);
        }
    });
}

function ToggleReply(e, idx) {
    e.preventDefault();
    var reply = document.getElementById('reply' + idx);
    var otherReplies = document.getElementsByClassName('reply');

    for (var i = 0; i < otherReplies.length; i++) {
        if (otherReplies[i] !== reply) {
            otherReplies[i].classList.add('hidden');
        }
    }

    reply.classList.toggle('hidden');
}