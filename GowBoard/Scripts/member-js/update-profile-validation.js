const fields = {
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


function validateCheck(field) {
    let chkResult = true;

    if (field.element === fields.PasswordChk.element) {
        if (fields.Password.element.value !== fields.PasswordChk.element.value) {
            field.validationTag.textContent = field.invalidText;
            field.validationTag.style.color = "red";
            chkResult = false;
        } else {
            field.validationTag.textContent = field.validText;
            field.validationTag.style.color = "green";
            chkResult = true;
        }
        validationResults[field.element.id] = chkResult;
        return chkResult;
    }

    if (!field.regex) {
        field.validationTag.textContent = '';
        validationResults[field.element.id] = chkResult;
        return chkResult;
    }

    if (!field.regex.test(field.element.value)) {
        field.validationTag.textContent = field.invalidText;
        field.validationTag.style.color = "red";
        chkResult = false;
    } else {
        field.validationTag.textContent = field.validText;
        field.validationTag.style.color = "green";
        chkResult = true;
    }
    validationResults[field.element.id] = chkResult;
    return chkResult;
}

function validateCheckFields() {
    for (let key in fields) {
        const field = fields[key];

        field.initialValue = field.element.value;

        validationResults[key] = false;

        field.element.addEventListener('input', function () {
            validateCheck(field);
        });
    }
}

function validateField(fieldName) {
    const field = fields[fieldName];
    return validateCheck(field);
}


validateCheckFields();
