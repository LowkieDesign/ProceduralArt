using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AudioReader : MonoBehaviour
{
    public ComputeShader slimeMoldShader; // Shader die we aansturen
    public Slider frequencySlider; // UI Slider om frequentie te kiezen
    private AudioSource audioSource;
    public float minSpeed = -64.0f;  // Minimale groeisnelheid
    public float maxSpeed = 64.0f;  // Maximale groeisnelheid
    private float frequency = 440f; // Standaardfrequentie (Hz)
    private int sampleRate = 44100;
    private float time;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.clip = AudioClip.Create("SineWave", sampleRate, 1, sampleRate, true, OnAudioRead);
        audioSource.Play();
    }

    void Update()
    {
        if (slimeMoldShader == null || frequencySlider == null)
            return;

        // Haal de frequentie op uit de slider (20 Hz - 2000 Hz)
        float newFrequency = Mathf.Lerp(20, 2000, frequencySlider.value);

        // Controleer of de frequentie daadwerkelijk veranderd is (ter voorkoming van onnodige updates)
        if (Mathf.Abs(newFrequency - frequency) > 0.1f)
        {
            frequency = newFrequency;

            // Zet de frequentie om naar een groeisnelheid
            float speed = Mathf.Lerp(minSpeed, maxSpeed, frequencySlider.value);

            // Zet de waarden om voor de Compute Shader
            slimeMoldShader.SetFloat("speed", speed);
            slimeMoldShader.SetFloat("frequencyFactor", frequency / 2000f);  // Verhouding van de frequentie

            // Gebruik de frequentie om de helderheid te berekenen (meer helderheid bij hogere frequenties)
            float brightness = Mathf.Lerp(1.5f, 2.0f, frequency / 2000f);
            slimeMoldShader.SetFloat("brightness", brightness);

            // **BELANGRIJK**: Dispatch de Compute Shader
            int threadGroupsX = Mathf.CeilToInt(Screen.width / 8.0f);
            int threadGroupsY = Mathf.CeilToInt(Screen.height / 8.0f);

            slimeMoldShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);
        }
    }

    void OnAudioRead(float[] data)
    {
        // Genereer een sinusgolf gebaseerd op de frequentie
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = Mathf.Sin(2 * Mathf.PI * frequency * time);
            time += 1.0f / sampleRate;
            if (time > 1.0f) time -= 1.0f; // Reset de tijd om overflow te voorkomen
        }
    }
}
