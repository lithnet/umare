namespace Lithnet.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Security.Principal;
    using Lithnet.MetadirectoryServices;
    using Microsoft.MetadirectoryServices;
    using System.Collections;
    /// <summary>
    /// Extracts a domain name or domain SID from a user SID
    /// </summary>
    [DataContract(Name = "sid-to-domain", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("SID to domain")]
    public class SidToDomainTransform : Transform
    {
        private static Dictionary<SecurityIdentifier, string> resolvedNames = new Dictionary<SecurityIdentifier, string>();

        /// <summary>
        /// Initializes a new instance of the SidToDomainTransform class
        /// </summary>
        public SidToDomainTransform()
        {
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
            }
        }

        /// <summary>
        /// Gets or sets the format which to return the domain information as
        /// </summary>
        [DataMember(Name = "format")]
        public DomainFormat Format { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (inputValue is string)
            {
                return this.GetDomainSid(TypeConverter.ConvertData<string>(inputValue));
            }
            else if (inputValue is byte[])
            {
                return this.GetDomainSid(TypeConverter.ConvertData<byte[]>(inputValue));
            }
            else
            {
                throw new UnknownOrUnsupportedDataTypeException("Unsupported input format");
            }
        }

        /// <summary>
        /// Gets the domain information from the specified SID
        /// </summary>
        /// <param name="sid">The string representation of the SID</param>
        /// <returns>The domain information, in the format requested by the <see cref="Format">format</see> parameter</returns>
        private object GetDomainSid(string sid)
        {
            if (sid.StartsWith("s-", StringComparison.OrdinalIgnoreCase))
            {
                byte[] binaryValue = Utils.ConvertStringToSid(sid);
                return this.GetDomainSid(binaryValue);
            }
            else
            {
                try
                {
                    byte[] binaryValue = Convert.FromBase64String(sid);
                    return this.GetDomainSid(binaryValue);
                }
                catch (Exception ex)
                {
                    throw new UnknownOrUnsupportedDataTypeException("The input value was not a SID string or a base 64 encoded value", ex);
                }
            }
        }

        /// <summary>
        /// Gets the domain information from the specified SID
        /// </summary>
        /// <param name="sid">The binary representation of the SID</param>
        /// <returns>The domain information, in the format requested by the <see cref="Format">format</see> parameter</returns>
        private object GetDomainSid(byte[] sid)
        {
            SecurityIdentifier sidObject = new SecurityIdentifier(sid, 0);
            SecurityIdentifier domainSid = sidObject.AccountDomainSid;

            switch (this.Format)
            {
                case DomainFormat.DomainSidString:
                    return domainSid.Value;

                case DomainFormat.DomainSidBinary:
                    byte[] domainSidBytes = new byte[domainSid.BinaryLength];
                    domainSid.GetBinaryForm(domainSidBytes, 0);
                    return domainSidBytes;

                case DomainFormat.DomainName:
                    if (!SidToDomainTransform.resolvedNames.ContainsKey(domainSid))
                    {
                        string name;

                        try
                        {
                            NTAccount account = (NTAccount)sidObject.Translate(typeof(NTAccount));
                            name = account.ToString().Split('\\')[0];
                        }
                        catch
                        {
                            name = null;
                        }

                        SidToDomainTransform.resolvedNames.Add(domainSid, name);
                        return name;
                    }
                    else
                    {
                        return SidToDomainTransform.resolvedNames[domainSid];
                    }
                default:
                    throw new NotSupportedException();
            }
        }
    }
}