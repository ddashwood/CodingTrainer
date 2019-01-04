function IdeServices(signalRFactory, diagnostics, autoCompletions, parameters) {
    var self = this;

    //////////////////////////////
    // Nested class: CodeSubmitter
    //////////////////////////////

    // Constructor parameter: submitFunction - a callback that takes (code,  data1, data2, data3...)
    var CodeSubmitter = function (submitFunction) {
        this.currentCode = new Rx.Subject();
        this.currentCode.pipe(
            Rx.operators.debounceTime(300),
            Rx.operators.distinctUntilChanged(function (o) { return o.text; })
        ).subscribe( function(d) {
            var args = [d.code];
            for (var i = 0; i < d.data.length; i++) {
                args.push(d.data[i]);
            }
            submitFunction.apply(null, args);
        });
    };
    // Methood: submit - call submit with code and data - it will call submitFunction if code has changed
    CodeSubmitter.prototype.submit = function (code, data) {
        if (self.signalRFactory.hubConnected) {
            this.currentCode.next({ code: code, data: data });
        }
    };

    ///////////////////
    // End nested class
    ///////////////////


    this.ideHub = signalRFactory.createHub('ideHub');
    this.signalRFactory = signalRFactory;


    this.ideHub.client.diagsCallback = diagnostics;
    this.ideHub.client.completionsCallback = autoCompletions;
    this.ideHub.client.paramsCallback = parameters;

    this.diagsCaller = new CodeSubmitter(this.ideHub.server.requestDiags);
    this.completionsCaller = new CodeSubmitter(this.ideHub.server.requestCompletions);
    this.parametersCaller = new CodeSubmitter(this.ideHub.server.requestParameters);
}

IdeServices.prototype.requestDiagnostics = function (code, generation) {
    this.diagsCaller.submit(code, [ generation ]);
};
IdeServices.prototype.requestCompletions = function (code, pos, tokenStart) {
    this.completionsCaller.submit(code, [ pos, tokenStart ]);
};
IdeServices.prototype.requestParameters = function (code, pos, tokenStart) {
    this.parametersCaller.submit(code, [ pos, tokenStart ]);
};