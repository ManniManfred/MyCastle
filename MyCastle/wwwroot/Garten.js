import Vue from 'https://cdn.jsdelivr.net/npm/vue@2.6.11/dist/vue.esm.browser.js'
import vueResource from 'https://cdn.jsdelivr.net/npm/vue-resource@1.5.1/dist/vue-resource.esm.js'

Vue.use(vueResource);

const activeFill = "#007bff";
const hoverFill = "#cccccc";

var app = new Vue({
	el: '#app',
	data: {
		areas: [],
		activeArea: null,
		menuArea: null,
		gartenSvg: null,
	},
	methods: {
		initSvg: function() {
			this.gartenSvg = document.getElementById("GartenSvg");
			this.gartenSvg.addEventListener("load", this.gartenSvgOnLoad, false);

			window.document.addEventListener("click", this.hideContextMenu);
		},
		hideContextMenu: function() {
			this.$refs.myContextMenu.style.display = "none";
		},
		gartenSvgOnLoad: function() {
			let svgDoc = this.gartenSvg.contentDocument;
			svgDoc.addEventListener("mousedown", this.hideContextMenu);
			for (const key in this.areas) {
				if (this.areas.hasOwnProperty(key)) {
					const area = this.areas[key];

					var zoneElement = svgDoc.getElementById(area.name);
					area.element = zoneElement;
					area.color = zoneElement.style.fill;

					zoneElement.addEventListener("click", this.zoneClick, false);
					zoneElement.addEventListener("mouseenter", this.zoneMouseEnter, false);
					zoneElement.addEventListener("mouseleave", this.zoneMouseLeave, false);
				}
			}
		},
		zoneClick: function (e) {
			this.menuArea = this.getArea(e.target.id);
			if (this.menuArea == null)
				return;
				
			let box = this.gartenSvg.getBoundingClientRect();
			var top = e.pageY + box.top + window.pageYOffset;
			var left = e.pageX + box.left + window.pageXOffset;
			
			this.$refs.myContextMenu.style.display = "block";
			this.$refs.myContextMenu.style.top = top + "px";
			this.$refs.myContextMenu.style.left = left + "px";
		},	
		zoneMouseEnter: function (e) {
			e.target.style.fill = hoverFill;
			e.target.style.strokeWidth = 0.7;
			e.target.style.stroke = "black";
		},
		zoneMouseLeave: function (e) {
			const area = this.getArea(e.target.id);
			if (this.activeArea == area) {
				e.target.style.fill = activeFill;
				e.target.style.strokeWidth = 0.7;
			}
			else {
				e.target.style.fill = area.color;
				e.target.style.strokeWidth = 0.0;
			}
		},
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
					this.areas = areasArr;
					this.initSvg();
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
			for (const key in this.areas) {
				if (this.areas.hasOwnProperty(key)) {
					var work = this.areas[key];
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

			if (this.activeArea) {
				var areaEle = this.activeArea.element;
				if (areaEle) {
					areaEle.style.fill = this.activeArea.color;
					areaEle.style.strokeWidth = 0.0;
				}
			}

			this.activeArea = area;

			if (this.activeArea) {
				var areaEle = this.activeArea.element;
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





window.addEventListener('load', e => {
  registerSW(); 
});

async function registerSW() { 
  if ('serviceWorker' in navigator) { 
    try {
      await navigator.serviceWorker.register('./sw.js'); 
    } catch (e) {
      alert('ServiceWorker registration failed. Sorry about that.'); 
    }
  } else {
    document.querySelector('.alert').removeAttribute('hidden'); 
  }
}
