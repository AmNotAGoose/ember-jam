using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;

    public AudioSource bombHiss;
    public AudioSource bombExplode;
    public AudioSource bombPickUp;

    public AudioSource winLevel;
    public AudioSource footsteps;

    private void Start()
    {
        Level level = FindFirstObjectByType<Level>();
        music = Instantiate(level.resources.music).GetComponent<AudioSource>();

        music.Play();
    }
}
