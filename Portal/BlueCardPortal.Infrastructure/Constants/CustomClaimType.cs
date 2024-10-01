namespace BlueCardPortal.Infrastructure.Constants
{
    public static class CustomClaimType
    {
        public static class IdStampit
        {
            public const string PersonalId = "urn:stampit:pid";

            public const string Organization = "urn:stampit:organization";

            public const string PublicKey = "urn:stampit:public_key";

            public const string Certificate = "urn:stampit:certificate";

            public const string CertificateNumber = "urn:stampit:certno";
        }

        public const string FullName = "urn:io:full_name";

        public const string AdministrationId = "urn:io:administration_id";

        public const string DivisionId = "urn:io:division_id";

        public static class EAuth
        {
            /// <summary>
            /// Person identifier
            /// </summary>
            public const string PersonIdentifier = "urn:eauth:personIdentifier";

            /// <summary>
            /// Person name
            /// </summary>
            public const string PersonName = "urn:eauth:personName";

            /// <summary>
            /// Identifier type
            /// </summary>
            public const string IdentifierType = "urn:eauth:identifierType";

            /// <summary>
            /// Email
            /// </summary>
            public const string Email = "urn:eauth:email";

            /// <summary>
            /// EIK of the organization
            /// </summary>
            public const string Eik = "urn:eauth:eik";

            /// <summary>
            /// Organization name
            /// </summary>
            public const string OrganizationName = "urn:eauth:organizationName";
        }
    }
}
