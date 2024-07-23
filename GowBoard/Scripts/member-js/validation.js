const fields = {
    MemberId: {
        element: document.querySelector("#MemberId"),
        alertText: "아이디를 입력하세요.",
        regex: /^[a-zA-Z0-9]{4,12}$/,
        validationTag: document.querySelector("#MemberId-validation-text"),
        invalidText: "사용 할 수 없는 아이디입니다.",
        validText: "사용 가능한 아이디입니다."
    },
    Password: {
        element: document.querySelector("#Password"),
        alertText: "비밀번호를 입력하세요.",
        regex: /^(?=.*[a-zA-Z])(?=.*[\d$`~!@$!%*#^?&()\[\]\-_=+]).{8,}$/,
        validationTag: document.querySelector("#Password-validation-text"),
        invalidText: "사용 할 수 없는 비밀번호입니다.",
        validText: "사용 가능한 비밀번호입니다."
    },
    PasswordChk: {
        element: document.querySelector("#PasswordCheck"),
        alertText: "비밀번호를 확인해주세요.",
        validationTag: document.querySelector("#PasswordCheck-validation-text"),
        invalidText: "비밀번호가 일치하지 않습니다.",
        validText: "비밀번호가 일치합니다."
    },
    Email: {
        element: document.querySelector("#Email"),
        alertText: "이메일을 입력하세요.",
        regex: /^[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*@[0-9a-zA-Z]([-_\.]?[0-9a-zA-Z])*\.[a-zA-Z]{2,3}$/,
        validationTag: document.querySelector("#Email-validation-text"),
        invalidText: "사용 할 수 없는 이메일입니다.",
        validText: "사용 가능한 이메일입니다."
    },
    Name: {
        element: document.querySelector("#Name"),
        alertText: "이름을 입력하세요.",
        regex: /^[a-zA-Z가-힣]+$/,
        validationTag: document.querySelector("#Name-validation-text"),
        invalidText: "사용 할 수 없는 이름입니다.",
        validText: "사용 가능한 이름입니다."

    },
    Nickname: {
        element: document.querySelector("#Nickname"),
        alertText: "닉네임을 입력하세요.",
        regex: /^[a-zA-Zㄱ-힣0-9][a-zA-Zㄱ-힣0-9]*$/,
        validationTag: document.querySelector("#Nickname-validation-text"),
        invalidText: "사용 할 수 없는 닉네임입니다.",
        validText: "사용 가능한 닉네임입니다."
    }
};

let validationResults = {};

function emptyCheck(field) {

    if (!field.element.value) {

        alert(field.alertText);
        setTimeout(() => {
            field.element.focus();
        }, 0);
        return false;
    }
    return true;
}

function validateEmptyFields() {
    for (let key in fields) {
        if (!emptyCheck(fields[key])) {
            return false;
        }
    }
    return true;
}


function validateCheck(field) {

    if (field.element === fields.PasswordChk.element) {

        if (fields.Password.element.value !== fields.PasswordChk.element.value) {

            field.validationTag.textContent = field.invalidText;
            field.validationTag.style.color = "red";
            return false;
        } else {

            field.validationTag.textContent = field.validText;
            field.validationTag.style.color = "green";
            return true;
        }
    }

    if (!field.regex) {
        field.validationTag.textContent = '';
        return true;
    }

    if (!field.regex.test(field.element.value)) {
        field.validationTag.textContent = field.invalidText;
        field.validationTag.style.color = "red";
        return false;
    } else {
        field.validationTag.textContent = field.validText;
        field.validationTag.style.color = "green";
        return true;
    }
}

function validateCheckFields() {
    for (let key in fields) {
        const field = fields[key];

        validationResults[key] = false;

        field.element.addEventListener('input', function () {
            validationResults[key] = validateCheck(field);
        });
    }
    return validationResults
}

function validateField(fieldName) {
    const field = fields[fieldName];
    return validateCheck(field);
}


validateCheckFields();