using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Array of AudioSource components that hold different destruction sound clips
    public AudioSource[] destroyNoise;
    public AudioSource backgroundMusic;

    void Start()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.Play();
                backgroundMusic.volume = 1;
            }
        }
        else
        {
            backgroundMusic.Play();
            backgroundMusic.volume = 1;
        }
    }

    public void adjustVolume()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 0)
            {
                backgroundMusic.volume = 0;
            }
            else
            {
                backgroundMusic.volume = 1;
            }
        }
    }

    // Method to play a random destruction sound effect from the destroyNoise array
    public void PlayRandomDestroyNoise()
    {
        if (PlayerPrefs.HasKey("Sound"))
        {
            if (PlayerPrefs.GetInt("Sound") == 1)
            {
                // Randomly select an index within the range of available sound clips
                int clipToPlay = Random.Range(0, destroyNoise.Length);

                // Play the selected audio clip from the destroyNoise array
                destroyNoise[clipToPlay].Play();
            }
        }
        else
        {
            // Randomly select an index within the range of available sound clips
            int clipToPlay = Random.Range(0, destroyNoise.Length);

            // Play the selected audio clip from the destroyNoise array
            destroyNoise[clipToPlay].Play();
        }
    }
}