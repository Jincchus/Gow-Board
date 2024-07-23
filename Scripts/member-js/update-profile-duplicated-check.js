
let idChecked = false;
let nicknameChecked = false;

const nicknameCheckBtn = document.querySelector("#Nickname-check-ajax-btn");


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
    if (!nicknameChecked) {
        alert('닉네임 중복확인이 필요합니다');
        return false;
    }
    return true;
}





