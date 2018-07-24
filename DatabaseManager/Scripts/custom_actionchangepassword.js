// Event listeners.
document.getElementById("NewPassword").addEventListener('change', uniquePassword);

// Functions.
function uniquePassword() {
    if (document.getElementById("UniquePassword").style.display == "none") {
        document.getElementById("UniquePassword").style.display = "inline";
        document.getElementById("GeneratedPassword").style.display = "none";
        document.getElementById("DoNotSaveBlock").style.display = "block";
    }
    document.getElementById("NewPasswordVerify").value = "";
}