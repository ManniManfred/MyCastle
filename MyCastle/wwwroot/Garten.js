import Vue from 'https://cdn.jsdelivr.net/npm/vue@2.6.11/dist/vue.esm.browser.js'
import vueResource from 'https://cdn.jsdelivr.net/npm/vue-resource@1.5.1/dist/vue-resource.esm.js'

Vue.use(vueResource);

var app = new Vue({
	el: '#app',
	data: {
		message: 'Hello Vue!',
		areas: []
	},
	methods: {
		loadSettings: function () {
			this.$http.get('./settings.json').then(response => {
				if (response && response.ok) {
					this.$data.areas = response.data.areas;
				}
			});
		}
	},
	mounted: function () {
		this.loadSettings();
	}
});



var gartenSvg = document.getElementById("GartenSvg");

var zoneMouseDown = function () {
	alert('zoneMouseDown!' + this.id);
}

var zoneMouseEnter = function () {
	this.prevFill = this.style.fill;
	this.style.fill = "#cccccc";
	this.style.strokeWidth = 0.7;
	this.style.stroke = "black";
}

var zoneMouseLeave = function () {
	this.style.fill = this.prevFill;
	this.style.strokeWidth = 0.0;
}

gartenSvg.addEventListener("load", function () {


	for (const key in app.$data.areas) {
		if (app.$data.areas.hasOwnProperty(key)) {
			const zone = app.$data.areas[key];

			var svgDoc = gartenSvg.contentDocument;
			var zoneElement = svgDoc.getElementById(zone.name);

			zoneElement.addEventListener("mousedown", zoneMouseDown, false);
			zoneElement.addEventListener("mouseenter", zoneMouseEnter, false);
			zoneElement.addEventListener("mouseleave", zoneMouseLeave, false);
		}
	}

}, false);

