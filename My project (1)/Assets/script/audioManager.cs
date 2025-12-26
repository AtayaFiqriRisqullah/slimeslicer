using UnityEngine;

public class audioManager : MonoBehaviour
{
    [SerializeField] AudioSource backgroundSource;
    [SerializeField] AudioSource SFXSource;

    [Header("sound")]
    public AudioClip background;
    public AudioClip hit;
    public AudioClip damaged;
    public AudioClip gameOver;

    private void Start()
    {
        backgroundSource.clip = background;
        backgroundSource.loop = true;
        backgroundSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
