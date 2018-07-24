// Event listeners.
document.addEventListener('DOMContentLoaded', noPassword);

// Functions.
function showPassword() {
    document.getElementById("hiddenPassword").style.display = "inline";
    document.getElementById("hidePasswordButton").style.display = "inline";
    document.getElementById("showPasswordButton").style.display = "none";
}

function hidePassword() {
    document.getElementById("hiddenPassword").style.display = "none";
    document.getElementById("hidePasswordButton").style.display = "none";
    document.getElementById("showPasswordButton").style.display = "inline";
}

function noPassword() {
    if (document.getElementById("hiddenPassword").innerText == "")
        document.getElementById("showPasswordButton").style.display = "none";
}