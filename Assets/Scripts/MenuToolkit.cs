using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

    [SerializeField] private UniversalRenderPipelineAsset urpAsset;
    [SerializeField] private Volume volume;
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

    private Camera _mainCamera;
    private UniversalAdditionalCameraData _cameraData;
    
    #region Unity Methods

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        Setup();
        
        // Resolution
        _resolutionField = _root.Q<DropdownField>("ResolutionDrop");
        _resolutionField.RegisterValueChangedCallback(OnResolutionSelected);

        // Quality
        _qualityField = _root.Q<DropdownField>("QualityDropdown");
        _qualityField.choices = new List<string>(QualitySettings.names);
        _qualityField.index = settings.quality;
    }
    
    private IEnumerator Start()
    {
        _cameraData = _mainCamera.GetUniversalAdditionalCameraData();
        
        _currentResolution = Screen.currentResolution;
        SwitchPanel(Panels.Main);
        
        yield return null;
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
    
    #region  Button Events
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
        SwitchPanel(Panels.Main);
    }
    
    private void QuitPressed()
    {
        SwitchPanel(Panels.Exit, PanelType.Additive);
    }

    private void ApplyPressed()
    {
        print("Apply Pressed");
        ApplyAllChanges();
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
    
    private void Setup()
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

        _playButton.clicked += PlayPressed;
        _optionButton.clicked += OptionsPressed;
        _quitButton.clicked += QuitPressed;
        
        _backButton.clicked += BackPressed;
        _applyButton.clicked += ApplyPressed;
        
        _yesButton.clicked += YesPressed;
        _noButton.clicked += NoPressed;
    }
    
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

    private void ApplyAllChanges()
    {
        if (_targetResolution != Vector2Int.zero)
            Screen.SetResolution(_targetResolution.x, _targetResolution.y, Screen.fullScreenMode);
    }
    private void SetPostProEnabled(bool state)
    {
        _cameraData.renderPostProcessing = state;
    }

    private void SetAntialiasing(AntialiasingMode mode)
    {
        _cameraData.antialiasing = mode;
    }

    private void SetVsyncEnabled(bool state)
    {
        QualitySettings.vSyncCount = (state) ? 1 : 0;
    }

    private void SetRenderScale(int scale)
    {
        urpAsset.renderScale = scale;
    }

    private void SetShadowsDistance(float distance)
    {
        urpAsset.shadowDistance = distance;
    }

    private void SetBloomEnabled(bool state)
    {
        if (volume.profile.TryGet<Bloom>(out var bloom))
            bloom.active = state;
    }
    
}
