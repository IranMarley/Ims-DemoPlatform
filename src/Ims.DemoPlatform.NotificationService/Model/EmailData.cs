namespace Ims.DemoPlatform.NotificationService.Model;

public record EmailData(
    string subject,
    string preheader,
    string email_tag,
    string title,
    string name,
    string body_html,
    string body_text,
    string? cta_text,
    string? cta_url,
    string brand_name,
    string brand_url,
    string brand_logo,
    string brand_address,
    string support_url,
    string? unsubscribe_url,
    string legal_reason
);