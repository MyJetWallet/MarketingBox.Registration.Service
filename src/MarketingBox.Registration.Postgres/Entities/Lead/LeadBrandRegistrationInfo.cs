namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadBrandRegistrationInfo
    {
        public long AffiliateId { get; set; }
        public long CampaignId { get; set; }
        public long BoxId { get; set; }
        public string Brand { get; set; }
        public long BrandId { get; set; }
        public string CustomerId { get; set; }
        public string BrandResponse { get; set; }
    }
}