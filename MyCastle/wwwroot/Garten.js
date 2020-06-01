import Vue from 'https://cdn.jsdelivr.net/npm/vue@2.6.11/dist/vue.esm.browser.js'
import vueResource from 'https://cdn.jsdelivr.net/npm/vue-resource@1.5.1/dist/vue-resource.esm.js'

Vue.use(vueResource);

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
		setActiveArea: function(area) {
			if (typeof area == "string")
			{
				for (const key in app.$data.areas) {
					if (app.$data.areas.hasOwnProperty(key)) {
						var work = app.$data.areas[key];
						if (work.name == area) {
							area = work;
							break;
						}
					}
				}
			}
			
			if (this.$data.activeArea)
			{
				var areaEle = this.$data.activeArea.element;
				if (areaEle)
				{
					areaEle.style.fill = areaEle.prevFill;
					areaEle.style.strokeWidth = 0.0;
				}
			}

			this.$data.activeArea = area;
			
			if (this.$data.activeArea)
			{
				var areaEle = this.$data.activeArea.element;
				if (areaEle)
				{
					areaEle.prevFill = areaEle.style.fill;
					areaEle.style.fill = "#007bff";
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
			const area = app.$data.areas[key];

			var svgDoc = gartenSvg.contentDocument;
			var zoneElement = svgDoc.getElementById(area.name);
			area.element = zoneElement;

			zoneElement.addEventListener("mousedown", zoneMouseDown, false);
			zoneElement.addEventListener("mouseenter", zoneMouseEnter, false);
			zoneElement.addEventListener("mouseleave", zoneMouseLeave, false);
		}
	}

}, false);

