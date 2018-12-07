function CodeRunnerWithCorrections(signalRFactory, consoleOut, complete, errors, corrections) {
    var self = this;
    this.corrections = corrections;

    var correctedErrors = function (errorList) {
        for (var i = 0; i < errorList.length; i++) {
            errorList[i].Location.SourceSpan.Start =
                self.corrections.positionCorrectionFromServer(errorList[i].Location.SourceSpan.Start);
            errorList[i].Location.SourceSpan.End =
                self.corrections.positionCorrectionFromServer(errorList[i].Location.SourceSpan.End);
        }

        errors(errorList);
    };

    CodeRunner.call(this, signalRFactory, consoleOut, complete, correctedErrors);
}

CodeRunnerWithCorrections.prototype = Object.create(CodeRunner.prototype);

CodeRunnerWithCorrections.prototype.run = function (code) {
    CodeRunner.prototype.run.call(this, this.corrections.codeWithCorrections(code));
};