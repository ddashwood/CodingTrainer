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
using CodingTrainer.CSharpRunner.CodeHost;
using System.Threading;
using CodingTrainer.CSharpRunner.CodeHost.Factories;
using System.Security.Principal;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;

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
            mockRunner.Setup(r => r.RunCode(code)).Returns(Task.CompletedTask).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));
            var mockFactory = new Mock<ICodeRunnerFactory>();
            mockFactory.Setup(f => f.GetCodeRunner()).Returns(() => mockRunner.Object);

            // Arrange - Repository
            var mockRepository = new Mock<IHubContextRepository>();
            mockRepository.Setup(r => r.GetUserIdFromContext(It.IsAny<HubCallerContext>())).Returns("UnitTestId");

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockFactory.Object, mockRepository.Object)
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

        private class CodeRunnerStub : ICodeRunner
        {
            private ManualResetEvent done = new ManualResetEvent(false);

            public event ConsoleWriteEventHandler ConsoleWrite;

            public void ConsoleIn(string text)
            {
                if (text == "Console In")
                {
                    ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs("Received"));
                    done.Set();
                }
            }
            public async Task RunCode(string code)
            {
                await Task.Run(()=>ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs("Hello Test")));
                done.WaitOne();
            }
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
            ICodeRunner stubRunner = new CodeRunnerStub(); 

            var mockFactory = new Mock<ICodeRunnerFactory>();
            mockFactory.Setup(f => f.GetCodeRunner()).Returns(() => stubRunner);

            // Arrange - Repository
            var mockRepository = new Mock<IHubContextRepository>();
            mockRepository.Setup(r => r.GetUserIdFromContext(It.IsAny<HubCallerContext>())).Returns("UnitTestId");

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockFactory.Object, mockRepository.Object)
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

        [Test]
        public async Task CodeRunnerLoggedOffTestAsync()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<ICodeRunnerHubClient>>();
            var mockCaller = new Mock<ICodeRunnerHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "CodeRunnerLoggedOffTestAsyncConnection");

            // Arrange - Runner
            string code = "<Test code>";
            var mockRunner = new Mock<ICodeRunner>();
            mockRunner.Setup(r => r.RunCode(code)).Returns(Task.CompletedTask).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));
            var mockFactory = new Mock<ICodeRunnerFactory>();
            mockFactory.Setup(f => f.GetCodeRunner()).Returns(() => mockRunner.Object);

            // Arrange - Repository
            var mockRepository = new Mock<IHubContextRepository>();
            mockRepository.Setup(r => r.GetUserIdFromContext(It.IsAny<HubCallerContext>())).Returns((string)null);

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockFactory.Object, mockRepository.Object)
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            await hub.Run(code);

            // Assert
            Assert.Multiple(() => {
                mockCaller.Verify(c => c.ConsoleOut("Can't run code because you are not logged in"), Times.Once);
                mockCaller.Verify(c => c.Complete(), Times.Once);
            });
        }

    }
}
