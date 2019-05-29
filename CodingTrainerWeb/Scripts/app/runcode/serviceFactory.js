function ServiceFactory(signalRFactory, chapter, exercise) {
    this.signalRFactory = signalRFactory;
    this.chapter = chapter;
    this.exercise = exercise;
}

ServiceFactory.prototype.connect = function (done) {
    this.signalRFactory.connect(done);
};

ServiceFactory.prototype.getCodeRunner = function (callbacks) {
    return new CodeRunner(this.signalRFactory,
        callbacks.consoleOut.bind(callbacks),
        callbacks.runComplete.bind(callbacks),
        callbacks.showErrors.bind(callbacks),
        callbacks.assessmentComplete.bind(callbacks),
        this.chapter, this.exercise
    );
};

ServiceFactory.prototype.getIdeServices = function (callbacks) {
    return new IdeServices(this.signalRFactory,
        callbacks.showErrorsForGeneration.bind(callbacks),
        callbacks.showAutoCompletions.bind(callbacks),
        callbacks.showParameters.bind(callbacks)
    );
};
