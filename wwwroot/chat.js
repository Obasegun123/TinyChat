// chat.js

class ChatClient {
    constructor() {
        this.connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();
        this.currentRoom = '';
        this.initialize();
    }

    initialize() {
        this.connection.start().then(() => {
            console.log("SignalR Connected");
            this.loadAvailableRooms();
        }).catch(err => console.error(err.toString()));

        this.connection.on("ReceiveMessage", this.receiveMessage.bind(this));
        this.connection.on("RoomCreated", this.roomCreated.bind(this));

        document.getElementById("createRoomButton").addEventListener("click", this.createRoom.bind(this));
        document.getElementById("joinRoomButton").addEventListener("click", this.joinRoom.bind(this));
        document.getElementById("sendButton").addEventListener("click", this.sendMessage.bind(this));
    }

    async loadAvailableRooms() {
        try {
            const rooms = await this.connection.invoke("GetAllRooms");
            const roomList = document.getElementById("roomList");
            roomList.innerHTML = '';
            rooms.forEach(room => {
                this.addRoomToList(room.name, room.minimumAge, room.country);
            });
        } catch (err) {
            console.error(err);
        }
    }

    async createRoom(event) {
        event.preventDefault();
        const roomName = document.getElementById("roomNameInput").value;
        const minimumAge = document.getElementById("minimumAgeInput").value;
        const country = document.getElementById("countryInput").value;

        try {
            await this.connection.invoke("CreateRoom", roomName, parseInt(minimumAge), country);
            this.updateRoomStatus(`Room ${roomName} created successfully`, 'success');
        } catch (err) {
            this.updateRoomStatus('Failed to create room', 'error');
            console.error(err);
        }
    }

    async joinRoom(event) {
        event.preventDefault();
        const roomName = document.getElementById("roomInput").value;
        if (roomName.trim() === '') {
            this.updateRoomStatus('Please enter a room name', 'error');
            return;
        }

        try {
            await this.connection.invoke("JoinRoom", roomName);
            this.currentRoom = roomName;
            this.updateRoomStatus(`Joined room: ${roomName}`, 'success');
            document.getElementById("roomInput").classList.add('joined');
        } catch (err) {
            this.updateRoomStatus('Failed to join room', 'error');
            console.error(err);
        }
    }

    async sendMessage(event) {
        event.preventDefault();
        const message = document.getElementById("messageInput").value;
        
        if (this.currentRoom === '') {
            this.updateRoomStatus('Please join a room before sending messages', 'error');
            return;
        }

        try {
            await this.connection.invoke("SendMessage", message);
            document.getElementById("messageInput").value = '';
        } catch (err) {
            this.updateRoomStatus('Failed to send message', 'error');
            console.error(err);
        }
    }

    receiveMessage(username, message) {
        const messageItem = document.createElement("li");
        messageItem.textContent = `${username}: ${message}`; 
        const conversationList = document.getElementById("conversationList");
        conversationList.prepend(messageItem);
    }

    roomCreated(roomName, minimumAge, country) {
        this.addRoomToList(roomName, minimumAge, country);
    }

    addRoomToList(roomName, minimumAge, country) {
        const roomList = document.getElementById("roomList");
        const roomItem = document.createElement("li");
        roomItem.textContent = `${roomName} (Age: ${minimumAge}+, Country: ${country || 'Any'})`;
        roomList.appendChild(roomItem);
    }

    updateRoomStatus(message, status) {
        const statusElement = document.getElementById("roomStatus");
        statusElement.textContent = message;
        statusElement.className = status;
    }
}

// Initialize the chat client when the DOM is fully loaded
document.addEventListener('DOMContentLoaded', () => {
    new ChatClient();
});