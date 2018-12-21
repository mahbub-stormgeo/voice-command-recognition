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
        public MainWindow()
        {
            InitializeComponent();
            speechRecognizer.SetInputToDefaultAudioDevice();
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized; 
            speechRecognizer.LoadGrammarAsync(GetGrammarForZooming());
            speechRecognizer.LoadGrammarAsync(GetGrammerForFontStyle());
            speechRecognizer.LoadGrammarAsync(GetMathGrammar("plus"));
            speechRecognizer.LoadGrammarAsync(GetMathGrammar("multiply"));
        }
        private void btnStartListen_Click(object sender, RoutedEventArgs e)
        {
            SpeechRecognizer recognizer = new SpeechRecognizer();
            btnStartListen.Content = "Listening started...";
            btnStartListen.IsEnabled = false;
            btnStopListen.IsEnabled = true;
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
            if(txt.Contains("naviplanner") && (txt.Contains("zoom in") || txt.Contains("zoomin")))
            {
                txtSpeech.FontSize += 2;
                Debug.WriteLine(txtSpeech.FontSize.ToString());
            }
            else if (txt.Contains("naviplanner") && (txt.Contains("zoom out") || txt.Contains("zoomout")))
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
            btnStopListen.IsEnabled = false;
            speechRecognizer.RecognizeAsyncStop();
        }        
        private Grammar GetGrammarForZooming()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append("Hey NaviPlanner");
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
                    txtSpeech.FontWeight = (FontWeight)weightConverter.ConvertFromString(value);
                    break;
                case "color":
                    txtSpeech.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value));
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
