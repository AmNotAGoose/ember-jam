using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource music;

    public AudioSource bombHiss;
    public AudioSource bombExplode;
    public AudioSource bombPickUp;

    public AudioSource boxPush;

    public AudioSource goalSatisfy;
    public AudioSource layerFall;

    public AudioSource winLevel;
    public AudioSource footstep1;
    public AudioSource footstep2;

    bool curFootstep = true;

    private void Start()
    {
        Level level = FindFirstObjectByType<Level>();
        music = Instantiate(level.resources.music).GetComponent<AudioSource>();

        music.Play();
    }

    public void PlayFootstep()
    {
        if (curFootstep)
        {
            footstep1.Play();
        } else
        {
            footstep2.Play();
        }
        curFootstep = !curFootstep;
    }
}
