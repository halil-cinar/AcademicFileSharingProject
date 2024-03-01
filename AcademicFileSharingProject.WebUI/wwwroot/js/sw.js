// SignalR hub'�na ba�lan
// Grup ID'sini localStorage'den okuma
function getGroupId() {
    return localStorage.getItem("groupId");
}

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();
// SignalR ba�lant�s�n� olu�tur
var connection = $.hubConnection();
var hubProxy = connection.createHubProxy('BildirimHub');

// Gruba kat�l
var groupId = getGroupId(); // Taray�c�ya �zel bir grup kimli�i
if (groupId != null && groupId != "") {
    
    hubProxy.on('ReceiveNotification', function (message) {
        console.log('Bildirim al�nd�:', message);
        showNotification(message)
    });


    connection.start().done(function () {
        hubProxy.invoke('BildirimGonder', groupId, 'Merhaba, bu bir bildirim!');
    });

}
function showNotification(message) {
    var options = {
        body: message,
        icon: 'path/to/icon.png',
        badge: 'path/to/badge.png'
    };

    self.registration.showNotification('Ba�l�k Burada', options);
}