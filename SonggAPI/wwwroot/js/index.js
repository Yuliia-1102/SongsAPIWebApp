// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
window.onload = function () {
    fetchSongs();
};

async function fetchSongs() {
    const response = await fetch('api/Songs');
    const songs = await response.json();
    displaySongs(songs);
}

function displaySongs(songs) {
    const container = document.getElementById('songs');
    container.innerHTML = '';

    songs.forEach(song => {
        const songElement = document.createElement('div');
        songElement.style.display = 'flex';
        songElement.style.alignItems = 'center'; 
        songElement.style.marginBottom = '10px'; 
        songElement.innerHTML = `   
            <h3>${song.name} - ${song.price} грн</h3>
            <button onclick="details(${song.id})">Детальніше</button>
        `;

        container.appendChild(songElement);
    });
} 

function details(songId) {
    window.location.href = `details.html?id=${songId}`;
}

