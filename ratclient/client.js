var button = document.querySelector("#send"),
    connectbutton = document.querySelector("#connect"),
    output = document.querySelector("#output"),
    userlist = "",
    currentcommand = "newuser",
    currentfield = "#user",
    awaitingusers = 0
    wsUri = ("ws://" + document.getElementById("ip").value + ":1337/PSCommand"),
    websocket = new WebSocket(wsUri);

// Universal buttons to connect to web socket server
send.addEventListener("click", onClickSend);
connectbutton.addEventListener("click", onClickManualConnect);
$('form').hide()
// Universal buttons to connect to web socket server

// web socket functions //
websocket.addEventListener('open', (event) => {
    writeToScreen("Verbunden");
});

function onClickConnect() {
	wsUri = ("ws://" + document.getElementById("ip").value + ":1337/PSCommand");
    websocket = WebSocket(wsUri);
}

websocket.onclose = function (e) {
    writeToScreen("Verbindung geschlossen.");
};

websocket.onmessage = function (e) {
    if (awaitingusers == 1) {
        userlist = e.data;
        awaitingusers = 0
        updateUserList()
    }
    else
    {
        writeToScreen("<span>Antwort: " + e.data + "</span>");
    }
};

websocket.onerror = function (e) {
    writeToScreen("<span class=error>ERROR:</span> " + e.data);
};

function doSend(message) {
    if (!message.includes("getuser/")) { writeToScreen("SENT: " + message); }
    websocket.send(message);
}

function writeToScreen(message) {
    output.insertAdjacentHTML("afterbegin", "<p>" + message + "</p>");
}
// web socket functions //

// buttons for sending and reconnecting //
function onClickSend() {
    var text = currentcommand + "/";
    $(currentfield + ' .value').each(function () {
        text += this.value + "/";
    });
    $(currentfield + ' .checkedvalue').each(function () {
        if ($(this).prop('checked')) {
            text += this.value 
        }
        text += "/"

    });
    text && doSend(text);
}
function onClickManualConnect() {
    websocket = new WebSocket(wsUri);
}
// buttons for sending and reconnecting //
function chooseNewUser() {
    $(currentfield).hide();
    currentfield = "#newuser";
    $(currentfield).toggle();
    currentcommand = "newuser";
}

function chooseEditUser() {
    GetUsers();
    $(currentfield).hide();
    currentfield = "#edituser";
    $(currentfield).toggle();
    currentcommand = "edituser";
}

function chooseDeleteUser() {
    GetUsers();
    $(currentfield).hide();
    currentfield = "#deleteuser";
    $(currentfield).toggle();
    currentcommand = "deleteuser";
}

function chooseCustom() {
    $(currentfield).hide();
    currentfield = "#custom";
    $(currentfield).toggle();
    currentcommand = "custom";
}

function GetUsers() {
    awaitingusers = 1;
    doSend("getuser/");
}
function updateUserList() {
    var select = $('[id=userlist]');
    for (let element = 0; element < select.length; element++) {
        var child = select[element].lastElementChild;
        while (child)
        {
            select[element].removeChild(child);
            child = select[element].lastElementChild;
        }
        for (let user = 0; user < userlist.split('\n').length - 1; user++) {
            var opt = document.createElement('option');
            opt.value = userlist.split('\n')[user];
            opt.innerHTML = userlist.split('\n')[user];
            select[element].appendChild(opt);
        }
    }
}