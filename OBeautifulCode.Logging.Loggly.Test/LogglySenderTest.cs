// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogglySenderTest.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Loggly.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using Its.Configuration;

    using Naos.Recipes.Configuration.Setup;

    using OBeautifulCode.Logging.Recipes;

    using Xunit;

    public class LogglySenderTest : ConfigRequiredTest
    {
        public LogglySenderTest()
        {
            Config.ResetConfigureSerializationAndSetValues(string.Empty);
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentNullException___When_parameter_logglySettings_is_null()
        {
            // Arrange, Act
            var ex = Record.Exception(() => LogglySender.Initialize(null));

            // Assert
            ex.Should().BeOfType<ArgumentNullException>();
            ex.Message.Should().Contain("logglySettings");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentNullException___When_parameter_logglySettings_CustomerToken_is_null()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.CustomerToken = null;

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentNullException>();
            ex.Message.Should().Contain("CustomerToken");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentException___When_parameter_logglySettings_CustomerToken_is_white_space()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.CustomerToken = "  \r\n  ";

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Contain("CustomerToken");
            ex.Message.Should().Contain("white space");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentNullException___When_parameter_logglySettings_LogglyServerCertificatePemEncoded_is_null()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.LogglyServerCertificatePemEncoded = null;

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentNullException>();
            ex.Message.Should().Contain("LogglyServerCertificatePemEncoded");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentException___When_parameter_logglySettings_LogglyServerCertificatePemEncoded_is_white_space()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.LogglyServerCertificatePemEncoded = "  \r\n  ";

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Contain("LogglyServerCertificatePemEncoded");
            ex.Message.Should().Contain("white space");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentNullException___When_parameter_logglySettings_LogglyPrivateEnterpriseNumber_is_null()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.LogglyPrivateEnterpriseNumber = null;

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentNullException>();
            ex.Message.Should().Contain("LogglyPrivateEnterpriseNumber");
        }

        [Fact]
        public void Initialize___Should_throw_ArgumentException___When_parameter_logglySettings_LogglyPrivateEnterpriseNumber_is_white_space()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.LogglyPrivateEnterpriseNumber = "  \r\n  ";

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Contain("LogglyPrivateEnterpriseNumber");
            ex.Message.Should().Contain("white space");
        }

        [Fact]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Portis", Justification = "This is spelled correctly.")]
        public void Initialize___Should_throw_ArgumentException___When_parameter_logglySettings_SecurePort_is_default_value()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            settings.SecurePort = default(int);

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Contain("SecurePort");
            ex.Message.Should().Contain("default");
        }

        [Fact]
        public void SendLogMessageToLoggly___Should_throw_InvalidOperationException___When_Initialize_has_not_been_called()
        {
            // Arrange, Act
            var ex = Record.Exception(() => LogglySender.SendLogMessageToLoggly(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>(), A.Dummy<Severity>(), DateTime.UtcNow, A.Dummy<string>(), A.Dummy<string>()));

            // Assert
            ex.Should().BeOfType<InvalidOperationException>();
            ex.Message.Should().Be("Initialize has not yet been called.");
        }

        [Fact(Skip = "For local testing only.")]
        public void SendLogMessageToLoggly()
        {
            // Arrange
            var settings = Settings.Get<LogglySettings>();
            LogglySender.Initialize(settings);

            var machineName = "MachineName";
            var applicationName = "ApplicationName";
            var processId = "ProcessId";
            var severity = Severity.Notice;
            var timestamp = DateTime.UtcNow;
            var messagePayload = "{ \"property1\": true, \"property2\": \"some text\" }";
            var messageType = "MessageType";
            var tags = new[] { "tag1", "tag2" };

            // Act, Assert
            LogglySender.SendLogMessageToLoggly(machineName, applicationName, processId, severity, timestamp, messagePayload, messageType, tags);
        }
    }
}
