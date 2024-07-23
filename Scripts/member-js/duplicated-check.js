
let idChecked = false;
let nicknameChecked = false;

const idCheckBtn = document.querySelector("#MeMberId-check-ajax-btn");
const nicknameCheckBtn = document.querySelector("#Nickname-check-ajax-btn");

idCheckBtn.addEventListener('click', function () {
    const memberId = document.querySelector("#MemberId").value;

    fetch('/Member/DuplicatedCheckId', {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ MemberId: memberId })
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok')
            }
            return response.json();
        })
        .then(data => {
            alert(data.message);
            idChecked = data.success;
        })

        .catch(error => {
            console.error('Error:', error)
            alert('catch error : ', error.message)
            idChecked = false;
        });

});

nicknameCheckBtn.addEventListener('click', function () {
    const nickname = document.querySelector("#Nickname").value;

    fetch('/Member/DuplicatedCheckNickname', {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Nickname: nickname })
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok')
            }
            return response.json();
        })
        .then(data => {
            alert(data.message);
            nicknameChecked = data.success;
        })

        .catch(error => {
            console.error('Error:', error)
            alert('catch error : ', error.message)
            nicknameChecked = false;
        });
});

function canProceedToRegister() {
    if (!idChecked || !nicknameChecked) {
        alert('아이디 또는 닉네임 중복확인이 필요합니다');
        return false;
    }
    return true;
}





