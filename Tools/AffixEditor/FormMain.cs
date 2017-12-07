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

        // Variables to hold our changes
        private AffixType localType;
        private AffixValueType localValueType;
        private string localName;
        private string localDescription;
        private AffixValueSingle localBaseValueMinSingle = new AffixValueSingle();
        private AffixValueSingle localBaseValueMaxSingle = new AffixValueSingle();
        private AffixValueRange localBaseValueMinRange = new AffixValueRange();
        private AffixValueRange localBaseValueMaxRange = new AffixValueRange();
        private float[] localProgressionParameters;
        private string localProgressionFunctionName;

        private string dataPath = ""; // String holding the path to the data directory

        #region Properties
        // Some properties to check for changes
        private bool _nameChanged;
        private bool nameChanged {
            get { return _nameChanged; }
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

        #region Initialization
        public FormMain()
        {
            InitializeComponent();

            WrapFloatTextBoxes();

            Serializer.LoadAllAffixInfosFromDisk();
            InitializeAffixTypeList();
            PopulateAffixInfoListBox();
            PopulateAffixValueTypeComboBox();
            PopulateAffixProgressionComboBox();
        }

        /// <summary>
        /// Wraps all text boxes for floats in a validator and registers the proper events
        /// </summary>
        private void WrapFloatTextBoxes()
        {
            // Value type single
            var validator = FloatTextBoxValidator.Create(AffixValueTypeSingleMinTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMinSingle = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeSingleChanged();
            };

            validator = FloatTextBoxValidator.Create(AffixValueTypeSingleMaxTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMaxSingle = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeSingleChanged();
            };

            // Value type range
            validator = FloatTextBoxValidator.Create(AffixValueTypeRangeMinMinTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMinRange.MinValue = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeRangeMinChanged();
            };

            validator = FloatTextBoxValidator.Create(AffixValueTypeRangeMinMaxTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMinRange.MaxValue = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeRangeMinChanged();
            };

            validator = FloatTextBoxValidator.Create(AffixValueTypeRangeMaxMinTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMaxRange.MinValue = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeRangeMaxChanged();
            };

            validator = FloatTextBoxValidator.Create(AffixValueTypeRangeMaxMaxTextBox, "0");
            validator.TextChanged += textbox =>
            {
                localBaseValueMaxRange.MaxValue = FloatTextBoxValidator.GetValue(textbox);
                CheckAffixValueTypeRangeMaxChanged();
            };
        }

        private void InitializeAffixTypeList()
        {
            affixTypes = new List<AffixType>();
            foreach (AffixType type in AffixType.Types)
            {
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

        int _last_index = -1;
        private void AffixInfosListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_last_index != -1 && AffixInfosListBox.SelectedIndex == _last_index)
                return;

            if (HasUnsavedChanges())
            {
                if (MessageBox.Show("Unsaved changes will be lost. Proceed?", "Unsaved Changes", MessageBoxButtons.OKCancel) == DialogResult.Cancel)
                {
                    AffixInfosListBox.SelectedIndex = _last_index;
                    return;
                }
            }

            if (AffixInfosListBox.SelectedIndex == -1)
            {
                currentInfo = null;
            }
            else
            {
                currentInfo = AffixInfo.GetAffixInfo((AffixType)AffixInfosListBox.Items[AffixInfosListBox.SelectedIndex]);

                if (localType != currentInfo.Type)
                    UpdateCurrentAffixInfoDisplay();

                localType = currentInfo.Type;
            }
            _last_index = AffixInfosListBox.SelectedIndex;
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

        private void SaveAffixInfoButton_Click(object sender, EventArgs e)
        {
            AffixProgression newProgression = new AffixProgression(localProgressionFunctionName, localProgressionParameters);

            AffixValueInfo newValueInfo = null;
            if (localValueType == AffixValueType.SingleValue)
                newValueInfo = new AffixValueInfo(localBaseValueMinSingle, localBaseValueMaxSingle, newProgression);
            else if (localValueType == AffixValueType.Range)
                newValueInfo = new AffixValueInfo(localBaseValueMinRange, localBaseValueMaxRange, newProgression);

            AffixInfo newInfo = new AffixInfo(localType, localValueType, localName, newValueInfo, localDescription);

            Serializer.SaveAffixInfoToDisk(newInfo);
            AffixInfo.Register(newInfo);
            currentInfo = newInfo;
            UpdateAllLabels();
        }
        #endregion

        #region Affix Info

        #region Name, Description, ValueType, Progression
        private void AffixValueTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            localValueType = (AffixValueType)Enum.Parse(typeof(AffixValueType), AffixValueTypeComboBox.Text);
            SwitchAffixValuePanel();
            CheckValueTypeChanged();
        }

        private void AffixNameTextBox_TextChanged(object sender, EventArgs e)
        {
            localName = AffixNameTextBox.Text;
            CheckNamechanged();
        }

        private void AffixDescriptionTextBox_TextChanged(object sender, EventArgs e)
        {
            localDescription = AffixDescriptionTextBox.Text;
            CheckDescriptionChanged();
        }

        private void AffixProgressionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            localProgressionFunctionName = AffixProgressionComboBox.Text;
            CheckAffixProgressionChanged();
        }

        private void EditProgressionParametersButton_Click(object sender, EventArgs e)
        {
            EditProgressionParametersForm paramEditor;
            if (progressionChanged)
                paramEditor = new EditProgressionParametersForm(AffixProgression.ParameterRequirements[AffixProgressionComboBox.Text]);
            else
                paramEditor = new EditProgressionParametersForm(currentInfo.ValueInfo.Progression.Parameters);

            if (paramEditor.ShowDialog() == DialogResult.OK)
            {
                localProgressionParameters = paramEditor.Parameters;
                CheckAffixProgressionChanged();
            }
        }
        #endregion // Name, Description, ValueType, Progression

        #endregion // Affix Info

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

                // Copy parameters first since changing the combobox index calls the SelectedIndexChanged event
                // at which point we already compare the original values to our local copy
                localProgressionParameters = new float[currentInfo.ValueInfo.Progression.Parameters.Length];
                currentInfo.ValueInfo.Progression.Parameters.CopyTo(localProgressionParameters, 0);
                AffixProgressionComboBox.SelectedIndex = AffixProgressionComboBox.Items.IndexOf(currentInfo.ValueInfo.Progression.GetName());
                AffixProgressionComboBox.Enabled = true;

                if (currentInfo.ValueType == AffixValueType.SingleValue)
                {
                    AffixValueTypeSingleMinTextBox.Text = currentInfo.ValueInfo.BaseValueMin.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeSingleMinTextBox);

                    AffixValueTypeSingleMaxTextBox.Text = currentInfo.ValueInfo.BaseValueMax.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeSingleMaxTextBox);
                }
                else if (currentInfo.ValueType == AffixValueType.Range)
                {
                    AffixValueTypeRangeMinMinTextBox.Text = (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MinValue.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeRangeMinMinTextBox);

                    AffixValueTypeRangeMinMaxTextBox.Text = (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value.MaxValue.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeRangeMinMaxTextBox);

                    AffixValueTypeRangeMaxMinTextBox.Text = (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MinValue.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeRangeMaxMinTextBox);

                    AffixValueTypeRangeMaxMaxTextBox.Text = (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value.MaxValue.ToString();
                    FloatTextBoxValidator.CaptureContentsAsDefault(AffixValueTypeRangeMaxMaxTextBox);
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

        private void generateSampleAffixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(AffixInfo.GenerateAffix(currentInfo.Type, 1).ToString());
        }

        #region Change Checking

        #region Name, Description, ValueType, Progression
        private void CheckNamechanged()
        {
            if (currentInfo.Name == localName)
                nameChanged = false;
            else
                nameChanged = true;
        }

        private void CheckDescriptionChanged()
        {
            if (currentInfo.Description == localDescription)
                descriptionChanged = false;
            else
                descriptionChanged = true;
        }

        private void CheckValueTypeChanged()
        {
            if (currentInfo.ValueType == localValueType)
                valueTypeChanged = false;
            else
                valueTypeChanged = true;
        }

        private void CheckAffixProgressionChanged()
        {
            progressionChanged = currentInfo.ValueInfo.Progression != new AffixProgression(localProgressionFunctionName, localProgressionParameters);
        }
        #endregion

        #region Affix Values
        private void CheckAffixValueTypeRangeMinChanged()
        {
            if (localValueType != AffixValueType.Range)
                return;

            Range original = currentInfo.ValueType == AffixValueType.Range ? (currentInfo.ValueInfo.BaseValueMin as AffixValueRange).Value : new Range();
            CheckAffixValueChanged(localBaseValueMinRange, original, AffixValueTypeRangeMinLabel, "Minimum Value");
        }

        private void CheckAffixValueTypeRangeMaxChanged()
        {
            Range original = currentInfo.ValueType == AffixValueType.Range ? (currentInfo.ValueInfo.BaseValueMax as AffixValueRange).Value : new Range();
            CheckAffixValueChanged(localBaseValueMaxRange, original, AffixValueTypeRangeMaxLabel, "Maximum Value");
        }

        private void CheckAffixValueTypeSingleChanged()
        {
            if (localValueType != AffixValueType.SingleValue)
                return;

            Range original = currentInfo.ValueType == AffixValueType.SingleValue
                ? new Range(currentInfo.ValueInfo.BaseValueMin as AffixValueSingle, currentInfo.ValueInfo.BaseValueMax as AffixValueSingle)
                : new Range();
            CheckAffixValueChanged(new Range(localBaseValueMinSingle, localBaseValueMaxSingle), original, AffixValueTypeSingleLabel, "Value");
        }

        private void CheckAffixValueChanged(Range current, Range original, Label label, string labelText)
        {
            if (valueTypeChanged)
                label.Text = labelText + "*";
            
            UpdateAffixValueLabel(label, current, original, labelText);
        }

        private void UpdateAffixValueLabel(Label label, Range original, Range current, string labelText)
        {
            if (original == current)
                label.Text = labelText;
            else
                label.Text = labelText + "*";
        }
        #endregion

        private void UpdateAllLabels()
        {
            CheckAffixProgressionChanged();
            CheckAffixValueTypeRangeMaxChanged();
            CheckAffixValueTypeRangeMinChanged();
            CheckAffixValueTypeSingleChanged();
            CheckNamechanged();
            CheckDescriptionChanged();
            CheckValueTypeChanged();
        }

        private bool HasUnsavedChanges()
        {
            return !(!nameChanged && !descriptionChanged && !progressionChanged && !valueTypeChanged &&
                !AffixValueTypeSingleLabel.Text.EndsWith("*") && !AffixValueTypeRangeMaxLabel.Text.EndsWith("*") &&
                !AffixValueTypeRangeMinLabel.Text.EndsWith("*"));
        }
        #endregion
    }
}
