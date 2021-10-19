namespace MarketingBox.Registration.Service.Messages.Common
{
    public enum LeadCrmStatus
    {
        New,
        FullyActivated,
        NoAnswer,
        HighPriority,
        Callback, // "Potential", "No Money", "Not Reached", "Objections"
        AutoCall, // "Answered", "Hung Up", "Agreement"
        FailedExpectation,
        NotValidDeletedAccount,
        NotValidWrongNumber,
        NotValidNoPhonenumber,
        NotValidDuplicateUser,
        NotValidTestLead,
        NotValidUnderage,
        NotValidNoLanguageSupport,
        NotValidNeverRegistered,
        NotValidNonEligibleCountries,
        NotInterested,
        Transfer,
        FollowUp,
        ConversionRenew
    }
}