import {
    errorDiv,
    deleteErrorMessage
} from "./additional-components.js";

/*Список URL*/
const URL_HOME_POSITIONS = '/positions';
const URL_HOME_PRICE = '/price';
const URL_HOME_TIME = '/time';
const URL_HOME_INFO = '/info';

/*Заблокировать элементы формы*/
function disableFormInput(isDisabled) {
    $("#infoScheduleIdInput").html("");

    if (isDisabled) {
        $("#infoScheduleIdInput").attr("disabled", "disabled");
        $("#infoBuyBtn").addClass("disabled");
    }
    else {
        $("#infoScheduleIdInput").attr("disabled", false);
        $("#infoBuyBtn").removeClass("disabled");
    }
}

/*Функция показа цены*/
async function showPrice() {
    let divContainer = $("#pricePlace").parent().parent();
    deleteErrorMessage(divContainer, ".col-12", 0, false);

    $.ajax({
        url: URL_HOME_PRICE,
        method: 'get',
        data: {
            scheduleId: $("#infoScheduleIdInput").val(),
            ticketCount: $("#infoPositionInput").find("option").length,
        },
        async: true,
        success: function (result) {
            $("#pricePlace").text(result);
        },
        error: function (request) {
            divContainer.append(errorDiv(request.responseText, 12));
        }
    });
}

/*Выбор места*/
function createPositionClickHandler() {
    $(".cinema-place-position-circle").off("click");
    $(".cinema-place-position-circle").on("click", function () {
        let circle = $(this);
        let classArray = circle.attr("class").split(' ');
        if (classArray.indexOf("selected") == -1) {
            if (classArray.indexOf("chosen") == -1) {
                circle.addClass("chosen");
                
                $("#infoPositionInput").append(`
                    <option data-row=${circle.attr('data-row')} data-column=${circle.attr('data-column')}></option>
                `);
                $('#infoPositionInput option').prop('selected', true);
            }
            else {
                circle.removeClass("chosen");
                let options = $("#infoPositionInput").find(`option[data-row=${circle.attr('data-row')}][data-column=${circle.attr('data-column')}]`);
                
                if (options.length > 0) {
                    if (options.length == 1) {
                        $('#infoPositionInput option').prop('selected', false);
                    }
                    options.remove();
                }
            }
        }

        showPrice();
    });
}

/*Функция отрисовки мест в зале*/
function drawCinemaPlace() {
    let row = $('#infoHallIdInput option:selected').attr("data-row");
    let column = $('#infoHallIdInput option:selected').attr("data-column");

    let rowPlace = $(".cinema-place-row");
    if (rowPlace.length > 0) {
        rowPlace.remove();
    }

    let rowNumberElement = (num, position) => `
        <div class="d-flex flex-row justify-content-${position} align-items-center cinema-place-number">
            <p class="fs-5">${num}</p>
        </div>
    `;

    for (let rowNum = 1; rowNum <= row; rowNum++) {
        let rowContainer = $('<div class="d-flex flex-row justify-content-center align-items-center cinema-place-row"></div>');
        rowContainer.append(rowNumberElement(rowNum, "start"));

        for (let colNum = 1; colNum <= column; colNum++) {
            rowContainer.append(`
                <div class="d-flex flex-row justify-content-center align-items-center cinema-place-position">
                    <div class="cinema-place-position-circle" data-row=${rowNum} data-column=${colNum}></div>
                </div>
            `);
        }

        rowContainer.append(rowNumberElement(rowNum, "end"));

        $(".cinema-place").append(rowContainer);
    }

    createPositionClickHandler();
}

/*Функция отрисовки занятых мест*/
function drawSelectedPositions(positions) {
    let rowContainers = $(".cinema-place").find(".cinema-place-row");
    positions.forEach(pos => {
        let colContainers = rowContainers.eq(pos.row - 1).find(".cinema-place-position");
        colContainers.eq(pos.column - 1).find(".cinema-place-position-circle").addClass("selected");
    });
}

/*Функция показа занятых мест*/
async function showSelectedPositions() {
    let divContainer = $("#infoScheduleIdInput").parent().parent();
    deleteErrorMessage(divContainer.parent(), ".col-12", 0, false);

    $.ajax({
        url: URL_HOME_POSITIONS,
        method: 'get',
        data: {
            scheduleId: $("#infoScheduleIdInput").val()
        },
        async: true,
        success: function (result) {
            drawCinemaPlace();
            drawSelectedPositions(result);
        },
        error: function (request) {
            $(errorDiv(request.responseText, 12)).insertAfter(divContainer);
        }
    });
}

/*Функция очистки цены*/
function clearPrice() {
    $("#pricePlace").text("0");
    deleteErrorMessage($("#infoPositionInput"), "option", 0, false);
}

/*Функция по загрузке списка времен*/
async function showTimeList() {
    let divContainer = $("#infoScheduleIdInput").parent().parent().parent();
    deleteErrorMessage(divContainer, ".col-12", 0, false);

    return $.ajax({
        url: URL_HOME_TIME,
        method: 'get',
        data: {
            hallId: $("#infoHallIdInput").val(),
            movieId: $("#infoMovieIdInput").val(),
            date: $("#infoDateInput").val(),
        },
        async: true,
        success: function (result) {
            if (result.length > 0) {
                disableFormInput(false);

                result.forEach(item => {
                    $("#infoScheduleIdInput").append(`
                            <option value="${item.id}">${item.name}</option>
                        `);
                });

                $('#infoScheduleIdInput option:first').prop('selected', true);
            }
            else {
                disableFormInput(true);
            }
        },
        error: function (request) {
            divContainer.append(errorDiv(request.responseText, 12));

            disableFormInput(true);
        }
    });
}

$(document).ready(function () {
    /*Запустить после загрузки страницы*/
    (async () => {
        await showTimeList().then(() => {
            showSelectedPositions();
        });
    })();

    /*Действия при смене зала*/
    $("#infoHallIdInput").change(async function () {
        drawCinemaPlace();
        await showTimeList().then(() => {
            showSelectedPositions();
            clearPrice();
        });
    });

    /*Действия при смене времени*/
    $("#infoScheduleIdInput").change(async function () {
        await showSelectedPositions();
        clearPrice();
    });

    /*Форма заказа места в зале*/
    $("#orderForm").submit(function (event) {
        event.preventDefault();
        if ($(this).validate().checkForm()) {
            let formData = {
                scheduleId: $("#infoScheduleIdInput").val(),
                positions: [],
            };
            $("#infoPositionInput").find("option").each(function () {
                formData.positions.push({
                    row: $(this).attr("data-row"),
                    column: $(this).attr("data-column")
                });
            });

            $.ajax({
                url: URL_HOME_INFO,
                method: "post",
                data: formData,
                success: function (result) {
                    //console.log(Object.keys({ result.guidId })[0]);
                    //window.location.href = `/Home/BuyInfo?guidId=${result}`;
                    window.location.href = `${result.uri}${result.guidId}`;
                },
                error: function (request) {
                    let currentForm = $("#orderForm");
                    deleteErrorMessage(currentForm, ".col-12", 0, false);
                    currentForm.append(errorDiv(request.responseText, 12));
                }
            });
        }
    });

    /*Правила валидации*/
    $.validator.addMethod("positionRequired", function (value, element) {
        return $(element).find('option:selected').length > 0;
    },
        "Выберете место в зале"
    );

    $.validator.setDefaults({
        ignore: []
    });

    /*Валидация формы*/
    $("#orderForm").validate({
        rules: {
            movieId: {
                required: true,
            },
            date: {
                required: true,
            },
            hallId: {
                required: true,
            },
            scheduleId: {
                required: true,
            },
            positions: {
                positionRequired: true,
            },
        },
        messages: {
            movieId: {
                required: "Отсутствуют необходимые данные. Перезагрузите страницу",
            },
            date: {
                required: "Отсутствуют необходимые данные. Перезагрузите страницу",
            },
            hallId: {
                required: "Выберете зал",
            },
            scheduleId: {
                required: "Выберете время сеанса",
            },
        },
        errorPlacement: function (error, element) {
            let divContainer = element.parent().parent().parent();
            deleteErrorMessage(divContainer, ".error", 1, true);

            divContainer.append(error);
        },
        success: function (label) {
            label.remove();
        }
    });
});