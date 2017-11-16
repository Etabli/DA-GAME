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
        public FormMain()
        {
            InitializeComponent();

            Serializer.LoadAffixInfoFromDisk(AffixType.Health);
        }

        private void BtnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show(AffixInfo.GenerateAffix(AffixType.PhysDmgFlat, 5).ToString());
        }
    }
}
