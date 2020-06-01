import loadedAreas from './areas.json.js'

var app = new Vue({
    el: '#app',
    data: {
        message: 'Hello Vue!',
        areas: loadedAreas
    }
});



var gartenSvg = document.getElementById("GartenSvg");

var zoneMouseDown = function() {
    alert('zoneMouseDown!' + this.id);
}

var zoneMouseEnter = function() {
    this.prevFill = this.style.fill;
    this.style.fill = "#cccccc";
    this.style.strokeWidth = 0.7;
    this.style.stroke = "black";
}

var zoneMouseLeave = function() {
    this.style.fill = this.prevFill;
    this.style.strokeWidth = 0.0;
}

gartenSvg.addEventListener("load",function() {


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

