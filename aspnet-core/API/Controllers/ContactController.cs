using Domain.DomainModel.Entity;
using Domain.DomainModel.Interface;
using Infrastucture;
using Infrastucture.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly IContactRepository _contactRepository;

        public ContactController(DataContext dataContext, IContactRepository contactRepository)
        {
            _dataContext = dataContext;
            _contactRepository = contactRepository;
        }

        #region Get Data
        [HttpGet("GetContacts")]
        public async Task<ActionResult<List<Contact>>> GetContacts()
        {
            return Ok(await _contactRepository.GetContacts());
        }

        [HttpGet("GetContactById")]
        public async Task<ActionResult<Contact>> GetContactById(int contactId)
        {
            var contact = await _contactRepository.GetContact(contactId);
            if (contact == null)
                return NotFound("Contact not found!");
            return Ok(contact);
        }

        [HttpGet("GetContactsByCustomerId")]
        public async Task<ActionResult<List<Contact>>> GetContactsByCustomerId(int customerId)
        {
            var contacts = await _contactRepository.GetContactsByCustomerId(customerId);
            if (contacts == null)
                return NotFound("Contacts not found!");

            return Ok(contacts);
        }
        #endregion

        #region Save Data
        [HttpPost("CreateContact")]
        public async Task<ActionResult<List<Contact>>> CreateContact(Contact contact)
        {
            var contactList = await _contactRepository.Create(contact);

            //if (contactList == null)
            //    return NotFound("New contact not created!");

            return Ok(contactList);
        }

        [HttpPut("UpdateContact")]
        public async Task<ActionResult<List<Contact>>> UpdateContact(Contact contact)
        {
            var contactList = await _contactRepository.Update(contact);

            //if (contactList == null)
            //    return NotFound("Error encountered while updating contact!");

            return Ok(contactList);
        }

        [HttpDelete("DeleteContact")]
        public async Task<ActionResult<List<Contact>>> DeleteContact(List<int> contactIds)
        {
            var contactList = await _contactRepository.Delete(contactIds);

            //if (contactList == null)
            //    return NotFound("Error encountered when deleting contact!");

            return Ok(contactList);
        }
        #endregion
    }
}
