var app = new Vue({
    el: '#app',
    data: {
        message: 'Hello Vue!',
        areas: [{ name: 'Rasen1' },
            { name: 'Rasen2' }, 
            { name: 'Hecke' }, 
            { name: 'Beet1' }, 
            { name: 'Beet2' }]
    }
});



var gartenSvg = document.getElementById("GartenSvg");



gartenSvg.addEventListener("load",function() {



    var svgDoc = gartenSvg.contentDocument;
    var delta = svgDoc.getElementById("Rasen1");
    // add behaviour
    delta.addEventListener("mousedown",function(){
    alert('hello world!')
    }, false);
}, false);

