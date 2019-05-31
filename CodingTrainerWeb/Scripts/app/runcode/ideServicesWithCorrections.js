function IdeServicesWithCorrections(signalRFactory, diagnostics, autoCompletions, parameters, corrections) {
    var self = this;
    this.corrections = corrections;

    var correctedErrors = function (errorList, generation) {
        diagnostics(corrections.correctErrors(errorList), generation);
    };

    IdeServices.call(this, signalRFactory, correctedErrors, autoCompletions, parameters);
}

IdeServicesWithCorrections.prototype = Object.create(IdeServices.prototype);

IdeServicesWithCorrections.prototype.requestDiagnostics = function (code, generation) {
    IdeServices.prototype.requestDiagnostics.call(this, this.corrections.codeWithCorrections(code), generation);

};

IdeServicesWithCorrections.prototype.requestCompletions = function (code, pos, tokenStart) {
    IdeServices.prototype.requestCompletions.call(this,
        this.corrections.codeWithCorrections(code),
        this.corrections.positionCorrectionToServer(pos),
        tokenStart
    );
};

IdeServicesWithCorrections.prototype.requestParameters = function (code, pos, tokenStart) {
    IdeServices.prototype.requestParameters.call(this,
        this.corrections.codeWithCorrections(code),
        this.corrections.positionCorrectionToServer(pos),
        tokenStart
    );
};
