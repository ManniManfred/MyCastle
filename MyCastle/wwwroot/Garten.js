import Vue from 'https://cdn.jsdelivr.net/npm/vue@2.6.11/dist/vue.esm.browser.js'
import vueResource from 'https://cdn.jsdelivr.net/npm/vue-resource@1.5.1/dist/vue-resource.esm.js'

Vue.use(vueResource);

const activeFill = "#007bff";
const hoverFill = "#cccccc";

var app = new Vue({
	el: '#app',
	data: {
		message: 'Hello Vue!',
		areas: [],
		activeArea: null
	},
	methods: {
		loadSettings: function () {
			this.$http.get('./settings.json').then(response => {
				if (response && response.ok) {

					// add "open" property, to auto watch
					var areasArr = response.data.areas;
					for (const key in areasArr) {
						if (areasArr.hasOwnProperty(key)) {
							const area = areasArr[key];
							area.open = false;
						}
					}
					this.$data.areas = areasArr;
				}
			});
		},
		loadOpened: function () {
			this.$http.get('/api/open').then(response => {
				if (response && response.ok) {
					for (let i = 0; i < response.data.length; i++) {
						var area = this.getArea(response.data[i]);
						area.open = true;
						//area.name = "Test " + area.name;
					}
				}
			});
		},
		getArea: function (areaName) {
			for (const key in app.$data.areas) {
				if (app.$data.areas.hasOwnProperty(key)) {
					var work = app.$data.areas[key];
					if (work.name == areaName)
						return work;
				}
			}
			return null;
		},
		switchOpen: function (area) {
			this.setOpen(area, !area.open);
		},
		setOpen: function (area, value) {
			if (value)
				this.$http.put('/api/open/' + area.pin);
			else
				this.$http.delete('/api/open/' + area.pin);

			area.open = value;
		},
		setActiveArea: function (area) {
			if (typeof area == "string")
				area = this.getArea(area);
			
			if (area === this.$data.activeArea)
				area = null;

			if (this.$data.activeArea) {
				var areaEle = this.$data.activeArea.element;
				if (areaEle) {
					areaEle.style.fill = this.$data.activeArea.color;
					areaEle.style.strokeWidth = 0.0;
				}
			}

			this.$data.activeArea = area;

			if (this.$data.activeArea) {
				var areaEle = this.$data.activeArea.element;
				if (areaEle) {
					areaEle.style.fill = activeFill;
					areaEle.style.strokeWidth = 0.7;
					areaEle.style.stroke = "black";
				}
			}
		}
	},
	mounted: function () {
		this.loadSettings();
		this.loadOpened();
	}
});



var gartenSvg = document.getElementById("GartenSvg");

var zoneMouseDown = function () {
	app.setActiveArea(this.id);
}

var zoneMouseEnter = function () {
	this.style.fill = hoverFill;
	this.style.strokeWidth = 0.7;
	this.style.stroke = "black";
}

var zoneMouseLeave = function () {
	const area = app.getArea(this.id);
	if (app.$data.activeArea == area) {
		this.style.fill = activeFill;
		this.style.strokeWidth = 0.7;
	}
	else {
		this.style.fill = area.color;
		this.style.strokeWidth = 0.0;
	}
}

gartenSvg.addEventListener("load", function () {

	for (const key in app.$data.areas) {
		if (app.$data.areas.hasOwnProperty(key)) {
			const area = app.$data.areas[key];

			var svgDoc = gartenSvg.contentDocument;
			var zoneElement = svgDoc.getElementById(area.name);
			area.element = zoneElement;
			area.color = zoneElement.style.fill;

			zoneElement.addEventListener("mousedown", zoneMouseDown, false);
			zoneElement.addEventListener("mouseenter", zoneMouseEnter, false);
			zoneElement.addEventListener("mouseleave", zoneMouseLeave, false);
		}
	}

}, false);

