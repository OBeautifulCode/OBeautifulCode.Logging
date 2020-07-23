// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogglySenderTest.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Loggly.Recipes.Test
{
    using System;

    using FakeItEasy;

    using FluentAssertions;

    using OBeautifulCode.Logging.Syslog.Recipes;

    using Xunit;

    public class LogglySenderTest
    {
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
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
            var settings = BuildLogglySettings();
            settings.SecurePort = default(int);

            // Act
            var ex = Record.Exception(() => LogglySender.Initialize(settings));

            // Assert
            ex.Should().BeOfType<ArgumentException>();
            ex.Message.Should().Be("'SecurePort' has not be set");
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
            var settings = BuildLogglySettings();
            LogglySender.Initialize(settings);

            var machineName = "MachineName";
            var applicationName = "ApplicationName";
            var processId = "ProcessId";
            var severity = Severity.Notice;
            var timestamp = DateTime.UtcNow;
            var messagePayload = "{ \"property1\": true, \"property2\": \"some text\" }";
            var messageType = "MessageType";
            var tags = new[] { new LogglyTag("tag1"), new LogglyTag("tag2") };

            // Act, Assert
            LogglySender.SendLogMessageToLoggly(machineName, applicationName, processId, severity, timestamp, messagePayload, messageType, tags);
        }

        private static LogglySettings BuildLogglySettings()
        {
            var result = new LogglySettings
            {
                CustomerToken = "CUSTOMER_TOKEN_HERE",
                LogglyServerCertificatePemEncoded = "-----BEGIN CERTIFICATE-----\r\n  MIIFbDCCBFSgAwIBAgIJAMHkt98BolVLMA0GCSqGSIb3DQEBCwUAMIHGMQswCQYD\r\n  VQQGEwJVUzEQMA4GA1UECBMHQXJpem9uYTETMBEGA1UEBxMKU2NvdHRzZGFsZTEl\r\n  MCMGA1UEChMcU3RhcmZpZWxkIFRlY2hub2xvZ2llcywgSW5jLjEzMDEGA1UECxMq\r\n  aHR0cDovL2NlcnRzLnN0YXJmaWVsZHRlY2guY29tL3JlcG9zaXRvcnkvMTQwMgYD\r\n  VQQDEytTdGFyZmllbGQgU2VjdXJlIENlcnRpZmljYXRlIEF1dGhvcml0eSAtIEcy\r\n  MB4XDTE4MDIwODE1MTgwM1oXDTIwMDQxMDAwMTA0N1owQDEhMB8GA1UECxMYRG9t\r\n  YWluIENvbnRyb2wgVmFsaWRhdGVkMRswGQYDVQQDExJsb2dzLTAxLmxvZ2dseS5j\r\n  b20wggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQDNe6dCgVg1Glrg5ow+\r\n  lAd4Hsitfy36Waawx0oyfux33SV9tiqRGjuhbiya2vQmksMqvL8L1ZKPdyLNAZHV\r\n  rYXBYBPPSOVF9eonhQlW/1nwA5hDPJSAdxJE5kpB4JW2niPsGBIeYVMzoWr6UFU2\r\n  fVxr2WS8j78FGjT8K/z3406liJSi8jWGSH0MLgA3m9AuZnqEsJiPsgu3GuKwrzAy\r\n  0BMswiXQ2Fff7oCVKhqtroQNnPhBTj/zL3OdTddbqViDBYBqD1oBk7qmzgCn+8OO\r\n  saEjcYDWjBQNFbB1vh5sSpwEOT7jzbIXhAkE9zasb1zKns9L6HPELsgZaylUa8Xr\r\n  uYdNAgMBAAGjggHgMIIB3DAMBgNVHRMBAf8EAjAAMB0GA1UdJQQWMBQGCCsGAQUF\r\n  BwMBBggrBgEFBQcDAjAOBgNVHQ8BAf8EBAMCBaAwPAYDVR0fBDUwMzAxoC+gLYYr\r\n  aHR0cDovL2NybC5zdGFyZmllbGR0ZWNoLmNvbS9zZmlnMnMxLTg0LmNybDBjBgNV\r\n  HSAEXDBaME4GC2CGSAGG/W4BBxcBMD8wPQYIKwYBBQUHAgEWMWh0dHA6Ly9jZXJ0\r\n  aWZpY2F0ZXMuc3RhcmZpZWxkdGVjaC5jb20vcmVwb3NpdG9yeS8wCAYGZ4EMAQIB\r\n  MIGCBggrBgEFBQcBAQR2MHQwKgYIKwYBBQUHMAGGHmh0dHA6Ly9vY3NwLnN0YXJm\r\n  aWVsZHRlY2guY29tLzBGBggrBgEFBQcwAoY6aHR0cDovL2NlcnRpZmljYXRlcy5z\r\n  dGFyZmllbGR0ZWNoLmNvbS9yZXBvc2l0b3J5L3NmaWcyLmNydDAfBgNVHSMEGDAW\r\n  gBQlRYFoUCY4PTstLL7Natm2PbNmYzA1BgNVHREELjAsghJsb2dzLTAxLmxvZ2ds\r\n  eS5jb22CFnd3dy5sb2dzLTAxLmxvZ2dseS5jb20wHQYDVR0OBBYEFHJVoumW5gC3\r\n  VeypY2XWDy/rwZaAMA0GCSqGSIb3DQEBCwUAA4IBAQBevXdgk2ojUQnmnCMGlbzW\r\n  pnH3G27fEvqi/HOgGDVy3wkZc6dgEqrsBFg4dBeZas0RQJEJqLVey9bu9Kol9PVK\r\n  M1F/UphbhRKxFBmqq1jO3PQAqb6NtnVK5NRK0pgm/SRDrCT9wp32drU3P4xT2KPN\r\n  gwdYtHiiWA7jFlftEdRQThkFh4Pdpxb0ioOpf+/Tj9bdw9ycHTVSVyJLD9vA6b75\r\n  Rb9ZN4GaGf/bPICXKYOL++T605C3iiiY4nbNZfT6H12Nxbjncr7z73gZ9EEpi47p\r\n  mqELSLeRV7IVsffcXuIBV1GRmhEht4vxEP46kEy58qgJhEvM1XUh04PYfeIPT0Ie\r\n  -----END CERTIFICATE-----",
            };

            return result;
        }
    }
}
