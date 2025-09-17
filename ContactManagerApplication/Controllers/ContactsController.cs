using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ContactManagerApplication.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ContactManagerApplication.Models;
using ContactManagerApplication.Utilities;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApplication.Controllers
{
	public class ContactsController : Controller
	{
		private readonly ILogger<ContactsController> _logger;
		private readonly ContactContext _context;

		public ContactsController(ILogger<ContactsController> logger, ContactContext context)
		{
			_logger = logger;
			_context = context;
		}

		public async Task<IActionResult> Index(CancellationToken cancellationToken)
		{
			List<Contact> contacts = await _context.Contacts.ToListAsync(cancellationToken);
			return View(contacts);
		}

		[HttpPost]
		public async Task<IActionResult> UploadCsv(IFormFile file, CancellationToken cancellationToken)
		{
			if (file == null || !file.FileName.EndsWith(".csv") || file.Length == 0)
			{
				return BadRequest();
			}

			try
			{
				await using Stream stream = file.OpenReadStream();
				using StreamReader reader = new StreamReader(stream);
				using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);

				await _context.Contacts.AddRangeAsync(csv.GetRecords<ContactDto>().Select(ContactDto.ToContactMap), cancellationToken);
				await _context.SaveChangesAsync(cancellationToken);

				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error processing CSV file");
				return BadRequest("Error processing CSV file");
			}
		}

		[HttpPatch]
		public async Task<IActionResult> Update(int id, [FromBody] JsonPatchDocument<Contact> patch, CancellationToken cancellationToken)
		{
			if (patch == null || patch.Operations.Count == 0)
			{
				return BadRequest("Invalid patch content");
			}

			Contact contact = await _context.Contacts.FindAsync(new object[] { id }, cancellationToken);
			if (contact == null)
			{
				return NotFound();
			}

			try
			{
				patch.ApplyTo(contact, ModelState);

				if (!ModelState.IsValid)
				{
					return BadRequest(ModelState);
				}

				foreach (Operation<Contact> _ in patch.Operations.Where(operation => operation.path.Equals("/Phone", StringComparison.OrdinalIgnoreCase)))
				{
					contact.Phone = contact.Phone.RemoveAllNonDigitSymbols();
				}

				contact.ModifiedDate = DateTime.UtcNow;
				await _context.SaveChangesAsync(cancellationToken);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error updating contact");
				return BadRequest("Error updating contact");
			}
		}

		[HttpDelete]
		public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
		{
			Contact contact = await _context.Contacts.FindAsync(new object[] { id }, cancellationToken);
			if (contact == null)
			{
				return NotFound();
			}

			try
			{
				_context.Contacts.Remove(contact);
				await _context.SaveChangesAsync(cancellationToken);
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error deleting a contact");
				return BadRequest("Error deleting a contact");
			}
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
