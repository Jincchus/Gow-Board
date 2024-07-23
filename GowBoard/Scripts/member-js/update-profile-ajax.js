document.addEventListener('DOMContentLoaded', function () {

    const updateBtn = document.getElementById("update-profile-ajax-btn");

    updateBtn.addEventListener('click', function (e) {
        e.preventDefault();

        const originPassword = document.getElementById("OriginPassword").value.trim();
        const newPassword = document.getElementById("Password").value.trim();

        if (originPassword === '') {
            alert('현재 비밀번호를 입렵 후 프로필 수정이 가능합니다.');
            return;
        }

        if (originPassword === newPassword) {
            alert("변경 할 비밀번호가 현재 비밀번호와 같습니다.");
            return;
        }



        let canSubmit = true;
        let isModified = false;
        let isEmailModified = false;
        let isNicknameModified = false

        for (let key in fields) {
            let field = fields[key];
            if (field.element.value !== field.initialValue) {
                isModified = true;
                if (key === 'Email') {
                    isEmailModified = true;
                }
                if (key === 'Nickname') {
                    isNicknameModified = true;
                }
                if (!validateCheck(field)) {
                    canSubmit = false;
                }
            }
        }
        if (!isModified) {
            alert('수정 항목이 존재하지 않습니다.');
            return;
        }
        if (!canSubmit) {
            alert('수정 항목의 유효성을 확인해주세요.');
            return;
        }
        if (isEmailModified && !onAuthNumberValidated()) {
            alert('이메일 인증을 확인해주세요111');
            return;
        }
        if (isNicknameModified && !canProceedToRegister()) {
            
            return;
        }
          


        const formData = new FormData(document.querySelector("#update-profile-form"));

        let formDataObj = {};
        formData.forEach((value, key) => {

            formDataObj[key] = value;

        });


        fetch('/MyPage/UpdateProfile', {
            method: "POST",
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(formDataObj)
        })
            .then((response) => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }

                return response.json();
            })
            .then(data => {
                if (data.success) {
                    alert(data.message);
                    window.location.href = '/MyPage/MyProfile';
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                alert('catch error : ', error.message)
            });

    });

});