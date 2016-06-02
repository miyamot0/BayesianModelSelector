using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public BitmapImage images;

        public ImageWindow()
        {
            InitializeComponent();
        }

        private void saveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "TIFF Image|*.tiff|PNG Image|*.png|Jpeg Image|*.jpg|Bitmap Image|*.bmp|Gif Image|*.gif";
            saveFileDialog1.Title = "Save an Image File";
            saveFileDialog1.ShowDialog();

            string mExt = Path.GetExtension(saveFileDialog1.FileName);

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)imageGrid.ActualWidth, (int)imageGrid.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bitmap.Render(imageGrid);
            BitmapFrame frame = BitmapFrame.Create(bitmap);

            if (mExt.Equals(".jpg"))
            {
                using (var fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(images));
                    encoder.Save(fileStream);
                }
            }
            else if (mExt.Equals(".png"))
            {
                using (var fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(images));
                    encoder.Save(fileStream);
                }
            }
            else if (mExt.Equals(".gif"))
            {
                using (var fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new GifBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(images));
                    encoder.Save(fileStream);
                }
            }
            else if (mExt.Equals(".tiff"))
            {
                using (var fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    BitmapEncoder encoder = new TiffBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(images));
                    encoder.Save(fileStream);
                }
            }
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
