using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    [SerializeField]
    Button NewButton;
    [SerializeField]
    Button DeleteButton;
    [SerializeField]
    Button RenameButton;
    [SerializeField]
    Button SaveButton;

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

        // async await to silence warnings
        NewButton.onClick.AddListener(async () => await CreateNew());
        DeleteButton.onClick.AddListener(async () => await DeleteSelectedType());
        RenameButton.onClick.AddListener(async () => await RenameSelectedType());
        SaveButton.onClick.AddListener(async () => await SaveSelectedType());
    }

    private async void Update()
    {
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && Input.GetKeyDown(KeyCode.S))
        {
            await SaveSelectedType();
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
    /// Shows a prompt for the user to enter a new name.
    /// </summary>
    public async Task CreateNew()
    {
        var result = await DialogStringInput.ShowBlocking("Enter a name:", name => !AffixType.Exists(name) && AffixType.IsValidName(name), ShowInvalidNameMessage).result;
        if (result.IsOK)
        {
            string name = result.Value;

            AffixInfo info = new AffixInfo(name, new AffixValueInfo());
            AffixInfo.Register(info);
            AddTypeToLIst(info.Type);
            UpdateHeight();
            SelectType(info.Type);
        }
    }

    /// <summary>
    /// Saves the selected affix type to disk and updates the entry in the AffixInfo dictionary.
    /// </summary>
    /// <returns>Whether or not there were any problems saving. Also true if there were no changes.</returns>
    public async Task<bool> SaveSelectedType()
    {
        if (selectedType == AffixType.None)
            return false;

        if (!AffixInfoDisplay.IsValid)
        {
            await DialogBasic.Show("Some inputs are invalid! Make sure no fields are empty.").result;
            return false;
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
        return true;
    }

    /// <summary>
    /// Delets the selected affix type.
    /// </summary>
    /// <returns>A task that returns whether or not the affix type was deleted.</returns>
    public async Task<bool> DeleteSelectedType()
    {
        if (selectedType == AffixType.None)
            return false;

        if (await DialogCancellable.ShowBlocking($"Are you sure you want to delete {selectedType.Name}?").result == DialogResult.OK)
        {
            AffixInfo.Deregister(selectedType);
            AffixType.Remove(selectedType);
            Destroy(displays[selectedType].gameObject);
            displays.Remove(selectedType);
            Serializer.DeleteAffixInfo(selectedType);
            selectedType = AffixType.None;
            UpdateHeight();
            return true;
        }
        return false;
    }

    public async Task RenameSelectedType()
    {
        if (selectedType == AffixType.None)
            return;

        if (!AffixInfoDisplay.IsValid)
        {
            DialogBasic.Show("Renaming requires that all changes are saved first, but there are invalid entries. Please fix all errors before trying again.");
            return;
        }

        if (AffixInfoDisplay.IsChanged)
        {
            if ((await DialogCancellable.Show("Renaming requires that all changes be saved first. Save now?").result).IsCancelled)
                return;

            if (await SaveSelectedType() == false)
            {
                DialogBasic.Show("There was a problem saving. Please recheck the values and try again.");
                return;
            }
        }

        var result = await DialogStringInput.ShowBlocking($"Enter a new name for '{selectedType.Name}'",
            name => !AffixType.Exists(name) && AffixType.IsValidName(name), ShowInvalidNameMessage).result;

        if (result.IsOK)
        {
            string name = result.Value;

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
        }
    }
}
