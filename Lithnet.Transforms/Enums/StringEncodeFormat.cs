// -----------------------------------------------------------------------
// <copyright file="HashType.cs" company="Lithnet.
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Specifies the format to use when converting a value to a string
    /// </summary>
    [DataContract(Name = "string-encode-format", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public enum StringEncodeFormat
    {
        /// <summary>
        /// Base-64
        /// </summary>
        [Description("Base-64")]
        [EnumMember(Value = "base64")]
        Base64,

        /// <summary>
        /// Base-32 algorithm
        /// </summary>
        [Description("Base-32")]
        [EnumMember(Value = "base32")]
        Base32,

        /// <summary>
        /// UTF 7
        /// </summary>
        [Description("UTF-7")]
        [EnumMember(Value = "utf7")]
        UTF7,

        /// <summary>
        /// UTF 8
        /// </summary>
        [Description("UTF-8")]
        [EnumMember(Value = "utf8")]
        UTF8,

        /// <summary>
        /// UTF 16
        /// </summary>
        [Description("UTF-16")]
        [EnumMember(Value = "utf16")]
        UTF16,

        /// <summary>
        /// UTF 32
        /// </summary>
        [Description("UTF-32")]
        [EnumMember(Value = "utf32")]
        UTF32,

        /// <summary>
        /// UTF 32
        /// </summary>
        [Description("ASCII")]
        [EnumMember(Value = "ascii")]
        ASCII,
    }
}
