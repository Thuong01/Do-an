﻿"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

$(function () {
	connection.start().then(function () {
		alert('Connected to notificationHub');

		InvokeNotifications();

	}).catch(function (err) {
		return console.error(err.toString());
	});
});

function InvokeNotifications() {
	debugger;
	connection.invoke("SendNotification").catch(function (err) {
		return console.error(err.toString());
	});
}

connection.on("ReceiveNotification", function (notis) {
	BindNotificationsToGrid(notis);
});

function BindNotificationsToGrid(notis) {
	console.log(notis);
}