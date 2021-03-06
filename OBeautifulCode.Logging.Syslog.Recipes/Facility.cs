﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Facility.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// <auto-generated>
//   Sourced from NuGet package. Will be overwritten with package update except in OBeautifulCode.Logging.Syslog.Recipes source.
// </auto-generated>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Syslog.Recipes
{
    using global::System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Syslog facilities.
    /// </summary>
    /// <remarks>
    /// Adapted from enumeration of same name in <a href="https://github.com/emertechie/SyslogNet"/>.
    /// </remarks>
#if !OBeautifulCodeLoggingSolution
    [global::System.CodeDom.Compiler.GeneratedCode("OBeautifulCode.Logging.Syslog.Recipes", "See package version number")]
    internal
#else
    public
#endif
    enum Facility
    {
        /// <summary>
        /// kernel messages
        /// </summary>
        Kernel = 0,

        /// <summary>
        /// random user-level messages
        /// </summary>
        User = 1,

        /// <summary>
        /// mail system
        /// </summary>
        Mail = 2,

        /// <summary>
        /// system daemons
        /// </summary>
        Daemons = 3,

        /// <summary>
        /// security/authorization messages
        /// </summary>
        Authorization = 4,

        /// <summary>
        /// messages generated internally by syslog
        /// </summary>
        Syslog = 5,

        /// <summary>
        /// line printer subsystem
        /// </summary>
        Printer = 6,

        /// <summary>
        /// network news subsystem
        /// </summary>
        News = 7,

        /// <summary>
        /// UUCP subsystem
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Uucp", Justification = "This is spelled correctly.")]
        Uucp = 8,

        /// <summary>
        /// clock daemon
        /// </summary>
        Clock = 9,

        /// <summary>
        /// security/authorization  messages (private)
        /// </summary>
        Authorization2 = 10,

        /// <summary>
        /// ftp daemon
        /// </summary>
        Ftp = 11,

        /// <summary>
        /// NTP subsystem
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Ntp", Justification = "This is spelled correctly.")]
        Ntp = 12,

        /// <summary>
        /// log audit
        /// </summary>
        Audit = 13,

        /// <summary>
        /// log alert
        /// </summary>
        Alert = 14,

        /// <summary>
        /// clock daemon
        /// </summary>
        Clock2 = 15,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local0 = 16,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local1 = 17,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local2 = 18,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local3 = 19,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local4 = 20,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local5 = 21,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local6 = 22,

        /// <summary>
        /// reserved for local use
        /// </summary>
        Local7 = 23,
    }
}
