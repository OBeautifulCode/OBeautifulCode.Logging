﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyslogMessageBuilder.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Logging.Syslog.Recipes source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Syslog.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using static System.FormattableString;

    /// <summary>
    /// This class provides a method to build a syslog message.
    /// </summary>
    /// <remarks>
    /// Adapted from <a href="https://github.com/emertechie/SyslogNet"/>.
    /// Adapted from <a href="https://github.com/graffen/NLog.Targets.Syslog"/>.
    /// Adapted from <a href="https://github.com/Curit/le_dotnet"/>.
    /// </remarks>
#if !OBeautifulCodeLoggingRecipesProject
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Logging.Syslog.Recipes", "See package version number")]
    internal
#else
    public
#endif
    static class SyslogMessageBuilder
    {
        /// <summary>
        /// The version of syslog to use, see RFC5234.
        /// </summary>
        private const string SyslogVersion = "1";

        /// <summary>
        /// Nil value per RFC5234.
        /// </summary>
        private const string NilValue = "-";

        private static readonly CultureInfo UnitedStatesEnglishCultureInfo = new CultureInfo("en-US");

        /// <summary>
        /// Builds a syslog message that complies with RFC5424.
        /// </summary>
        /// <param name="fullyQualifiedDomainName">The fully qualified domain name of the originator.  Can be null in the rare case that it's unknown.</param>
        /// <param name="applicationName">The name of the application that originated the message.  Can be null if unknown or explicitly withheld.</param>
        /// <param name="processIdentifier">The process name or process identifier, having no interoperable meaning, except that a change in the value indicates that there has been a discontinuity in the syslog reporting.  Can also be used to identify which messages belong to a group of messages.  Can be null if not available.</param>
        /// <param name="timestamp">The timestamp of the message, in UTC.</param>
        /// <param name="facility">The facility to use.</param>
        /// <param name="severity">The severity to use.</param>
        /// <param name="logMessage">The message to log.  Can be null if none exists.</param>
        /// <param name="encodeMessageInUtf8">Determines if the logMessage should be encoded in UTF-8.</param>
        /// <param name="messageId">The message identifier, which identifies the type of message.  Messages with the same identifier should reflect events of the same semantics.  Can be null if none exists.</param>
        /// <param name="structuredDataId">The structured data identifier.  Can be null if none exists.</param>
        /// <param name="structuredData">The structured data as an ordered set of key/value pairs.  Can be null if none exists.</param>
        /// <returns>The syslog message encoded in bytes.</returns>
        public static byte[] BuildRfc5424Message(
            string fullyQualifiedDomainName,
            string applicationName,
            string processIdentifier,
            DateTime timestamp,
            Facility facility,
            Severity severity,
            string logMessage,
            bool encodeMessageInUtf8,
            string messageId,
            string structuredDataId,
            IReadOnlyList<KeyValuePair<string, string>> structuredData)
        {
            if (timestamp.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException(Invariant($"{nameof(timestamp)}.{nameof(DateTime.Kind)} is {timestamp.Kind}, expecting {DateTimeKind.Utc}."));
            }

            var bytes = new List<byte[]>();

            // build header tokens
            var priorityToken = BuildPriorityToken(facility, severity);
            var timestampToken = timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", UnitedStatesEnglishCultureInfo);
            var fullyQualifiedDomainNameToken = FormatSyslogAsciiField(fullyQualifiedDomainName, 255, false);
            var applicationNameToken = FormatSyslogAsciiField(applicationName, 48, false);
            var processIdentifierToken = FormatSyslogAsciiField(processIdentifier, 48, false);
            var messageIdToken = FormatSyslogAsciiField(messageId, 32, false);

            // build the header
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(priorityToken);
            messageBuilder.Append(SyslogVersion);
            messageBuilder.Append(" ");
            messageBuilder.Append(timestampToken);
            messageBuilder.Append(" ");
            messageBuilder.Append(fullyQualifiedDomainNameToken);
            messageBuilder.Append(" ");
            messageBuilder.Append(applicationNameToken);
            messageBuilder.Append(" ");
            messageBuilder.Append(processIdentifierToken);
            messageBuilder.Append(" ");
            messageBuilder.Append(messageIdToken);
            messageBuilder.Append(" ");

            bytes.Add(Encoding.ASCII.GetBytes(messageBuilder.ToString()));

            // build structured data
            // there are two kinds of SD-ID, one with the @ sign and one without
            // and rules differ between the two.  Here we are just fixing for control characters
            // and some special characters that are not allowed.
            var structuredDataIdToken = FormatSyslogAsciiField(structuredDataId, 42, true);
            if (structuredDataIdToken == NilValue)
            {
                // if the data id is empty then insert Nil
                bytes.Add(Encoding.ASCII.GetBytes(NilValue));
            }
            else
            {
                // start the structured data token and insert the id
                bytes.Add(Encoding.ASCII.GetBytes("[" + structuredDataIdToken));

                // add all structured data key/value pairs
                if (structuredData != null)
                {
                    foreach (var structuredDataItem in structuredData)
                    {
                        // is the param name valid?  if not, skip this kvp
                        var paramNameToken = FormatSyslogAsciiField(structuredDataItem.Key, 32, true);
                        if (paramNameToken == NilValue)
                        {
                            continue;
                        }

                        // param name is valid, insert SP Name="
                        bytes.Add(Encoding.ASCII.GetBytes(" " + paramNameToken + @"="""));
                        var structuredDataItemValue = structuredDataItem.Value;
                        var structuredDataValueBuilder = new StringBuilder();
                        if (structuredDataItemValue != null)
                        {
                            // the param value must be encoded in UTF-8
                            foreach (var c in structuredDataItemValue)
                            {
                                // need to escape ", \, and ]
                                if ((c == 34) || (c == 92) || (c == 93))
                                {
                                    structuredDataValueBuilder.Append('\\');
                                }

                                structuredDataValueBuilder.Append(c);
                            }

                            bytes.Add(Encoding.UTF8.GetBytes(structuredDataValueBuilder.ToString()));
                        }

                        // add the ending quotes to complete encoding the kvp
                        bytes.Add(Encoding.ASCII.GetBytes(@""""));
                    }
                }

                // add the ending square bracket to complete the structured data token
                bytes.Add(Encoding.ASCII.GetBytes(@"]"));
            }

            // build the logged message
            if (logMessage != null)
            {
                bytes.Add(Encoding.ASCII.GetBytes(" "));
                if (encodeMessageInUtf8)
                {
                    bytes.Add(Encoding.UTF8.GetPreamble());
                    bytes.Add(Encoding.UTF8.GetBytes(logMessage));
                }
                else
                {
                    bytes.Add(Encoding.ASCII.GetBytes(logMessage));
                }
            }

            // newline is used by syslog to determine when an event is terminated
            bytes.Add(Encoding.ASCII.GetBytes("\n"));

            // build the syslog message
            var result = Combine(bytes.ToArray());

            return result;
        }

        /// <summary>
        /// Builds a syslog priority token string from the facility and severity of the message.
        /// </summary>
        /// <param name="facility">The syslog facility of a particular message.</param>
        /// <param name="severity">The syslog severity of a particular message.</param>
        /// <returns>A string with the priority token.</returns>
        private static string BuildPriorityToken(
            Facility facility,
            Severity severity)
        {
            var calculatedPriority = ((int)facility * 8) + (int)severity;

            var result = "<" + calculatedPriority.ToString(CultureInfo.InvariantCulture) + ">";

            return result;
        }

        /// <summary>
        /// Formats a Syslog ASCII field according to the RFC5234.
        /// </summary>
        /// <param name="fieldValue">The data to format.</param>
        /// <param name="maxLength">The max length of the field.</param>
        /// <param name="isStructuredDataName">Is this field a structured data name?  RFC imposes other restrictions.</param>
        /// <returns>The field formatted in ASCII and compliant with RFC5234.</returns>
        private static string FormatSyslogAsciiField(
            string fieldValue,
            int maxLength,
            bool isStructuredDataName)
        {
            var result = fieldValue;

            // just return Nil if fieldValue is empty
            if (result == null)
            {
                return NilValue;
            }

            // strip out characters that are not allowed
            // include all printing characters except space and delete
            var charBuffer = new char[result.Length];
            var bufferIndex = 0;
            foreach (char c in result)
            {
                if (c >= 33 && c <= 126)
                {
                    // strucured data parameter names have more restrictions: can't use '=', ']', or '"'
                    if (isStructuredDataName && ((c == 61) || (c == 93) || (c == 34)))
                    {
                        continue;
                    }

                    charBuffer[bufferIndex++] = c;
                }
            }

            // reconstruct the string
            result = new string(charBuffer, 0, bufferIndex);

            // fieldValue must respect the maxLength
            if (result.Length > maxLength)
            {
                result = result.Substring(0, maxLength);
            }

            // after all the processing, are we just left with an empty string?
            if (string.IsNullOrEmpty(result))
            {
                return NilValue;
            }

            return result;
        }

        /// <summary>
        /// Combines an array of byte[] into a single byte[].
        /// </summary>
        /// <remarks>
        /// See here: <a href="http://stackoverflow.com/a/415839/356790" />.
        /// </remarks>
        /// <param name="arrays">The byte[]s to combine.</param>
        /// <returns>The result of combining all byte[] in arrays.</returns>
        private static byte[] Combine(
            params byte[][] arrays)
        {
            var result = new byte[arrays.Sum(x => x.Length)];
            var offset = 0;
            foreach (var data in arrays)
            {
                Buffer.BlockCopy(data, 0, result, offset, data.Length);
                offset += data.Length;
            }

            return result;
        }
    }
}