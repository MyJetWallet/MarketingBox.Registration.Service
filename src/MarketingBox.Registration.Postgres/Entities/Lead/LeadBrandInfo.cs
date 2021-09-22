namespace MarketingBox.Registration.Postgres.Entities.Lead
{
    public class LeadBrandInfo
    {
        public long AffiliateId { get; set; }
        public long CampaignId { get; set; }
        public long BoxId { get; set; }
        public string Brand { get; set; }
    }
}