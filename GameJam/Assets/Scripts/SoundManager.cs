using UnityEngine;

public class SoundManager : MonoBehaviour
{
public static SoundManager instance;
    [SerializeField] private AudioSource SFXObject;

    private void Awake()
    {
        if(instance == null)
            instance = this;
    }

    public void PlaySFXClip(AudioClip clip, Transform transform, float volume)
    {
        AudioSource audioSource = Instantiate(SFXObject, transform.position, Quaternion.identity);
        audioSource.clip = clip;
        audioSource.volume = volume;
        audioSource.Play();
        float ClipLength = audioSource.clip.length;
        Destroy(audioSource.gameObject, ClipLength);
    }
}
