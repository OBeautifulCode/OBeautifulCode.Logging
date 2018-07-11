// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Severity.cs" company="OBeautifulCode">
//   Copyright (c) OBeautifulCode 2018. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace OBeautifulCode.Logging.Syslog
{
    /// <summary>
    /// Syslog severities.
    /// </summary>
    /// <remarks>
    ///  Adapted from enumeration of same name in <a href="https://github.com/emertechie/SyslogNet"/>.
    /// </remarks>
    public enum Severity
    {
        /// <summary>
        /// system is unusable
        /// </summary>
        Emergency = 0,

        /// <summary>
        /// action must be taken immediately
        /// </summary>
        Alert = 1,

        /// <summary>
        /// critical conditions
        /// </summary>
        Critical = 2,

        /// <summary>
        /// error conditions
        /// </summary>
        Error = 3,

        /// <summary>
        /// warning conditions
        /// </summary>
        Warning = 4,

        /// <summary>
        /// normal but significant condition
        /// </summary>
        Notice = 5,

        /// <summary>
        /// informational messages
        /// </summary>
        Informational = 6,

        /// <summary>
        /// debug-level messages
        /// </summary>
        Debug = 7,
    }
}
