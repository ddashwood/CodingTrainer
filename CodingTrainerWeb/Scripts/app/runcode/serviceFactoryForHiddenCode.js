function ServiceFactoryForHiddenCode(signalRFactory, hiddenCodeHeader, chapter, exercise) {
    // These corrections deal with hidden code in the model
    // They will be passed into the relevant services to be used as required
    this.corrections = {
        codeWithCorrections: function (code) {
            if (hiddenCodeHeader) {
                code = hiddenCodeHeader + "\n" + code;
            }
            return code;
        },
        positionCorrectionToServer: function (pos) {
            if (hiddenCodeHeader) {
                var adjustment = hiddenCodeHeader.length + 1;
                pos += adjustment;
            }
            return pos;
        },
        positionCorrectionFromServer: function (pos) {
            if (hiddenCodeHeader) {
                var adjustment = hiddenCodeHeader.length + 1;
                pos -= adjustment;
            }
            return pos;
        }
    };

    ServiceFactory.call(this, signalRFactory, chapter, exercise);
}

ServiceFactoryForHiddenCode.prototype = Object.create(ServiceFactory.prototype);

ServiceFactoryForHiddenCode.prototype.getCodeRunner = function (callbacks) {
    return new CodeRunnerWithCorrections(this.signalRFactory,
        callbacks.consoleOut.bind(callbacks),
        callbacks.runComplete.bind(callbacks),
        callbacks.showErrors.bind(callbacks),
        this.corrections, this.chapter, this.exercise
    );
};

ServiceFactoryForHiddenCode.prototype.getIdeServices = function (callbacks) {
    return new IdeServicesWithCorrections(this.signalRFactory,
        callbacks.showErrorsForGeneration.bind(callbacks),
        callbacks.showAutoCompletions.bind(callbacks),
        callbacks.showParameters.bind(callbacks),
        this.corrections
    );
};
