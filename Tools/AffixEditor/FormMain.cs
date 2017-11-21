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

namespace AffixEditor
{
    public partial class FormMain : Form
    {
        // List of all affix types for easier access
        // (Don't need to filter out None and Random)
        private List<AffixType> affixTypes;        
        private AffixInfo currentInfo; // A reference to the AffixInfo object we're currently looking at

        private string dataPath;

        #region Initialization
        public FormMain()
        {
            InitializeComponent();

            Serializer.LoadAllAffixInfosFromDisk();
            InitializeAffixTypeList();
            PopulateAffixInfoListBox();
            PopulateAffixValueTypeComboBox();
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
        #endregion

        #region UI Events
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

        private void AffixValueTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SwitchAffixValuePanel((AffixValueType)AffixValueTypeComboBox.Items[AffixValueTypeComboBox.SelectedIndex]);
        }
        #endregion

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

                AffixValueTypeSinglePanel.Visible = false;
                AffixValueTypeRangePanel.Visible = false;

                AffixValueTypeSingleMinTextBox.Text = "";
                AffixValueTypeSingleMaxTextBox.Text = "";
            }
            else
            {
                AffixNameTextBox.Text = currentInfo.Name;
                AffixNameTextBox.Enabled = true;

                AffixDescriptionTextBox.Text = currentInfo.Description;
                AffixDescriptionTextBox.Enabled = true;

                AffixValueTypeComboBox.SelectedIndex = AffixValueTypeComboBox.Items.IndexOf(currentInfo.ValueType);
                AffixValueTypeComboBox.Enabled = true;

                if (currentInfo.ValueType == AffixValueType.SingleValue)
                {
                    AffixValueTypeSingleMinTextBox.Text = currentInfo.ValueInfo.BaseValueMin.ToString();
                    AffixValueTypeSingleMaxTextBox.Text = currentInfo.ValueInfo.BaseValueMax.ToString();
                }
            }
        }

        private void SwitchAffixValuePanel(AffixValueType type)
        {
            if (type == AffixValueType.SingleValue)
            {
                AffixValueTypeSinglePanel.Visible = true;
                AffixValueTypeRangePanel.Visible = false;
            }
            else if (type == AffixValueType.Range)
            {
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
    }
}
