function deleteSubject(subjectId) {
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