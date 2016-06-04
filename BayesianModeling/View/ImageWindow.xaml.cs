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
// Bayesian Model Selector utilizes SharpVectors to render SVG file formats
//
//    SharpVectors is distributed under this license:
//
//    Copyright (c) 2010 SharpVectorGraphics
//
//    All rights reserved.
//    
//    Redistribution and use in source and binary forms, with or without modification,
//    are permitted provided that the following conditions are met:
//    
//    Redistributions of source code must retain the above copyright notice,
//    this list of conditions and the following disclaimer.
//    
//    Redistributions in binary form must reproduce the above copyright notice,
//    this list of conditions and the following disclaimer in the documentation
//    and/or other materials provided with the distribution.
//    
//    Neither the name of SharpVectorGraphics nor the names of its contributors
//    may be used to endorse or promote products derived from this software
//    without specific prior written permission.
//    
//    THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
//    AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//    IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//    DISCLAIMED.IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE
//    FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
//    DAMAGES(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
//    SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
//    HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT,
//    STRICT LIABILITY, OR TORT(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY
//    WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY
//    OF SUCH DAMAGE.
//
// </summary>
//----------------------------------------------------------------------------------------------

using Microsoft.Win32;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using System.IO;
using System.Windows;

namespace BayesianModeling.View
{
    /// <summary>
    /// Interaction logic for ImageWindow.xaml
    /// </summary>
    public partial class ImageWindow : Window
    {
        public string filePath = null;

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

            WpfDrawingSettings settings = new WpfDrawingSettings();
            settings.IncludeRuntime = true;
            settings.TextAsGeometry = false;

            ImageSvgConverter converter = new ImageSvgConverter(settings);

            if (mExt.Equals(".jpg"))
            {
                converter.EncoderType = ImageEncoderType.JpegBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".png"))
            {
                converter.EncoderType = ImageEncoderType.PngBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".gif"))
            {
                converter.EncoderType = ImageEncoderType.GifBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
            else if (mExt.Equals(".tiff"))
            {
                converter.EncoderType = ImageEncoderType.TiffBitmap;
                converter.Convert(filePath, saveFileDialog1.FileName);
            }
        }

        private void closeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
