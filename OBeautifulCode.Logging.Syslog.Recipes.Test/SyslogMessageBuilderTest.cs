// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SyslogMessageBuilderTest.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Syslog.Recipes.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Diagnostics.Recipes;

    using OBeautifulCode.AutoFakeItEasy;
    using OBeautifulCode.Logging.Recipes;

    using Xunit;

    public static class SyslogMessageBuilderTest
    {
        private static readonly string FullyQualifiedDomainName = MachineName.GetFullyQualifiedDomainName();

        private static readonly string ApplicationName = ProcessHelpers.GetRunningProcess().GetName();

        private static readonly string ProcessId = ProcessHelpers.GetRunningProcess().Id.ToString();

        [Fact]
        public static void BuildRfc5424Message___Should_throw_ArgumentException___When_parameter_timestamp_DateTimeKind_is_not_Utc()
        {
            // Arrange
            var timestamp1 = new DateTime(1, DateTimeKind.Local);
            var timestamp2 = new DateTime(1, DateTimeKind.Unspecified);
            var facility = A.Dummy<Facility>();
            var severity = A.Dummy<Severity>();
            var logMessage = A.Dummy<string>();
            var encodeMessageInUtf8 = A.Dummy<bool>();
            var messageId = A.Dummy<string>();
            var structuredDataId = A.Dummy<string>();
            var structuredData = Some.ReadOnlyDummies<KeyValuePair<string, string>>();

            // Act
            var ex1 = Record.Exception(() => SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timestamp1, facility, severity, logMessage, encodeMessageInUtf8, messageId, structuredDataId, structuredData));
            var ex2 = Record.Exception(() => SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timestamp2, facility, severity, logMessage, encodeMessageInUtf8, messageId, structuredDataId, structuredData));

            // Assert
            ex1.Should().BeOfType<ArgumentException>();
            ex1.Message.Should().Be("timestamp.Kind is Local, expecting Utc.");

            ex2.Should().BeOfType<ArgumentException>();
            ex2.Message.Should().Be("timestamp.Kind is Unspecified, expecting Utc.");
        }

        [Fact]
        public static void BuildRfc5424Message___ProperlyEncodesPriorityFromFacilityAndSeverityInHeader()
        {
            // Arrange
            byte[] expected1 = Encoding.ASCII.GetBytes("<0>");
            byte[] expected2 = Encoding.ASCII.GetBytes("<4>");
            byte[] expected3 = Encoding.ASCII.GetBytes("<7>");

            byte[] expected4 = Encoding.ASCII.GetBytes("<184>");
            byte[] expected5 = Encoding.ASCII.GetBytes("<188>");
            byte[] expected6 = Encoding.ASCII.GetBytes("<191>");

            byte[] expected7 = Encoding.ASCII.GetBytes("<64>");
            byte[] expected8 = Encoding.ASCII.GetBytes("<68>");
            byte[] expected9 = Encoding.ASCII.GetBytes("<71>");

            byte[] expected10 = Encoding.ASCII.GetBytes("<120>");
            byte[] expected11 = Encoding.ASCII.GetBytes("<124>");
            byte[] expected12 = Encoding.ASCII.GetBytes("<127>");

            // Act
            byte[] message1 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Kernel, Severity.Emergency, "my message", true, null, null, null);
            byte[] message2 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Kernel, Severity.Warning, "my message", true, null, null, null);
            byte[] message3 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Kernel, Severity.Debug, "my message", true, null, null, null);

            byte[] message4 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Local7, Severity.Emergency, "my message", true, null, null, null);
            byte[] message5 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Local7, Severity.Warning, "my message", true, null, null, null);
            byte[] message6 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Local7, Severity.Debug, "my message", true, null, null, null);

            byte[] message7 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Uucp, Severity.Emergency, "my message", true, null, null, null);
            byte[] message8 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Uucp, Severity.Warning, "my message", true, null, null, null);
            byte[] message9 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Uucp, Severity.Debug, "my message", true, null, null, null);

            byte[] message10 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Clock2, Severity.Emergency, "my message", true, null, null, null);
            byte[] message11 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Clock2, Severity.Warning, "my message", true, null, null, null);
            byte[] message12 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, DateTime.UtcNow, Facility.Clock2, Severity.Debug, "my message", true, null, null, null);

            // Assert
            Assert.True(message1.Take(expected1.Length).SequenceEqual(expected1));
            Assert.True(message2.Take(expected2.Length).SequenceEqual(expected2));
            Assert.True(message3.Take(expected3.Length).SequenceEqual(expected3));

            Assert.True(message4.Take(expected4.Length).SequenceEqual(expected4));
            Assert.True(message5.Take(expected5.Length).SequenceEqual(expected5));
            Assert.True(message6.Take(expected6.Length).SequenceEqual(expected6));

            Assert.True(message7.Take(expected7.Length).SequenceEqual(expected7));
            Assert.True(message8.Take(expected8.Length).SequenceEqual(expected8));
            Assert.True(message9.Take(expected9.Length).SequenceEqual(expected9));

            Assert.True(message10.Take(expected10.Length).SequenceEqual(expected10));
            Assert.True(message11.Take(expected11.Length).SequenceEqual(expected11));
            Assert.True(message12.Take(expected12.Length).SequenceEqual(expected12));
        }

        [Fact]
        public static void BuildRfc5424Message___ProperlyEncodesVersionAndTimestampInHeader()
        {
            // Arrange
            var timeStamp1 = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            byte[] expected1 = Encoding.ASCII.GetBytes("<188>1 2014-05-20T07:42:06.083Z");

            var timeStamp2 = new DateTime(2014, 10, 2, 14, 5, 39, 841, DateTimeKind.Utc);
            byte[] expected2 = Encoding.ASCII.GetBytes("<188>1 2014-10-02T14:05:39.841Z");

            // Act
            byte[] message1 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp1, Facility.Local7, Severity.Warning, "my message", true, null, null, null);
            byte[] message2 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp2, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            Assert.True(message1.Take(expected1.Length).SequenceEqual(expected1));
            Assert.True(message2.Take(expected2.Length).SequenceEqual(expected2));
        }

        [Fact]
        public static void BuildRfc5424Message___ProperlyEncodesHostNameAppNameAndProcessIdInHeader()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            byte[] expected = Encoding.ASCII.GetBytes($"<188>1 2014-05-20T07:42:06.083Z {FullyQualifiedDomainName} {ApplicationName} {ProcessId}");

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___Should_use_Nil_for_FQDN___When_fullyQualifiedDomainName_is_null()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            var expected = Encoding.ASCII.GetBytes($"<188>1 2014-05-20T07:42:06.083Z - {ApplicationName} {ProcessId}");

            // Act
            var actual = SyslogMessageBuilder.BuildRfc5424Message(null, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            actual.Take(expected.Length).Should().Equal(expected);
        }

        [Fact]
        public static void BuildRfc5424Message___Should_use_Nil_for_APP_NAME___When_applicationName_is_null()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            var expected = Encoding.ASCII.GetBytes($"<188>1 2014-05-20T07:42:06.083Z {FullyQualifiedDomainName} - {ProcessId}");

            // Act
            var actual = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, null, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            actual.Take(expected.Length).Should().Equal(expected);
        }

        [Fact]
        public static void BuildRfc5424Message___Should_use_Nil_for_PROCID___When_processIdentifier_is_null()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            var expected = Encoding.ASCII.GetBytes($"<188>1 2014-05-20T07:42:06.083Z {FullyQualifiedDomainName} {ApplicationName} -");

            // Act
            var actual = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, null, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            actual.Take(expected.Length).Should().Equal(expected);
        }

        [Fact(Skip = "No good way to test this, would need to fool .net to think that the host name is a very large string.")]
        public static void BuildRfc5424Message___WhenHostNameIsGreaterThan255Characters_LimitsHostNameTo255CharactersInHeader()
        {
        }

        [Fact(Skip = "No good way to test this, would fool .net to thinking that the process name is a very long string.")]
        public static void BuildRfc5424Message___WhenAppNameIsGreaterThan48Characters_LimitsAppNameTo48CharactersInHeader()
        {
        }

        [Fact(Skip = "No good way to test this, would need to fool .net to think that the process id is a very large string.")]
        public static void BuildRfc5424Message___WhenProcessIdIsGreaterThan128Characters_LimitsProcessIdTo128CharactersInHeader()
        {
        }

        [Fact]
        public static void BuildRfc5424Message___WhenMessageIdIsNull_InsertsNilInHeader()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " -";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___WhenMessageIdContainsOnlyInvalidCharacters_InsertsNilInHeader()
        {
            // Arrange
            string messageId = " ";
            for (int i = 1; i <= 32; i++)
            {
                messageId += (char)i;
            }

            messageId += (char)127;
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " -";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, messageId, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___WhenMessageIdIsNotNull_InsertsMessageIdInHeader()
        {
            // Arrange
            const string MessageId = "29292817";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " " + MessageId;
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, MessageId, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___WhenMessageIdContainsAsciiCharactersOutsideOf32To126_InsertsMessageIdWithInvalidCharactersRemoved()
        {
            // Arrange
            string messageId = "2!z )!@#$%" + ((char)1) + ((char)2) + "^&*" + ((char)31) + "()3a" + ((char)127);
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " " + "2!z)!@#$%^&*()3a";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, messageId, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___WhenMessageIdContainsMoreThan32Characters_InsertsMessageIdTruncatedTo32Characters()
        {
            // Arrange
            const string MessageId = "asdfg  hkl;qwertyuiop{]<.sdcvn.zxmncvb";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " " + "asdfghkl;qwertyuiop{]<.sdcvn.zxm";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, MessageId, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___StructuredDataIdIsNull_InsertsNilForStructuredData()
        {
            // Arrange
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " -" + " -";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, null, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___StructuredDataIdHasNoValidCharacters_InsertsNilForStructuredData()
        {
            // Arrange
            const string StructuredDataId = "= ]\"";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " -" + " -";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        public static void BuildRfc5424Message___StructuredDataHasSomeInvalidCharacters_RemovesInvalidCharacters()
        {
            // Arrange
            const string StructuredDataId = "This\"Is My@Ident ]ifier!{;=";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            expectedHeader = expectedHeader + " -" + " [ThisIsMy@Identifier!{;]";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact(Skip = "We have not fully implemented the SD-ID, which has some nuance depending on whether the @ sign is included in the identifier.  And Loggly violates the RFC, so we do not restrict to 32-characters.")]
        public static void BuildRfc5424Message___StructuredDataIdentifierHasMoreThan32ValidCharacters_RemovesInvalidCharacters()
        {
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParams_IsNull_NotIncludedInStructuredData()
        {
            // Arrange
            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}]", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, null);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParamsNamesAreAllNullOrEmptyOrContainAllInvalidCharacters_NoParamsIncludedInStructuredData()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>(null, "value1"),
                                         new KeyValuePair<string, string>("= ]\"", "value2"),
                                         new KeyValuePair<string, string>(string.Empty, "value2"),
                                         new KeyValuePair<string, string>(" \r\n   ", "value2"),
                                         new KeyValuePair<string, string>(null, "value3"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}]", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParamNameContainsInvalidCharacters_InvalidCharactersAreRemoved()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("= myName]\"", "value2"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " myName=";

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParamNameContainsMoreThan32ValidCharacters_ParamNameTruncatedTo32Characters()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("= myName]\"asdfghjkl;'qwertyuiop$#()*^!@12395813", "value2"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " myNameasdfghjkl;'qwertyuiop$#()*=";

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParamValueIsNull_ParamValueIsSetToEmptyString()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", null),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " myName1=\"\"";

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___StructuredDataParamValueContainsCharactersThatMustBeEscaped_CharactersAreEscaped()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", "val\"id]val\\id"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " myName1=\"val\\\"id\\]val\\\\id\"]";

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Params", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___MultipleValidStructuredDataParams_AllParamsAreEncoded()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", "myValue1"),
                                         new KeyValuePair<string, string>("myName2", "myValue2"),
                                         new KeyValuePair<string, string>("myName3", "myValue3"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3}", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " myName1=\"myValue1\" myName2=\"myValue2\" myName3=\"myValue3\"]";

            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, "my message", true, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.Take(expected.Length).SequenceEqual(expected));
        }

        [Fact(Skip = "Being lazy here - need to test this.")]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Param", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___EncodesStructuredDataParamValuesInUtf8()
        {
        }

        [Fact]
        public static void BuildRfc5424Message___MessageIsNull_LastElementInEncodedMessageIsStructuredDataTerminatedWithNewLine()
        {
            // Arrange
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", "myValue1"),
                                         new KeyValuePair<string, string>("myName2", "myValue2"),
                                         new KeyValuePair<string, string>("myName3", "myValue3"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} -", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id);
            string expectedHeader1 = expectedHeader + " [ThisIsMy@Identifier myName1=\"myValue1\" myName2=\"myValue2\" myName3=\"myValue3\"]\n";
            string expectedHeader2 = expectedHeader + " -\n";

            byte[] expected1 = Encoding.ASCII.GetBytes(expectedHeader1);
            byte[] expected2 = Encoding.ASCII.GetBytes(expectedHeader2);

            // Act
            byte[] message1 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, null, true, null, StructuredDataId, structuredData);
            byte[] message2 = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, null, true, null, null, structuredData);

            // Assert
            Assert.True(message1.SequenceEqual(expected1));
            Assert.True(message2.SequenceEqual(expected2));
        }

        [Fact]
        public static void BuildRfc5424Message___MessageIsNotNullAndUtf8EncodingIsFalse_MessageIsEncodedInAsciiAndAddedToTailWithNewLineTerminator()
        {
            // Arrange
            const bool EnecodeMessageInUtf8 = false;
            const string Message = "This is my messageDKLJ!!@#$%^&*()-=+_[]{}';/,";
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", "myValue1"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3} myName1=\"myValue1\"]", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            expectedHeader = expectedHeader + " " + Message + "\n";
            byte[] expected = Encoding.ASCII.GetBytes(expectedHeader);

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, Message, EnecodeMessageInUtf8, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.SequenceEqual(expected));
        }

        [Fact]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Bom", Justification = "This is spelled correctly.")]
        public static void BuildRfc5424Message___MessageIsNotNullAndUtf8EncodingIsTrue_MessageIsEncodedWithBomFollowedByUtf8StringAndAddedToTailWithNewLineTerminator()
        {
            // Arrange
            const bool EnecodeMessageInUtf8 = true;
            const string Message = "Tюhiસs iϖs my mஇeύᄻ";
            var structuredData = new List<KeyValuePair<string, string>>
                                     {
                                         new KeyValuePair<string, string>("myName1", "myValue1"),
                                     };

            const string StructuredDataId = "ThisIsMy@Identifier";
            var timeStamp = new DateTime(2014, 5, 20, 7, 42, 6, 83, DateTimeKind.Utc);
            string expectedHeader = string.Format("<188>1 2014-05-20T07:42:06.083Z {0} {1} {2} - [{3} myName1=\"myValue1\"] ", MachineName.GetFullyQualifiedDomainName(), Process.GetCurrentProcess().ProcessName, Process.GetCurrentProcess().Id, StructuredDataId);
            byte[] expected =
                Encoding.ASCII.GetBytes(expectedHeader)
                    .Concat(Encoding.UTF8.GetPreamble())
                    .Concat(Encoding.UTF8.GetBytes(Message + "\n"))
                    .ToArray();

            // Act
            byte[] message = SyslogMessageBuilder.BuildRfc5424Message(FullyQualifiedDomainName, ApplicationName, ProcessId, timeStamp, Facility.Local7, Severity.Warning, Message, EnecodeMessageInUtf8, null, StructuredDataId, structuredData);

            // Assert
            Assert.True(message.SequenceEqual(expected));
        }
    }
}
