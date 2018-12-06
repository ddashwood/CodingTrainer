// Start everything going
(function () {
    var signalRFactory = new SignalRFactory();
    var serviceFactory = new ServiceFactory(signalRFactory);
    new Ide(serviceFactory);
})();
