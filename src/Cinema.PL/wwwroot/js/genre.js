import {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
} from "./additional-components.js";

/*Список URL*/
const URL_ADMIN_GENRE = '/genre';

$(document).ready(function () {
    /*Отправка формы*/
    $("#genreForm").submit(function (event) {
        event.preventDefault();
        if ($(this).validate().checkForm()) {
            $.ajax({
                url: URL_ADMIN_GENRE,
                method: 'post',
                data: $(this).serialize(),
                success: function (result) {
                    alert("Жанр добавлен");
                    location.reload();
                },
                error: function (request) {
                    let divSection = $("#genreForm").find(".genre-values");
                    deleteErrorMessage(divSection, ".col-12", 0, false);

                    divSection.append(errorDiv(request.responseText, 12));
                }
            });
        }
    });

    /*Форма изменения жанра*/
    $(".genre-list-form").submit(function (event) {
        let thisForm = $(this);
        event.preventDefault();
        if ($(this).validate().checkForm()) {
            $.ajax({
                url: URL_ADMIN_GENRE,
                method: 'put',
                data: $(this).serialize(),
                success: function (result) {
                    alert("Жанр изменен");
                },
                error: function (request) {
                    let divSection = thisForm.parent();
                    deleteErrorMessage(divSection, ".col-12", 0, false);

                    divSection.append(errorDiv(request.responseText));
                }
            });
        }
    });

    /*Кнопка удаления жанра*/
    $(".delete-action-btn").on("click", function () {
        let divInfo = $(this).parent().parent().parent();
        let genreId = divInfo.find(".genre-values").find("input[type=hidden]").val();

        $.ajax({
            url: URL_ADMIN_GENRE,
            method: 'delete',
            data: { id: genreId },
            success: function (result) {
                //divInfo.parent().remove();
                location.reload();
            },
            error: function (request) {
                let divTotal = divInfo.parent();
                deleteErrorMessage(divTotal, ".col-12", 1, true);
                
                divTotal.append(errorDiv(request.responseText));
            }
        });
    });

    /*Правила валидации*/
    $.validator.addMethod("cyrillicAndSpace", function (value, element) {
        return /^[А-Яа-яЁё\- ]+$/.test(value);
    },
        "Используйте кириллицу, тире и знак пробела"
    );

    /*Валидация формы добавления жанра*/
    $("#genreForm").validate({
        rules: {
            name: {
                required: true,
                minlength: 2,
                maxlength: 30,
                cyrillicAndSpace: true,
            },
        },
        messages: {
            name: {
                required: REQUIRED_FIELD_MESSAGE,
                minlength: "Жанр должен иметь минимум 2 символа",
                maxlength: "Максимальное число символов - 30",
            },
        }
    });

    /*Валидация форм редактирования жанра*/
    $(".genre-list-form").validate({
        rules: {
            name: {
                required: true,
                minlength: 2,
                maxlength: 30,
                cyrillicAndSpace: true,
            },
        },
        messages: {
            name: {
                required: "Это поле обязательно для заполнения",
                minlength: "Жанр должен иметь минимум 2 символа",
                maxlength: "Максимальное число символов - 30",
            },
        },
        errorPlacement: function (error, element) {
            let section = element.parent().parent().parent();
            deleteErrorMessage(section, ".error", 1, true);
            
            error.addClass("col-11 ps-4");
            section.append(error);
        },
        success: function (label) {
            label.remove();
        }
    });
})