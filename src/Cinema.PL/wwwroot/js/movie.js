import {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
} from "./additional-components.js";

/*Список URL*/
const URL_ADMIN_MOVIE = '/movie';
const URL_ADMIN_MOVIE_INFO = '/movie-info';

/*Функция отрисовки выбранных жанров*/
function showGenreSelection() {
    let values = $('#movieGenresIdInput option:selected').text().split(" ");
    values.pop();
    $("#selectGenreList").html("");
    values.forEach(item => {
        $("#selectGenreList").append(`
            <div class="bg-light-subtle border border-2 rounded ms-2 mt-2 p-1">
                <p class="fs-5">${item}</p>
            </div>
        `);
    });
}

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
        alert("Прежде создайте жанры фильмов");
    });

    /*Изменение картинки при загрузке файла*/
    $("#movieFileInput").change(function () {
        if (FileReader && this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                $('#imgshow').attr('src', e.target.result);
            }
            reader.readAsDataURL(this.files[0]);
        }
    });

    /*Выбор в select*/
    $("#movieGenresIdInput").change(function() {
        showGenreSelection();
    });

    /*Форма поиска*/
    $("#searchForm").submit(function (event) {
        event.preventDefault();
        let searchText = $("#movieSearchInput").val();
        if ($(this).validate().checkForm() || searchText == "") {
            let pageNumber = $("#moviePageInput").val();
            window.location.href = `${URL_ADMIN_MOVIE}?page=${pageNumber}&&search=${searchText}`;
        }
    });

    /*Сброс валидации при очистке поля поиска*/
    $("#movieSearchInput").on("input", function () {
        if ($(this).val() === "") {
            searchValidator.resetForm();
        }
    });

    /*Добаление value для checkbox формы создания/редактирования фильмов*/
    $("#movieForm").on("change", "input[type='checkbox']", function () {
        var value = $(this).prop('checked');
        $(this).val(value);
    });

    /*Отправка формы создания/редактирования фильма*/ 
    $("#movieForm").submit(function (event) {
        event.preventDefault();
        if (window.FormData === undefined) {
            alert('В вашем браузере не поддерживается загрузка файлов');
        } else if ($(this).validate().checkForm()) {
            let formData = new FormData(this);
            console.log(formData);

            let formUrl = URL_ADMIN_MOVIE_INFO;
            let formMethod = "post";
            let completeMessage = "Фильм добавлен"

            if ($(this).attr("data-send-type") == "change") {
                formMethod = "put";
                completeMessage = "Информация о фильме обновлена";
            }

            $.ajax({
                url: formUrl,
                method: formMethod,
                data: formData,
                cache: false,
                contentType: false,
                processData: false,
                success: function (result) {
                    alert(completeMessage);
                    location.reload();
                },
                error: function (request) {
                    let currentForm = $("#movieForm");
                    deleteErrorMessage(currentForm, ".col-12", 0, false);

                    currentForm.append(errorDiv(request.responseText, 12));
                }
            });
        }
    });

    /*Отправка формы удаления фильма*/
    $(".movie-list-form").submit(function (event) {
        event.preventDefault();
        let article = $(this).parent().parent();
        if ($(this).validate().checkForm()) {
            $.ajax({
                url: URL_ADMIN_MOVIE_INFO,
                method: 'delete',
                data: $(this).serialize(),
                success: function (result) {
                    location.reload();
                },
                error: function (request) {
                    deleteErrorMessage(article, ".col-12", 0, false);
                    
                    article.append(errorDiv(request.responseText, 11));
                }
            });
        }
    });

    /*Просмотр подробной информации*/
    $(".watch-action-a").on("click", function (event) {
        event.preventDefault();
        let article = $(this).parent().parent().parent();
        deleteErrorMessage(article, ".col-12", 0, false);

        $.ajax({
            url: URL_ADMIN_MOVIE_INFO,
            method: 'get',
            data: { id: $(this).attr("data-id") },
            success: function (result) {
                $("#movieIdInput").val(`${result.id}`);
                $("#movieNameInput").val(`${result.name}`);
                $("#movieDescriptionInput").val(`${result.description}`);
                $("#movieDurationInput").val(`${result.duration}`);
                $("#moviePriceInput").val(`${result.price}`);
                $('#movieIsActualInput').prop('checked', result.isActual);
                $('#movieIsActualInput').val(result.isActual);
                result.genresId.forEach(id => {
                    $(`#movieGenresIdInput option[value=${id}]`).prop('selected', true);
                });
                $('#imgshow').attr('src', `/uploads/${result.imageName}`);

                showGenreSelection();

                movieValidator.resetForm();
                changeFormType("#movieForm", "#creteMovieBtn", "change");
            },
            error: function (request) {
                article.append(errorDiv(request.responseText, 11));
            }
        });
    });

    /*Кпопка новый фильм*/
    $("#newMovieBtn").on("click", function () {
        $('#movieForm').trigger("reset");
        showGenreSelection();
        $('#movieForm').find(".col-12").remove();
        $('#imgshow').attr('src', "/img/image-place.jpg");
        movieValidator.resetForm();
        changeFormType("#movieForm", "#creteMovieBtn", "create");
    });

    /*Правила валидации*/
    $.validator.addMethod("cyrillicNumberСolonAndSpace", function (value, element) {
        return /^[А-Яа-яЁё0-9:\- ]+$/.test(value);
    },
        "Используйте кириллицу, цифры, тире, а также знак пробела и двоеточия"
    );

    $.validator.addMethod("cyrillicLatinNumberCommaAndSpace", function (value, element) {
        return /^[А-Яа-яЁёA-Za-z0-9.,\- ]+$/.test(value);
    },
        "Используйте кириллицу, латиницу, цифры, точку, запятую, тире и пробел"
    );

    /*Валидация формы*/
    var movieValidator = $("#movieForm").validate({
                            rules: {
                                id: {
                                    required: true,
                                },
                                name: {
                                    required: true,
                                    minlength: 2,
                                    maxlength: 50,
                                    cyrillicNumberСolonAndSpace: true,
                                },
                                description: {
                                    required: true,
                                    minlength: 1,
                                    maxlength: 200,
                                    cyrillicLatinNumberCommaAndSpace: true,
                                },
                                image: {
                                    required: {
                                        depends: function (element) {
                                            return $("#movieForm").attr("data-send-type") == "create";
                                        }
                                    },
                                },
                                genresId: {
                                    required: true,
                                },
                                duration: {
                                    required: true,
                                },
                                price: {
                                    required: true,
                                },
                            },
                            messages: {
                                id: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                },
                                name: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                    minlength: "Название фильма должно иметь минимум 2 символа",
                                    maxlength: "Максимальное число символов - 30",
                                },
                                description: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                    minlength: "Описание фильма должно иметь минимум 1 символ",
                                    maxlength: "Максимальное число символов - 200",
                                },
                                image: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                },
                                genresId: {
                                    required: "Эта графа обязательна для выбора",
                                },
                                duration: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                },
                                price: {
                                    required: REQUIRED_FIELD_MESSAGE,
                                },
                            }
    });

    var searchValidator = $("#searchForm").validate({
        rules: {
            search: {
                minlength: 1,
                maxlength: 50,
                cyrillicNumberСolonAndSpace: true,
            },
        },
        messages: {
            search: {
                minlength: "Название фильма должно иметь минимум 2 символа",
                maxlength: "Максимальное число символов - 30",
            },
        },
        errorPlacement: function (error, element) {
            let formSection = element.parent().parent().parent();
            deleteErrorMessage(formSection, ".error", 1, true);

            formSection.append(error);
        },
        success: function (label) {
            label.remove();
        }
    });
});