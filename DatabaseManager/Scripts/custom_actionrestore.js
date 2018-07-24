// Event listeners.
document.addEventListener('DOMContentLoaded', controlBackupType);
var radiosbuttons = document.getElementsByName("Method");
for (var i = 0; i < radiosbuttons.length; i++) {
    radiosbuttons[i].addEventListener('change', controlBackupType);
}

// Functions.
function controlBackupType() {
    for (var i = 0; i < radiosbuttons.length; i++) {
        if (radiosbuttons[i].checked) {
            if (radiosbuttons[i].value == "Select") {
                document.getElementById("FilePath").disabled = false;
                document.getElementById("File").disabled = true;
            }
            if (radiosbuttons[i].value == "Upload") {
                document.getElementById("FilePath").disabled = true;
                document.getElementById("FilePath").value = "default";
                document.getElementById("File").disabled = false;
            }
            break;
        }
    }
}