using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------- Audio Source -------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource sfxSource;

    [Header("------- Music Clips -------")]
    public AudioClip Corvo_Musica_Tema_Final;
    public AudioClip Creditos;
    public AudioClip Leao_Musica_Tema_Final;
    public AudioClip Mago_Musica_Tema_Final;
    public AudioClip Principal_Musica_Final;

    private void Start()
    {
        musicSource.clip = Principal_Musica_Final;
        musicSource.Play();
    }
}
