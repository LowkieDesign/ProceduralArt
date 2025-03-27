using UnityEngine;
using UnityEngine.UI;

public class RandomButton : MonoBehaviour
{
    public SlimeSimulationV1 slimeSimulationV1; // Sleep hier je object in de Inspector
    public Button randomizeButton; // Sleep hier je UI-knop in de Inspector

    void Start()
    {
        if (randomizeButton != null)
        {
            randomizeButton.onClick.AddListener(RandomizeSettings);
        }
    }

    void RandomizeSettings()
    {
        if (slimeSimulationV1 != null)
        {
            slimeSimulationV1.RandomizeAgentSettings();
        }
    }
}
