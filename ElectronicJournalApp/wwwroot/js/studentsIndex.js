// Получаем значение cookie UserLogin
const userLogin = getCookie("login");
if (!userLogin) {
    console.error('UserLogin cookie not found'); // Если cookie не найден, выводим ошибку
}

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
        if (!response.ok) throw new Error('Network response was not ok');
        return response.json();
    })
    .then(data => {
        const subjectsList = document.getElementById('subjects-list');
        const subjectButtons = []; // Массив для хранения кнопок предметов

        data.forEach(subject => {
            const subjectItem = document.createElement('li');
            subjectItem.className = 'mb-1';
            const subjectButton = document.createElement('button');
            subjectButton.className = 'btn-custom w-full text-left p-2 font-bold bg-[#1B9AAA] rounded subject-name';
            subjectButton.textContent = subject.name;

            const groupList = document.createElement('ul');
            groupList.className = 'ml-4 mt-1 group-list';
            groupList.innerHTML = subject.groups.map(group => `
                <li class="mb-1">
                    <button class="block w-full text-left p-2 group-name rounded" data-group-id="${group.idGroup}">${group.name}</button>
                </li>
            `).join('');

            subjectsList.appendChild(subjectItem);
            subjectItem.appendChild(subjectButton);
            subjectItem.appendChild(groupList);
            subjectButtons.push(subjectButton); // Добавляем кнопку предмета в массив
        });

        // Добавляем обработчики событий для кнопок предметов
        subjectButtons.forEach(subjectElement => {
            subjectElement.addEventListener('click', function () {
                this.nextElementSibling.classList.toggle('active');
            });
        });

        // Добавляем обработчики событий для кнопок групп
        const groupButtons = document.querySelectorAll('.group-name');
        groupButtons.forEach(groupElement => {
            groupElement.addEventListener('click', function () {
                const groupId = this.dataset.groupId;
                console.log(groupId);
                if (!groupId) {
                    console.error('Group ID is undefined');
                    return;
                }

                // Убираем выделение у всех групп и выделяем текущую
                document.querySelectorAll('.group-name').forEach(el => el.classList.remove('highlight'));
                this.classList.add('highlight');

                // Запрашиваем студентов по ID группы
                fetch(`https://localhost:7022/api/students/group/${groupId}`)
                    .then(response => {
                        if (!response.ok) throw new Error('Network response was not ok');
                        return response.json();
                    })
                    .then(students => {
                        const studentsList = document.getElementById('students-list');
                        studentsList.innerHTML = ''; // Очищаем список студентов

                        // Проверяем, есть ли записи в расписании
                        if (students.length === 0) {
                            const row = document.createElement('tr');
                            row.innerHTML = `<td colspan="6" class="py-2 px-4 border-b text-center">Нет записей для этой группы</td>`;
                            studentsList.appendChild(row); // Добавляем строку с сообщением
                            return; // Если записей нет, выходим
                        }

                        console.log(students);
                        students.forEach((student, index) => {
                            const studentRow = document.createElement('tr');
                            studentRow.innerHTML = `
                                <td>${index + 1}</td>
                                <td>${student.surname}</td>
                                <td>${student.name}</td>
                                <td>${student.patronymic || ''}</td>
                                <td>${student.phone}</td>
                                <td>${student.parentPhone}</td>
                            `;
                            studentsList.appendChild(studentRow);
                        });
                    })
                    .catch(error => {
                        console.error('Ошибка при получении студентов:', error);
                    });
            });
        });

        // Открываем первый предмет и первую группу по умолчанию
        if (data.length > 0 && data[0].groups.length > 0) {
            const firstSubjectButton = subjectButtons[0]; // Получаем первую кнопку предмета
            firstSubjectButton.click(); // Вызываем событие клика на первом предмете

            const firstGroupButton = groupButtons[0]; // Получаем первую группу
            firstGroupButton.click(); // Вызываем событие клика на первой группе
        }
    })
    .catch(error => {
        console.error('Ошибка при получении предметов:', error);
    });
