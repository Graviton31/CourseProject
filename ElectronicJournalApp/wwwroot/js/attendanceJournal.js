// Получаем значение cookie UserLogin
const userLogin = getCookie("login");
if (!userLogin) console.error('UserLogin cookie not found'); // Если cookie не найден, выводим ошибку

// Получаем роль пользователя
const userRole = getCookie("role");
if (!userRole) {
    console.error('UserRole cookie not found'); // Если cookie не найден, выводим ошибку
} else {
    console.log('User Role:', userRole); // Проверяем значение роли
}
console.log('User Login:', userLogin);
// Запрашиваем данные предметов в зависимости от роли пользователя
let fetchUrl;
if (userRole === 'руководитель' || userRole === 'администратор') {
    fetchUrl = `https://localhost:7022/api/subjects/subjectsGrops`; // Запрос всех предметов
} else if (userRole === 'учитель') {
    fetchUrl = `https://localhost:7022/api/subjects/teacher/${userLogin}`; // Запрос предметов для учителя
} else {
    console.error('Неизвестная роль пользователя');
}

console.log('Fetch URL:', fetchUrl); // Проверяем сформированный URL


fetch(fetchUrl)
    .then(response => {
        if (!response.ok) throw new Error('Network response was not ok'); // Проверяем успешность ответа
        return response.json(); // Преобразуем ответ в JSON
    })
    .then(data => {
        const subjectsList = document.getElementById('subjects-list'); // Получаем элемент списка предметов
        data.forEach(subject => {
            // Создаем элемент списка для каждого предмета
            const subjectItem = document.createElement('li');
            subjectItem.className = 'mb-1';
            subjectItem.innerHTML = `
            <button class="btn-custom w-full text-left p-2 font-bold bg-[#1B9AAA] rounded subject-name">${subject.name}</button>
            <ul class="ml-4 mt-1 group-list">
                ${subject.groups.map(group => `
                    <li class="mb-1">
                        <button class="block w-full text-left p-2 group-name rounded" data-group-id="${group.idGroup}">${group.name}</button>
                    </li>
                `).join('')}
            </ul>
            `;
            subjectsList.appendChild(subjectItem); // Добавляем предмет в список
        });

        // Добавляем обработчики событий для кнопок предметов
        document.querySelectorAll('.subject-name').forEach(subjectElement => {
            subjectElement.addEventListener('click', function () {
                this.nextElementSibling.classList.toggle('active'); // Переключаем видимость групп
            });
        });

        // Добавляем обработчики событий для кнопок групп
        document.querySelectorAll('.group-name').forEach(groupElement => {
            groupElement.addEventListener('click', function () {
                const groupId = this.dataset.groupId; // Получаем ID группы
                if (!groupId) console.error('Group ID is undefined'); // Проверяем наличие ID

                // Убираем выделение у всех групп и выделяем текущую
                document.querySelectorAll('.group-name').forEach(el => el.classList.remove('highlight'));
                this.classList.add('highlight');
                document.getElementById('addEntry').style.display = 'block'; // Показываем кнопку добавления записи
                document.getElementById('exportEntries').style.display = 'block'; 
                clearDateColumns(); // Очищаем столбцы с датами

                // Запрашиваем студентов группы с записями из журнала
                fetch(`https://localhost:7022/api/subjects/group/${groupId}`)
                    .then(response => {
                        if (!response.ok) throw new Error('Network response was not ok');
                        return response.json(); // Преобразуем ответ в JSON
                    })
                    .then(data => {
                        const tbody = document.querySelector('tbody'); // Получаем тело таблицы
                        const lessonDates = data.lessonDates; // Получаем уникальные даты уроков
                        const students = data.students; // Получаем студентов

                        // Очищаем текущее содержимое
                        tbody.innerHTML = '';

                        // Проверяем, есть ли записи в расписании
                        if (students.length === 0) {
                            const row = document.createElement('tr');
                            row.innerHTML = `<td colspan="5" class="py-2 px-4 border-b text-center">Нет записей для этой группы</td>`;
                            tbody.appendChild(row); // Добавляем строку с сообщением
                            return; // Если записей нет, выходим
                        }

                        var dateObj;
                        var day;
                        var month;

                        // Создаем заголовки для уникальных дат
                        const tableHeader = document.querySelector('thead tr');
                        lessonDates.forEach(date => {
                            const newHeaderCell = document.createElement('th'); // Создаем новый заголовок
                            newHeaderCell.className = 'border-b dynamic-column'; // Добавляем классы для стилей

                            // Извлекаем день и месяц из даты
                            dateObj = new Date(date);
                            day = dateObj.getDate();
                            month = dateObj.getMonth() + 1;

                            newHeaderCell.textContent = `${day}/${month}`; // Устанавливаем текст заголовка
                            newHeaderCell.value = date;
                            tableHeader.appendChild(newHeaderCell); // Добавляем новый заголовок в таблицу
                        });

                        // Создаем строки для студентов
                        students.forEach((student, index) => {
                            const row = document.createElement('tr');
                            row.innerHTML = `<td class="border-b number-column">${index + 1}</td><td class="py-2 px-4 border-b student-name student-column" data-student-id="${student.idStudent}">${student.name} ${student.surname}</td>`;

                            // Добавляем ячейки для статусов по датам
                            lessonDates.forEach(date => {
                                const journalEntry = student.journals.find(j => j.lessonDate === date);
                                const statusCell = document.createElement('td');
                                statusCell.className = 'border-b dynamic-column';

                                // Проверяем, есть ли запись для данной даты
                                if (journalEntry) {
                                    statusCell.textContent = journalEntry.statusShortName; // Отображаем короткое имя статуса
                                    statusCell.dataset.statusId = journalEntry.statusId; // Добавляем ID статуса в атрибут
                                } else {
                                    statusCell.textContent = ''; // Если записи нет, оставляем ячейку пустой
                                }

                                row.appendChild(statusCell); // Добавляем ячейку в строку
                            });

                            tbody.appendChild(row); // Добавляем строку в таблицу
                        });
                        addClickEventToCells(); // Добавляем обработчики клика на ячейки
                    })
                    .catch(console.error); // Обработка ошибок
            });
        });
    })
    .catch(console.error); 

// Функция для очистки столбцов с датами
function clearDateColumns() {
    const tableHeader = document.querySelector('thead tr');
    tableHeader.querySelectorAll('th:not(:nth-child(1)):not(:nth-child(2))').forEach(header => header.remove()); // Удаляем заголовки дат
    document.querySelectorAll('tbody tr').forEach(row => {
        row.querySelectorAll('td:not(:nth-child(1)):not(:nth-child(2))').forEach(cell => cell.remove()); // Удаляем ячейки с датами
    });
}

// Обработчик для кнопки "Добавить запись"
document.getElementById('addEntry').addEventListener('click', function () {
    // Показываем модальное окно для добавления даты
    $('#dateModal').modal('show');;
});

// Обработчик для кнопки "Добавить запись"
document.getElementById('closeModal').addEventListener('click', function () {
    // Показываем модальное окно для добавления даты
    $('#dateModal').modal('hide');;
});

// Функция для получения статусов посещаемости
function fetchUnvisitedStatuses() {
    return fetch('https://localhost:7022/api/unvisitedstatuses')
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok'); // Проверяем успешность ответа
            return response.json(); // Преобразуем ответ в JSON
        })
        .then(data => {
            console.log('Статусы загружены:', data); // Логируем загруженные статусы
            return data; // Возвращаем данные статусов
        })
        .catch(error => console.error('Ошибка при получении статусов:', error)); // Обработка ошибок
}

// Функция для добавления обработчиков клика на ячейки таблицы
function addClickEventToCells() {
    document.querySelectorAll('tbody tr').forEach(row => {
        row.querySelectorAll('td:not(:nth-child(1)):not(:nth-child(2))').forEach(cell => {
            cell.addEventListener('click', function () {
                fetchUnvisitedStatuses().then(statuses => {
                    const dropdown = document.createElement('select'); // Создаем выпадающий список
                    dropdown.className = 'form-select'; // Добавляем класс для стилей
                    statuses.forEach(status => {
                        const option = document.createElement('option'); // Создаем опцию для статуса
                        option.value = status.idUnvisitedStatus; // Устанавливаем значение
                        option.textContent = status.shortName; // Устанавливаем текст
                        dropdown.appendChild(option); // Добавляем опцию в выпадающий список
                    });
                    cell.innerHTML = ''; // Очищаем ячейку перед добавлением выпадающего списка
                    cell.appendChild(dropdown); // Добавляем выпадающий список в ячейку
                    dropdown.focus(); // Устанавливаем фокус на выпадающий список

                    // Обработчик изменения значения в выпадающем списке
                    dropdown.addEventListener('change', () => {
                        const selectedOption = dropdown.options[dropdown.selectedIndex];
                        cell.textContent = selectedOption.text; // Устанавливаем текст ячейки
                        cell.setAttribute('data-status-id', selectedOption.value); // Сохраняем id в атрибуте
                    });

                    // Обработчик потери фокуса
                    dropdown.addEventListener('blur', (event) => {
                        event.stopPropagation(); // Остановите всплытие события
                        const selectedOption = dropdown.options[dropdown.selectedIndex];
                        cell.textContent = selectedOption.text; // Устанавливаем текст ячейки
                        cell.setAttribute('data-status-id', selectedOption.value); // Сохраняем id в атрибуте
                    });

                    // Остановка всплытия события при клике на выпадающий список
                    dropdown.addEventListener('click', function (event) {
                        event.stopPropagation(); // Остановите всплытие события
                    });
                });
            });
        });
    });
}

// Обработчик для добавления столбцов с датами
document.getElementById('addDateButton').addEventListener('click', function () {
    const date = document.getElementById('dateInput').value; // Получаем значение даты
    // Извлекаем день и месяц из даты
    const dateObj = new Date(date);
    const day = dateObj.getDate();
    const month = dateObj.getMonth() + 1;

    if (date) {
        const tableHeader = document.querySelector('thead tr');
        const newHeaderCell = document.createElement('th'); // Создаем новый заголовок
        newHeaderCell.className = 'border-b dynamic-column'; // Добавляем класс для динамических столбцов
        newHeaderCell.textContent = `${day}/${month}`;
        newHeaderCell.value = date;
        // Устанавливаем текст заголовка
        tableHeader.appendChild(newHeaderCell); // Добавляем новый заголовок в таблицу

        // Добавляем новый столбец в каждую строку таблицы
        const rows = document.querySelectorAll('tbody tr');
        rows.forEach(row => {
            const newCell = document.createElement('td'); // Создаем новую ячейку
            newCell.className = 'border-b dynamic-column'; // Добавляем класс для динамических ячеек

            row.appendChild(newCell); // Добавляем новую ячейку в строку
        });

        // Закрываем модальное окно после добавления столбца
        $('#dateModal').modal('hide');
        document.getElementById('dateInput').value = ''; // Очищаем поле ввода даты

        // Добавляем обработчики клика на новые ячейки
        addClickEventToCells();
    } else {
        alert("Пожалуйста, выберите дату."); // Предупреждение, если дата не выбрана
    }
});

// Обработчик для сохранения записей
document.getElementById('saveEntries').addEventListener('click', function () {
    const journalEntries = []; // Массив для хранения записей журнала

    // Проходим по всем строкам таблицы
    document.querySelectorAll('tbody tr').forEach(row => {
        const idStudent = row.querySelector('.student-name').dataset.studentId; // Получаем ID студента

        // Проходим по всем ячейкам в строке, кроме первых двух
        row.querySelectorAll('td:not(:nth-child(1)):not(:nth-child(2))').forEach((cell, index) => {
            // Получаем idUnvisitedStatus из атрибута data-status-id ячейки
            const idUnvisitedStatus = cell.getAttribute('data-status-id'); // Получаем значение id из атрибута
            const lessonDate = document.querySelector(`thead tr th:nth-child(${index + 3})`).value; // Получаем дату из заголовка

            // Проверяем, заполнены ли все необходимые поля
            if (idStudent) {
                journalEntries.push({
                    LessonDate: lessonDate, // Дата урока
                    IdGroup: document.querySelector('.group-name.highlight').dataset.groupId, // Получаем ID группы
                    IdStudent: idStudent, // ID студента
                    IdUnvisitedStatus: idUnvisitedStatus ? parseInt(idUnvisitedStatus) : null // Преобразуем в число или null
                });
            }
        });
    });

    // Проверяем, есть ли записи для сохранения
    if (journalEntries.length === 0) {
        alert('Нет данных для сохранения.'); // Предупреждение, если нет данных
        return;
    }

    console.log(journalEntries);
    // Отправка данных на сервер
    fetch('https://localhost:7022/api/Journals/CreateJournal', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(journalEntries)
    })
        .then(response => {
            if (!response.ok) {
                return response.json().then(errData => {
                    throw new Error(`Сетевая ошибка: ${response.status} ${response.statusText} - ${JSON.stringify(errData)}`);
                });
            }
            return response.json();
        })
        .then(data => {
            console.log('Записи успешно сохранены:', data);
            alert('Записи успешно сохранены!');
            document.querySelector('.group-name.highlight').click();
        })
        .catch(error => {
            console.error('Ошибка при сохранении записей:', error);
            alert('Ошибка при сохранении записей.');
        });
});

// Обработчик для кнопки "Экспорт"
document.getElementById('exportEntries').addEventListener('click', function () {
    const groupId = document.querySelector('.group-name.highlight').dataset.groupId; // Получаем ID текущей группы
    if (!groupId) {
        console.error('Group ID is undefined'); // Проверяем наличие ID
        return;
    }

    // Формируем URL для запроса Excel-файла
    const exportUrl = `https://localhost:7022/api/export/journalsExport/${groupId}`;

    // Выполняем запрос на сервер
    fetch(exportUrl)
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok'); // Проверяем успешность ответа
            return response.blob(); // Преобразуем ответ в Blob
        })
        .then(blob => {
            // Создаем URL для Blob и скачиваем файл
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = 'attendance_data.xlsx'; // Имя файла для скачивания
            document.body.appendChild(a);
            a.click(); // Имитируем клик для скачивания
            a.remove(); // Удаляем элемент после скачивания
            window.URL.revokeObjectURL(url); // Освобождаем память
        })
        .catch(error => {
            console.error('Ошибка при экспорте данных:', error); // Обработка ошибок
            alert('Ошибка при экспорте данных.');
        });
});