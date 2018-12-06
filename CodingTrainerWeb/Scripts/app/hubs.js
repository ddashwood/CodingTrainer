var hubs = function (editor, codeConsole, errorHandler) {
    // Objects of this class include the following properties, which represent the hubs
    // used to communicate with the server:

    this.runnerHub = null;
    this.ideHub = null;

    this.hubConnected = false;

    // Below this point is private properties, etc.
    var self = this;

    // Store the two code-mirror editors away so we can access them later
    this.editor = editor;
    this.codeConsole = codeConsole;

    // Set up and maintain the connection
    $.connection.hub.start().done(function () {
        self.hubConnected = true;
        // Once connected, allow users to submit code
        $('#run').prop('disabled', false).text('Run Code');
    });
    $.connection.hub.disconnected(function () {
        // Attempt to reconnect
        $.connection.hub.start();
    });


    // Code for handling SignalR events for running code
    this.runnerHub = $.connection['codeRunnerHub'];

    // Called when the script wants to display something on the console
    this.runnerHub.client.consoleOut = function (message) {
        self.codeConsole.consoleAppend(message);
    };

    // Called when the script has finished running
    var complete = function () {
        $('#run').prop('disabled', false);
    };
    this.runnerHub.client.complete = complete;

    // Called when there is an error compiling the script
    this.runnerHub.client.compilerError = errorHandler;
};