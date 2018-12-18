using System.Diagnostics;
using System.Globalization;
using System.Speech.Recognition;
using System.Windows;

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
            speechRecognizer.LoadGrammarAsync(GetMathGrammer());
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
            if (txt.IndexOf("naviplanner") >= 0 && txt.IndexOf("zoom in") > 0)
            {
                txtSpeech.FontSize--;
                Debug.WriteLine(txtSpeech.FontSize.ToString());
            }
            if (txt.IndexOf("naviplanner") >= 0 && txt.IndexOf("zoom out") > 0)
            {
                txtSpeech.FontSize++;
                Debug.WriteLine(txtSpeech.FontSize.ToString());
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
        private Grammar GetGrammar()
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
        private Grammar GetGrammarForZooming()
        {
            GrammarBuilder grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append("Hey NaviPlanner");
            Choices valueChoices = new Choices();
            valueChoices.Add("zoom in");
            valueChoices.Add("zoom out");
            grammarBuilder.Append(valueChoices);
            return new Grammar(grammarBuilder);
        }
        private Grammar GetMathGrammer()
        {
            Choices ch_Numbers = new Choices();
            ch_Numbers.Add("1");
            ch_Numbers.Add("2");
            ch_Numbers.Add("3");
            ch_Numbers.Add("4");
            GrammarBuilder gb_WhatIsXplusY = new GrammarBuilder();
            gb_WhatIsXplusY.Append("What is");
            gb_WhatIsXplusY.Append(ch_Numbers);
            gb_WhatIsXplusY.Append("plus");
            gb_WhatIsXplusY.Append(ch_Numbers);
            return new Grammar(gb_WhatIsXplusY);
        }
    }
}
