using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace MyWebApplication.Models
{
	public static class ImageConverter
	{
		public static byte[] Resize(byte[] imageData, Int32 width, Int32 height, bool cropImg)
		{
			
			using var inStream = new MemoryStream(imageData);
			using var outStream = new MemoryStream();
			using (Image<Rgba32> image = Image.Load<Rgba32>(inStream)) 
			{
				// get width and height of image
				int originalWidth = image.Width;
				int originalHeight = image.Height;
				
				if (!cropImg)
				{
					if (originalWidth > originalHeight)
					{
						// expand image to new height by adding white space on left and right
						var padding = (originalWidth - originalHeight) / 2;
						image.Mutate(x => x.Pad(originalHeight, originalHeight + 2 * padding, Color.White));
					}
					else
					{
						// expand image to new width by adding white space on top and bottom
						var padding = (originalHeight - originalWidth) / 2;
						image.Mutate(x => x.Pad(originalWidth, originalWidth + 2 * padding, Color.White));
					}
				}
				else
				{
					if (originalWidth > originalHeight)
					{
						// crop the image to original height by centering it (a.k. cutting on both sides equally)
						var diff = originalWidth - originalHeight;
						var offsetX = diff / 2;
						image.Mutate(x => x.Crop(new Rectangle(offsetX, 0, originalHeight, originalHeight)));
					}
					else
					{
						// crop the image to original width
						var diff = originalHeight - originalWidth;
						var offsetY = diff / 2;
						image.Mutate(x => x.Crop(new Rectangle(0, offsetY, originalWidth, originalWidth)));
					}
				}
				
				// resize image to final size
				image.Mutate(x => x.Resize(width, height));
				image.SaveAsJpeg(outStream);
			}
			return outStream.ToArray();
		}
		

		public static byte[] ConvertToBW(byte[] imageData)
		{
			using var inStream = new MemoryStream(imageData);
			using var outStream = new MemoryStream();
			
			using (Image<Rgba32> image = Image.Load<Rgba32>(inStream)) 
			{
				image.Mutate(x => x.Grayscale());
				image.SaveAsJpeg(outStream);
			}

			return outStream.ToArray();

		}

		public static string[,] ConvertToBwBitArray(byte[] imageData, int threshold)
		{
			float thresholdFloat = threshold / 100f;

			using var inStream = new MemoryStream(imageData);
	    
			using Image<Rgba32> image = Image.Load<Rgba32>(inStream);

			string[,] bitArray = new string[image.Width, image.Height];

			// Convert each pixel to 1 or 0
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Rgba32 pixelColor = image[y, x];

					// Calculate the brightness of the pixel. Formula for luminance considering human perception
					double brightness = (0.299 * pixelColor.R + 0.587 * pixelColor.G + 0.114 * pixelColor.B) / 255;

					// Assuming a threshold of 0.5 to determine if a pixel is on or off
					// Adjust the threshold according to your needs
					bitArray[x, y] = brightness < thresholdFloat ? "1" : "0";
				}
			}
			return bitArray;
		}
		
		public static string[,] ConvertToColorStringArray(byte[] imageData)
		{
			using var inStream = new MemoryStream(imageData);
			using Image<Rgba32> image = Image.Load<Rgba32>(inStream);
			string[,] hexArray = new string[image.Width, image.Height];

			// Convert each pixel to HEX
			for (int y = 0; y < image.Height; y++)
			{
				for (int x = 0; x < image.Width; x++)
				{
					Rgba32 pixelColor = image[y, x];
					hexArray[x, y] = $"#{pixelColor.R:X2}{pixelColor.G:X2}{pixelColor.B:X2}";
				}
			}
			return hexArray;
		}

		private static string[] ImageToStringArray(byte[] imageData)
		{
			using var image = Image.Load<Rgba32>(imageData);

			string[] result = new string[image.Height];
			for (int i = 0; i < image.Height; i++)
			{
				for (int j = 0; j < image.Width; j++)
				{
					var pixel = image[j, i];
					result[i] += pixel.R.ToString("X2") + pixel.G.ToString("X2") + pixel.B.ToString("X2") + " ";
				}
			}
			return result;
		}
		
		public static byte[] ConvertToGreyScale(byte[] imageData)
		{
			using var inStream = new MemoryStream(imageData);
			using var outStream = new MemoryStream();
    
			using (Image<Rgba32> image = Image.Load<Rgba32>(inStream)) 
			{
				image.Mutate(x => x.Grayscale());
				image.SaveAsJpeg(outStream);
			}

			return outStream.ToArray();
		}

		private static byte[] ImageToByteArray(Image<Rgba32> img)
		{
			using var outStream = new MemoryStream();
			img.SaveAsJpeg(outStream);
			return outStream.ToArray();
		}
		
		private static byte[] ImageToBW(byte[] imageData)
		{
			using var inStream = new MemoryStream(imageData);
			using var outStream = new MemoryStream();
    
			using (Image<Rgba32> image = Image.Load<Rgba32>(inStream)) 
			{
				image.Mutate(x => x.BinaryThreshold(0.5f) );
				image.SaveAsJpeg(outStream);
			}

			return outStream.ToArray();
		}
		
		public static string ConvertCellsToBase64Image(List<Cell> cells, int imgSize, string bgColor="#FFFFFF")
		{
			// Create an image from the cells
			using var image = new Image<Rgba32>(imgSize, imgSize);

			for(int x = 0; x < image.Width; x++)
			{
				for(int y = 0; y < image.Height; y++)
				{
					var cell = cells.FirstOrDefault(c => c.X == x && c.Y == y);
					if(cell == null)
					{
						image[x, y] = Rgba32.ParseHex(bgColor);
					}
					else
					{
						image[x, y] = Rgba32.ParseHex(cell.Color);
					}
				}
			}
			
			// Convert the image to a byte array
			using var outStream = new MemoryStream();
			image.SaveAsPng(outStream);
			byte[] imageData = outStream.ToArray();

			// Convert the byte array to a base64 string
			string b64String = Convert.ToBase64String(imageData);
			return b64String;
		}
		
		public static byte[] ConvertCellsToPng(List<Cell> cells, int imgSize, string bgColor="#FFFFFF")
		{
			// Create an image from the cells
			using var image = new Image<Rgba32>(imgSize, imgSize);

			for(int x = 0; x < image.Width; x++)
			{
				for(int y = 0; y < image.Height; y++)
				{
					var cell = cells.FirstOrDefault(c => c.X == x && c.Y == y);
					if(cell == null)
					{
						image[x, y] = Rgba32.ParseHex(bgColor);
					}
					else
					{
						image[x, y] = Rgba32.ParseHex(cell.Color);
					}
				}
			}
			
			// Convert the image to a byte array
			using var outStream = new MemoryStream();
			image.SaveAsPng(outStream);
			byte[] imageData = outStream.ToArray();

			return imageData;
		}
	}
}