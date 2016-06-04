//----------------------------------------------------------------------------------------------
// <copyright file="ImageWindow.cs" 
// Copyright 2016 Shawn Gilroy
//
// This file is part of Bayesian Model Selector.
//
// Bayesian Model Selector is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 2.
//
// Bayesian Model Selector is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Bayesian Model Selector.  If not, see http://www.gnu.org/licenses/. 
// </copyright>
//
// <summary>
// The Bayesian Model Selector is a tool to assist researchers in behavior economics.
// 
// Email: shawn(dot)gilroy(at)temple.edu
//
// </summary>
//----------------------------------------------------------------------------------------------

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
