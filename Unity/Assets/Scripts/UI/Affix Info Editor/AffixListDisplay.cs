using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UserDialog;

public class AffixListDisplay : MonoBehaviour
{
    const int HEIGHT_PER_AFFIX = 20;

    public GameObject AffixTypeDisplay;
    public RectTransform RectTransform;
    public AffixInfoDisplay AffixInfoDisplay;

    AffixType selectedType = AffixType.None;
    Dictionary<AffixType, AffixTypeDisplay> displays = new Dictionary<AffixType, AffixTypeDisplay>();

    private void Start()
    {
        foreach (var type in AffixType.Types)
        {
            AddTypeToLIst(type);
        }

        UpdateHeight();

        AffixInfoDisplay.OnChangedStatusUpdated += changed =>
        {
            if (selectedType == AffixType.None)
                return;

            displays[selectedType].SetChanged(changed);
        };
    }

    private void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
        {
            SaveSelectedType();
        }
    }

    void SelectType(AffixType type)
    {
        // Check names too because when renaming ID stays the same
        if (type == selectedType && type.Name == selectedType.Name)
            return;

        if (AffixInfoDisplay.IsChanged)
            DialogController.ShowBlocking("There are unsaved changes, are you sure you want to select a different type?"
                , () =>
                {
                    displays[selectedType].SetChanged(false);
                    selectedType = type;
                    AffixInfoDisplay.SetType(selectedType);
                }, null);
        else
        {
            selectedType = type;
            AffixInfoDisplay.SetType(selectedType);
        }
    }

    void AddTypeToLIst(AffixType type)
    {
        var display_go = Instantiate(AffixTypeDisplay, transform);
        AffixTypeDisplay display = display_go.GetComponent<AffixTypeDisplay>();
        displays[type] = display;

        display.SetAffixType(type);
        display.OnClick += SelectType;
    }

    void UpdateHeight()
    {
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, AffixType.Types.Count * HEIGHT_PER_AFFIX);
    }

    void ShowInvalidNameMessage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            DialogController.Show("Name cannot be emtpy!");
        else
            DialogController.Show($"An affix type with the name '{name}' already exists!");
    }

    /// <summary>
    /// Shows a prompt for the user to enter a new name
    /// </summary>
    public void CreateNew()
    {
        DialogController.ShowBlocking("Enter a name:", 
            name =>
            {
                AffixInfo info = new AffixInfo(name, new AffixValueInfo());
                AffixInfo.Register(info);
                AddTypeToLIst(info.Type);
                UpdateHeight();
                SelectType(info.Type);
            }
            , null, name => !AffixType.Exists(name) && AffixType.IsValidName(name)
            , ShowInvalidNameMessage);
    }

    public void SaveSelectedType()
    {
        if (selectedType == AffixType.None)
            return;

        if (!AffixInfoDisplay.IsValid)
        {
            DialogController.Show("Some inputs are invalid! Make sure no fields are empty.");
            return;
        }

        if (AffixInfoDisplay.IsChanged)
        {
            var newInfo = AffixInfoDisplay.Info;
            AffixInfo.Deregister(selectedType);
            AffixInfo.Register(newInfo);
            Serializer.SaveAffixInfoToDisk(newInfo);

            // Updates all the changed variables
            AffixInfoDisplay.SetType(newInfo.Type);  
        }
    }

    public void DeleteSelectedType()
    {
        if (selectedType == AffixType.None)
            return;

        DialogController.ShowBlocking($"Are you sure you want to delete {selectedType.Name}?",
            () =>
            {
                AffixInfo.Deregister(selectedType);
                AffixType.Remove(selectedType);
                Destroy(displays[selectedType].gameObject);
                displays.Remove(selectedType);
                Serializer.DeleteAffixInfo(selectedType);
                selectedType = AffixType.None;
                UpdateHeight();
            }, null);        
    }

    public void RenameSelectedType()
    {
        if (selectedType == AffixType.None)
            return;

        DialogController.ShowBlocking($"Enter a new name for '{selectedType.Name}'",
            name =>
            {
                var oldInfo = AffixInfo.GetAffixInfo(selectedType);
                AffixInfo.Deregister(selectedType);
                Serializer.DeleteAffixInfo(oldInfo.Type);

                var newType = AffixType.Replace(selectedType.ID, name);
                var newInfo = new AffixInfo(newType, oldInfo.ValueInfo, oldInfo.Description);
                AffixInfo.Register(newInfo);
                Serializer.SaveAffixInfoToDisk(newInfo);

                var display = displays[selectedType];
                displays.Remove(selectedType);
                displays.Add(newType, display);
                display.SetAffixType(newType);
                SelectType(newType);

            }, null, name => !AffixType.Exists(name) && AffixType.IsValidName(name), ShowInvalidNameMessage);
    }
}
