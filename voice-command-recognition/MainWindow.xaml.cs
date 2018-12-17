using System.Speech.Recognition;
using System.Windows;

namespace voice_command_recognition
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine speechRecognizer = new SpeechRecognitionEngine();
        public MainWindow()
        {
            InitializeComponent();
            speechRecognizer.SpeechRecognized += speechRecognizer_SpeechRecognized;
            speechRecognizer.LoadGrammar(GetGrammar());
            speechRecognizer.SetInputToDefaultAudioDevice();            
        }
        private void btnStartListen_Click(object sender, RoutedEventArgs e)
        {
            SpeechRecognizer recognizer = new SpeechRecognizer();
            btnStartListen.Content = "Listening started...";
            btnStartListen.IsEnabled = false;
            btnStopListen.IsEnabled = true;
            txtSpeech.Text = "";
            speechRecognizer.RecognizeAsync(RecognizeMode.Single);
        }
        private void speechRecognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            txtSpeech.Text = txtSpeech.Text + " \n" + e.Result.Text;
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
    }
}
