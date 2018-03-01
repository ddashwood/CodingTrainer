using CodingTrainer.CodingTrainerWeb.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using Microsoft.AspNet.SignalR.Hubs;
using CodingTrainer.CodingTrainerWeb.Hubs.Helpers;

namespace CodingTrainer.CodingTrainerWeb.Hubs.Tests
{
    [Category("Ide Hub")]
    [TestFixture]
    public class IdeHubTests
    {
        [Test]
        public async Task RequestParametersTest()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<IIdeHubClient>>();
            var mockCaller = new Mock<IIdeHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "RequestParametersTestConnection");

            // Arrange - Hub
            var hub = new IdeHub()
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            // N.b. we specifically test DateTime.Parse, because:
            //  a) it has several overloads, and
            //  b) some of its parameter types have C# aliases and some don't
            await hub.RequestParameters("System.DateTime.Parse(\"\", null, System.Globalization.DateTimeStyles.None)",
                50, 1234);

            // Assert
            mockCaller.Verify(m => m.ParamsCallback(It.Is<Overloads>(o => VerifyDateTimeParseOverloads(o)), 1234));
        }

        private bool VerifyDateTimeParseOverloads(Overloads overloads)
        {
            if (overloads.Name != "Parse") return false;
            if (overloads.ReturnType != "DateTime") return false;

            if (overloads.Count() != 3) return false;
            var threeParams = overloads.Single(method => method.Parameters.Count == 3).Parameters.ToArray();

            if (threeParams[0].Name != "s") return false;
            if (threeParams[0].Type != "string") return false;
            if (threeParams[1].Name != "provider") return false;
            if (threeParams[1].Type != "IFormatProvider") return false;
            if (threeParams[2].Name != "styles") return false;
            if (threeParams[2].Type != "DateTimeStyles") return false;

            return true;
        }

        [Test]
        public async Task BuiltInReturnTypeTest()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<IIdeHubClient>>();
            var mockCaller = new Mock<IIdeHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "BuiltInReturnTypeTestConnection");

            // Arrange - Hub
            var hub = new IdeHub()
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            await hub.RequestParameters("System.Console.ReadLine()",
                24, 4321);

            // Assert
            mockCaller.Verify(m => m.ParamsCallback(It.Is<Overloads>(o => VerifyReadLineParseOverloads(o)), 4321));
        }

        private bool VerifyReadLineParseOverloads(Overloads overloads)
        {
            if (overloads.Name != "ReadLine") return false;
            if (overloads.ReturnType != "string") return false;
            if (overloads.Count() != 1) return false;
            return true;
        }

        [Test]
        public async Task BeforeParamsTest()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<IIdeHubClient>>();
            var mockCaller = new Mock<IIdeHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "BeforeParamsTestConnection");

            // Arrange - Hub
            var hub = new IdeHub()
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            await hub.RequestParameters("System.Console.ReadLine()",
                22, 4321);

            // Assert
            mockCaller.Verify(m => m.ParamsCallback(It.IsAny<Overloads>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task AfterParamsTest()
        {
            // Arrange - Clients
            var mockClients = new Mock<IHubCallerConnectionContext<IIdeHubClient>>();
            var mockCaller = new Mock<IIdeHubClient>();
            mockClients.Setup(m => m.Caller).Returns(mockCaller.Object);

            // Arrange - Context
            var context = new HubCallerContext(null, "AfterParamsTestConnection");

            // Arrange - Hub
            var hub = new IdeHub()
            {
                Clients = mockClients.Object,
                Context = context
            };

            // Act
            await hub.RequestParameters("System.Console.ReadLine()",
                25, 4321);

            // Assert
            mockCaller.Verify(m => m.ParamsCallback(It.IsAny<Overloads>(), It.IsAny<int>()), Times.Never);
        }
    }
}