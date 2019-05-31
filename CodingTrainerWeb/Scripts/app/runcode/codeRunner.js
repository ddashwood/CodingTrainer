function CodeRunner(signalRFactory, consoleOut, complete, errors, assessmentComplete, chapter, exercise) {
    this.complete = complete;
    this.runnerHub = signalRFactory.createHub('codeRunnerHub');
    this.chapter = chapter;
    this.exercise = exercise;

    // Callbacks from the server:

    // Called when the script wants to display something on the console
    this.runnerHub.client.consoleOut = consoleOut;
    this.runnerHub.client.consoleOutHighlight = function (m) { consoleOut(m, "#00f"); };

    // Called when the script has finished running
    this.runnerHub.client.complete = complete;

    // Called when there is an error compiling the script
    this.runnerHub.client.compilerError = errors;

    // Called to indicate the success or otherwise of a submitted assessment
    this.runnerHub.client.assessmentComplete = assessmentComplete;
}

CodeRunner.prototype.run = function (code, forAssessment) {
    var self = this;
    try {
        var hubProcess;
        if (forAssessment) {
            hubProcess = this.runnerHub.server.assess(code, this.chapter, this.exercise);
        }
        else {
            hubProcess = this.runnerHub.server.run(code);
        }
        hubProcess.fail(function (e) {
            if (e.data) {
                e.message += "\r\n\r\nThe error message is:\r\n    " + e.data.Message;
            }
            alert(e.message);
            self.complete();
        });
    } catch (e) {
        alert(e.message);
        this.complete();
    }
};

CodeRunner.prototype.consoleIn = function (message) {
    this.runnerHub.server.consoleIn(message);
};