// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogglyTagTest.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Loggly.Recipes.Test
{
    using System;

    using FluentAssertions;

    using Xunit;

    public static class LogglyTagTest
    {
        [Fact]
        public static void Constructor___Should_throw_ArgumentNullException___When_parameter_tag_is_null()
        {
            // Arrange, Act
            var actual = Record.Exception(() => new LogglyTag(null));

            // Assert
            actual.Should().BeOfType<ArgumentNullException>();
            actual.Message.Should().Contain("tag");
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_parameter_tag_is_white_space()
        {
            // Arrange, Act
            var actual = Record.Exception(() => new LogglyTag("  \r\n "));

            // Assert
            actual.Should().BeOfType<ArgumentException>();
            actual.Message.Should().Contain("tag");
            actual.Message.Should().Contain("white space");
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_parameter_tag_is_greater_than_64_characters_long()
        {
            // Arrange,
            var tag = "zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP6b";
            var expectedExceptionMessage = "The specified tag is too long.  It has 65 characters.  The valid character set for a tag value includes all alpha-numeric characters, dash, period, and underscore. There is an exception to this, where the first character of a tag may only be alpha-numeric. The maximum length of an individual tag is 64 characters.  Specified tag: " + tag;

            // Act
            var actual = Record.Exception(() => new LogglyTag(tag));

            // Assert
            actual.Should().BeOfType<ArgumentException>();
            actual.Message.Should().Be(expectedExceptionMessage);
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_parameter_tag_has_first_character_that_is_not_alphanumeric()
        {
            // Arrange,
            var tag1 = "-zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP";
            var tag2 = ".zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP";
            var tag3 = "_zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP";
            var tag4 = ":zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP";

            var expectedExceptionMessagePrefix = "The first character of the specified tag is not alpha-numeric.  The valid character set for a tag value includes all alpha-numeric characters, dash, period, and underscore. There is an exception to this, where the first character of a tag may only be alpha-numeric. The maximum length of an individual tag is 64 characters.  Specified tag: ";

            var expectedExceptionMessage1 = expectedExceptionMessagePrefix + tag1;
            var expectedExceptionMessage2 = expectedExceptionMessagePrefix + tag2;
            var expectedExceptionMessage3 = expectedExceptionMessagePrefix + tag3;
            var expectedExceptionMessage4 = expectedExceptionMessagePrefix + tag4;

            // Act
            var actual1 = Record.Exception(() => new LogglyTag(tag1));
            var actual2 = Record.Exception(() => new LogglyTag(tag2));
            var actual3 = Record.Exception(() => new LogglyTag(tag3));
            var actual4 = Record.Exception(() => new LogglyTag(tag4));

            // Assert
            actual1.Should().BeOfType<ArgumentException>();
            actual1.Message.Should().Be(expectedExceptionMessage1);

            actual2.Should().BeOfType<ArgumentException>();
            actual2.Message.Should().Be(expectedExceptionMessage2);

            actual3.Should().BeOfType<ArgumentException>();
            actual3.Message.Should().Be(expectedExceptionMessage3);

            actual4.Should().BeOfType<ArgumentException>();
            actual4.Message.Should().Be(expectedExceptionMessage4);
        }

        [Fact]
        public static void Constructor___Should_throw_ArgumentException___When_parameter_tag_contains_an_invalid_character()
        {
            // Arrange,
            var tag = "zlkoL761L2ccOw97SlIEKHxEP6VE:HedSopujllUblUNHUIp3gRDMc7r8QUMoYP6";

            var expectedExceptionMessage = "The tag contains an invalid character.  The valid character set for a tag value includes all alpha-numeric characters, dash, period, and underscore. There is an exception to this, where the first character of a tag may only be alpha-numeric. The maximum length of an individual tag is 64 characters.  Specified tag: " + tag;

            // Act
            var actual = Record.Exception(() => new LogglyTag(tag));

            // Assert
            actual.Should().BeOfType<ArgumentException>();
            actual.Message.Should().Be(expectedExceptionMessage);
        }

        [Fact]
        public static void Tag___Should_return_same_tag_parameter_passed_to_constructor___When_getting()
        {
            // Arrange,
            var expected1 = "zlkoL761L2ccOw97SlIEKHxEP6VENHedSopujllUblUNHUIp3gRDMc7r8QUMoYP6";
            var expected2 = "zlkoL761L2cc-w97SlI.KHxEP6VEN_edSopujll_blUNHUIp.gRDMc7r8QUM-YP6";

            // Act
            var actual1 = new LogglyTag(expected1);
            var actual2 = new LogglyTag(expected2);

            // Assert
            actual1.Tag.Should().Be(expected1);
            actual2.Tag.Should().Be(expected2);
        }
    }
}
