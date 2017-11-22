using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace AffixEditor
{
    public partial class FormMain : Form
    {
        // List of all affix types for easier access
        // (Don't need to filter out None and Random)
        private List<AffixType> affixTypes;        
        private AffixInfo currentInfo; // A reference to the AffixInfo object we're currently looking at

        #region Properties
        // Some properties to check for changes
        private bool _nameChanged;
        private bool nameChanged {
            get
            {
                return _nameChanged;
            }
            set
            {
                _nameChanged = value;
                if (_nameChanged)
                    AffixNameLabel.Text = "Name*";
                else
                    AffixNameLabel.Text = "Name";
            }
        }
        private bool _descriptionChanged;
        private bool descriptionChanged
        {
            get { return _descriptionChanged; }
            set
            {
                _descriptionChanged = value;
                if (_descriptionChanged)
                    AffixDescriptionLabel.Text = "Description*";
                else
                    AffixDescriptionLabel.Text = "Description";
            }
        }
        private bool _progressionChanged;
        private bool progressionChanged
        {
            get { return _progressionChanged; }
            set
            {
                _progressionChanged = value;
                if (_progressionChanged)
                    AffixProgressionLabel.Text = "Progression*";
                else
                    AffixProgressionLabel.Text = "Progression";
            }
        }
        private bool _valueTypeChanged;
        private bool valueTypeChanged
        {
            get { return _valueTypeChanged; }
            set
            {
                _valueTypeChanged = value;
                if (_valueTypeChanged)
                    AffixValueTypeLabel.Text = "Type*";
                else
                    AffixValueTypeLabel.Text = "Type";
            }
        }
        #endregion

        private string dataPath;

        #region Initialization
        public FormMain()
        {
            InitializeComponent();

            Serializer.LoadAllAffixInfosFromDisk();
            InitializeAffixTypeList();
            PopulateAffixInfoListBox();
            PopulateAffixValueTypeComboBox();
            PopulateAffixProgressionComboBox();
        }

        private void InitializeAffixTypeList()
        {
            affixTypes = new List<AffixType>();
            foreach (AffixType type in Enum.GetValues(typeof(AffixType)))
            {
                if (type != AffixType.None && type != AffixType.Random)
                    affixTypes.Add(type);
            }
        }

        private void PopulateAffixInfoListBox()
        {
            AffixInfosListBox.Items.Clear();
            foreach (var type in affixTypes)
                AffixInfosListBox.Items.Add(type);
        }

        private void PopulateAffixValueTypeComboBox()
        {
            AffixValueTypeComboBox.Items.AddRange(Enum.GetValues(typeof(AffixValueType)).Cast<object>().ToArray());
        }

        private void PopulateAffixProgressionComboBox()
        {
            AffixProgressionComboBox.Items.AddRange(typeof(AffixProgression)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(method => method.ReturnType == typeof(AffixValue))
                .Select(method => method.Name).ToArray());
        }
        #endregion

        #region UI Events

        #region Main
        private void AffixInfosSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            AffixInfosListBox.Items.Clear();

            var filteredTypes = affixTypes.FindAll(type => type.ToString().ToLower().Contains(AffixInfosSearchTextBox.Text.ToLower()));
            foreach (var type in filteredTypes)
                AffixInfosListBox.Items.Add(type);
        }

        private void AffixInfosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AffixInfosListBox.SelectedIndex == -1)
            {
                currentInfo = null;
            }
            else
            {
                currentInfo = AffixInfo.GetAffixInfo((AffixType)AffixInfosListBox.Items[AffixInfosListBox.SelectedIndex]);
            }

            UpdateCurrentAffixInfoDisplay();
        }

        private void openDataFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browser = new FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                dataPath = browser.SelectedPath + "\\";
                Serializer.LoadAllAffixInfosFromDisk(dataPath + "Affix\\AffixInfo\\");
            }
        }
        #endregion

        #region Affix Info

        #region Name, Description, ValueType, Progression
        private void AffixValueTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchAffixValuePanel();

            if (currentInfo.ValueType.ToString() == AffixValueTypeComboBox.Text)
                valueTypeChanged = false;
            else
                valueTypeChanged = true;
        }

        private void AffixNameTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentInfo.Name == AffixNameTextBox.Text)
                nameChanged = false;
            else
                nameChanged = true;
        }

        private void AffixDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            if (currentInfo.Description == AffixDescriptionTextBox.Text)
                descriptionChanged = false;
            else
                descriptionChanged = true;
        }

        private void AffixProgressionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (currentInfo.ValueInfo.Progression.GetName() == AffixProgressionComboBox.Text)
                progressionChanged = false;
            else
                progressionChanged = true;
        }
        #endregion

        #region ValueType Range Panel
        // TODO: Make this prettier
        private void AffixValueTypeRangeMinMinTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't range we don't need to check for changes here
            if (valueTypeChanged)
                return;

            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MinValue.ToString());

            CheckAffixValueTypeRangeMinChanged();
        }

        private void AffixValueTypeRangeMinMaxTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't range we don't need to check for changes here
            if (valueTypeChanged)
                return;

            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MaxValue.ToString());

            CheckAffixValueTypeRangeMinChanged();
        }

        private void AffixValueTypeRangeMaxMinTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't range we don't need to check for changes here
            if (valueTypeChanged)
                return;
            
            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MinValue.ToString());

            CheckAffixValueTypeRangeMaxChanged();
        }

        private void AffixValueTypeRangeMaxMaxTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't range we don't need to check for changes here
            if (valueTypeChanged)
                return;

            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MaxValue.ToString());

            CheckAffixValueTypeRangeMaxChanged();
        }
        #endregion

        #region ValueType Single Panel
        private void AffixValueTypeSingleMinTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't single we don't need to check for changes here
            if (valueTypeChanged)
                return;

            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMin as AffixValueSingle).ToString());
            CheckAffixValueTypeSingleChanged();
        }

        private void AffixValueTypeSingleMaxTextBox_TextChanged(object sender, EventArgs e)
        {
            // If the original type wasn't single we don't need to check for changes here
            if (valueTypeChanged)
                return;

            ValidateFloatTextBoxText(sender as TextBox, (currentInfo.ValueInfo.BaseValueMax as AffixValueSingle).ToString());
            CheckAffixValueTypeSingleChanged();
        }
        #endregion

        #endregion

        #endregion // UI Events

        /// <summary>
        /// Fills in all forms based on currentInfo
        /// </summary>
        private void UpdateCurrentAffixInfoDisplay()
        {
            if (currentInfo == null)
            {
                AffixNameTextBox.Text = "";
                AffixNameTextBox.Enabled = false;

                AffixDescriptionTextBox.Text = "";
                AffixDescriptionTextBox.Enabled = false;

                AffixValueTypeComboBox.SelectedIndex = 0;
                AffixValueTypeComboBox.Enabled = false;

                AffixProgressionComboBox.SelectedIndex = 0;
                AffixProgressionComboBox.Enabled = false;

                AffixValueTypeSinglePanel.Visible = false;
                AffixValueTypeRangePanel.Visible = false;

                AffixValueTypeSingleMinTextBox.Text = "";
                AffixValueTypeSingleMaxTextBox.Text = "";

                AffixValueTypeRangeMinMinTextBox.Text = "";
                AffixValueTypeRangeMinMaxTextBox.Text = "";
                AffixValueTypeRangeMaxMinTextBox.Text = "";
                AffixValueTypeRangeMaxMaxTextBox.Text = "";
            }
            else
            {
                AffixNameTextBox.Text = currentInfo.Name;
                AffixNameTextBox.Enabled = true;

                AffixDescriptionTextBox.Text = currentInfo.Description;
                AffixDescriptionTextBox.Enabled = true;

                AffixValueTypeComboBox.SelectedIndex = AffixValueTypeComboBox.Items.IndexOf(currentInfo.ValueType);
                AffixValueTypeComboBox.Enabled = true;

                AffixProgressionComboBox.SelectedIndex = AffixProgressionComboBox.Items.IndexOf(currentInfo.ValueInfo.Progression.GetName());
                AffixProgressionComboBox.Enabled = true;

                if (currentInfo.ValueType == AffixValueType.SingleValue)
                {
                    AffixValueTypeSingleMinTextBox.Text = currentInfo.ValueInfo.BaseValueMin.ToString();
                    AffixValueTypeSingleMaxTextBox.Text = currentInfo.ValueInfo.BaseValueMax.ToString();
                }
                else if (currentInfo.ValueType == AffixValueType.Range)
                {
                    AffixValueTypeRangeMinMinTextBox.Text = (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MinValue.ToString();
                    AffixValueTypeRangeMinMaxTextBox.Text = (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MaxValue.ToString();
                    AffixValueTypeRangeMaxMinTextBox.Text = (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MinValue.ToString();
                    AffixValueTypeRangeMaxMaxTextBox.Text = (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MaxValue.ToString();
                }
            }
        }

        private void SwitchAffixValuePanel()
        {
            AffixValueType type = (AffixValueType)Enum.Parse(typeof(AffixValueType), AffixValueTypeComboBox.Text);

            if (type == AffixValueType.SingleValue)
            {
                AffixValueTypeSinglePanel.Location = new Point(6, 237);
                AffixValueTypeSinglePanel.Visible = true;
                AffixValueTypeRangePanel.Visible = false;
            }
            else if (type == AffixValueType.Range)
            {
                AffixValueTypeRangePanel.Location = new Point(6, 237);
                AffixValueTypeRangePanel.Visible = true;
                AffixValueTypeSinglePanel.Visible = false;
            }
            else if (type == AffixValueType.Multiple)
            {
                AffixValueTypeRangePanel.Visible = false;
                AffixValueTypeSinglePanel.Visible = false;
            }
        }

        // Checks keypresses to only allow number inputs
        private void CheckAffixValueTextBoxInput(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
            (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '-') && (((sender as TextBox).Text.IndexOf('-') > -1) || ((sender as TextBox).SelectionStart > 0)))
            {
                e.Handled = true;
            }
        }

        private void generateSampleAffixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(currentInfo.GenerateAffix(1).ToString());
        }
        
        private void CheckAffixValueTypeRangeMinChanged()
        {
            string labelText = "Minimum Value";
            if (currentInfo.ValueType != AffixValueType.Range)
            {
                AffixValueTypeRangeMinLabel.Text = labelText + "*";
                return;
            }

            // Check if both textboxes are already filled
            if (AffixValueTypeRangeMinMinTextBox.Text == "" || AffixValueTypeRangeMinMaxTextBox.Text == "")
            {
                AffixValueTypeRangeMinLabel.Text = labelText;
                return;
            }

            Range original = (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value;
            Range current = new Range(float.Parse(AffixValueTypeRangeMinMinTextBox.Text), float.Parse(AffixValueTypeRangeMinMaxTextBox.Text));
            UpdateAffixValueLabel(AffixValueTypeRangeMinLabel, original, current, labelText);
        }

        private void CheckAffixValueTypeRangeMaxChanged()
        {
            string labelText = "Maximum Value";
            if (currentInfo.ValueType != AffixValueType.Range)
            {
                AffixValueTypeRangeMaxLabel.Text = labelText + "*";
                return;
            }

            // Check if both textboxes are already filled
            if (AffixValueTypeRangeMaxMinTextBox.Text == "" || AffixValueTypeRangeMaxMaxTextBox.Text == "")
            {
                AffixValueTypeRangeMaxLabel.Text = labelText;
                return;
            }

            Range original = (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value;
            Range current = new Range(float.Parse(AffixValueTypeRangeMaxMinTextBox.Text), float.Parse(AffixValueTypeRangeMaxMaxTextBox.Text));
            UpdateAffixValueLabel(AffixValueTypeRangeMaxLabel, original, current, labelText);
        }

        private void CheckAffixValueTypeSingleChanged()
        {
            string labelText = "Value";
            if (currentInfo.ValueType != AffixValueType.SingleValue)
            {
                AffixValueTypeSingleLabel.Text = labelText + "*";
            }

            // Check if both textboxes are already filled
            if (AffixValueTypeSingleMinTextBox.Text == "" || AffixValueTypeSingleMaxTextBox.Text == "")
            {
                AffixValueTypeSingleLabel.Text = labelText;
                return;
            }

            Range original = new Range(currentInfo.ValueInfo.BaseValueMin as AffixValueSingle, currentInfo.ValueInfo.BaseValueMax as AffixValueSingle);
            Range current = new Range(float.Parse(AffixValueTypeSingleMinTextBox.Text), float.Parse(AffixValueTypeSingleMaxTextBox.Text));
            UpdateAffixValueLabel(AffixValueTypeSingleLabel, original, current, labelText);
        }

        private void UpdateAffixValueLabel(Label label, Range original, Range current, string labelText)
        {
            if (original == current)
                label.Text = labelText;
            else
                label.Text = labelText + "*";
        }

        private void ValidateFloatTextBoxText(TextBox textBox, string defaultText)
        {
            if (!float.TryParse(textBox.Text, out _))
            {
                textBox.Text = defaultText;
            }
        }
    }
}
