// spoilerScript 

var elements = document.querySelectorAll('[data-toggle="hidden"]');

Array.prototype.forEach.call(elements, function (el, i)
{
	el.onclick = function() {
		el.nextElementSibling.classList.toggle("hidden");
	}
});

