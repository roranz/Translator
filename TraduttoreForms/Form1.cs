using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using System.Text;

namespace TraduttoreForms
{
    public partial class Form1 : Form
    {
        String api = "";

        public Form1()
        {
            InitializeComponent();
        }
        
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            StreamWriter myStream = File.CreateText(saveFileDialog1.FileName);
            myStream.Write(richTextBox2.Text);
            myStream.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            openFileDialog1.OpenFile();
            var filename = openFileDialog1.FileName;
            var fileStream = openFileDialog1.OpenFile();

            using (StreamReader reader = new StreamReader(fileStream))
            {
                StreamReader fileRead = File.OpenText(openFileDialog1.FileName);
                richTextBox1.Text = Estrattore(fileRead);
                fileRead.Close();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void aaaToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void apriToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void salvaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Inseritore(StreamReader file)
        {
            var esiste = false;
            if (File.Exists("traduzione.txt"))
                esiste = true;
            char ch;
            StreamWriter myStream = File.CreateText(@"traduzione.txt");
            myStream.Write(richTextBox2.Text);
            myStream.Write(" ");
            myStream.Close();
            StreamReader fileTrad = File.OpenText(@"traduzione.txt");
            saveFileDialog2.ShowDialog();
            StreamWriter str = File.CreateText(saveFileDialog2.FileName);
            while (!file.EndOfStream)
            {
                ch = (char)file.Read();
                str.Write(ch);
                if (ch == '0')
                {
                    ch = (char)file.Read();
                    str.Write(ch);
                    if (ch == ',')
                    {
                        ch = (char)file.Read();
                        str.Write(ch);
                        if (ch == ',')
                        {
                            while (ch != '\n')
                            {
                                if (!fileTrad.EndOfStream)
                                    ch = (char)fileTrad.Read();
                                else
                                {
                                    str.Write("\n");
                                    break;
                                }
                                if (ch == ' ')
                                {
                                    if (!fileTrad.EndOfStream)
                                        ch = (char)fileTrad.Read();
                                    if (ch != '\\')
                                        str.Write(' ');
                                    else
                                    {
                                        str.Write(ch);
                                        if (!fileTrad.EndOfStream)
                                        {
                                            ch = (char)fileTrad.Read();
                                            str.Write(ch);
                                        }
                                        if (!fileTrad.EndOfStream)
                                            ch = (char)fileTrad.Read();
                                        if (!fileTrad.EndOfStream)
                                            ch = (char)fileTrad.Read();
                                    }
                                }
                                if (!fileTrad.EndOfStream)
                                    str.Write(ch);
                            }
                            file.ReadLine();
                        }
                    }
                }
            }
            fileTrad.Close();
            if (!esiste)
                File.Delete("traduzione.txt");
            file.Close();
            str.Close();
        }

        private string Estrattore(StreamReader file)
        {
            string str = "";
            char ch;
            while (!file.EndOfStream)
            {
                ch = (char)file.Read();
                if (ch == '0')
                {
                    ch = (char)file.Read();
                    if (ch == ',')
                    {
                        ch = (char)file.Read();
                        if (ch == ',')
                        {
                            for (; ch != '\n';)
                            {
                                ch = (char)file.Read();
                                if (ch == '{')
                                {
                                    while (!(ch == '}'))
                                    {
                                        ch = (char)file.Read();
                                    }
                                }
                                else if (ch == '\\')
                                {
                                    str += " " + ch;
                                    ch = (char)file.Read();
                                    if (ch == 'N' || ch == 'n')
                                    {
                                        str += ch + " ";
                                    }
                                }
                                else
                                {
                                    str += ch;
                                }
                            }
                        }
                    }
                }
            }
            return str + "\n";
        }

        internal class Messaggio
        {
            public short code { get; set; }
            public string lang { get; set; }
            public string[] text { get; set; }
            public string getText()
            {
                string str = "";
                for (int i = 0; i < text.Length; ++i)
                {
                    str += text[i];
                }
                return str;
            }
        }
        public void traduciToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(api == "")
            {
                api = InputBox("Inserisci l'api da https://translate.yandex.com/developers/keys", "API");
            }
            if (richTextBox2.Text == "" && api != "")
            {
                string encodedString = System.Net.WebUtility.UrlEncode(richTextBox1.Text);
                WebClient webClient = new WebClient();
                webClient.Encoding = Encoding.UTF8;
                var trad = "";
                string[] result = contaCaratteri(encodedString,9000);
                try
                {
                    for(int i = 0; i < result.Length; ++i)
                    {
                        if (result[i].Length > 3)
                        {
                        //MessageBox.Show("Blocco " + i + " tradotto" );
                        string link = "https://translate.yandex.net/api/v1.5/tr.json/translate?key=" + api + "&text=" + result[i] + "&lang=it";
                        string response = webClient.DownloadString(link);
                        trad += JsonConvert.DeserializeObject<Messaggio>(response).getText();
                        }
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                    return;
                }
                richTextBox2.Text = trad;


            }
            else
            {
                if (api != "")
                    MessageBox.Show("La textbox della traduzione dev'essere vuota!", "Errore");
                else
                    MessageBox.Show("API non valido", "Errore");
            }
        }

        private string[] contaCaratteri(string s,int car)
        {
            string[] stringhe = s.Split(new string[] { "%0A" }, StringSplitOptions.RemoveEmptyEntries);
            string[] owo = new string[5]{"", "", "", "", ""};
            int count = 0;
            int nStr = 0;
            for (int i = 0; i < stringhe.Length; ++i)
            {
                count += stringhe[i].Length;

                if(count >= car)
                {
                    count = 0;
                    nStr++;
                }                   
                owo[nStr] += stringhe[i] += "%0A";                
            }
                return owo;
        }

        private void apiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            api = InputBox("Inserisci l'api da https://translate.yandex.com/developers/keys", "API");
        }

        public static string InputBox(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Label textLabel = new Label() { Left = 50, Top = 20, Width = 320, Text = text };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 300 };
            Button confirmation = new Button() { Text = "Ok", Left = 250, Width = 100, Top = 80, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }

        private void saveFileDialog2_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void inniettaTraduzioneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamReader file;
            if (richTextBox1.Text.Length > 5)
            {
                Inseritore(file = new StreamReader(openFileDialog1.FileName));
                file.Close();
            }
            else MessageBox.Show("Non ho niente il file di origine", "Errore!");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
