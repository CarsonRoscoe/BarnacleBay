var buttons = {left: null, right: null, start: null}; 
buttons.left = document.getElementById("leftButton");
buttons.right = document.getElementById("rightButton");
buttons.start = document.getElementById("startButton");

assignNewListeners(buttons.left);
assignNewListeners(buttons.right);
assignNewListeners(buttons.start);

function assignNewListeners(elem) {
	elem.addEventListener("touchstart", function(e) {
		rateLimiter.message(elem.name, false);
		elem.id = elem.id + "Active";
		e.preventDefault();
	});
	elem.addEventListener("touchmove", function(e) {
		e.preventDefault();
	});
	elem.addEventListener("touchend", function(e) {
		rateLimiter.message(elem.name, true);
		elem.id = elem.name;
		e.preventDefault();
	});
}

