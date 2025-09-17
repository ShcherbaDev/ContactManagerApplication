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
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace ContactManagerApplication.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly ContactContext _context;

		public HomeController(ILogger<HomeController> logger, ContactContext context)
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

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
