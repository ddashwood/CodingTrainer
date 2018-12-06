function SignalRFactory() {
    this.hubConnected = false;
    this.connecting = false;
}

SignalRFactory.prototype.connect = function (done) {
    if (this.connecting || this.hubConnected) {
        done();
        return;
    }

    var self = this;
    this.connecting = true;

    // Set up and maintain the connection
    $.connection.hub.start().done(function () {
        self.hubConnected = true;
        done();
    });

    $.connection.hub.disconnected(function () {
        // Attempt to reconnect
        $.connection.hub.start();
    });
};

SignalRFactory.prototype.createHub = function (name) {
    return $.connection[name];
};