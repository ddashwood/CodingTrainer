function IdeServices(signalRFactory, diagnostics, autoCompletions, parameters) {
    this.ideHub = signalRFactory.createHub('ideHub');

    this.ideHub.client.diagsCallback = diagnostics;
    this.ideHub.client.completionsCallback = autoCompletions;
    this.ideHub.client.paramsCallback = parameters;

    this.signalRFactory = signalRFactory;
}

IdeServices.prototype.requestDiagnostics = function (code, generation) {
    if (this.signalRFactory.hubConnected)
        this.ideHub.server.requestDiags(code, generation);
};
IdeServices.prototype.requestCompletions = function (code, pos, tokenStart) {
    if (this.signalRFactory.hubConnected)
        this.ideHub.server.requestCompletions(code, pos, tokenStart);
};
IdeServices.prototype.requestParameters = function (code, pos, tokenStart) {
    if (this.signalRFactory.hubConnected)
        this.ideHub.server.requestParameters(code, pos, tokenStart);
};