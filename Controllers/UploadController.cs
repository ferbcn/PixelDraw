﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyWebApplication.Models;
using System.Text;
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

		private byte[] resizeImageToGreyscale(byte[] data)
		{
			// resize image
			data = ImageConverter.Resize(data, CONV_SIZE, CONV_SIZE);
			data = ImageConverter.ConvertToGreyScale(data);
			//data = ImageConverter.ConvertToBW(data);
			return data;
		}

		private string[,] ImageToBitString(byte[] data, int threshold, bool invert=false)
		{	
			string[,] bw_data_str = ImageConverter.ConvertToBwBitArray(data, threshold);
			if (invert)
			{
				bw_data_str = InvertImage(bw_data_str, CONV_SIZE);
			}
			return bw_data_str;
		}
		
		private string[,] InvertImage(string[,] imageData, Int16 imageSize)
		{
			for(Int16 y = 0; y < imageSize; y++)
			{
				for(Int16 x = 0; x < imageSize; x++)
				{
					imageData[y, x] = imageData[y, x] == "0" ? "1" : "0";
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
				
				// resize image and convert to greyscale byte string array
				byte[] img_data_grey = resizeImageToGreyscale(img_data_raw);
				
				// convert to byte string array
				string[,] bw_data_str = ImageToBitString(img_data_grey, threshold);	
				
				// prepare image for client
				ViewData["image_data"] = $"data:image/png;base64,{Convert.ToBase64String(img_data_raw)}";
				ViewData["image_data_sm"] = $"data:image/png;base64,{Convert.ToBase64String(img_data_grey)}";

				// create board from reduced image
				Board_DTO myboard = new Board_DTO(CONV_SIZE, bw_data_str);
				
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
		public async Task<IActionResult> UpdateThreshold(int threshold, string filename, bool invert)
		{
			// load image from temp.png
			byte[] img_data_raw = System.IO.File.ReadAllBytes("Temp/" + filename);
			
			// resize image and convert to greyscale byte string array
			byte[] img_data_grey = resizeImageToGreyscale(img_data_raw);
			
			// convert to byte string array
			string[,] bw_data_str = ImageToBitString(img_data_grey, threshold, invert);	

			// create board from reduced image
			Board_DTO myboard = new Board_DTO(CONV_SIZE, bw_data_str);
			
			ViewData["Board"] = myboard.GetBoardString();
			ViewData["Size"] = myboard.GetSize();

			return View("Board");
		}
		
		[HttpPost]
		public async Task<IActionResult> Save (int threshold, string filename, bool invert)
		{
			// Console.WriteLine("Save image: " + filename);
			// load image from temp.png
			byte[] img_data_raw = System.IO.File.ReadAllBytes("Temp/" + filename);
			
			// resize image and convert to greyscale byte string array
			byte[] img_data_grey = resizeImageToGreyscale(img_data_raw);
				
			// convert to byte string array
			string[,] bw_data_str = ImageToBitString(img_data_grey, threshold, invert);	
				
			// create board from reduced image
			Board_DTO myboard = new Board_DTO(CONV_SIZE, bw_data_str);
			
			string[,] boardStr = myboard.GetBoardString();
			
			// Create new board in DB
			Board newBoard = new Board();
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
