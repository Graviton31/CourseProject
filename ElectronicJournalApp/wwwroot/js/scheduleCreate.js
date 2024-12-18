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
                console.log(groupId)
                if (!groupId) {
                    console.error('Group ID is undefined'); // Проверяем наличие ID
                    return; // Выходим, если ID не найден
                }

                // Убираем выделение у всех групп и выделяем текущую
                document.querySelectorAll('.group-name').forEach(el => el.classList.remove('highlight'));
                this.classList.add('highlight');
                document.getElementById('add-entry').style.display = 'block'; // Показываем кнопку добавления записи

                // Функция для преобразования номера дня недели в название
                function getDayName(dayNumber) {
                    const days = [
                        'Понедельник', // 1
                        'Вторник',     // 2
                        'Среда',       // 3
                        'Четверг',     // 4
                        'Пятница',     // 5
                        'Суббота',     // 6
                    ];
                    return days[dayNumber - 1] || 'Неизвестный день'; // Возвращаем название дня или 'Неизвестный день'
                }

                // Запрашиваем расписание группы
                fetch(`https://localhost:7022/api/schedules/group/${groupId}`)
                    .then(response => {
                        if (!response.ok) {
                            console.error('Ошибка при получении расписания:', response.status, response.statusText);
                            throw new Error('Network response was not ok');
                        }
                        return response.json(); // Преобразуем ответ в JSON
                    })
                    .then(data => {
                        const tbody = document.querySelector('tbody'); // Получаем тело таблицы

                        // Очищаем текущее содержимое
                        tbody.innerHTML = ''; // Очищаем текущее содержимое

                        // Проверяем, есть ли записи в расписании
                        if (data.length === 0) {
                            const row = document.createElement('tr');
                            row.innerHTML = `<td colspan="5" class="py-2 px-4 border-b text-center">Нет записей для этой группы</td>`;
                            tbody.appendChild(row); // Добавляем строку с сообщением
                            return; // Если записей нет, выходим
                        }

                        data.forEach((schedule, index) => {
                            const row = document.createElement('tr');
                            row.innerHTML = `
                                <td class="py-2 px-2 border-b">${index + 1}</td>
                                <td class="py-2 px-4 border-b">${getDayName(schedule.weekDay)}</td>
                                <td class="py-2 px-4 border-b">${schedule.startTime}</td>
                                <td class="py-2 px-4 border-b">${schedule.endTime}</td>
                                <td class="py-2 px-4 border-b">
                                <button class="btn btn-danger btn-sm delete-entry" data-id="${schedule.idSchedule}">
                                    <i class="fas fa-trash-alt me-1"></i> Удалить
                                </button>
                                </td>
                            `;
                            tbody.appendChild(row); // Добавляем строку в таблицу
                        });

                        // Обработчик для кнопок удаления
                        document.querySelectorAll('.delete-entry').forEach(button => {
                            button.addEventListener('click', function () {
                                const entryId = this.dataset.id; // Получаем ID записи
                                if (confirm('Вы уверены, что хотите удалить эту запись?')) {
                                    // Отправляем запрос на удаление записи
                                    fetch(`https://localhost:7022/api/schedules/delete/${entryId}`, {
                                        method: 'DELETE'
                                    })
                                        .then(response => {
                                            if (!response.ok) throw new Error('Network response was not ok');
                                            return response.json();
                                        })
                                        .then(data => {
                                            console.log('Запись удалена:', data);
                                            // Обновляем расписание
                                            document.querySelector('.group-name.highlight').click(); // Перезагружаем расписание для текущей группы
                                        })
                                        .catch(error => {
                                            console.error('Ошибка при удалении записи:', error);
                                        });
                                }
                            });
                        });

                    })
                    .catch(error => {
                        console.error('Ошибка при получении расписания:', error); // Обработка ошибок
                    });
            });
        });
    })
    .catch(error => {
        console.error('Ошибка при получении предметов:', error); // Обработка ошибок
    });

// Обработчик для кнопки "Добавить запись"
document.getElementById('add-entry').addEventListener('click', function () {
    // Показываем модальное окно для добавления даты
    $('#scheduleModal').modal('show');
});

// Обработчик для кнопки "Закрыть модальное окно"
document.getElementById('closeModal').addEventListener('click', function () {
    // Закрываем модальное окно
    $('#scheduleModal').modal('hide');
});

// Обработчик для кнопки "Добавить" в модальном окне
document.getElementById('addDateButton').addEventListener('click', function () {
    const dayOfWeek = document.getElementById('dayOfWeek').value; // Убедитесь, что это значение соответствует типу sbyte
    const startTime = document.getElementById('startTime').value; // Убедитесь, что это значение в формате TimeOnly
    const endTime = document.getElementById('endTime').value; // Убедитесь, что это значение в формате TimeOnly

    // Добавляем секунды, если нужно
    const formattedStartTime = `${startTime}:00`; // "01:12:00"
    const formattedEndTime = `${endTime}:00`; // "03:14:00"


    console.log(dayOfWeek, formattedStartTime, formattedEndTime);
    if (!dayOfWeek || !startTime || !endTime) {
        alert('Пожалуйста, заполните все поля.');
        return;
    }

    // Проверка, что время начала меньше времени окончания
    if (startTime >= endTime) {
        alert('Время начала должно быть меньше времени окончания.');
        return;
    }

    // Получаем ID выделенной группы
    const groupId = document.querySelector('.group-name.highlight')?.dataset.groupId;
    if (!groupId) {
        alert('Пожалуйста, выберите группу.');
        return;
    }
    console.log(groupId);

    // Отправляем данные на сервер
    fetch(`https://localhost:7022/api/schedules/create`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            WeekDay: parseInt(dayOfWeek), // Преобразуем в целое число
            StartTime: formattedStartTime, // Убедитесь, что это в правильном формате
            EndTime: formattedEndTime, // Убедитесь, что это в правильном формате
            IdGroup: parseInt(groupId) // Преобразуем в целое число
        })
    })
        .then(response => {
            if (!response.ok) throw new Error('Network response was not ok');
            return response.json();
        })
        .then(data => {
            console.log('Запись добавлена:', data);
            $('#scheduleModal').modal('hide'); // Закрываем модальное окно

            // Обновляем расписание
            document.querySelector('.group-name.highlight').click(); // Перезагружаем расписание для текущей группы
        })
        .catch(error => {
            console.error('Ошибка при добавлении записи:', error);
        });
});
