function IdeServicesWithCorrections(signalRFactory, diagnostics, autoCompletions, parameters, corrections) {
    var self = this;
    this.corrections = corrections;

    var correctedErrors = function (errorList, generation) {
        if (errorList) {
            for (var i = 0; i < errorList.length; i++) {
                errorList[i].Location.SourceSpan.Start =
                    self.corrections.positionCorrectionFromServer(errorList[i].Location.SourceSpan.Start);
                errorList[i].Location.SourceSpan.End =
                    self.corrections.positionCorrectionFromServer(errorList[i].Location.SourceSpan.End);
            }
        }
        diagnostics(errorList, generation);
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
