using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AffixEditor
{
    public partial class FormMain : Form
    {
        // List of all affix types for easier access
        // (Don't need to filter out None and Random)
        private List<AffixType> affixTypes;

        #region Initialization
        public FormMain()
        {
            InitializeComponent();

            Serializer.LoadAllAffixInfosFromDisk();
            InitializeAffixTypeList();
            PopulateAffixInfoListBox();
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
            foreach (var type in affixTypes)
                AffixInfosListBox.Items.Add(type);
        }
        #endregion

        private void AffixInfosSearchTextBox_TextChanged(object sender, EventArgs e)
        {
            AffixInfosListBox.Items.Clear();

            var filteredTypes = affixTypes.FindAll(type => type.ToString().ToLower().Contains(AffixInfosSearchTextBox.Text.ToLower()));
            foreach (var type in filteredTypes)
                AffixInfosListBox.Items.Add(type);
        }
    }
}
