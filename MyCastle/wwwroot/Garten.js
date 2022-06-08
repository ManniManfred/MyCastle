import Vue from 'https://cdn.jsdelivr.net/npm/vue@2.6.11/dist/vue.esm.browser.min.js'
import vueResource from 'https://cdn.jsdelivr.net/npm/vue-resource@1.5.1/dist/vue-resource.esm.min.js'

Vue.use(vueResource);

const openFill = "#007bff";
const hoverFill = "#cccccc";

var app = new Vue({
	el: '#app',
	data: {
		areas: [],
		programs: [],
		polling: false,
		pollingId: null,
		zoneMenuVisible: false,
		programsMenuVisible: false,
		menuArea: null,
		gartenSvg: null,
		editMode: false,
		editFeedback: ""
	},
	methods: {
		initSvg: function() {
			this.gartenSvg = document.getElementById("GartenSvg");
			this.gartenSvg.addEventListener("load", this.gartenSvgOnLoad, false);

			window.document.addEventListener("click", this.hideContextMenu);
		},
		hideContextMenu: function() {
			this.$refs.myContextMenu.style.visibility = "hidden";
		},
		gartenSvgOnLoad: function() {
			let svgDoc = this.gartenSvg.contentDocument;
			svgDoc.addEventListener("mousedown", this.hideContextMenu);
			for (const key in this.areas) {
				if (this.areas.hasOwnProperty(key)) {
					const area = this.areas[key];

					var zoneElement = svgDoc.getElementById(area.name);
					if (zoneElement) {
						area.element = zoneElement;
						area.color = zoneElement.style.fill;

						zoneElement.addEventListener("click", this.zoneClick, false);
						zoneElement.addEventListener("mouseenter", this.zoneMouseEnter, false);
						zoneElement.addEventListener("mouseleave", this.zoneMouseLeave, false);
					}
					else {
						console.log("No area with name \"" + area.name + "\" found.");
					}
				}
			}
			this.loadOpenStatus();
			this.switchPolling();
		},
		zoneClick: function (e) {
			this.menuArea = this.getArea(e.target.id);
			if (this.menuArea == null)
				return;

			let box = this.gartenSvg.getBoundingClientRect();
			var top = e.pageY + box.top;
			var left = e.pageX + box.left;
			
			this.$refs.myContextMenu.style.display = "block";
			this.$refs.myContextMenu.style.visibility = "visible";

			if (top + this.$refs.myContextMenu.offsetHeight >= window.innerHeight)
				top = window.innerHeight - this.$refs.myContextMenu.offsetHeight - 1;
			
			if (left + this.$refs.myContextMenu.offsetWidth >= window.innerWidth)
				left = window.innerWidth - this.$refs.myContextMenu.offsetWidth - 1;
				
			top +=  window.pageYOffset;
			left += window.pageXOffset;

			this.$refs.myContextMenu.style.top = top + "px";
			this.$refs.myContextMenu.style.left = left + "px";
		},	
		zoneMouseEnter: function (e) {
			const area = this.getArea(e.target.id);
			if (area == null)
				return;

			area.mouseOver = true;
			this.setAreaFill(area);
		},
		zoneMouseLeave: function (e) {
			const area = this.getArea(e.target.id);
			if (area == null)
				return;

			area.mouseOver = false;
			this.setAreaFill(area);
		},
		setEditFeedback: function(text) {
			this.editFeedback = text;
			var self = this;
			window.setTimeout( function() { self.editFeedback = ""; }, 4000);
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
		loadPrograms: function () {
			this.$http.get('/api/program').then(response => {
				if (response && response.ok) {
					this.programs = response.data;
				}
			});
		},
		removeProgram: function(programIndex) { 
			this.programs.splice(programIndex, 1);
		},
		appendProgram: function() {
			this.programs.push({jobRunning: false, active: false, name: "Programm", cron: "0 16 * * *", tasks: [] });
		},
		savePrograms: function() {
			this.$http.post('/api/program', this.programs).then(response => {
				if (response && response.ok)
					this.setEditFeedback("Erfolgreich gespeichert");
				else
					this.setEditFeedback("Fehler beim speichern");
			});
		},
		switchProgramRun: function(program) {
			program.jobRunning = !program.jobRunning;
			this.$http.put('/api/program/' + program.name, program.jobRunning).then(response => {
				
			});
		},
		appendTask: function(program) {
			program.tasks.push({"area": 11, "duration": "00:10:00"});
		},
		removeTask: function(program, tIndex) {
			program.tasks.splice(tIndex, 1);
		},
		switchPolling: function() {
			this.setPolling(!this.polling);
		},
		setPolling: function(value) {
			this.polling = value;
			if (this.pollingId != null) {
				window.clearInterval(this.pollingId);
				this.pollingId = null;
			}
			
			if (this.polling)
				this.pollingId = window.setInterval(this.loadOpenStatus, 1000);
		},
		loadOpenStatus: function() {
			this.$http.get('/api/open', {timeout: 500}).then(response => {
				if (response && response.ok) {
					for (const key in this.areas) {
						if (this.areas.hasOwnProperty(key)) {
							var area = this.areas[key];
							area.open = response.data.includes(area.name);
							this.setAreaFill(area);
						}
					}
				}
				else
					this.setPolling(false);
			},
			response => { this.setPolling(false); });
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
		getAreaByPin: function (pin) {
			for (const key in this.areas) {
				if (this.areas.hasOwnProperty(key)) {
					var work = this.areas[key];
					if (work.pin == pin)
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
			this.setAreaFill(area);
		},
		setAreaFill: function (area) {
			if (!area.element)
				return;

			if (area.mouseOver) {
				area.element.style.fill = hoverFill;
				area.element.strokeWidth = 0.7;
				area.element.stroke = "black";
			}
			else {
				area.element.style.fill = area.open ? openFill : area.color;
				area.element.strokeWidth = 0.0;
			}
		}
	},
	mounted: function () {
		this.loadSettings();
		this.loadPrograms();
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
    document.getElementById('alertServiceWorker').removeAttribute('hidden'); 
  }
}
