using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;


namespace MyWebApplication.Controllers
{
	public class UploadController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("OnPostUploadAsync")]
		public async Task<IActionResult> OnPostUploadAsync(IFormFile formFile)
		{
			long size = formFile.Length;
			var filePath = Path.GetTempFileName();

			using (var stream = System.IO.File.Create(filePath))
			
			{
				await formFile.CopyToAsync(stream);
			}

			// check if file is an image
			if (formFile.ContentType.Contains("image"))
			{
				// read data from file
				byte[] data_original = System.IO.File.ReadAllBytes(filePath);
				// resize image to 100x100
				byte[] data = ImageConverter.Resize(data_original, 100, 100);
				data = ImageConverter.Convert(data);
				// return image to client
				ViewData["image_data"] = $"data:image/png;base64,{Convert.ToBase64String(data_original)}";
				ViewData["image_data_sm"] = $"data:image/png;base64,{Convert.ToBase64String(data)}";
				return View("Image");
				//return Ok(new { formFile.FileName, size });
			}

			var mes = "Not and Image";
			// Process uploaded files
			// Don't rely on or trust the FileName property without validation.
			return Ok(new { mes });
		}
	}
}
