const userRole = getCookie("role");
console.log("userRole", userRole);

if (userRole === 'руководитель' || userRole === 'администратор') {
    document.getElementById('SchedulesEditButton').style.display = 'block';
} else {
    document.getElementById('SchedulesEditButton').style.display = 'none';
}

function deleteSubject(subjectId) {
    console.log(`Deleting subject with ID: ${subjectId}`); // Для отладки
    fetch(`https://localhost:7022/api/Subjects/DeleteSubject/${subjectId}`, {
        method: 'DELETE'
    })
        .then(response => {
            if (response.ok) {
                const subjectRow = document.getElementById(`subject-row-${subjectId}`);
                if (subjectRow) {
                    subjectRow.remove(); // Удаляем строку из DOM
                }
            } else if (response.status === 404) {
                alert('Предмет не найден');
            } else {
                alert('Ошибка при удалении предмета');
            }
        })
        .catch(error => {
            console.error('Ошибка:', error);
            alert('Произошла ошибка при удалении предмета');
        });
}