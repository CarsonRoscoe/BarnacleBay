var buttons = {
	gameMode0: null,
	gameMode1: null,
	gameMode2: null,
	gameMode3: null,
	gameMode4: null,
	gameMode5: null,
	gameMode6: null,
	gameMode7: null,
    readyMaster: null,
	ready: null,
	jump: null
}; 
var score = document.getElementById("score");
var gameMode = document.getElementById("gameMode");

//controller color in hex without the symbol.
var controllerColor = "FFFFFF";

var controllers = {
	menuMaster: document.getElementById("menuControllerMaster"),
	menu: document.getElementById("menuController"),
	game: document.getElementById("gameController"),
};

init = function() {
	buttons.gameMode0 = document.getElementById("gameMode0");
	buttons.gameMode1 = document.getElementById("gameMode1");
	buttons.gameMode2 = document.getElementById("gameMode2");
	buttons.gameMode3 = document.getElementById("gameMode3");
	buttons.gameMode4 = document.getElementById("gameMode4");
	buttons.gameMode5 = document.getElementById("gameMode5");
	buttons.gameMode6 = document.getElementById("gameMode6");
	buttons.gameMode7 = document.getElementById("gameMode7");
    buttons.readyMaster = document.getElementById("readyMaster");
	buttons.ready = document.getElementById("ready");
	buttons.jump = document.getElementById("jump");	
	
	for (c in controllers) {
		controllers[c].style.display = "none";
	}
	
	buttons["gameMode0"] = assignNewListeners(buttons["gameMode0"], 3);
	buttons["gameMode1"] = assignNewListeners(buttons["gameMode1"], 3);
	buttons["gameMode2"] = assignNewListeners(buttons["gameMode2"], 3);
	buttons["gameMode3"] = assignNewListeners(buttons["gameMode3"], 3);
	buttons["gameMode4"] = assignNewListeners(buttons["gameMode4"], 3);
	buttons["gameMode5"] = assignNewListeners(buttons["gameMode5"], 3);
	buttons["gameMode6"] = assignNewListeners(buttons["gameMode6"], 3);
	buttons["gameMode7"] = assignNewListeners(buttons["gameMode7"], 3);
	buttons["readyMaster"] = assignNewListeners(buttons["readyMaster"], 2);
	buttons["ready"] = assignNewListeners(buttons["ready"], 1);
	buttons["jump"] = assignNewListeners(buttons["jump"], -1);
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
		console.log(elem.getAttribute("name") + " pressed");
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
		console.log(elem.getAttribute("name") + " released");
		e.preventDefault();
	});
	
	var child = document.createElement("p");
	child.classList.add("buttonText");
	child.id = elem.id + "Text";
	child.appendChild(document.createTextNode(elem.getAttribute("name")));
	document.getElementById(elem.id).appendChild(child);
	return elem;
}

function sendMessage(name, pressed) {
	var o = {"name": name, "pressed": pressed};
	airConsole.message(0, o);
}
