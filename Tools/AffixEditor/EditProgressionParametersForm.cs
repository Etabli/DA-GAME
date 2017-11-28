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
    public partial class EditProgressionParametersForm : Form
    {
        private const int TEXTBOX_SPACING = 25;

        private readonly float[] originalParameters;
        public float[] Parameters;

        public EditProgressionParametersForm(float[] parameters)
        {
            originalParameters = parameters;
            Parameters = (float[])originalParameters.Clone();

            for (int i = 0; i < parameters.Length; i++)
            {
                TextBox textbox = new TextBox
                {
                    Location = new Point(10, 10 + (i * TEXTBOX_SPACING)),
                    Text = parameters[i].ToString()
                };
                FloatTextBoxValidator validator = FloatTextBoxValidator.Create(textbox);

                int index = i;
                validator.TextChanged += (_) => { Parameters[index] = FloatTextBoxValidator.GetValue(textbox); };

                Controls.Add(textbox);
            }

            InitializeComponent();
            Height = parameters.Length * TEXTBOX_SPACING + 100;
        }

        public EditProgressionParametersForm(int nParams) : this(new float[nParams])
        { }
    }
}
