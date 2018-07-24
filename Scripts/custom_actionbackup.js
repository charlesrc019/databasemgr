// Event listeners.
document.addEventListener('DOMContentLoaded', makeFilename);
document.getElementById("Nickname").addEventListener('keyup', makeFilename);
document.getElementById("Nickname").addEventListener('change', makeFilename);

// Functions.
function makeFilename() {
    if (document.getElementById("Nickname").value != "")
        document.getElementById("BackupNickname").innerHTML = "_" + document.getElementById("Nickname").value;
    else
        document.getElementById("BackupNickname").innerHTML = "";
}