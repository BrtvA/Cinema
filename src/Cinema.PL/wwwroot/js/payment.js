import {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
} from "./additional-components.js";

/*Список URL*/
const URL_PAYMENT_INDEX = '/pay';


/*Функция ограничения вводимых символов*/
function limitInput(element, regex) {
    let value = element.value;
    if (value.match(regex)) {
        element.value = value.replace(regex, '');
    }
}

$(document).ready(function () {
    /*Ввод только цифр*/
    $('#paymentCardNumberInput').bind("change keyup", function () {
        limitInput(this, /[^0-9 ]/g);
    });

    $('#paymentMonthEndInput, #paymentYearEndInput, #paymentCvcInput').bind("change keyup", function () {
        limitInput(this, /[^0-9]/g);
    });

    /*Форматирование номера карты*/
    $("#paymentCardNumberInput").on("input", function () {
        $(this).val(
            $(this).val().replace(/\s+/g, "").replace(/(\d{4})/g, "$1 ").trim()
        );
    });

    /*Форма оплаты билетов*/
    $("#paymentForm").submit(function (event) {
        event.preventDefault();
        if ($(this).validate().checkForm()) {
            let article = $("#btnArticle");
            deleteErrorMessage(article, ".col-12", 0, false);

            let formData = {
                guidId: $("#paymentGuidIdInput").val(),
                cardNumber: $("#paymentCardNumberInput").val().replace(/\s/g, ''),
                monthEnd: $("#paymentMonthEndInput").val(),
                yearEnd: $("#paymentYearEndInput").val(),
                cvc: $("#paymentCvcInput").val(),
            }

            $.ajax({
                url: URL_PAYMENT_INDEX,
                method: "post",
                data: formData,
                success: function (result) {
                    alert("Оплата произведена");
                    window.location.href = result;
                    //window.location.href = "/Home/Index";
                },
                error: function (request) {
                    article.append(errorDiv(request.responseText, 12));
                }
            });
        }
    });

    /*Правила валидации*/
    $.validator.addMethod("cardNumberFormat", function (value, element) {
        return /^[0-9]{4} [0-9]{4} [0-9]{4} [0-9]{4}/.test(value);
    },
        "Заполните в соответствии с форматом 0000 0000 0000 0000"
    );

    $.validator.addMethod("threeNumberFormat", function (value, element) {
        return /^[0-9]{3}/.test(value);
    },
        "Необходим трёхзначный код"
    );

    $.validator.addMethod("twoNumberFormat", function (value, element) {
        return /^[0-9]{2}/.test(value);
    },
        "Необходимо двухзначное число"
    );

    /*Валидация формы*/
    $("#paymentForm").validate({
        rules: {
            cardNumber: {
                required: true,
                cardNumberFormat: true,
            },
            monthEnd: {
                required: true,
                twoNumberFormat: true,
            },
            yearEnd: {
                required: true,
                twoNumberFormat: true,
            },
            cvc: {
                required: true,
                threeNumberFormat: true,
            },
        },
        messages: {
            cardNumber: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            monthEnd: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            yearEnd: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            cvc: {
                required: REQUIRED_FIELD_MESSAGE,
            },
        }
    });

});