/// <reference path="../jquery-1.10.2.min.js" />
var d = document.getElementById('depts');
var s = document.getElementById('states');
var i = document.getElementById('items');
var r = document.getElementById('region');
var rType = document.getElementById('requestType');

var sdate = document.getElementById('sdate');
var edate = document.getElementById('edate');
function deptF() {
    if (d.value != "") {
        s.disabled = true
        i.disabled = true
        r.disabled = true
        rType.disabled = true
        i.selectedIndex = 0
        s.selectedIndex = 0      
        r.selectedIndex = 0
        rType.selectedIndex = 0
    } else {
        s.disabled = false
        i.disabled = false
        r.disabled = false
        rType.disabled = false
        i.selectedIndex = 0
        s.selectedIndex = 0       
        r.selectedIndex = 0
        rType.selectedIndex = 0
    }

}
function stateF() {
    if (s.value != "") {
        d.disabled = true
        i.disabled = true
        r.disabled = true
        rType.disabled = true
        i.selectedIndex = 0        
        d.selectedIndex = 0
        r.selectedIndex = 0
        rType.selectedIndex = 0
    } else {
        d.disabled = false
        i.disabled = false
        r.disabled = false
        rType.disabled = false
        i.selectedIndex = 0        
        d.selectedIndex = 0
        r.selectedIndex = 0
        rType.selectedIndex = 0
    }

}
function itemF() {
    if (i.value != "") {
        s.disabled = true
        d.disabled = true
        r.disabled = true
        rType.disabled = true      
        s.selectedIndex = 0
        d.selectedIndex = 0
        r.selectedIndex = 0
        rType.selectedIndex = 0
    } else {
        s.disabled = false
        d.disabled = false
        r.disabled = false
        rType.disabled = false      
        s.selectedIndex = 0
        d.selectedIndex = 0
        r.selectedIndex = 0
        rType.selectedIndex = 0
    }

}
function RegionF() {
    if (r.value != "") {
        s.disabled = true
        d.disabled = true
        i.disabled = true
        rType.disabled = true
        i.selectedIndex = 0
        s.selectedIndex = 0
        d.selectedIndex = 0       
        rType.selectedIndex = 0
    } else {
        s.disabled = false
        d.disabled = false
        i.disabled = false
        rType.disabled = false
        i.selectedIndex = 0
        s.selectedIndex = 0
        d.selectedIndex = 0      
        rType.selectedIndex = 0
    }

}
function RTpyeF() {
    if (rType.value != "") {
        s.disabled = true
        d.disabled = true
        r.disabled = true
        i.disabled = true
         i.selectedIndex = 0
    s.selectedIndex = 0
    d.selectedIndex = 0
    r.selectedIndex = 0
   
    } else {
        s.disabled = false
        d.disabled = false
        r.disabled = false
        i.disabled = false
        i.selectedIndex = 0
        s.selectedIndex = 0
        d.selectedIndex = 0
        r.selectedIndex = 0
      
    }

}
function reset() {
    i.selectedIndex = 0
    s.selectedIndex = 0
    d.selectedIndex = 0
    r.selectedIndex = 0
    rType.selectedIndex = 0
    d.disabled = false
    s.disabled = false
    i.disabled = false
    r.disabled = false
    rType.disabled = false
    edate.value = "";
    sdate.value = ""
}