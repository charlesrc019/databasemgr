// Event listeners.
document.addEventListener('DOMContentLoaded', makeAdminUsername);
document.getElementById("DatabaseName").addEventListener('keyup', makeAdminUsername);
document.getElementById("DatabaseName").addEventListener('change', makeAdminUsername);
document.getElementById("CustomUsername").addEventListener('change', controlCustomUsername);
document.getElementById("DatabasePassword").addEventListener('change', uniquePassword);

// Functions.
function showDatabases(serverID) {
    var showID = "ShowDatabases_" + serverID;
    var hideID = "HideDatabases_" + serverID;
    var tableID = "Databases_" + serverID;
    document.getElementById(tableID).style.display = "table";
    document.getElementById(hideID).style.display = "inline";
    document.getElementById(showID).style.display = "none";
}

function hideDatabases(serverID) {
    var showID = "ShowDatabases_" + serverID;
    var hideID = "HideDatabases_" + serverID;
    var tableID = "Databases_" + serverID;
    document.getElementById(tableID).style.display = "none";
    document.getElementById(hideID).style.display = "none";
    document.getElementById(showID).style.display = "inline";
}

function makeAdminUsername() {
    if (document.getElementById("DatabaseName").value != "")
        document.getElementById("DatabaseUser").value = document.getElementById("DatabaseName").value;
    else
        document.getElementById("DatabaseUser").value = "";
}

function controlCustomUsername() {
    if (!document.getElementById("CustomUsername").checked) {
        document.getElementById("DatabaseUser").readOnly = true;
        document.getElementById("UniqueUsername").style.display = "none";
        if (document.getElementById("DatabaseName").value != "")
            makeAdminUsername();
        document.getElementById("DatabaseName").addEventListener('keyup', makeAdminUsername);
    }
    else {
        document.getElementById("DatabaseUser").readOnly = false;
        document.getElementById("UniqueUsername").style.display = "inline";
        document.getElementById("DatabaseUser").value = "";
        document.getElementById("DatabaseName").removeEventListener('keyup', makeAdminUsername);
    }
}

function uniquePassword() {
    if (document.getElementById("UniquePassword").style.display == "none") {
        document.getElementById("UniquePassword").style.display = "inline";
        document.getElementById("GeneratedPassword").style.display = "none";
        document.getElementById("DoNotSaveBlock").style.display = "block";
    }
    document.getElementById("DatabasePasswordVerify").value = "";
}