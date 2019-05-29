function CodeRunnerWithCorrections(signalRFactory, consoleOut, complete, errors, assessmentComplete, corrections, chapter, exercise) {
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

    CodeRunner.call(this, signalRFactory, consoleOut, complete, correctedErrors, assessmentComplete, chapter, exercise);
}

CodeRunnerWithCorrections.prototype = Object.create(CodeRunner.prototype);

CodeRunnerWithCorrections.prototype.run = function (code, forAssessment) {
    CodeRunner.prototype.run.call(this, this.corrections.codeWithCorrections(code), forAssessment);
};