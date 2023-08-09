import {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
} from "./additional-components.js";

/*Список URL*/
const URL_USER_LOGIN = '/login';
const URL_USER_REGISTER = '/register';

/*Функция отправки формы*/
function sendForm(event, currentForm, sendUrl) {
    event.preventDefault();
    if (currentForm.validate().checkForm()) {
        $.ajax({
            url: sendUrl,
            method: "post",
            data: currentForm.serialize(),
            success: function (result) {
                window.location.href = result;
            },
            error: function (response) {
                deleteErrorMessage(currentForm, ".col-12", 0, false);
                currentForm.append(errorDiv(response.responseText, 12));
            }
        });
    }
}

$(document).ready(function () {
    /*Форма входа*/
    $("#loginForm").submit(async function (event) {
        sendForm(event, $(this), URL_USER_LOGIN);
    });

    /*Форма регистрации*/
    $("#registerFrom").submit(function (event) {
        sendForm(event, $(this), URL_USER_REGISTER);
    });

    /*Правила валидации*/
    $.validator.addMethod("cyrillicLatinAndSpace", function (value, element) {
        return /^[А-Яа-яЁёA-Za-z ]+$/.test(value);
    },
        "Используйте кириллицу, латиницу и пробел"
    );

    $.validator.addMethod("cyrillicLatinNumberAndSpace", function (value, element) {
        return /^[А-Яа-яЁёA-Za-z0-9]+$/.test(value);
    },
        "Используйте кириллицу, латиницу и цифры"
    );

    $.validator.addMethod("email", function (value, element) {
        return /^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$/.test(value);
    },
        "Неверный формат email"
    );

    /*Валидация формы входа*/
    $("#loginForm").validate({
        rules: {
            email: {
                required: true,
                email: true,
            },
            password: {
                required: true,
                minlength: 4,
                cyrillicLatinNumberAndSpace: true,
            },
        },
        messages: {
            email: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            password: {
                required: REQUIRED_FIELD_MESSAGE,
                minlength: "Пароль должнен иметь минимум 4 символа",
            }
        }
    });

    /*Валидация формы регистрации*/
    $("#registerFrom").validate({
        rules: {
            email: {
                required: true,
                email: true,
            },
            password: {
                required: true,
                minlength: 4,
                cyrillicLatinNumberAndSpace: true,
            },
            name: {
                required: true,
                cyrillicLatinAndSpace: true,
                minlength: 2,
            }
        },
        messages: {
            email: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            password: {
                required: REQUIRED_FIELD_MESSAGE,
                minlength: "Пароль должнен иметь минимум 4 символа",
            },
            name: {
                required: REQUIRED_FIELD_MESSAGE,
                minlength: "Имя должно содержать минимум 2 символа",
            }
        }
    });
});