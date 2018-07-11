﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyslogMessageBuilder.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Logging source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Syslog.Recipes
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    using Naos.Diagnostics.Recipes;

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
    [System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Logging", "See package version number")]
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
        /// The fully qualified domain name of the local computer, formatted to comply with RFC5234.
        /// </summary>
        private static readonly string Fqdn = FormatSyslogAsciiField(MachineName.GetFullyQualifiedDomainName(), 255, false);

        /// <summary>
        /// The name of the current process, formatted to comply with RFC5234.
        /// </summary>
        private static readonly string ProcessName = FormatSyslogAsciiField(ProcessHelpers.GetRunningProcess().GetName(), 48, false);

        /// <summary>
        /// The process identifier, formatted to comply with RFC5234.
        /// </summary>
        private static readonly string ProcessId = FormatSyslogAsciiField(ProcessHelpers.GetRunningProcess().Id.ToString(UnitedStatesEnglishCultureInfo), 48, false);

        /// <summary>
        /// Builds a syslog message that complies with RFC5424.
        /// </summary>
        /// <param name="timestamp">The timestamp of the message.</param>
        /// <param name="facility">The facility to use.</param>
        /// <param name="severity">The severity to use.</param>
        /// <param name="logMessage">The message to log.  Can be null if none exists.</param>
        /// <param name="encodeMessageInUtf8">Determines if the logMessage should be encoded in UTF-8.</param>
        /// <param name="messageId">The message identifier.  Can be null if none exists.</param>
        /// <param name="structuredDataId">The structured data identifier.  Can be null if none exists.</param>
        /// <param name="structuredData">The structured data as an ordered set of key/value pairs.  Can be null if none exists.</param>
        /// <returns>The syslog message encoded in bytes.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Rfc", Justification = "This is spelled correctly.")]
        public static byte[] BuildRfc5424Message(
            DateTime timestamp,
            Facility facility,
            Severity severity,
            string logMessage,
            bool encodeMessageInUtf8,
            string messageId,
            string structuredDataId,
            IReadOnlyList<KeyValuePair<string, string>> structuredData)
        {
            var bytes = new List<byte[]>();

            // build the header
            var messageBuilder = new StringBuilder();
            messageBuilder.Append(BuildPriorityToken(facility, severity));
            messageBuilder.Append(SyslogVersion);
            messageBuilder.Append(" ");
            messageBuilder.Append(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", UnitedStatesEnglishCultureInfo));
            messageBuilder.Append(" ");
            messageBuilder.Append(Fqdn);
            messageBuilder.Append(" ");
            messageBuilder.Append(ProcessName);
            messageBuilder.Append(" ");
            messageBuilder.Append(ProcessId);
            messageBuilder.Append(" ");
            messageBuilder.Append(FormatSyslogAsciiField(messageId, 32, false));
            messageBuilder.Append(" ");
            bytes.Add(Encoding.ASCII.GetBytes(messageBuilder.ToString()));

            // build structured data
            // there are two kinds of SD-ID, one with the @ sign and one without
            // and rules differ between the two.  Here we are just fixing for control characters
            // and some special characters that are not allowed.
            structuredDataId = FormatSyslogAsciiField(structuredDataId, 42, true);
            if (structuredDataId == NilValue)
            {
                // if the data id is empty then insert Nil
                bytes.Add(Encoding.ASCII.GetBytes(NilValue));
            }
            else
            {
                // start the structured data token and insert the id
                bytes.Add(Encoding.ASCII.GetBytes("[" + structuredDataId));

                // add all structured data key/value pairs
                if (structuredData != null)
                {
                    foreach (var structuredDataItem in structuredData)
                    {
                        // is the param name valid?  if not, skip this kvp
                        var paramName = FormatSyslogAsciiField(structuredDataItem.Key, 32, true);
                        if (paramName == NilValue)
                        {
                            continue;
                        }

                        // param name is valid, insert SP Name="
                        bytes.Add(Encoding.ASCII.GetBytes(" " + paramName + @"="""));
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
