function CodeRunner(signalRFactory, consoleOut, complete, errors, chapter, exercise) {
    this.complete = complete;
    this.runnerHub = signalRFactory.createHub('codeRunnerHub');
    this.chapter = chapter;
    this.exercise = exercise;

    // Callbacks from the server:

    // Called when the script wants to display something on the console
    this.runnerHub.client.consoleOut = consoleOut;

    // Called when the script has finished running
    this.runnerHub.client.complete = complete;

    // Called when there is an error compiling the script
    this.runnerHub.client.compilerError = errors;
}

CodeRunner.prototype.run = function (code, forAssessment) {
    try {
        var serverFunc = forAssessment ?
            code => this.runnerHub.server.assess (code, this.chapter, this.exercise) :
            this.runnerHub.server.run;
        serverFunc(code).fail(function (e) {
            if (e.data) {
                e.message += "\r\n\r\nThe error message is:\r\n    " + e.data.Message;
            }
            alert(e.message);
            this.complete();
        });
    } catch (e) {
        alert(e.message);
        this.complete();
    }
};

CodeRunner.prototype.consoleIn = function (message) {
    this.runnerHub.server.consoleIn(message);
};