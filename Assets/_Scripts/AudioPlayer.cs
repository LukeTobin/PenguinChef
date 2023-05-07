using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    public static AudioPlayer Instance;

    [SerializeField] AudioClip gameplayTrack;
    [SerializeField] AudioClip menuTrack;
    
    AudioSource source;

    void Awake(){
        Instance = this;
        source = GetComponent<AudioSource>();    
    }

    public void GameplayTrack(){
        source.Stop();
        source.clip = gameplayTrack;
        source.Play();
    }

    public void MenuTrack(){
        source.Stop();
        source.clip = menuTrack;
        source.Play();
    }

}
