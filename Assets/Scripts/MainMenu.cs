using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text highscoreText, gamesText, roundsText;
    private AudioManager audioManager;
    private GameController controller;
    private Slider musicSlider, sfxSlider;
    private Toggle vToggle;

    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        controller = FindObjectOfType<GameController>();
        audioManager.sounds[0].source.volume = controller.MusicVolume;
        audioManager.sounds[1].source.volume = controller.SfxVolume;
        audioManager.Play("song");
        Slider[] sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.gameObject.name == "MusicVolume")
                musicSlider = slider;
            else
                sfxSlider = slider;
        }
        vToggle = FindObjectOfType<Toggle>();
        SetSettings();
    }

    private void SetSettings()
    {
        musicSlider.value = controller.MusicVolume;
        sfxSlider.value = controller.SfxVolume;
        vToggle.isOn = controller.Vibrate;
        highscoreText.text = "$" + controller.Highscore.ToString();
        gamesText.text = controller.GamesPlayed.ToString();
        roundsText.text = controller.RoundsPlayed.ToString();
    }

    public void VolumeChange(Slider slider)
    {
        if (slider.gameObject.name == "MusicVolume")
        {
            controller.MusicVolume = slider.value;
            audioManager.sounds[0].source.volume = controller.MusicVolume;
        }
        else if (slider.gameObject.name == "SfxVolume")
        {
            controller.SfxVolume = slider.value;
            audioManager.sounds[1].source.volume = controller.SfxVolume;
        }
    }

    public void ToggleChange(Toggle toggle)
    {
        controller.Vibrate = toggle.isOn;
    }

    public void DeleteData()
    {
        controller.Highscore = 0;
        controller.GamesPlayed = 0;
        controller.RoundsPlayed = 0;
        controller.MusicVolume = 1f;
        controller.SfxVolume = 1f;
        controller.Vibrate = true;
        audioManager.sounds[0].source.volume = 1f;
        audioManager.sounds[1].source.volume = 1f;
        SetSettings();
    }

    public void PlayButtonAudio()
    {
        audioManager.Play("select");
        if (controller.Vibrate) Vibration.Vibrate(30);
    }

    public void LoadGame()
    {
        controller.LoadLevel(1);
    }

    public void QuitGame()
    {
        StartCoroutine(controller.Quit());
    }
}
