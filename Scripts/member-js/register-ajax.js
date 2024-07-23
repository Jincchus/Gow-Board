document.addEventListener('DOMContentLoaded', function () {

    const registerBtn = document.getElementById("register-ajax-btn");

    registerBtn.addEventListener('click', function (e) {

        e.preventDefault();

        if (!validateEmptyFields()) {
            return ;
        }

        const allValid = Object.values(validationResults).every(result => result === true);
        if (!allValid) {
            debugger
            alert('유효성 검사를 확인 후 다시 시도하여주십시오');
            return;
        }

        if (!onAuthNumberValidated()) {
            alert('이메일 인증을 확인해주세요.');
            return;
        }

        if (!canProceedToRegister()) {
            return ;
        }

        const formData = new FormData(document.querySelector("#register-form"));

        let formDataObj = {};
        formData.forEach((value, key) => {
            formDataObj[key] = value;
        });

        fetch('/Member/Register', {
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
                    window.location.href = '/Member/LogIn';
                } else {
                    alert(data.message);
                }
            })
            .catch(error => {
                alert('catch error : ', error.message)
            });

    });

});