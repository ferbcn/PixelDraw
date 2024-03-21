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
				using (var output = new MemoryStream())
				{
					bitm.Save(output, ImageFormat.Png);
					return output.ToArray();
				}
			}
		}
		
		public static byte[] Convert(byte[] imageData)
		{
			using (var ms = new MemoryStream(imageData))
			{
				Image img = Image.FromStream(ms);
				img = ImageToBW(img);
				Bitmap bitm = new Bitmap(img);

				using (var output = new MemoryStream())
				{
					bitm.Save(output, ImageFormat.Png);
					return output.ToArray();
				}
			}
		}

		public static Image ImageToGrayScale(Image img)
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

		public static Image ImageToBW(Image img)
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