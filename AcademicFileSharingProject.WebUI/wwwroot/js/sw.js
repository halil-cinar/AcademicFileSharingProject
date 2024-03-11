// SignalR hub'�na ba�lan
// Grup ID'sini localStorage'den okuma
//importScripts("../lib/jquery/dist/jquery.min.js")
importScripts("../microsoft/signalr/dist/browser/signalr.js")
function getGroupId() {
    return localStorage.getItem("groupId");
}

var connection = new signalR.HubConnectionBuilder().withUrl("http://localhost:26823/connection").build();

// Gruba kat�l
var groupId = getGroupId(); // Taray�c�ya �zel bir grup kimli�i
if (groupId != null && groupId != "") {

    
    connection.on('ReceiveNotification', function (message) {
        console.log('Bildirim al�nd�:', message);

        showNotification(message)
    });


    connection.start().done(function () {
        hubProxy.invoke('RegisterService', groupId);
    });

}
function showNotification(message) {
    var options = {
        body: message,
        icon: 'path/to/icon.png',
        badge: 'path/to/badge.png'
    };

    self.registration.showNotification('Bir bildiriminiz var ', options);
}

