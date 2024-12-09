﻿        // Получаем значение cookie UserLogin
        const userLogin = document.cookie.split('; ').find(row => row.startsWith('UserLogin='))?.split('=')[1];
    if (!userLogin) console.error('UserLogin cookie not found'); // Если cookie не найден, выводим ошибку

    document.getElementById('closeModal').addEventListener('click', function () {
        document.getElementById('dateModal').style.display = 'none'; // Закрываем модальное окно
        });

    // Запрашиваем данные предметов для учителя по UserLogin
    fetch(`https://localhost:7022/api/subjects/teacher/${userLogin}`)
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
            document.getElementById('add-entry').style.display = 'block'; // Показываем кнопку добавления записи
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

                    // Создаем заголовки для уникальных дат
                    const tableHeader = document.querySelector('thead tr');
                    lessonDates.forEach(date => {
                        const newHeaderCell = document.createElement('th'); // Создаем новый заголовок
                        newHeaderCell.className = 'py-2 px-4 border-b'; // Добавляем классы для стилей
                        newHeaderCell.textContent = date; // Устанавливаем текст заголовка
                        tableHeader.appendChild(newHeaderCell); // Добавляем новый заголовок в таблицу
                    });

                    // Создаем строки для студентов
                    students.forEach((student, index) => {
                        const row = document.createElement('tr');
                        row.innerHTML = `<td class="py-2 px-2 border-b">${index + 1}</td><td class="py-2 px-4 border-b student-name" data-student-id="${student.idStudent}">${student.name} ${student.surname}</td>`;

                        // Добавляем ячейки для статусов по датам
                        lessonDates.forEach(date => {
                            const journalEntry = student.journals.find(j => j.lessonDate === date);
                            const statusCell = document.createElement('td');
                            statusCell.className = 'py-2 px-4 border-b';

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
    .catch(console.error); // Обработка ошибок

    // Функция для очистки столбцов с датами
    function clearDateColumns() {
            const tableHeader = document.querySelector('thead tr');
            tableHeader.querySelectorAll('th:not(:nth-child(1)):not(:nth-child(2))').forEach(header => header.remove()); // Удаляем заголовки дат
            document.querySelectorAll('tbody tr').forEach(row => {
        row.querySelectorAll('td:not(:nth-child(1)):not(:nth-child(2))').forEach(cell => cell.remove()); // Удаляем ячейки с датами
            });
        }

    // Обработчик для кнопки "Добавить запись"
    document.getElementById('add-entry').addEventListener('click', function () {
        // Показываем модальное окно для добавления даты
        document.getElementById('dateModal').style.display = 'block';
        });

    // Функция для получения статусов посещаемости
    function fetchUnvisitedStatuses() {
            return fetch('https://localhost:7022/api/unvisitedstatus')
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
    document.getElementById('submitDate').addEventListener('click', function () {
            const date = document.getElementById('entryDate').value; // Получаем значение даты
    if (date) {
                const tableHeader = document.querySelector('thead tr');
    const newHeaderCell = document.createElement('th'); // Создаем новый заголовок
    newHeaderCell.className = 'py-2 px-4 border-b'; // Добавляем классы для стилей
    newHeaderCell.textContent = date; // Устанавливаем текст заголовка
    tableHeader.appendChild(newHeaderCell); // Добавляем новый заголовок в таблицу

    // Добавляем новый столбец в каждую строку таблицы
    const rows = document.querySelectorAll('tbody tr');
                rows.forEach(row => {
                    const newCell = document.createElement('td'); // Создаем новую ячейку
    newCell.className = 'py-2 px-4 border-b'; // Добавляем классы для стилей
    newCell.textContent = ''; // Здесь можно установить значение по умолчанию, если нужно
    row.appendChild(newCell); // Добавляем новую ячейку в строку
                });

    // Закрываем модальное окно после добавления столбца
    document.getElementById('dateModal').style.display = 'none';
    document.getElementById('entryDate').value = ''; // Очищаем поле ввода даты

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
    const lessonDate = document.querySelector(`thead tr th:nth-child(${index + 3})`).textContent; // Получаем дату из заголовка

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
                })
                .catch(error => {
                    console.error('Ошибка при сохранении записей:', error);
                alert('Ошибка при сохранении записей.');
                });
        });