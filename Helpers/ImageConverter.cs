using System.Drawing;
using System.Drawing.Imaging;

namespace MyWebApplication.Models
{
	public static class ImageConverter
	{
		public static byte[] Resize(byte[] imageData, int width, int height)
		{
			using (var ms = new MemoryStream(imageData))
			{
				Image img = Image.FromStream(ms);
				Bitmap bitm = new Bitmap(img, new Size(width, height));

				return ImageToByteArray(bitm);
			}
		}
		

		public static byte[] ConvertToBW(byte[] imageData)
		{
			using (var ms = new MemoryStream(imageData))
			{
				Image img = Image.FromStream(ms);
				img = ImageToBW(img);
				Bitmap bitm = new Bitmap(img);

				return ImageToByteArray(bitm);
			}
		}

		public static string[,] ConvertToBwBitArray(byte[] imageData, int treshold)
		{
			float treshold_f = treshold / 100f;
			//Console.WriteLine("Treshold: " + treshold_f);

			using (var ms = new MemoryStream(imageData))
			{
				Image img = Image.FromStream(ms);
				Bitmap bitmap = new Bitmap(img);

				string[,] bitArray = new string[50, 50];

				// Convert each pixel to 1 or 0
				for (int y = 0; y < bitmap.Height; y++)
				{
					for (int x = 0; x < bitmap.Width; x++)
					{
						Color pixelColor = bitmap.GetPixel(x, y);

						// Calculate the brightness of the pixel
						double brightness = pixelColor.GetBrightness();

						// Assuming a threshold of 0.5 to determine if a pixel is on or off
						// Adjust the threshold according to your needs
						bitArray[x, y] = brightness < treshold_f ? "1" : "0";
					}
				}

				return bitArray;
			}
		}

		private static string[] ImageToStringArray(Bitmap img)
		{
			string[] result = new string[img.Height];
			for (int i = 0; i < img.Height; i++)
			{
				for (int j = 0; j < img.Width; j++)
				{
					Color pixel = img.GetPixel(j, i);
					result[i] += pixel.R.ToString("X2") + pixel.G.ToString("X2") + pixel.B.ToString("X2") + " ";
				}
			}
			return result;
		}

		public static byte[] ConvertToGreyScale(byte[] imageData)
		{
			using (var ms = new MemoryStream(imageData))
			{
				Image img = Image.FromStream(ms);
				img = ImageToGrayScale(img);
				Bitmap bitm = new Bitmap(img);

				return ImageToByteArray(bitm);
			}
		}

		private static byte[] ImageToByteArray(Image img)
		{
			using (var ms = new MemoryStream())
			{
				img.Save(ms, ImageFormat.Png);
				return ms.ToArray();
			}
		}

		private static Image ImageToGrayScale(Image img)
		{
			Bitmap bmp = new Bitmap(img);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					Color pixel = bmp.GetPixel(i, j);
					int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
					Color newColor = Color.FromArgb(pixel.A, grayScale, grayScale, grayScale);
					bmp.SetPixel(i, j, newColor);

				}
			}
			return bmp;
		}

		private static Image ImageToBW(Image img)
		{
			Bitmap bmp = new Bitmap(img);
			for (int i = 0; i < bmp.Width; i++)
			{
				for (int j = 0; j < bmp.Height; j++)
				{
					Color pixel = bmp.GetPixel(i, j);
					int grayScale = (int)((pixel.R * 0.3) + (pixel.G * 0.59) + (pixel.B * 0.11));
					bmp.SetPixel(i, j, grayScale < 128 ? Color.Black : Color.White);
				}
			}
			return bmp;
		}
	}

}