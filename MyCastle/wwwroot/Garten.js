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
					this.$data.areas = response.data.areas;
				}
			});
		},
		getArea: function(areaName)
		{
			for (const key in app.$data.areas) {
				if (app.$data.areas.hasOwnProperty(key)) {
					var work = app.$data.areas[key];
					if (work.name == areaName)
						return work;
				}
			}
			return null;
		},

		setActiveArea: function(area) {
			if (typeof area == "string")
				area = this.getArea(area);
			
			if (this.$data.activeArea)
			{
				var areaEle = this.$data.activeArea.element;
				if (areaEle)
				{
					areaEle.style.fill = this.$data.activeArea.color;
					areaEle.style.strokeWidth = 0.0;
				}
			}

			this.$data.activeArea = area;
			
			if (this.$data.activeArea)
			{
				var areaEle = this.$data.activeArea.element;
				if (areaEle)
				{
					areaEle.style.fill = activeFill;
					areaEle.style.strokeWidth = 0.7;
					areaEle.style.stroke = "black";
				}
			}
		}
	},
	mounted: function () {
		this.loadSettings();
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
	if (app.$data.activeArea == area)
	{
		this.style.fill = activeFill;
		this.style.strokeWidth = 0.7;
	}
	else
	{
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

