using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Lithnet.Common.Presentation;
using Lithnet.MetadirectoryServices;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Transforms.Presentation
{
    public class XPathQueryTransformViewModel : TransformViewModel
    {
        private XPathQueryTransform model;

        private List<EnumExtension.EnumMember> allowedReturnTypes;

        public XPathQueryTransformViewModel(XPathQueryTransform model)
            : base(model)
        {
            this.model = model;

            if (this.model.UserDefinedReturnType == ExtendedAttributeType.Undefined)
            {
                this.model.UserDefinedReturnType = ExtendedAttributeType.String;
            }
        }

        public OnMissingMatch OnMissingMatch
        {
            get
            {
                return model.OnMissingMatch;
            }
            set
            {
                model.OnMissingMatch = value;
            }
        }

        public string XPathQuery
        {
            get
            {
                return model.XPathQuery;
            }
            set
            {
                model.XPathQuery = value;
            }
        }
       
        public string DefaultValue
        {
            get
            {
                return model.DefaultValue;
            }
            set
            {
                model.DefaultValue = value;
            }
        }

        public bool DefaultValueIsEnabled
        {
            get
            {
                return this.OnMissingMatch == Transforms.OnMissingMatch.UseDefault;
            }
        }

        public ExtendedAttributeType UserDefinedReturnType
        {
            get
            {
                return this.model.UserDefinedReturnType;
            }
            set
            {
                this.model.UserDefinedReturnType = value;
            }
        }

        public IList<EnumExtension.EnumMember> AllowedReturnTypes
        {
            get
            {
                if (this.allowedReturnTypes == null)
                {
                    this.allowedReturnTypes = new List<EnumExtension.EnumMember>();

                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.String.GetEnumDescription(), Value = ExtendedAttributeType.String });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Integer.GetEnumDescription(), Value = ExtendedAttributeType.Integer });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Boolean.GetEnumDescription(), Value = ExtendedAttributeType.Boolean });
                    this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Binary.GetEnumDescription(), Value = ExtendedAttributeType.Binary });
                    //this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.Reference.GetEnumDescription(), Value = ExtendedAttributeType.Reference });

                    if (TransformGlobal.HostProcessSupportsNativeDateTime)
                    {
                        this.allowedReturnTypes.Add(new EnumExtension.EnumMember() { Description = ExtendedAttributeType.DateTime.GetEnumDescription(), Value = ExtendedAttributeType.DateTime });
                    }
                }

                return this.allowedReturnTypes;
            }
        }

        public override string TransformDescription
        {
            get
            {
                return strings.XPathQueryTransformDescription;
            }
        }
    }
}
