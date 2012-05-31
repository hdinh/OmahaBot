namespace OmahaBot.Core
{
    using System;
    using System.Runtime.Serialization;
    using System.Security.Permissions;
    using OmahaBot.Utilities;

    [Serializable]
    [CoverageExclude]
    public class OmahaException : Exception
    {
        public OmahaException() : base()
        {
        }

        public OmahaException(string message) : base(message)
        {
        }

        public OmahaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected OmahaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
