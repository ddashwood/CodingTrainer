function CodeRunner(signalRFactory, consoleOut, complete, errors) {
    this.runnerHub = signalRFactory.createHub('codeRunnerHub');

    // Called when the script wants to display something on the console
    this.runnerHub.client.consoleOut = consoleOut;

    // Called when the script has finished running
    this.runnerHub.client.complete = complete;

    // Called when there is an error compiling the script
    this.runnerHub.client.compilerError = errors;
}

CodeRunner.prototype.run = function (code) {
    try {
        this.runnerHub.server.run(code).fail(function (e) {
            if (e.data) {
                e.message += "\r\n\r\nThe error message is:\r\n    " + e.data.Message;
            }
            alert(e.message);
            complete();
        });
    } catch (e) {
        alert(e.message);
        complete();
    }
};

CodeRunner.prototype.consoleIn = function (message) {
    this.runnerHub.server.consoleIn(message);
};