// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Grup ID'sini localStorage'de saklama
function setGroupId(groupId) {
    localStorage.setItem("groupId", groupId);
}

// Grup ID'sini localStorage'den okuma
function getGroupId() {
    return localStorage.getItem("groupId");
}

// Grup ID'sini localStorage'den silme
function deleteGroupId() {
    localStorage.removeItem("groupId");
}


function requestNotificationPermission() {
    if (Notification.permission !== 'granted') {
        Notification.requestPermission().then(function (permission) {
            if (permission === 'granted') {
                new Notification('Bildirim Gönderme İzniniz Alınmıştır. İptal işlemini tarayıcınızın ayarlarından yapabilirsiniz');
            } else {
                console.log('Kullanıcı bildirim izni vermedi.');
            }
        });
    } else {
        //İzin varsa

    }
}



//Service worker kayıt işlemi   
if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        navigator.serviceWorker.register('/js/sw.js').then(function (registration) {
            console.log('Servis işçisi kaydedildi:', registration);
        }, function (err) {
            console.log('Servis işçisi kaydı başarısız oldu:', err);
        });
    });
}
