using System;
using System.Diagnostics;
using System.Globalization;
using System.Speech.Recognition;
using System.Windows;
using System.Windows.Media;

namespace voice_command_recognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine(new CultureInfo("en-us"));
        static Grammar g1, g2, g3, g4;
        public MainWindow()
        {
            InitializeComponent();
            speechRecognizer.SetInputToDefaultAudioDevice();
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            //speechRecognizer.LoadGrammarAsync(GetGrammarForZooming());
            //speechRecognizer.LoadGrammarAsync(GetGrammerForFontStyle());
            //speechRecognizer.LoadGrammarAsync(GetMathGrammar("plus"));
            //speechRecognizer.LoadGrammarAsync(GetMathGrammar("multiply"));
            speechRecognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder("Hey NaviPlanner start listening")));
            txtSpeech.Text = "To start say 'Hey NaviPlanner, start listening'";
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);
            g1 = GetGrammarForZooming();
            g2 = GetGrammerForFontStyle();
            g3 = GetMathGrammar("plus");
            g4 = GetMathGrammar("multiply");
        }
        private void btnStartListen_Click(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    // important for the first time to enable the speech recognition desktop app
            //    SpeechRecognizer recognizer = new SpeechRecognizer();
            //    recognizer.Enabled = false;
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine(ex.Message);
            //}
            
            btnStartListen.Content = "Listening started...";
            btnStartListen.IsEnabled = false;
            // btnStopListen.IsEnabled = true;
            txtSpeech.Text = "welcome to naviplanner...";
            speechRecognizer.RecognizeAsync(RecognizeMode.Multiple);            
        }
        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            float confidence = e.Result.Confidence;
            Debug.WriteLine(string.Format("Recognized {0} with confidence {1}", txt, confidence));            
            if (confidence < 0.60) return;
            txt = txt.ToLower();
            if (txt.Contains("naviplanner") && (txt.Contains("start listening")))
            {
                btnStartListen.IsEnabled = true;
                btnStartListen.Content = "Listening now";                
                speechRecognizer.LoadGrammarAsync(g1);
                speechRecognizer.LoadGrammarAsync(g2);
                speechRecognizer.LoadGrammarAsync(g3);
                speechRecognizer.LoadGrammarAsync(g4);
                speechRecognizer.LoadGrammarAsync(new Grammar(new GrammarBuilder("stop listening")));
                txtSpeech.Text = "say 'zoom in' or 'zoom out' to see effect";
            }
            if ((txt.Contains("stop listening")))
            {
                btnStartListen.IsEnabled = false;
                btnStartListen.Content = "not listening";
                txtSpeech.Text = "To start say 'Hey NaviPlanner, start listening'";
                speechRecognizer.UnloadGrammar(g1);
                speechRecognizer.UnloadGrammar(g2);
                speechRecognizer.UnloadGrammar(g3);
                speechRecognizer.UnloadGrammar(g4);
            }
            if ((txt.Contains("zoom in") || txt.Contains("zoomin")))
            {
                txtSpeech.FontSize += 2;
                Debug.WriteLine(txtSpeech.FontSize.ToString());
            }
            else if ((txt.Contains("zoom out") || txt.Contains("zoomout")))
            {
                txtSpeech.FontSize -= 2;
                Debug.WriteLine(txtSpeech.FontSize.ToString());
            }
            else if (txt.Contains("what") && txt.Contains("plus"))
            {
                RecognizePlusCommand(txt);
            }
            else if (txt.Contains("what") && txt.Contains("multiply"))
            {
                RecognizeMultiplyCommand(txt);
            }     
            else if (e.Result.Words.Count == 2)
            {
                ChangeFontStyle(e.Result.Words[0].Text.ToLower(), e.Result.Words[1].Text.ToLower());
            }
            
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            speechRecognizer.Dispose();
        }
        private void btnStopListen_Click(object sender, RoutedEventArgs e)
        {
            btnStartListen.Content = "start listeing";
            btnStartListen.IsEnabled = true;
            // btnStopListen.IsEnabled = false;
            speechRecognizer.RecognizeAsyncStop();
        }                
        private Grammar GetGrammarForZooming()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            // grammarBuilder.Append("Hey NaviPlanner");
            Choices valueChoices = new Choices();
            valueChoices.Add("zoom in", "zoomin");
            valueChoices.Add("zoom out", "zoomout");
            grammarBuilder.Append(valueChoices);
            return new Grammar(grammarBuilder);
        }        
        private Grammar GetMathGrammar(string operatorname)
        {
            Choices ch_Numbers = new Choices();
            ch_Numbers.Add("1", "2", "3", "4", "5", "6", "7", "8", "9");
            GrammarBuilder builder = new GrammarBuilder();
            builder.Append("What is");
            builder.Append(ch_Numbers);
            builder.Append(operatorname);
            builder.Append(ch_Numbers);
            return new Grammar(builder);
        }
        private Grammar GetGrammerForFontStyle()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            Choices commandChoices = new Choices("weight", "color", "size");
            grammarBuilder.Append(commandChoices);
            Choices valueChoices = new Choices();
            valueChoices.Add("normal", "bold");
            valueChoices.Add("red", "green", "blue");
            valueChoices.Add("small", "medium", "large");
            grammarBuilder.Append(valueChoices);
            return new Grammar(grammarBuilder);
        }
        private void RecognizePlusCommand(string txt)
        {
            string[] words = txt.Split(' ');
            int num1 = int.Parse(words[2]);
            int num2 = int.Parse(words[4]);
            int sum = num1 + num2;
            txtSpeech.Text = words[2] + " plus " + words[4] + " equals " + sum;
        }
        private void RecognizeMultiplyCommand(string txt)
        {
            string[] words = txt.Split(' ');
            int num1 = int.Parse(words[2]);
            int num2 = int.Parse(words[4]);
            txtSpeech.Text = words[2] + " multiply " + words[4] + " equals " + num1 * num2;
        }
        private void ChangeFontStyle(string command, string value)
        {            
            switch (command)
            {
                case "weight":
                    FontWeightConverter weightConverter = new FontWeightConverter();
                    try
                    {
                        txtSpeech.FontWeight = (FontWeight)weightConverter.ConvertFromString(value);
                    }
                    catch(Exception ex)
                    {

                    }                    
                    break;
                case "color":
                    try
                    {
                        txtSpeech.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
                    }
                    catch (Exception)
                    {

                        // throw;
                    }
                    break;
                case "size":
                    switch (value)
                    {
                        case "small":
                            txtSpeech.FontSize = 12;
                            break;
                        case "medium":
                            txtSpeech.FontSize = 24;
                            break;
                        case "large":
                            txtSpeech.FontSize = 48;
                            break;
                    }
                    break;
            }
        }
    }
}
