// SignalR hub'ýna baðlan
// Grup ID'sini localStorage'den okuma
function getGroupId() {
    return localStorage.getItem("groupId");
}

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();
// SignalR baðlantýsýný oluþtur
var connection = $.hubConnection();
var hubProxy = connection.createHubProxy('BildirimHub');

// Gruba katýl
var groupId = getGroupId(); // Tarayýcýya özel bir grup kimliði
if (groupId != null && groupId != "") {
    
    hubProxy.on('ReceiveNotification', function (message) {
        console.log('Bildirim alýndý:', message);
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

    self.registration.showNotification('Baþlýk Burada', options);
}