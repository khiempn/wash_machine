//show or hide pw
function toggle1() {
    var x = document.getElementById("id_password");
    if (x.type === "password") {
        x.type = "text";
        document.getElementById("eye1").style.color = '#5887ef';
    } else {
        x.type = "password";
        document.getElementById("eye1").style.color = '#7a797e';
    }
}
function toggle2() {
    var y = document.getElementById("new_password");
    if (y.type === "password") {
        y.type = "text";
        document.getElementById("eye2").style.color = '#5887ef';
    } else {
        y.type = "password";
        document.getElementById("eye2").style.color = '#7a797e';
    }
}
function toggle3() {
    var a = document.getElementById("cf_password");
    if (a.type === "password") {
        a.type = "text";
        document.getElementById("eye3").style.color = '#5887ef';
    } else {
        a.type = "password";
        document.getElementById("eye3").style.color = '#7a797e';
    }
}