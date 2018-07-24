// Event listeners.

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