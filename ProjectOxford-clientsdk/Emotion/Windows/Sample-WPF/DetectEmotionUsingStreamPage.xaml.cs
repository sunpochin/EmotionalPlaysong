﻿// 
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
// 
// Microsoft Cognitive Services (formerly Project Oxford): https://www.microsoft.com/cognitive-services
// 
// Microsoft Cognitive Services (formerly Project Oxford) GitHub:
// https://github.com/Microsoft/ProjectOxford-ClientSDK
// 
// Copyright (c) Microsoft Corporation
// All rights reserved.
// 
// MIT License:
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Windows;

// -----------------------------------------------------------------------
// KEY SAMPLE CODE STARTS HERE
// Use the following namesapce for EmotionServiceClient
// -----------------------------------------------------------------------
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
// -----------------------------------------------------------------------
// KEY SAMPLE CODE ENDS HERE
// -----------------------------------------------------------------------

namespace EmotionAPI_WPF_Samples
{
    internal class EmotionResultDisplay
    {
        public string EmotionString
        {
            get;
            set;
        }
        public float Score
        {
            get;
            set;
        }

        public int OriginalIndex
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Interaction logic for DetectEmotionUsingStreamPage.xaml
    /// </summary>
    public partial class DetectEmotionUsingStreamPage : Page
    {
        public DetectEmotionUsingStreamPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Uploads the image to Project Oxford and detect emotions.
        /// </summary>
        /// <param name="imageFilePath">The image file path.</param>
        /// <returns></returns>
        private async Task<Emotion[]> UploadAndDetectEmotions(string imageFilePath)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;
            string subscriptionKey = window.ScenarioControl.SubscriptionKey;

            window.Log("EmotionServiceClient is created");

            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE STARTS HERE
            // -----------------------------------------------------------------------

            //
            // Create Project Oxford Emotion API Service client
            //
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(subscriptionKey);

            window.Log("Calling EmotionServiceClient.RecognizeAsync()...");
            try
            {
                Emotion[] emotionResult;
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    //
                    // Detect the emotions in the URL
                    //
                    emotionResult = await emotionServiceClient.RecognizeAsync(imageFileStream);
                    return emotionResult;
                }
            }
            catch (Exception exception)
            {
                window.Log(exception.ToString());
                return null;
            }
            // -----------------------------------------------------------------------
            // KEY SAMPLE CODE ENDS HERE
            // -----------------------------------------------------------------------

        }

        private async void LoadImageButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = (MainWindow)Application.Current.MainWindow;

            Microsoft.Win32.OpenFileDialog openDlg = new Microsoft.Win32.OpenFileDialog();
            openDlg.Filter = "JPEG Image(*.jpg)|*.jpg";
            bool? result = openDlg.ShowDialog(window);

            if (!(bool)result)
            {
                return;
            }

            string imageFilePath = openDlg.FileName;
            Uri fileUri = new Uri(imageFilePath);

            BitmapImage bitmapSource = new BitmapImage();
            bitmapSource.BeginInit();
            bitmapSource.CacheOption = BitmapCacheOption.None;
            bitmapSource.UriSource = fileUri;
            bitmapSource.EndInit();

            _emotionDetectionUserControl.ImageUri = fileUri;
            _emotionDetectionUserControl.Image = bitmapSource;

            //
            // Create EmotionServiceClient and detect the emotion with URL
            //
            window.ScenarioControl.ClearLog();
            _detectionStatus.Text = "Detecting...";

            Emotion[] emotionResult = await UploadAndDetectEmotions(imageFilePath);

            _detectionStatus.Text = "Detection Done";

            //
            // Log detection result in the log window
            //
            window.Log("");
            window.Log("Detection Result:");
            window.LogEmotionResult(emotionResult);

            _emotionDetectionUserControl.Emotions = emotionResult;
            //            var success = await System.Windows.Launcher.LaunchUriAsync(uri);

            //resultDisplay[0] = new EmotionResultDisplay { EmotionString = "Anger", Score = emotion.Scores.Anger };
            //resultDisplay[1] = new EmotionResultDisplay { EmotionString = "Contempt", Score = emotion.Scores.Contempt };
            //resultDisplay[2] = new EmotionResultDisplay { EmotionString = "Disgust", Score = emotion.Scores.Disgust };
            //resultDisplay[3] = new EmotionResultDisplay { EmotionString = "Fear", Score = emotion.Scores.Fear };
            //resultDisplay[4] = new EmotionResultDisplay { EmotionString = "Happiness", Score = emotion.Scores.Happiness };
            //resultDisplay[5] = new EmotionResultDisplay { EmotionString = "Neutral", Score = emotion.Scores.Neutral };
            //resultDisplay[6] = new EmotionResultDisplay { EmotionString = "Sadness", Score = emotion.Scores.Sadness };
            //resultDisplay[7] = new EmotionResultDisplay { EmotionString = "Surprise", Score = emotion.Scores.Surprise };

            float fAnger = emotionResult[0].Scores.Anger;
            float fSadness = emotionResult[0].Scores.Sadness;
            float fHappiness = emotionResult[0].Scores.Happiness;
            string sSongURL = "";
            if (fAnger > 0.5)
            {
                // 許如芸 生氣
                sSongURL = "kkbox://play_song_849314";

            }
            if (fSadness > 0.5)
            {
                // 陳珊妮 如同悲傷被下載了兩次
                sSongURL = "kkbox://play_song_77706479";

            }
            if (fHappiness > 0.5)
            {
                // Happy Birthday	寶兒 (BoA)	千顏兒語 (THE FACE)
                sSongURL = "kkbox://play_song_1601466";

            }
            //            var uri = "C:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            var uri = "D:\\Program Files (x86)\\Mozilla Firefox\\firefox.exe";
            System.Diagnostics.Process.Start(uri, sSongURL);

        }
    }
}
