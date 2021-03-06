﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AffixEditor
{
    /// <summary>
    /// Wraps a textbox to ensure it can only contain a valid floating point number
    /// </summary>
    class FloatTextBoxValidator
    {
        private TextBox textBox;

        public string DefaultText;

        /// <summary>
        /// This event is called after the text of the wrapped text box has been validated
        /// </summary>
        public event Action<TextBox> TextChanged;

        public FloatTextBoxValidator(TextBox textBox, string defaultText)
        {
            if (!float.TryParse(defaultText, out _))
                throw new ArgumentException($"Default text for float text box has to be a valid floating point number! Passed value is {defaultText}", nameof(defaultText));

            this.textBox = textBox;
            DefaultText = defaultText;

            textBox.KeyPress += InterceptKeys;
            textBox.TextChanged += ValidateContents;
        }

        public FloatTextBoxValidator(TextBox textBox) : this(textBox, textBox.Text)
        { }

        ~FloatTextBoxValidator()
        {
            textBoxDictionary.Remove(textBox);
        }

        /// <summary>
        /// Saves the current contents of the wrapped text box as the new default value
        /// </summary>
        public void CaptureContentsAsDefault()
        {
            DefaultText = textBox.Text;
        }

        /// <summary>
        /// Returns the content of the text box as a float
        /// </summary>
        public float GetValue()
        {
            return textBox.Text == "" ? 0 : float.Parse(textBox.Text);
        }

        /// <summary>
        /// Checks keypresses and intercepts them if they aren't valid
        /// </summary>
        private void InterceptKeys(object sender, KeyPressEventArgs e)
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

        private void ValidateContents(object sender, EventArgs e)
        {
            if (!float.TryParse(textBox.Text, out _) && textBox.Text != "")
            {
                textBox.Text = DefaultText;
            }
            TextChanged?.Invoke(textBox);
        }

        #region Static Functionality
        private static Dictionary<TextBox, FloatTextBoxValidator> textBoxDictionary = new Dictionary<TextBox, FloatTextBoxValidator>();

        /// <summary>
        /// Creates a new instance of this class, adds it to the internal dictionary, and returns it
        /// </summary>
        public static FloatTextBoxValidator Create(TextBox textBox, string defaultText)
        {
            FloatTextBoxValidator validator = new FloatTextBoxValidator(textBox, defaultText);
            textBoxDictionary.Add(textBox, validator);
            return validator;
        }

        public static FloatTextBoxValidator Create(TextBox textBox) => Create(textBox, textBox.Text);

        /// <summary>
        /// Retrieves the corresponding validator for a TextBox
        /// </summary>
        public static FloatTextBoxValidator GetValidatorForTextBox(TextBox textBox)
        {
            if (!textBoxDictionary.ContainsKey(textBox))
            {
                return null;
            }
            return textBoxDictionary[textBox];
        }

        public static void CaptureContentsAsDefault(TextBox textBox)
        {
            GetValidatorForTextBox(textBox)?.CaptureContentsAsDefault();
        }

        /// <summary>
        /// Gets the float value through the validator corresponding to the passed text box.
        /// Throws an exception if there is no validator
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns></returns>
        public static float GetValue(TextBox textBox)
        {
            // Note that this throws an exception if there is no validator for this text box
            return (float)GetValidatorForTextBox(textBox)?.GetValue();
        }
        #endregion
    }
}
