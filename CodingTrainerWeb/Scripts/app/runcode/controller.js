// Start everything going
(function () {
    var signalRFactory = new SignalRFactory();
    //var serviceFactory = new ServiceFactory(signalRFactory);
    var serviceFactory = new ServiceFactoryForHiddenCode(signalRFactory);
    new Ide(serviceFactory);
})();
