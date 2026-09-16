using UnityEngine;
using UnityEngine.UIElements;

public class MenuToolkit : MonoBehaviour
{
    private enum Panels
    {
        Main,
        Options,
        Exit
    }

    private enum PanelType
    {
        Default,
        Additive
    }
    
    private UIDocument _doc;
    private VisualElement _root;
    
    private VisualElement _mainPanel;
    private VisualElement _optionsPanel;
    private VisualElement _exitPrompt;
    
    private Button _playButton;
    private Button _optionButton;
    private Button _quitButton;
    
    private Button _backButton;
    private Button _applyButton;
    
    private Button _yesButton;
    private Button _noButton;

    private DropdownField _resolutionField;

    private bool _is2D = true;
    
    #region Setup
    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;
        
        _mainPanel = _root.Q<VisualElement>("Main");
        _optionsPanel = _root.Q<VisualElement>("Options");
        _exitPrompt = _root.Q<VisualElement>("ExitPrompt");
        
        _playButton = GetButton("PlayButton");
        _optionButton = GetButton("OptionsButton");
        _quitButton = GetButton("QuitButton");
        
        _yesButton = GetButton("YesButton");
        _noButton = GetButton("NoButton");
        
        _backButton = GetButton("BackButton");
        _applyButton = GetButton("ApplyButton");
        
        _resolutionField = _root.Q<DropdownField>("ResolutionDrop");
        _resolutionField.RegisterValueChangedCallback(OnResolutionSelected);

        _playButton.clicked += PlayPressed;
        _optionButton.clicked += OptionsPressed;
        _quitButton.clicked += QuitPressed;
        
        _backButton.clicked += BackPressed;
        _applyButton.clicked += ApplyPressed;
        
        _yesButton.clicked += YesPressed;
        _noButton.clicked += NoPressed;
    }

    private void OnDisable()
    {
        _playButton.clicked -= PlayPressed;
        _optionButton.clicked -= OptionsPressed;
        _quitButton.clicked -= QuitPressed;
        
        _backButton.clicked -= BackPressed;
        _applyButton.clicked -= ApplyPressed;
        
        _yesButton.clicked -= YesPressed;
        _noButton.clicked -= NoPressed;
    }
    #endregion
    
    #region  Click Events
    private void PlayPressed()
    {
        print("Play Pressed");
    }
    
    private void OptionsPressed()
    {
        SwitchPanel(Panels.Options);
    }

    private void BackPressed()
    {
        print("Back Pressed");
        SwitchPanel(Panels.Main);
    }
    
    private void QuitPressed()
    {
        SwitchPanel(Panels.Exit, PanelType.Additive);
    }

    private void ApplyPressed()
    {
        print("Apply Pressed");
    }
    
    private void YesPressed()
    {
        SwitchPanel(Panels.Main);
    }
    
    private void NoPressed()
    {
        SwitchPanel(Panels.Main);
    }
    #endregion
    
    #region Panel Management
    private void SwitchPanel(Panels panel, PanelType panelType = PanelType.Default)
    {
        if (panelType == PanelType.Default)
        {
            SetPanelDisplay(_mainPanel, false);
            SetPanelDisplay(_optionsPanel, false);
            SetPanelDisplay(_exitPrompt, false);
        }
        
        var activePanel = panel switch
        {
            Panels.Main => _mainPanel,
            Panels.Options => _optionsPanel,
            Panels.Exit => _exitPrompt,
            _ => _mainPanel
        };
        
        SetPanelDisplay(activePanel);
    }

    private void SetPanelDisplay(VisualElement panel, bool visible = true)
    {
        panel.style.display = (visible) ? DisplayStyle.Flex : DisplayStyle.None;
    }
    
    private Button GetButton(string buttonName)
    {
        var container = _root.Q<TemplateContainer>(buttonName); 
        return container.Q<Button>("GameButton");
    }
    #endregion
    
    private void OnResolutionSelected(ChangeEvent<string> evt)
    {
        var newValue = evt.newValue;
        var previousValue = evt.previousValue;
        Debug.Log($"Changed from {previousValue} to {newValue}");

        var screenResolution = newValue.Split("x");
        var screenWidth = int.Parse(screenResolution[0]);
        var screenHeight = int.Parse(screenResolution[0]);
        
        //var currentResolution = Screen.currentResolution;
        Screen.SetResolution(screenWidth, screenHeight, Screen.fullScreenMode);
    }
}
