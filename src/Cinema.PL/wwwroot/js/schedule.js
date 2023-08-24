import {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
} from "./additional-components.js";

/*Список URL*/
const URL_ADMIN_SCHEDULE_INFO = '/schedule-info';

/*Функция изменения типа формы*/
function changeFormType(formId, btnId, actionType) {
    $(formId).attr("data-send-type", actionType);
    if (actionType == "create") {
        $(btnId).text("Создать");
    } else {
        $(btnId).text("Сохранить");
    }
}

$(document).ready(function () {
    /*Кнопка уведомления при создании фильма*/
    $("#noticeBtn").on("click", function () {
        alert("Прежде добавьте фильмы в прокат");
    });


    /*Кпопка новый фильм*/
    $("#newScheduleBtn").on("click", function () {
        $('#scheduleForm').trigger("reset");
        $('#scheduleForm').find(".col-12").remove();
        scheduleValidator.resetForm();
        changeFormType("#scheduleForm", "#creteScheduleBtn", "create");
    });

    /*Отправка формы создания записи*/
    $("#scheduleForm").submit(function (event) {
        event.preventDefault();
        if ($(this).validate().checkForm()) {
            let formUrl = URL_ADMIN_SCHEDULE_INFO;
            let formMethod = "post";
            let completeMessage = "Запись добавлена"

            if ($(this).attr("data-send-type") == "change") {
                formMethod = "put";
                completeMessage = "Информация о записе обновлена";
            }

            $.ajax({
                url: formUrl,
                method: formMethod,
                data: $(this).serialize(),
                success: function (result) {
                    alert(completeMessage);
                    location.reload();
                },
                error: function (request) {
                    let currentForm = $("#scheduleForm");
                    deleteErrorMessage(currentForm, ".col-12", 0, false);

                    currentForm.append(errorDiv(request.responseText, 12));
                }
            });
        }
    });

    /*Просмотр подробной информации*/
    $(".watch-action-a").on("click", function (event) {
        event.preventDefault();
        let article = $(this).parent().parent().parent().parent();
        deleteErrorMessage(article, ".col-12", 0, false);

        $.ajax({
            url: URL_ADMIN_SCHEDULE_INFO,
            method: 'get',
            data: { id: $(this).attr("data-id") },
            success: function (result) {
                $("#scheduleIdInput").val(`${result.id}`);
                $(`#scheduleMovieInput option[value=${result.movieId}]`).prop('selected', true);
                $(`#scheduleHallInput option[value=${result.hallId}]`).prop('selected', true);
                $("#scheduleTimeStart").val(`${result.startTime}`);

                scheduleValidator.resetForm();
                changeFormType("#scheduleForm", "#creteScheduleBtn", "change");
            },
            error: function (request) {
                article.append(errorDiv(request.responseText, 11));
            }
        });
    });

    /*Отправка формы удаления фильма*/
    $(".schedule-list-form").submit(function (event) {
        event.preventDefault();
        let article = $(this).parent().parent();
        if ($(this).validate().checkForm()) {
            $.ajax({
                url: URL_ADMIN_SCHEDULE_INFO,
                method: 'delete',
                data: $(this).serialize(),
                success: function (result) {
                    //article.remove();
                    location.reload();
                },
                error: function (request) {
                    deleteErrorMessage(article, ".col-12", 0, false);
                    
                    article.append(errorDiv(request.responseText, 11));
                }
            });
        }
    });

    /*Валидация формы*/
    var scheduleValidator = $("#scheduleForm").validate({
        rules: {
            id: {
                required: true,
            },
            movieId: {
                required: true,
            },
            hallId: {
                required: true,
            },
            hallId: {
                required: true,
            },
            startTime: {
                required: true,///Добавить правило что дата не меньше текущей
            },
        },
        messages: {
            id: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            movieId: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            hallId: {
                required: REQUIRED_FIELD_MESSAGE,
            },
            startTime: {
                required: REQUIRED_FIELD_MESSAGE,
            },
        }
    });
});