var buttons = {
	gameMode0: null,
	gameMode1: null,
	gameMode2: null,
	readySplash: null,
	ready: null,
	readyLeft: null,
	readyRight: null,
	//readyTeam: null,
    gameMoveLeft: null,
	gameMoveRight: null,
	gameShootLeft: null,
	gameShootRight: null
}; 
var healthPercent = 80; //includes initial styling, so 100% is 100% of the screen height/width
var totalLives = 5;
var lives = totalLives;
var healthBar = document.getElementById("healthBar");
var gameMode = document.getElementById("gameMode");
var welcomeText = document.getElementById("welcomeText");

//controller color in hex without the symbol.
var controllerColor = "FFFFFF";

var controllers = {
	lobby: document.getElementById("lobbyController"),
	splash: document.getElementById("splashController"),
	menu: document.getElementById("menuController"),
	game: document.getElementById("gameController"),
	dead: document.getElementById("deadController")
};

init = function() {
	buttons.gameMode0 = document.getElementById("gameMode0");
	buttons.gameMode1 = document.getElementById("gameMode1");
	buttons.gameMode2 = document.getElementById("gameMode2");
	buttons.readySplash = document.getElementById("readySplash");
	buttons.ready = document.getElementById("ready");
	buttons.readyLeft = document.getElementById("readyLeft");
	buttons.readyRight = document.getElementById("readyRight");
	//buttons.readyTeam = document.getElementById("readyTeam");
	buttons.gameMoveLeft = document.getElementById("gameMoveLeft");
    buttons.gameMoveRight = document.getElementById("gameMoveRight");
	buttons.gameShootLeft = document.getElementById("gameShootLeft");
	buttons.gameShootRight = document.getElementById("gameShootRight");	
	
	for (c in controllers) {
		controllers[c].style.display = "none";
	}
	
	setControllerType("lobby");
	
	buttons["gameMode0"] = assignNewListeners(buttons["gameMode0"], 1);
	buttons["gameMode1"] = assignNewListeners(buttons["gameMode1"], 1);
	buttons["gameMode2"] = assignNewListeners(buttons["gameMode2"], 1);
	buttons["readySplash"] = assignNewListeners(buttons["readySplash"], 2);
	buttons["ready"] = assignNewListeners(buttons["ready"], 3);
	buttons["readyLeft"] = assignNewListeners(buttons["readyLeft"], 3);
	buttons["readyRight"] = assignNewListeners(buttons["readyRight"], 3);
	//buttons["readyTeam"] = assignNewListeners(buttons["readyTeam"], 4);
	buttons["gameMoveLeft"] = assignNewListeners(buttons["gameMoveLeft"], -1);
	buttons["gameMoveRight"] = assignNewListeners(buttons["gameMoveRight"], -1);
	buttons["gameShootLeft"] = assignNewListeners(buttons["gameShootLeft"], -1);
	buttons["gameShootRight"] = assignNewListeners(buttons["gameShootRight"], -1);
}

function setControllerColor(colorHex) {
	controllerColor = colorHex;
	$('.buttonText').css('border-color', '#' + colorHex);
	$('.buttonText.active').css('border-color', '#' + colorHex);
	$('.buttonText').css('color', '#' + colorHex);
	$('.buttonText.active').css('color', 'black');
	$('.buttonText').css('background-color', 'black');
	$('.buttonText.active').css('background-color', '#' + colorHex);
}

function setControllerType(type) {
	for (c in controllers) {
		controllers[c].style.display = "none";
	}
	controllers[type].style.display = "inline";
}

function disableSwitchGroup(switchID) {
	for (b in buttons) {
		if (buttons[b].getAttribute("switchGroup") == switchID) {
			$('#' + buttons[b].id + "Text").css('border-color', '#' + controllerColor);
			$('#' + buttons[b].id + "Text").css('color', '#' + controllerColor);
			$('#' + buttons[b].id + "Text").css('background-color', 'black');
			buttons[b].children[0].classList.remove("active");
		}
	}
}

function assignNewListeners(elem, switchGroup) {
	elem.setAttribute("switchGroup", switchGroup);
	elem.addEventListener("touchstart", function(e) {
		if (elem.getAttribute("switchGroup") == -1) {
			elem.children[0].classList.add("active");
			sendMessage(elem.id, true);
			$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('color', 'black');
			$('#' + elem.id + "Text").css('background-color', '#' + controllerColor);
		} else {
			disableSwitchGroup(elem.getAttribute("switchGroup"));
			if (elem.children[0].classList.contains("active")) {
				elem.children[0].classList.remove("active");
				$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
				$('#' + elem.id + "Text").css('color', '#' + controllerColor);
				$('#' + elem.id + "Text").css('background-color', 'black');
			} else {
				sendMessage(elem.id, true);
				elem.children[0].classList.add("active");
				$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
				$('#' + elem.id + "Text").css('color', 'black');
				$('#' + elem.id + "Text").css('background-color', '#' + controllerColor);
			}
		}
		e.preventDefault();
	});
	
	elem.addEventListener("touchmove", function(e) {
		e.preventDefault();
	});
	
	elem.addEventListener("touchend", function(e) {
		sendMessage(elem.id, false);
		if (elem.getAttribute("switchGroup") == -1) {
			elem.children[0].classList.remove("active");
			$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('background-color', 'black');
		}
		e.preventDefault();
	});
	
	var child = document.createElement("p");
	child.classList.add("buttonText");
	child.id = elem.id + "Text";
	child.appendChild(document.createTextNode(elem.getAttribute("name")));
	document.getElementById(elem.id).appendChild(child);
	return elem;
}

function forceEnable(elemName) {
	var elem = document.getElementById(elemName);
	if (elem.getAttribute("switchGroup") == -1) {
		elem.children[0].classList.add("active");
		$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
		$('#' + elem.id + "Text").css('color', 'black');
		$('#' + elem.id + "Text").css('background-color', '#' + controllerColor);
	} else {
		disableSwitchGroup(elem.getAttribute("switchGroup"));
		if (elem.children[0].classList.contains("active")) {
			elem.children[0].classList.remove("active");
			$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('background-color', 'black');
		} else {
			elem.children[0].classList.add("active");
			$('#' + elem.id + "Text").css('border-color', '#' + controllerColor);
			$('#' + elem.id + "Text").css('color', 'black');
			$('#' + elem.id + "Text").css('background-color', '#' + controllerColor);
		}
	}
}

function sendMessage(name, pressed) {
	var o = {};
	o[name] = pressed;
	airConsole.message(0, o);
}

function resetHealth() {
	healthBar.style.height = "80%";
}

function getDeathText() {
	var rand = Math.floor(Math.random() * 9);
	switch(rand) {
		case 0: return "DO BETTER</br>NEXT TIME";
		case 1: return "YOU TRIED";
		case 2: return "DON'T BE</br>A CAPTAIN";
		case 3: return "DON'T WORRY</br>YOU HAD A</br>SAFETY BOAT";
		case 4: return "NICE</br>WATERWINGS";
		case 5: return "LOL";
		case 6: return "SIT OUT</br>NEXT ROUND";
		case 7: return "TAKE A</br>BREAK";
		case 8: return "THIRSTY</br>AREN'T YOU?";
		case 9: return "WHY TRY?";
	}
}

function setHP(hp) {
	lives = hp;
	if (lives < 1) {
		document.getElementById("deathText").innerHTML = getDeathText();
		setControllerType("dead");
		return;
	}
	healthBar.style.height = (healthPercent * (lives / totalLives)) + "%";
}
