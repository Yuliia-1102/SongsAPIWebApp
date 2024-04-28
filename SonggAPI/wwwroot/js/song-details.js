window.onload = function () {
    const queryParams = new URLSearchParams(window.location.search);
    const songId = queryParams.get('id');
    fetchSongDetails(songId);
};

async function fetchSongDetails(songId) {
    const response = await fetch(`api/Songs/${songId}/details`);
    if (!response.ok) {
        document.getElementById('songDetails').innerHTML = 'Помилка: Пісню не знайдено.';
        return;
    }
    const data = await response.json();
    displaySongDetails(data);
}

function displaySongDetails(data) {
    const song = data.song;
    const isPurchased = data.isPurchased;
    const details = document.getElementById('songDetails');
    let htmlContent = `
        <p>Назва пісні: ${song.name}</p>
        <p>Ціна: ${song.price} грн</p>
        <p>Жанр: ${song.genre.name}</p>
        <div>Виконавці: ${song.singersSongs.map(s => s.singer.name).join(', ')}</div>
    `;

    if (!isPurchased) {
        htmlContent += `<button onclick="purchaseSong(${song.id})" data-song-id="${song.id}">Купити</button>`;
    }

    details.innerHTML = htmlContent;
}


function purchaseSong(songId) {
    const modal = document.getElementById('purchaseModal');
    modal.style.display = "block";

    const closeBtn = document.querySelector('.close');
    closeBtn.onclick = function () {
        modal.style.display = "none";
    };

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = "none";
        }
    };

    const form = document.getElementById('purchaseForm');
    form.onsubmit = async function (event) {
        event.preventDefault();
        const cardNumber = document.getElementById('cardNumber').value;

        if (cardNumber.length !== 16 || !/^\d+$/.test(cardNumber)) {
            alert('Номер карти має містити рівно 16 цифр.');
            return;
        }

        try {
            const response = await fetch(`api/Songs/purchase/${songId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({ cardNumber: cardNumber })
            });

            if (response.ok) {
                alert("Пісня куплена.");
                modal.style.display = "none";
                document.querySelector(`button[data-song-id="${songId}"]`).remove(); 
            } else {
                const result = await response.json();
                alert(result.message || 'Не вдалося придбати пісню.');
            }
        } catch (error) {
            console.error('Помилка:', error);
            alert('Сталася помилка при спробі купівлі пісні.');
        }
    };
}





