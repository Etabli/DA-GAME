using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UserDialog
{
    public class DialogBasic : Dialog
    {
        [SerializeField]
        protected Button OKButton;

        protected override void Awake()
        {
            base.Awake();

            if (OKButton == null)
                throw new UserDialogException("User dialog's reference to OK button is not set!");

            OKButton.onClick.AddListener(Close);
        }
    }
}
