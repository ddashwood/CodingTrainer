function getConsole(consoleIn) {
    var console = CodeMirror.fromTextArea(document.getElementById("console"), {
        mode: "text/plain",
        theme: $('#Theme').val()
    });
    console.setSize(null, '35em');
    console.submitOnReturn(function (text) {
        consoleIn(text);
    });

    return console;
}