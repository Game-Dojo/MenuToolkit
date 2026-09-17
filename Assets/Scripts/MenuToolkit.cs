using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private SettingsScriptable settings;
    
    private UIDocument _doc;
    private VisualElement _root;
    
    private VisualElement _mainPanel;
    private VisualElement _optionsPanel;
    private VisualElement _exitPrompt;
    private VisualElement _logo;
    
    private Button _playButton;
    private Button _optionButton;
    private Button _quitButton;
    
    private Button _backButton;
    private Button _applyButton;
    
    private Button _yesButton;
    private Button _noButton;

    private DropdownField _resolutionField;
    private DropdownField _qualityField;
    
    private Resolution _currentResolution;
    private Vector2Int _targetResolution = Vector2Int.zero;
    
    #region Setup
    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _mainPanel = _root.Q<VisualElement>("Main");
        _optionsPanel = _root.Q<VisualElement>("Options");
        _exitPrompt = _root.Q<VisualElement>("ExitPrompt");

        _logo = _mainPanel.Q<VisualElement>("GameTitle");
            
        _playButton = GetButton("PlayButton", _root);
        _optionButton = GetButton("OptionsButton", _root);
        _quitButton = GetButton("QuitButton", _root);
        
        _yesButton = GetButton("YesButton", _root);
        _noButton = GetButton("NoButton", _root);
        
        _backButton = GetButton("BackMainButton", _optionsPanel);
        _applyButton = GetButton("ApplySettingsButton", _optionsPanel);
        
        // Resolution
        _resolutionField = _root.Q<DropdownField>("ResolutionDrop");
        _resolutionField.RegisterValueChangedCallback(OnResolutionSelected);

        // Quality
        _qualityField = _root.Q<DropdownField>("QualityDropdown");
        _qualityField.choices = new List<string>(QualitySettings.names);
        _qualityField.index = settings.quality;
        
        _playButton.clicked += PlayPressed;
        _optionButton.clicked += OptionsPressed;
        _quitButton.clicked += QuitPressed;
        
        _backButton.clicked += BackPressed;
        _applyButton.clicked += ApplyPressed;
        
        _yesButton.clicked += YesPressed;
        _noButton.clicked += NoPressed;
    }

    private void Start()
    {
        _currentResolution = Screen.currentResolution;
        SwitchPanel(Panels.Main);

        Invoke(nameof(AnimateIntro), 0.1f);
    }

    private void AnimateIntro()
    {
        _logo?.RemoveFromClassList("game-title-entrance");
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
        
        if (_targetResolution != Vector2Int.zero)
            Screen.SetResolution(_targetResolution.x, _targetResolution.y, Screen.fullScreenMode);
    }
    
    private void YesPressed()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
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
    
    private Button GetButton(string buttonName, VisualElement root)
    {
        var rootPath = root ?? _root;
        var container = rootPath.Q<TemplateContainer>(buttonName); 
        return container.Q<Button>("GameButton");
    }
    #endregion
    
    private void OnResolutionSelected(ChangeEvent<string> evt)
    {
        var newValue = evt.newValue;
        var previousValue = evt.previousValue;

        if (newValue == previousValue)
        {
            _targetResolution = Vector2Int.zero;
            return;
        }
        
        Debug.Log($"Changed from {previousValue} to {newValue}");

        var screenResolution = newValue.Split("x");
        var screenWidth = int.Parse(screenResolution[0]);
        var screenHeight = int.Parse(screenResolution[1]);

        _targetResolution.x = screenWidth;
        _targetResolution.y = screenHeight;
    }
}
