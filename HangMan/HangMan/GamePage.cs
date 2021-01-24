using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HangMan
{
    public partial class GamePage : Form
    {

        private Bitmap[] hangImages = { HangMan.Properties.Resources.hang1,
                                        HangMan.Properties.Resources.hang2,
                                        HangMan.Properties.Resources.hang3,
                                        HangMan.Properties.Resources.hang4,
                                        HangMan.Properties.Resources.hang5,
                                        HangMan.Properties.Resources.hang6,
                                        HangMan.Properties.Resources.hang7};

        public GamePage()
        {
            InitializeComponent();
        }
        private void GamePage_Load(object sender, EventArgs e)
        {
            LoadWords();
            ScoreControl();
            SetupWordChoice();

            scorePanel.Visible = false;
            buttonsPanel.Enabled = false;
            panel2.Visible = false;

            scoreListview.Columns.Add("Adı", 150);
            scoreListview.Columns.Add("Soyadı", 108);

            wordsListView.Columns.Add("Sayı", 50);
            wordsListView.Columns.Add("Kelime", 115);
            wordsListView.Columns.Add("Açıklaması", 143);
            refreshWordListView();
            

        }

        private int wrongGuesses = 0;
        public string[] words;
        public string current = "";
        public string copyCurrent;

        private void refreshWordListView()
        {
            wordsListView.Items.Clear();
            StreamReader readUser = new StreamReader("words.txt", true);
            while (!readUser.EndOfStream)
            {
                string[] row = readUser.ReadLine().Split(',');
                var line = new ListViewItem(row);
                wordsListView.Items.Add(line);
            }
            readUser.Close();
        }
        private void LoadWords()
        {
            string[] readTxt = File.ReadAllLines("words.txt");
            words = new string[readTxt.Length];
            int index = 0;
            foreach (string s in readTxt)
            {
                string[] line = s.Split(',');
                words[index++] = line[1];
            }
        }

        private void SetupWordChoice()
        {
            wrongGuesses = 0;
            hangImage.Image = hangImages[wrongGuesses];
            int guessIndex = (new Random()).Next(words.Length);
            current = words[guessIndex];
            copyCurrent = "";
            for (int index = 0; index < current.Length; index++)
            {
                copyCurrent += "_";
            }
            DisplayCopy();
        }

        private void DisplayCopy()
        {
            showWordLabel.Text = "";
            for (int index = 0; index < current.Length; index++)
            {
                showWordLabel.Text += copyCurrent.Substring(index, 1);
                showWordLabel.Text += " ";
            }
        }

        int score = 1000;
        private void cmdClick(object sender, EventArgs e)
        {
            Button choice = sender as Button;
            choice.Enabled = false;
            if (current.Contains(choice.Text))
            {
                char[] temp = copyCurrent.ToCharArray();
                char[] find = current.ToCharArray();
                char guessChar = choice.Text.ElementAt(0);
                for (int index = 0; index < find.Length; index++)
                {
                    if (find[index] == guessChar)
                    {
                        temp[index] = guessChar;
                    }
                }
                copyCurrent = new string(temp);
                DisplayCopy();
            }
            else
            {
                score -= 100;
                wrongGuesses++;
                scoreLabel.Text = score.ToString();
            }

            if (wrongGuesses < 7)
            {
                hangImage.Image = hangImages[wrongGuesses];
            }
            else
            {
                MessageBox.Show("Kaybettiniz.");
                ScoreWrite();
                ScoreControl();
            }
            if (copyCurrent.Equals(current))
            {
                MessageBox.Show("Kazandınız!");
                ScoreWrite();
                ScoreControl();
            }
        }

        public void ScoreControl()
        {
            StreamReader readUser = new StreamReader("score.txt", true);
            scoreListview.Items.Clear();
            while (!readUser.EndOfStream)
            {
                string[] row = readUser.ReadLine().Split(',');
                var line = new ListViewItem(row);
                scoreListview.Items.Add(line);
            }
            readUser.Close();
        }
        public void ScoreWrite()
        {
            using (StreamWriter score = new StreamWriter("score.txt", true))
            {
                score.WriteLine(nameTextbox.Text + "," + scoreLabel.Text);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (nameTextbox.Text == "")
            {
                MessageBox.Show("Lütfen isminizi boş bırakmayınız!");
                return;
            }
            buttonsPanel.Enabled = true;
            SetupWordChoice();
            foreach (var item in buttonsPanel.Controls)
            {
                (item as Button).Enabled = true;
            }
        }

        private void addWordButton_Click(object sender, EventArgs e)
        {
            if (addWordTxt.Text == "" || addWordDecription.Text == "")
            {
                MessageBox.Show("Lütfen boş bırakmayınız!");
                return;
            }

            List<object[]> words = new List<object[]>();
            foreach (var item in File.ReadAllLines("words.txt"))
            {
                object[] word = new object[] { item.Split(',')[0], item.Split(',')[1], item.Split(',')[2] };
                words.Add(word);
            }
            int index = Convert.ToInt32(words.Last()[0]) + 1;
            File.AppendAllText("words.txt", index.ToString() + "," + addWordTxt.Text.ToLower() + ",(" + addWordDecription.Text.ToLower() + ")\n");
            MessageBox.Show("Kelime kaydedildi!");
        }

        private void scoreButton_Click(object sender, EventArgs e)
        {
            scorePanel.Visible = true;
        }

        private void exitScoreButton_Click(object sender, EventArgs e)
        {
            scorePanel.Visible = false;
        }

        private void onlyStringKeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !(char.IsLetter(e.KeyChar) || e.KeyChar == (char)Keys.Back);
        }


        private void allWordButton_Click(object sender, EventArgs e)
        {
            panel2.Visible = true;


        }

        private void backAddWordButton_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var id = wordsListView.SelectedItems[0].SubItems[0].Text;
            var lines = File.ReadAllLines("words.txt");
            File.WriteAllLines("words.txt", lines.Except(lines.Where(x => x.Split(',')[0] == id)));
            refreshWordListView();
            //SİLME VE SKOR KAYDETME ARTIK SORUNLU DEĞİL
        }
    }
}
