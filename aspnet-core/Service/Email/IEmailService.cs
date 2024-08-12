using Domain.DomainModel.Entity;
using Domain.DomainModel.Entity.DTO;

namespace Service.Email
{
    public interface IEmailService
    {
        bool SendOrderEmail(Stream file, string fileName, Customer customer, string contactName, string email, string orderType);
        bool SendOrderEmailByContacts(Stream file, string fileName, Customer customer, List<Contact> contacts, string orderType);
        bool SendStatementEmail(Stream file, string fileName, Customer customer, DateTime statementDate, List<Contact> contacts);
        bool SendUserNotificationEmail(List<UserNotificationEmailDTO> userNotificationEmails);
        bool SendUserRegistrationEmail(RegisterUserDTO user);
        
        
    }
}
