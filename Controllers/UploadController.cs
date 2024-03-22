using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Models;
using System.Text;


namespace MyWebApplication.Controllers
{
	public class UploadController : Controller
	{
		private readonly ILogger<UploadController> _logger;

		const int CONV_SIZE = 50;

		public UploadController(ILogger<UploadController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("OnPostUploadAsync")]
		public async Task<IActionResult> OnPostUploadAsync(IFormFile formFile, int treshold)
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
				
				// resize image
				byte[] data = ImageConverter.Resize(data_original, CONV_SIZE, CONV_SIZE);
				data = ImageConverter.ConvertToGreyScale(data);
				//data = ImageConverter.ConvertToBW(data);
				string[,] bw_data_str = ImageConverter.ConvertToBwBitArray(data, treshold);
				
				// prepare image for client
				ViewData["image_data"] = $"data:image/png;base64,{Convert.ToBase64String(data_original)}";
				ViewData["image_data_sm"] = $"data:image/png;base64,{Convert.ToBase64String(data)}";

				// create board from reduced image
				Board myboard = new Board(CONV_SIZE);
				//Console.WriteLine("Length: " + bw_data_str.Length);

				for (int y = 0; y < bw_data_str.GetLength(0); y++)
				{
					for (int x = 0; x < bw_data_str.GetLength(1); x++)
					{
						var newColor = bw_data_str[y, x] == "1" ? "#FF3333" : "#FFFFFF";
						myboard.SetCell(x, y, newColor);
					}
				}	

				ViewData["Board"] = myboard.GetBoard();
				ViewData["Size"] = myboard.GetSize();

				return View("Image");
			}

			var mes = "Not and Image";
			// Process uploaded files
			// Don't rely on or trust the FileName property without validation.
			return Ok(new { mes });
		}
	}
}
