namespace BlueCardPortal.Infrastructure.Integrations.BlueCardCore
{
    internal class BlueCardToken
    {
        public string csrf_token { get; set; }
        public int expiration { get; set; }
    }
}
