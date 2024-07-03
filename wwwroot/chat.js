"use strict";

document.addEventListener('DOMContentLoaded', function() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub", {
        accessTokenFactory: () => "YOUR_AUTH_TOKEN_HERE"
    }).build();
    var currentRoom = '';

    connection.on("ReceiveMessage", function (username, message) {
        var messageItem = document.createElement("li");
        messageItem.textContent = `${username}: ${message}`; 
        var conversationList = document.querySelector("#conversationList");
        conversationList.prepend(messageItem);
    });

    connection.on("RoomCreated", function (roomName, minimumAge, country) {
        updateRoomList(roomName, minimumAge, country);
    });

    connection.start().then(function () {
        console.log("SignalR Connected");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    document.querySelector("#createRoomButton").addEventListener("click", function (event) {
        var roomName = document.querySelector("#roomNameInput").value;
        var minimumAge = document.querySelector("#minimumAgeInput").value;
        var country = document.querySelector("#countryInput").value;

        connection.invoke("CreateRoom", roomName, minimumAge, country)
        .catch(function (err) {
            updateRoomStatus('Failed to create room', 'error');
            console.error(err.toString());
        });
        event.preventDefault();
    });

    document.querySelector("#joinRoomButton").addEventListener("click", function (event) {
        var roomName = document.querySelector("#roomInput").value;
        if (roomName.trim() === '') {
            updateRoomStatus('Please enter a room name', 'error');
            return;
        }

        connection.invoke("JoinRoom", roomName)
        .then(function() {
            currentRoom = roomName;
            updateRoomStatus(`Joined room: ${roomName}`, 'success');
            document.querySelector("#roomInput").classList.add('joined');
        })
        .catch(function (err) {
            updateRoomStatus('Failed to join room', 'error');
            console.error(err.toString());
        });
        event.preventDefault();
    });

    document.querySelector("#sendButton").addEventListener("click", function (event) {
        var message = document.querySelector("#messageInput").value;
        
        if (currentRoom === '') {
            updateRoomStatus('Please join a room before sending messages', 'error');
            return;
        }

        connection.invoke("SendMessage", message)
        .catch(function (err) {
            updateRoomStatus('Failed to send message', 'error');
            console.error(err.toString());
        });
        event.preventDefault();
    });

    function updateRoomStatus(message, status) {
        var statusElement = document.querySelector("#roomStatus");
        statusElement.textContent = message;
        statusElement.className = status;
    }

    function updateRoomList(roomName, minimumAge, country) {
        var roomList = document.querySelector("#roomList");
        var roomItem = document.createElement("li");
        roomItem.textContent = `${roomName} (Age: ${minimumAge}+, Country: ${country || 'Any'})`;
        roomList.appendChild(roomItem);
    }
});