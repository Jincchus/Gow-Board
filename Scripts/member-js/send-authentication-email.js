let isAuthNumberValidated = false;
let currentAuthNumber = null;

const debounce = (func, delay) => {
    let timerId;
    return function (...args) {
        if (timerId) {
            clearTimeout(timerId);
        }
        timerId = setTimeout(() => {
            func.apply(this, args);
            timerId = null;
        }, delay);
    };
};
const debounceDelay = 1000;


const emailAuthenticationBtn = document.querySelector("#Email-authentication-ajax-btn");

emailAuthenticationBtn.addEventListener("click", function () {

    const emailField = fields.Email;
    const isEmailValid = validateField('Email');

    if (!isEmailValid) {
        alert(emailField.invalidText);
        return;
    }

    const email = emailField.element.value;

    fetch('/Member/SendAuthenticationEmail', {
        method: "POST",
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ Email: email })
    })
        .then((response) => {
            if (!response.ok) {
                throw new Error('Network response was not ok')
            }
            return response.json();
        })
        .then(data => {
            debugger

            const checkNum = document.querySelector("#CheckNum");
            const checkNumbOkBtn = document.querySelector("#CheckNum-ok-btn");

            if (data.success) {
                currentAuthNumber = data.authNumber;

                checkNum.disabled = false;
                checkNumbOkBtn.style.display = "block";

                emailAuthenticationBtn.textContent = "재전송";
                emailAuthenticationBtn.style.backgroundColor = "white";
                emailAuthenticationBtn.style.color = "black";
                emailAuthenticationBtn.style.border = "1px solid #000000";


                const endTime = new Date();
                endTime.setMinutes(endTime.getMinutes() + 3);

                const timerInterval = setInterval(updateTimer, 1000);

                function updateTimer() {
                    const currentTime = new Date().getTime();
                    const remainingTime = endTime - currentTime;

                    if (remainingTime <= 0) {
                        clearInterval(timerInterval);
                        checkNum.placeholder = "시간 초과";
                        checkNum.disabled = true;
                    } else {
                        const minutes = Math.floor((remainingTime % (1000 * 60 * 60)) / (1000 * 60));
                        const seconds = Math.floor((remainingTime % (1000 * 60)) / 1000);
                        checkNum.placeholder = `남은 시간 ${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;
                    }
                }

                checkNumbOkBtn.addEventListener("click", function () {
                    const enteredNum = checkNum.value;

                    if (enteredNum === currentAuthNumber) {
                        alert("인증번호를 확인했습니다.");

                        isAuthNumberValidated = true;

                        checkNum.disabled = true;
                        checkNumbOkBtn.style.display = "none";
                    } else {
                        alert("인증 번호가 일치하지 않습니다.");
                    }
                });
            } else {
                alert(data.message || '인증 이메일 전송에 실패했습니다. 다시 시도해주세요');
            }
        })
        .catch(error => {
            console.error('Error:', error)
            alert('catch error : ', error.message);
        });
}, debounceDelay);

function onAuthNumberValidated() {
    return isAuthNumberValidated === true;
}