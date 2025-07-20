namespace Pinnacle.IServices
{
    public interface IWhatsappService
    {
        Task SendMessageAsync(string PhoneNumber, string Message);
        Task SendImageAsync(string PhoneNumber, string Caption, string ImagePath);
        Task SendDocumentAsync(string PhoneNumber, string FileName, string Caption, MemoryStream stream);
    }
}
