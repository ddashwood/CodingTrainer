Ide.prototype.getConsole = function (consoleIn) {
    var console = CodeMirror.fromTextArea(document.getElementById("console"), {
        mode: "text/plain",
        theme: $('#Theme').val()
    });
    console.setSize(null, '20em');
    console.submitOnReturn(function (text) {
        consoleIn(text);
    });

    return console;
};