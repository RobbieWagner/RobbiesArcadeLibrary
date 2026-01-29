using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using RobbieWagnerGames.Managers;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using RobbieWagnerGames.Audio;
using AYellowpaper.SerializedCollections;

namespace RobbieWagnerGames.UI
{
    public class SettingsMenu : Menu
    {
        [Header("General")]
        public Button backButton;
        
        [Header("Audio Settings")]
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider uiVolumeSlider;
        [SerializeField] private Slider gameplayVolumeSlider;
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private AudioSource uiNavSource;

        [SerializedDictionary("MixerGroupName","MixerGroup")][SerializeField] private SerializedDictionary<AudioMixerGroupName, AudioMixerGroup> mixerGroups;

        protected override void Awake()
        {
            if (uiNavSource == null)
                uiNavSource = BasicAudioManager.Instance.audioSources[AudioSourceName.UINav];

            base.Awake();

            SetupAudioSliders();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void ForceControllerMode()
        {
            if (!EventSystemManager.Instance.HasSelection())
                RestoreOrSetDefaultSelection();
        }

        protected override void CheckForLostSelection()
        {
            if (!IsOpen || !maintainSelectionAlways || !isUsingController) return;
            
            if (!EventSystemManager.Instance.HasSelection())
                RestoreOrSetDefaultSelection();
        }

        protected override void HandleControllerNavigation()
        {
            if (!isUsingController || !canNavigateWithController) return;
            
            InputAction navigateAction = InputManager.Instance.GetAction(ActionMapName.UI, "Navigate");
            if (navigateAction != null)
            {
                Vector2 input = navigateAction.ReadValue<Vector2>();
                
                if (input != Vector2.zero && (lastControllerInput == Vector2.zero || Time.time - lastControllerNavigationTime > controllerRepeatRate))
                {
                    BasicAudioManager.Instance.Play(AudioSourceName.UINav, false);
                    HandleNavigationInput(input);
                    lastControllerNavigationTime = Time.time;
                }
                else if (input == Vector2.zero && lastControllerInput != Vector2.zero)
                    canNavigateWithController = true;
                
                lastControllerInput = input;
            }
        }

        protected override void HandleNavigationInput(Vector2 input)
        {
            bool inputHandled = false;
            
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                if (input.y > 0.5f)
                    inputHandled = true;
                else if (input.y < -0.5f) 
                    inputHandled = true;
            }
            else
            {
                if (input.x < -0.5f)
                {
                    HandleHorizontalNavigation(-1);
                    inputHandled = true;
                }
                else if (input.x > 0.5f)
                {
                    HandleHorizontalNavigation(1);
                    inputHandled = true;
                }
            }
            
            if (inputHandled)
            {
                canNavigateWithController = false;
                Invoke(nameof(ResetControllerNavigation), controllerNavigationDelay);
            }
        }

        private void HandleHorizontalNavigation(int direction)
        {
            GameObject selected = EventSystemManager.Instance.CurrentSelected;
            if (selected == null) return;
            
            Selectable currentElement = selected.GetComponent<Selectable>();
            if (currentElement == null) return;
            
            if (currentElement is Slider slider)
            {
                float step = (slider.maxValue - slider.minValue) / 20f;
                slider.value += direction * step;
                slider.onValueChanged?.Invoke(slider.value);
            }
            else if (currentElement is Toggle toggle)
            {
                toggle.isOn = !toggle.isOn;
                toggle.onValueChanged?.Invoke(toggle.isOn);
            }
        }

        private void SetupAudioSliders()
        {
            float masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
            float gameplayVolume = PlayerPrefs.GetFloat("GameplayVolume", 1f);
            
            musicVolumeSlider.value = musicVolume;
            uiVolumeSlider.value = uiVolume;
            gameplayVolumeSlider.value = gameplayVolume;
            
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            uiVolumeSlider.onValueChanged.AddListener(OnUIVolumeChanged);
            gameplayVolumeSlider.onValueChanged.AddListener(OnGameplayVolumeChanged);
            
            UpdateAudioMixer();
        }
        
        public override void Close()
        {
            base.Close();
            SaveSettings();
            uiNavSource.outputAudioMixerGroup = mixerGroups[AudioMixerGroupName.UI];
        }

        private void UpdateAudioMixer()
        {
            if (audioMixer == null) return;
            
            Dictionary<string, float> audioMixerVolumes = new Dictionary<string, float>
            {
                { "music", musicVolumeSlider.value },
                { "ui", uiVolumeSlider.value },
                { "gameplay", gameplayVolumeSlider.value }
            };

            AudioMixerController.Instance.UpdateAudioMixer(audioMixerVolumes);
        }

        private void SaveSettings()
        {
            PlayerPrefs.Save();
        }
        
        private void OnMusicVolumeChanged(float value)
        {            
            uiNavSource.outputAudioMixerGroup = mixerGroups[AudioMixerGroupName.MUSIC];
            PlayerPrefs.SetFloat("MusicVolume", value);
            UpdateAudioMixer();
        }
        
        private void OnUIVolumeChanged(float value)
        {
            uiNavSource.outputAudioMixerGroup = mixerGroups[AudioMixerGroupName.UI];
            PlayerPrefs.SetFloat("UIVolume", value);
            UpdateAudioMixer();
        }
        
        private void OnGameplayVolumeChanged(float value)
        {
            uiNavSource.outputAudioMixerGroup = mixerGroups[AudioMixerGroupName.GAMEPLAY];
            PlayerPrefs.SetFloat("GameplayVolume", value);
            UpdateAudioMixer();
        }

        protected override void OnElementSelected(Selectable element)
        {
            base.OnElementSelected(element);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (musicVolumeSlider != null)
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
            if (uiVolumeSlider != null)
                uiVolumeSlider.onValueChanged.RemoveAllListeners();
            if (gameplayVolumeSlider != null)
                gameplayVolumeSlider.onValueChanged.RemoveAllListeners();
        }
    }
}