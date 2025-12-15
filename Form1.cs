using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace GenerateRandomCharacters
{
    public partial class Form1 : Form
    {
        static Random rand = new Random();

        enum enCharType
        {
            SmallLetter = 1,
            CapitalLetter = 2,
            SpecialCharacter = 3,
            Digit = 4,
            Mix = 5
        };

        List<string> clipboardHistory = new List<string>();

        int currentIndex = -1;
        int maxHistory = 20;

        public Form1()
        {
            InitializeComponent();
        }

        private void AddToClipboardHistory(string key)
        {
            if (!clipboardHistory.Contains(key))
            {
                clipboardHistory.Add(key);
            }

            if (clipboardHistory.Count > maxHistory)
            {
                clipboardHistory.RemoveAt(0);
            }

            currentIndex = clipboardHistory.Count - 1;
        }

        private char GetRandomCharacter(List<enCharType> types)
        {
            enCharType randType = types[rand.Next(types.Count)];

            switch (randType)
            {
                case enCharType.SmallLetter:
                    return (char)(rand.Next(97, 123));

                case enCharType.CapitalLetter:
                    return (char)(rand.Next(65, 91));

                case enCharType.SpecialCharacter:
                    return (char)(rand.Next(33, 48));

                case enCharType.Digit:
                    return (char)(rand.Next(48, 58));

                case enCharType.Mix:
                    var allTypes = new List<enCharType>
                    {
                        enCharType.SmallLetter,
                        enCharType.CapitalLetter,
                        enCharType.SpecialCharacter,
                        enCharType.Digit
                    };
                    return GetRandomCharacter(allTypes);

                default:
                    return 'X';
            }
        }

        private string GenerateWord(List<enCharType> types, short Length)
        {
            StringBuilder Word = new StringBuilder();

            for (int i = 0; i < Length; i++)
            {
                Word.Append(GetRandomCharacter(types));
            }

            return Word.ToString();
        }

        private string GenerateKey(List<enCharType> types, short partLength, short partsCount)
        {
            List<string> parts = new List<string>();

            for (byte i = 0; i < partsCount; i++)
            {
                parts.Add(GenerateWord(types, partLength));
            }

            return string.Join("-", parts);
        }

        private void GenerateKeys(List<enCharType> types, short partLength, short partsCount)
        {
            txtOutput.Clear();

            string key = GenerateKey(types, partLength, partsCount);

            txtOutput.Text = key;

            AddToClipboardHistory(key);
        }

        private List<enCharType> GetSelectedTypes()
        {
            List<enCharType> types = new List<enCharType>();

            if (cbMix.Checked)
            {
                types.Add(enCharType.SmallLetter);
                types.Add(enCharType.CapitalLetter);
                types.Add(enCharType.SpecialCharacter);
                types.Add(enCharType.Digit);
                return types;
            }

            if (cbCapital.Checked) types.Add(enCharType.CapitalLetter);
            if (cbSmall.Checked) types.Add(enCharType.SmallLetter);
            if (cbSpecial.Checked) types.Add(enCharType.SpecialCharacter);
            if (cbDigits.Checked) types.Add(enCharType.Digit);

            return types;
        }

        private void Generate()
        {
            List<enCharType> types = GetSelectedTypes();

            if (types.Count == 0)
            {
                MessageBox.Show(
                    "Please select at least one character type.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (nudLength.Value == 0)
            {
                MessageBox.Show(
                    "Please specify the character length.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (nudPartsCount.Value == 0)
            {
                MessageBox.Show(
                    "Please specify the number of parts.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            GenerateKeys(types, (short)nudLength.Value, (short)nudPartsCount.Value);
        }

        private void Copy()
        {
            if (string.IsNullOrWhiteSpace(txtOutput.Text))
                return;

            Clipboard.SetText(txtOutput.Text);
            txtOutput.Focus();
            txtOutput.SelectAll();

            MessageBox.Show(
                "Copied Successfully",
                "Copy",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void Delete()
        {
            txtOutput.Clear();
        }

        private void SelectAll()
        {
            if (string.IsNullOrWhiteSpace(txtOutput.Text))
                return;

            txtOutput.Focus();
            txtOutput.SelectAll();
        }

        private void Reset()
        {
            nudLength.Value = 4;
            txtOutput.Clear();

            foreach (Control ctrl in gbOptions.Controls)
            {
                if (ctrl is CheckBox cb)
                {
                    cb.Checked = false;
                }
            }

            UpdateCharacterTypeOptionsState();
        }

        private void UpdateCharacterTypeOptionsState()
        {
            bool enabled = !cbMix.Checked;

            cbCapital.Enabled = enabled;
            cbSmall.Enabled = enabled;
            cbSpecial.Enabled = enabled;
            cbDigits.Enabled = enabled;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Generate();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Reset();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Delete();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectAll();
        }

        private void cbMix_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCharacterTypeOptionsState();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (clipboardHistory.Count == 0)
                return;

            if (e.KeyCode == Keys.Up)
            {
                if (currentIndex > 0)
                {
                    currentIndex--;
                    txtOutput.Text = clipboardHistory[currentIndex];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (currentIndex < clipboardHistory.Count - 1)
                {
                    currentIndex++;
                    txtOutput.Text = clipboardHistory[currentIndex];
                }
            }
        }
    }
}