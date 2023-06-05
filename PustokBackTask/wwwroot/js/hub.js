var connection = new signalR.HubConnectionBuilder().withUrl("/pustokHub").build();

connection.start();

connection.on("SetOnline", function (id) {
    $("#usersTable").find(`[data-id=` + id + `] span`).removeClass("dot-offline").addClass("dot-online");
})
connection.on("SetOffline", function (id) {
    $("#usersTable").find(`[data-id=` + id + `] span`).removeClass("dot-online").addClass("dot-offline");
})

connection.on("OrderAccepted", function () {
    toastr["success"]("Your order is accepted");
})
connection.on("OrderRejected", function () {
    toastr["error"]("Your order is rejected!");
})