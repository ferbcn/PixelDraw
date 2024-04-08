using Microsoft.AspNetCore.Mvc;
using MyWebApplication.Models;
using MyWebApplication.Data;


namespace MyWebApplication.Controllers
{
	
	public class UploadController : Controller
	{
		private readonly ILogger<UploadController> _logger;

		const int CONV_SIZE = 50;
		
		private readonly MyWebApplicationContext _context;
		
		public UploadController(ILogger<UploadController> logger, MyWebApplicationContext context)
		{
			_logger = logger;
			_context = context;
		}

		private string[,] ImageToBitString(byte[] data, int threshold, bool invert=false)
		{	
			string[,] bw_data_str = ImageConverter.ConvertToBwBitArray(data, threshold);
			if (invert)
			{
				bw_data_str = InvertBitImage(bw_data_str, CONV_SIZE);
			}
			return bw_data_str;
		}
		
		private string[,] ImageToBitHexString(byte[] data, int threshold, bool invert, string color)
		{	
			string[,] bw_data_str = ImageConverter.ConvertToBwBitArray(data, threshold);
			if (invert)
			{
				bw_data_str = InvertBitImage(bw_data_str, CONV_SIZE);
			}
			for (int y = 0; y < CONV_SIZE; y++)
			{
				for (int x = 0; x < CONV_SIZE; x++)
				{
					bw_data_str[y, x] = bw_data_str[y, x] == "1" ? color : "#FFFFFF";
				}
			}

			return bw_data_str;
		}
		
		private string[,] InvertBitImage(string[,] imageData, Int16 imageSize)
		{
			for(Int16 y = 0; y < imageSize; y++)
			{
				for(Int16 x = 0; x < imageSize; x++)
				{
					imageData[x, y] = imageData[x, y] == "0" ? "1" : "0";
				}
			}

			return imageData;
		}
		
		public IActionResult Index()
		{
			ViewData["Title"] = "Upload Image";
			return View();
		}

		[HttpPost]
		[Route("OnPostUploadAsync")]
		public async Task<IActionResult> OnPostUploadAsync(IFormFile formFile, int threshold=50)
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
				byte[] img_data_raw = System.IO.File.ReadAllBytes(filePath);
				
				var filename = formFile.FileName;
				
				// save to temp.png
				System.IO.File.WriteAllBytes("Temp/"+filename, img_data_raw);
				
				// prepare image for client
				ViewData["image_data"] = $"data:image/png;base64,{Convert.ToBase64String(img_data_raw)}";
				
				// resize image and convert to greyscale byte string array
				byte[] img_data_small = ImageConverter.Resize(img_data_raw, CONV_SIZE, CONV_SIZE, false);
			
				// convert to color string array
				string[,] color_data_str = ImageConverter.ConvertToColorStringArray(img_data_small);

				// create board from reduced image
				Board_DTO myboard = new Board_DTO(CONV_SIZE, color_data_str);

				
				ViewData["Board"] = myboard.GetBoardString();
				ViewData["Size"] = myboard.GetSize();

				return View("Image");
			}

			var mes = "Not and Image";
			// Process uploaded files
			// Don't rely on or trust the FileName property without validation.
			return Ok(new { mes });
		}
		
		[HttpPost]
		[Route("UpdateThreshold")]
		public async Task<IActionResult> UpdateThreshold(int threshold, string filename, bool invert, bool toBw, bool cropImg, string selectedColor)
		{
			// load image from temp.png
			byte[] img_data_raw = System.IO.File.ReadAllBytes("Temp/" + filename);
			
			// resize image and convert to greyscale byte string array
			byte[] data = ImageConverter.Resize(img_data_raw, CONV_SIZE, CONV_SIZE, cropImg);
			string[,] color_data_str;
			
			if (toBw)
			{
				data = ImageConverter.ConvertToGreyScale(data);
				color_data_str = ImageToBitHexString(data, threshold, invert, selectedColor);
			}
			else
			{
				// convert to color string array
				color_data_str = ImageConverter.ConvertToColorStringArray(data);
			}
			
			// create board from reduced image
			Board_DTO myboard = new Board_DTO(CONV_SIZE, color_data_str);
			
			ViewData["Board"] = myboard.GetBoardString();
			ViewData["Size"] = myboard.GetSize();

			return View("Board");
		}
		
		[HttpPost]
		public async Task<IActionResult> Save (int threshold, string filename, bool invert, bool toBw, bool cropImg, string selectedColor)
		{
			// Console.WriteLine("Save image: " + filename);
			// load image from temp.png
			byte[] img_data_raw = System.IO.File.ReadAllBytes("Temp/" + filename);
			
			// resize image and convert to greyscale byte string array
			byte[] data = ImageConverter.Resize(img_data_raw, CONV_SIZE, CONV_SIZE, cropImg);
			string[,] color_data_str;
			
			if (toBw)
			{
				data = ImageConverter.ConvertToGreyScale(data);
				color_data_str = ImageToBitHexString(data, threshold, invert, selectedColor);
			}
			else
			{
				// convert to color string array
				color_data_str = ImageConverter.ConvertToColorStringArray(data);
			}

			// create board from reduced image
			Board_DTO myboard = new Board_DTO(CONV_SIZE, color_data_str);

			string[,] boardStr = myboard.GetBoardString();
			
			// Create new board in DB
			Board newBoard = new Board();
			newBoard.Size = CONV_SIZE;
			newBoard.Name = filename.Split(".")[0];
			
			_context.Add(newBoard);
			await _context.SaveChangesAsync();
			
			// save board cells to DB
			for (Int16 y=0; y<CONV_SIZE; y++)
			{
				for (Int16 x=0; x<CONV_SIZE; x++)
				{
					if (boardStr[y, x] != "#FFFFFF")
					{
						Cell newCell = new Cell();
						newCell.BoardId = newBoard.Id;
						newCell.X = x; 
						newCell.Y = y;
						newCell.Color = boardStr[y, x];
						_context.Add(newCell);
					}
				}
			}
			await _context.SaveChangesAsync();
			
			System.IO.File.Delete("Temp/" + filename);
			
			// return RedirectToAction(nameof(Index));
			return RedirectToAction("Load", "Board", new { id = newBoard.Id });
		}
	}
}
