using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using System.Dynamic;
using CodingTrainer.CodingTrainerWeb.Hubs;
using CodingTrainer.CodingTrainerWeb.Factories;
using CodingTrainer.CSharpRunner.CodeHost;
using System.Threading;

namespace CodingTrainerWebTests.Hubs
{
    [Category("Code Runner Hub")]
    [TestFixture]
    public class CodeRunnerHubTests
    {
        [Test]
        public async Task CodeRunnerTestAsync()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<ICodeRunnerHubClient>>();
            var mockCaller = new Mock<ICodeRunnerHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "CodeRunnerTestAsyncConnection");

            // Arrange - Runner
            string code = "<Test code>";
            var mockRunner = new Mock<ICodeRunner>();
            mockRunner.Setup(r => r.RunCode(code)).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));
            var mockFactory = new Mock<ICodeRunnerFactory>();
            mockFactory.Setup(f => f.GetCodeRunner()).Returns(() => mockRunner.Object);

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockFactory.Object)
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            await hub.Run(code);

            // Assert
            Assert.Multiple(() => {
                mockCaller.Verify(c => c.ConsoleOut("Hello Test"), Times.Once);
                mockCaller.Verify(c => c.Complete(), Times.Once);
            });
        }

        [Test]
        public async Task CodeRunnerConsoleInTestAsync()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<ICodeRunnerHubClient>>();
            var mockCaller = new Mock<ICodeRunnerHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "CodeRunnerConsoleInTestAsyncConnection");

            // Arrange - Runner
            string code = "<Test code>";
            var mockRunner = new Mock<ICodeRunner>();
            var mockFactory = new Mock<ICodeRunnerFactory>();
            mockFactory.Setup(f => f.GetCodeRunner()).Returns(() => mockRunner.Object);
            mockRunner.Setup(r => r.RunCode(code)).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));
            mockRunner.Setup(r => r.ConsoleIn("Console In")).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Received"));

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockFactory.Object)
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Arrange - set up Caller
            mockCaller.Setup(c => c.ConsoleOut("Hello Test")).Callback(() => hub.ConsoleIn("Console In"));

            // Act
            await hub.Run(code);

            // Assert
            Assert.Multiple(() => {
                mockCaller.Verify(c => c.ConsoleOut("Hello Test"), Times.Once);
                mockCaller.Verify(c => c.ConsoleOut("Received"), Times.Once);
                mockCaller.Verify(c => c.Complete(), Times.Once);
            });
        }
    }
}
