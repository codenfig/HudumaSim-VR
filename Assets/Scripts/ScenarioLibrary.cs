using System.Collections.Generic;

namespace HudumaSimVR
{
    public static class ScenarioLibrary
    {
        private static ReplyOption Good(string subject) => new ReplyOption(
            "I understand your concern. Let me confirm the details, explain the next steps, and arrange a documented follow-up for " + subject + ".",
            "Thank you for listening. Please explain what happens next.", 95, 95, 95, -28);
        private static ReplyOption Medium() => new ReplyOption(
            "I can check the matter, but you will need to wait while I investigate.",
            "How long will that take? I need a clear answer.", 60, 65, 45, -4);
        private static ReplyOption Bad() => new ReplyOption(
            "That is not my problem. You should have read the rules before coming here.",
            "That is unacceptable! I want to speak to your supervisor.", 5, 0, 5, 30);
        private static ReplyOption[][] Steps(string subject)
        {
            return new[]
            {
                new[] { Good(subject), Medium(), Bad() },
                new[] {
                    new ReplyOption("Let me summarize what I heard, check the available records, and tell you what can be verified today.",
                        "Yes, that is the issue. Please check it.", 95, 90, 80, -24),
                    new ReplyOption("You can fill out a form and wait for someone to contact you.",
                        "I have already waited. What about my complaint?", 45, 55, 35, 8),
                    Bad()
                },
                new[] {
                    new ReplyOption("I will record the complaint, explain the escalation channel and expected update process, and give you a reference number.",
                        "That helps. I know how my case will be followed up.", 90, 95, 100, -30),
                    new ReplyOption("We will see what happens. There is nothing more I can say.",
                        "I still do not know what happens next.", 40, 55, 20, 10),
                    Bad()
                }
            };
        }
        private static ComplaintScenario Make(string id, Sector sector, string title, string opening, string subject)
        {
            return new ComplaintScenario(id, sector, title, opening,
                new[] {
                    "The complainant is upset. How do you open the conversation?",
                    "The complainant asks how you will verify the issue. What do you say?",
                    "The complainant wants a clear next step. How do you conclude?"
                }, Steps(subject));
        }
        public static List<ComplaintScenario> All()
        {
            return new List<ComplaintScenario> {
                Make("TAX-01", Sector.TaxOmbudsman, "Wrong Tax Assessment", "My tax assessment is wrong! Nobody is explaining why I owe this amount!", "the disputed assessment"),
                Make("TAX-02", Sector.TaxOmbudsman, "Delayed Tax Refund", "I have waited six months for my tax refund. This is frustrating!", "the delayed refund"),
                Make("TAX-03", Sector.TaxOmbudsman, "Rude Tax Officer", "A tax officer spoke to me rudely. I deserve respectful treatment!", "the staff conduct complaint"),
                Make("BNK-01", Sector.Banking, "Failed Transaction", "The bank deducted money, but the transfer failed. Where is my money?", "the failed transfer"),
                Make("BNK-02", Sector.Banking, "Long Service Queue", "I have been waiting in this bank queue for hours!", "the service delay"),
                Make("BNK-03", Sector.Banking, "Unexpected Account Charges", "There are charges on my account that I did not authorize!", "the disputed account charges"),
                Make("TEL-01", Sector.Telecommunications, "Bundle Exhausted Too Quickly", "My internet bundle finished much faster than expected!", "the data bundle usage"),
                Make("TEL-02", Sector.Telecommunications, "Persistent Network Problem", "The network keeps dropping, and I cannot make important calls!", "the network coverage issue"),
                Make("TEL-03", Sector.Telecommunications, "Unexpected Airtime Deduction", "My airtime disappeared, and I did not buy anything!", "the airtime deduction")
            };
        }
    }
}