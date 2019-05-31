function CodeRunnerWithCorrections(signalRFactory, consoleOut, complete, errors, assessmentComplete, corrections, chapter, exercise) {
    var self = this;
    this.corrections = corrections;

    var correctedErrors = function (errorList) {
        errors(corrections.correctErrors(errorList));
    };

    CodeRunner.call(this, signalRFactory, consoleOut, complete, correctedErrors, assessmentComplete, chapter, exercise);
}

CodeRunnerWithCorrections.prototype = Object.create(CodeRunner.prototype);

CodeRunnerWithCorrections.prototype.run = function (code, forAssessment) {
    CodeRunner.prototype.run.call(this, this.corrections.codeWithCorrections(code), forAssessment);
};