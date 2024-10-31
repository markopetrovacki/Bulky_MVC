using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
	public class EmailSender : IEmailSender
	{
		public async Task SendEmailAsync(string email, string subject, string htmlMessage)
		{
			//logic to send email
			//throw new NotImplementedException();

            Console.WriteLine($"Simulirano slanje e-pošte na: {email}");
            Console.WriteLine($"Predmet: {subject}");
            Console.WriteLine($"Poruka: {htmlMessage}");

            // Ili jednostavno vratite Task.CompletedTask ako ne želite nikakvu logiku ovde.
            await Task.CompletedTask;
        }
	}
}
