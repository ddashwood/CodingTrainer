function ServiceFactory(signalRFactory) {
    this.signalRFactory = signalRFactory;
}

ServiceFactory.prototype.connect = function (done) {
    this.signalRFactory.connect(done);
};

ServiceFactory.prototype.getCodeRunner = function (callbacks) {
    return new CodeRunner(this.signalRFactory,
        callbacks.consoleOut.bind(callbacks),
        callbacks.runComplete.bind(callbacks),
        callbacks.showErrors.bind(callbacks)
    );
};

ServiceFactory.prototype.getIdeServices = function (callbacks) {
    return new IdeServices(this.signalRFactory,
        callbacks.showErrorsForGeneration.bind(callbacks),
        callbacks.showAutoCompletions.bind(callbacks),
        callbacks.showParameters.bind(callbacks)
    );
};
