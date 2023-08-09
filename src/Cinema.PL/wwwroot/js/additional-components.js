/*Сообщения*/
const REQUIRED_FIELD_MESSAGE = "Это поле обязательно для заполнения";

/*Контейнер для вывода сообщей об ошибках*/
let errorDiv = (text, columnSize = 11) => `
    <div class="col-12 d-flex flex-row flex-wrap justify-content-end align-items-center">
        <div class="col-${columnSize}">
            <p class="text-danger container-fluid">${text}</p>
        </div>
    </div>
`;

/*Удаление сообщений с ошибкой*/
function deleteErrorMessage(section, errorClass, count, isLast) {
    let divSection = section.find(errorClass);
    if (divSection.length > count) {
        if (isLast) {
            divSection.last().remove();
        }
        else {
            divSection.remove();
        }
    }
}

export {
    errorDiv,
    deleteErrorMessage,
    REQUIRED_FIELD_MESSAGE
};