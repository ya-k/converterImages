using System.Drawing;

namespace Convert_Color_Picture_to_Grayscale
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG)|*.BMP;*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBoxOpenFile.Image = new Bitmap(openFileDialog.FileName);
                }
                catch
                {
                    MessageBox.Show("The selected file cannot be opened", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
            if (pictureBoxOpenFile.Image != null)
            {
                Bitmap inputImage = new Bitmap(pictureBoxOpenFile.Image);
                Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);

                int width = inputImage.Width;
                int height = inputImage.Height;

                Pixel[,] pixels = new Pixel[width, height];

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        Color pixelColor = inputImage.GetPixel(x, y);
                        pixels[x, y] = new Pixel(pixelColor.R, pixelColor.G, pixelColor.B);
                    }
                }

                Pixel[] arrayOfPixels = pixels.Cast<Pixel>().ToArray();

                List<Pixel[]> arrayOfArraysOfPixels = new List<Pixel[]>();

                int numThreads = 4;
                int chunkSize = numThreads;
                int startIndex = 0;

                while (startIndex < arrayOfPixels.Length)
                {
                    Pixel[] chunk = arrayOfPixels.Skip(startIndex).Take(chunkSize).ToArray();
                    arrayOfArraysOfPixels.Add(chunk);
                    startIndex += chunkSize;
                }

                List<Pixel[]> arrayOfArraysOfPixelsConvert = new List<Pixel[]>();

                Parallel.For(0, numThreads, threadIndex =>
                {
                    for (int x = 0; x < arrayOfArraysOfPixels.Count; x++)
                {
                    Pixel[] subArray = arrayOfArraysOfPixels[x];

                    for (int y = 0; y < arrayOfArraysOfPixels[x].Length; y++)
                    {
                        Pixel pixel = subArray[y];
                        byte grayscaleValue = (byte)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);

                        Pixel[] sArray = arrayOfArraysOfPixels[x];
                        Pixel pixelNew = sArray[y];
                        pixelNew = new Pixel(grayscaleValue, grayscaleValue, grayscaleValue);
                    }

                    lock (lockObject)
                    {

                    }

                }
            });

            Pixel[] convertResult = arrayOfArraysOfPixelsConvert.Cast<Pixel>().ToArray();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        Pixel pixel = convertResult[index];
                        Color color = Color.FromArgb(pixel.R, pixel.G, pixel.B);
                        outputImage.SetPixel(x, y, color);
                    }
                }

                pictureBoxSaveFile.Image = outputImage;
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (pictureBoxSaveFile.Image != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Title = "Save image as..";
                saveFileDialog.OverwritePrompt = true;
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.Filter = "Image File (*.BMP)|*.BMP|Image File (*.JPG)|*.JPG|Image File (*.GIF)|*.GIF|" +
                    "Image File (*.PNG)|*.PNG|All files (*.*)|*.*";
                saveFileDialog.ShowHelp = true;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        pictureBoxSaveFile.Image.Save(saveFileDialog.FileName);
                    }
                    catch
                    {
                        MessageBox.Show("The selected file cannot be saved", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

        }


    }
}