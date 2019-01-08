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
using CodingTrainer.CodingTrainerWeb.Users;
using CodingTrainer.CodingTrainerWeb.Dependencies;
using System.IO;

namespace CodingTrainer.CodingTrainerWebTests
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
            mockRunner.Setup(r => r.CompileAndRunAsync(code)).Returns(Task.CompletedTask).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));

            // Arrange - Repository
            var userServices = new Mock<IUserServices>();
            userServices.Setup(r => r.GetCurrentUserId()).Returns("UnitTestId");
            var mockDb = new Mock<ICodingTrainerRepository>();

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockRunner.Object, userServices.Object, mockDb.Object)
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
            public async Task CompileAndRunAsync(string code)
            {
                await Task.Run(()=>ConsoleWrite?.Invoke(this, new ConsoleWriteEventArgs("Hello Test")));
                done.WaitOne();
            }

            public Task<CompiledCode> CompileAsync(string code)
            {
                throw new NotImplementedException();
            }
            public Task RunAsync(CompiledCode compiledCode)
            {
                throw new NotImplementedException();
            }
            public Task RunAsync(CompiledCode compiledCode, TextReader consoleInTextReader)
            {
                throw new NotImplementedException();
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

            // Arrange - Repository
            var userServices = new Mock<IUserServices>();
            userServices.Setup(r => r.GetCurrentUserId()).Returns("UnitTestId");
            var mockDb = new Mock<ICodingTrainerRepository>();

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(stubRunner, userServices.Object, mockDb.Object)
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
            mockRunner.Setup(r => r.CompileAndRunAsync(code)).Returns(Task.CompletedTask).Raises(r => r.ConsoleWrite += null, new ConsoleWriteEventArgs("Hello Test"));

            // Arrange - Repository
            var userServices = new Mock<IUserServices>();
            userServices.Setup(r => r.GetCurrentUserId()).Returns<string>(null);
            var mockDb = new Mock<ICodingTrainerRepository>();

            // Arrange - Class under test - Hub
            var hub = new CodeRunnerHub(mockRunner.Object, userServices.Object, mockDb.Object)
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
