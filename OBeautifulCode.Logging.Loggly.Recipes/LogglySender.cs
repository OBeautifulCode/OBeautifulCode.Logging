﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogglySender.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Logging.Loggly.Recipes source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Loggly.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Security;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;

    using OBeautifulCode.Assertion.Recipes;
    using OBeautifulCode.Logging.Syslog.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// This class enables logging to Loggly using syslog formatted messages.
    /// </summary>
    /// <remarks>
    /// See Loggly documentation: <a href="https://www.loggly.com/docs/"/>.
    /// Adapted from <a href="https://github.com/emertechie/SyslogNet"/>.
    /// Adapted from <a href="https://github.com/graffen/NLog.Targets.Syslog"/>.
    /// Adapted from <a href="https://github.com/Curit/le_dotnet"/>.
    /// </remarks>
#if !OBeautifulCodeLoggingRecipesProject
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Logging.Loggly.Recipes", "See package version number")]
    internal
#else
    public
#endif
    static class LogglySender
    {
        private static X509Certificate2 logglyServerCertificate;

        private static TcpClient tcpClient;

        private static Stream tcpStream;

        private static LogglySettings settings;

        /// <summary>
        /// Initializes the sender.  Must be called before sending logs.
        /// </summary>
        /// <param name="logglySettings">The settings to use.</param>
        public static void Initialize(
            LogglySettings logglySettings)
        {
            new { logglySettings }.AsArg().Must().NotBeNull();
            new { logglySettings.CustomerToken }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { logglySettings.LogglyServerCertificatePemEncoded }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { logglySettings.LogglyPrivateEnterpriseNumber }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { logglySettings.SyslogServer }.AsArg().Must().NotBeNullNorWhiteSpace();
            new { logglySettings.SecurePort }.AsArg().Must().NotBeDefault();

            settings = logglySettings;
            logglyServerCertificate = new X509Certificate2(Encoding.UTF8.GetBytes(logglySettings.LogglyServerCertificatePemEncoded));
        }

        /// <summary>
        /// Sends a log message to Loggly.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name of the originator.  Can be null in the rare case that it's unknown.</param>
        /// <param name="applicationName">The name of the application that originated the message.  Can be null if unknown or explicitly withheld.</param>
        /// <param name="processIdentifier">The process name or process identifier, having no interoperable meaning, except that a change in the value indicates that there has been a discontinuity in the syslog reporting.  Can also be used to identify which messages belong to a group of messages.  Can be null if not available.</param>
        /// <param name="severity">The severity of the log message.</param>
        /// <param name="timestamp">The timestamp of the message, in UTC.</param>
        /// <param name="messagePayload">The message payload.  Can be as simple as a string message (e.g. "something happened") or an object serialized to json.</param>
        /// <param name="messageType">The type of message.  Can be null if unknown or not applicable.</param>
        /// <param name="logglyTags">Optional Loggly-compliant tags to apply to the log message.  Default is null (no tags).</param>
        public static void SendLogMessageToLoggly(
            string fullyQualifiedDomainName,
            string applicationName,
            string processIdentifier,
            Severity severity,
            DateTime timestamp,
            string messagePayload,
            string messageType,
            IReadOnlyCollection<LogglyTag> logglyTags = null)
        {
            if (settings == null)
            {
                throw new InvalidOperationException(Invariant($"{nameof(Initialize)} has not yet been called."));
            }

            logglyTags = logglyTags?.Where(_ => _ != null).ToList() ?? new List<LogglyTag>();

            // build the syslog message and queue-up
            var structuredDataId = settings.CustomerToken + "@" + settings.LogglyPrivateEnterpriseNumber;

            // add tags to structured data
            IReadOnlyList<KeyValuePair<string, string>> structuredData = null;
            if (logglyTags.Any())
            {
                structuredData = logglyTags.Select(_ => new KeyValuePair<string, string>("tag", _.Tag)).ToList();
            }

            // build the syslog message and queue up
            var messageBytes = SyslogMessageBuilder.BuildRfc5424Message(fullyQualifiedDomainName, applicationName, processIdentifier, timestamp, settings.Facility, severity, messagePayload, false, messageType, structuredDataId, structuredData);
            SendLogMessageToLoggly(messageBytes);
        }

        /// <summary>
        /// Processes the message queue, sending messages to Loggly.
        /// </summary>
        /// <param name="messageBytes">The syslog message encoded in bytes.</param>
        private static void SendLogMessageToLoggly(
            byte[] messageBytes)
        {
            try
            {
                // attempt to send log message to Loggly
                EnsureOpenAndWritableConnection();
                tcpStream.Write(messageBytes, 0, messageBytes.Length);

                // force a packet boundary so we can know that last packet made it to destination
                tcpStream.Flush();
            }
            catch (Exception)
            {
                // there's an issue sending the message
                // note that the message may have been partially sent and received
                CloseTcp();
                throw;
            }
        }

        /// <summary>
        /// Ensures that the TCP connection is open and that the underlying NetworkStream is writable.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Cannot make a static class IDisposble.  Anyways, the logic ensures that we are only ever dealing with one open TCP connection, so we won't exhaust those resources.  Also, the framework will handle closing/disposing the last open TCP connection just fine.")]
        private static void EnsureOpenAndWritableConnection()
        {
            // check existing TCP connection
            if (tcpClient != null)
            {
                // tcpStream is guaranteed to not be null - see below
                if (tcpClient.Connected && tcpStream.CanWrite)
                {
                    return;
                }

                CloseTcp();
            }

            // create a new TCP connection
            try
            {
                tcpClient = new TcpClient(settings.SyslogServer, settings.SecurePort) { NoDelay = true };
                tcpStream = new SslStream(
                    tcpClient.GetStream(),
                    false,
                    (sender, cert, chain, errors) => cert.GetCertHashString() == logglyServerCertificate.GetCertHashString());
                ((SslStream)tcpStream).AuthenticateAsClient(settings.SyslogServer);
            }
            catch (Exception)
            {
                // guarantees that tcpClient and tcpStream are both null or not null
                CloseTcp();
                throw;
            }
        }

        /// <summary>
        /// Closes the TCP connection.
        /// </summary>
        private static void CloseTcp()
        {
            if (tcpStream != null)
            {
                try
                {
                    tcpStream.Close();
                }
                catch (Exception)
                {
                }
            }

            if (tcpClient != null)
            {
                try
                {
                    tcpClient.Close();
                }
                catch (Exception)
                {
                }
            }

            tcpStream = null;
            tcpClient = null;
        }
    }
}