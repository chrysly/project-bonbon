using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager {

	/// <summary>
	/// From Nava Audio Manager Port;
	/// </summary>

	[Header("Audio Manager")]

	/// Declaring Audio Sources for music and sfx;
	[SerializeField] private AudioSource mainMusicSource;
	[SerializeField] private AudioSource sfxSource;
	/// Declaring arrays for audio files;
	/// The files must be assigned in the inspector;
	[SerializeField] private Sound[] musicSounds, sfxSounds;

	[System.Serializable]
	public class Sound {
		public string name;
		public AudioClip clip;
	}

	private float musicVolume = 1.0f;
	private float sfxVolume = 1.0f;

	private const string NULL_CLIP_TEXT = "Invalid clip string passed";
	private readonly float volumeChangeRate = 0.01f;

    public void CheckMusic() {
		int buildIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
		StopAllCoroutines();
		/*
		switch (buildIndex) {
			case 0:
				PlayMusic("Main");
				break;
			case 1:
				StartCoroutine(LoadMusic());
				break;
        }*/
	}

	private AudioClip FetchClip(string name, Sound[] sounds) {
		Sound sn = System.Array.Find(sounds, item => item.name == name);
		if (sn == null) throw new System.Exception(NULL_CLIP_TEXT);
		return sn.clip;
	}

	/// <summary>
	/// Method to play music tracks;
	/// <br></br> The music source supports a single music track at a time;
	/// </summary>
	/// <param name="name"> Name of the clip to play; </param>
	/// <param name="shouldLoop"> Whether the music should loop; </param>
	/// <returns> Returns the request track if found track requested if found; </returns>
	public AudioSource PlayMusic(string name, bool shouldLoop = true) {
		mainMusicSource.loop = shouldLoop;
		mainMusicSource.volume = musicVolume;
		AudioClip clip = FetchClip(name, musicSounds);
		mainMusicSource.clip = clip;
		mainMusicSource.Play();
		return mainMusicSource;
	}

	/// <summary>
	/// Method to play global sound effects;
	/// <br></br> The sounds are played on the manager's audio source;
	/// </summary>
	/// <param name="name"> Name of the clip to play; </param>
	/// <param name="pitchRChange"> Pitch variance of the clip (additive); </param>
	/// <param name="volumeMultiplier"> Volume of the clip (between 0 and 1); </param>
	/// <returns> Returns the length of the clip played if one is found; </returns>
	public void PlaySFX(string name, float pitchRChange = 0, float volumeMultiplier = 1) {

		AudioClip clip = FetchClip(name, sfxSounds);
		sfxSource.pitch = 1f + Random.Range(-pitchRChange, pitchRChange);
		sfxSource.volume = sfxVolume * volumeMultiplier;
		sfxSource.PlayOneShot(clip);
	}

	/// <summary>
	/// Method to play global sound effects;
	/// <br></br> The sounds are played on the manager's audio source;
	/// </summary>
	/// <param name="name"> Name of the clip to play; </param>
	/// <param name="optionalLengthReturn"> Optional clip length output; </param>
	/// <param name="pitchRChange"> Pitch variance of the clip (additive); </param>
	/// <param name="volumeMultiplier"> Volume of the clip (between 0 and 1); </param>
	public void PlaySFX(string name, out float optionalLengthReturn,
							float pitchRChange = 0, float volumeMultiplier = 1) {

		AudioClip clip = FetchClip(name, sfxSounds);
		sfxSource.pitch = 1f + UnityEngine.Random.Range(-pitchRChange, pitchRChange);
		sfxSource.volume = sfxVolume * volumeMultiplier;
		sfxSource.PlayOneShot(clip);
		optionalLengthReturn = clip.length;
	}

	/// <summary>
	/// Method to fade away the music.
	/// </summary>
	/// <param name="stopsMusic"> Whether the music should be stopped (true), or paused (false); </param>
	/// <param name="stopsAbruptly"> Whether the fadeout happens immediately (true), or with linear interpolation (false); </param>
	public void FadeMusic(bool stopsMusic, bool stopsAbruptly = false) {
		StopAllCoroutines();
		if (stopsAbruptly) mainMusicSource.Stop();
		else StartCoroutine(_FadeMusic(stopsMusic));
	}

	public void ResumeMusic() => StartCoroutine(_ResumeMusic());

	/// <summary>
	/// Coroutine to fade away the music. Hopefully more efficient than running a bool in Update;
	/// </summary>
	IEnumerator _FadeMusic(bool stopsMusic) {
		while (mainMusicSource.volume > 0) {
			mainMusicSource.volume = Mathf.MoveTowards(mainMusicSource.volume, 0, volumeChangeRate);
			yield return null;
		}
		if (stopsMusic) {
			mainMusicSource.Stop();
			mainMusicSource.volume = musicVolume;
		}
	}

	IEnumerator _ResumeMusic() {
		if (mainMusicSource) mainMusicSource.UnPause();
		while (mainMusicSource.volume < musicVolume) {
			mainMusicSource.volume = Mathf.MoveTowards(mainMusicSource.volume, musicVolume, volumeChangeRate);
			yield return null;
		}
	}

	/*
	IEnumerator LoadMusic() {
		var musicSource = PlayMusic("Exploration Opening", false);
		while (musicSource.isPlaying) {
			yield return null;
		} PlayMusic("Exploration");
	}*/

	/// <summary>
	/// Update the music volume utilized by the manager;
	/// </summary>
	/// <param name="value"> New music volume (between 0 and 1); </param>
	public void SetMusicVolume(float value) {
		musicVolume = value;
		mainMusicSource.volume = value;
	}

	public float GetMusicVolume() {
		return musicVolume;
	}

	/// <summary>
	/// Update the sfx volume utilized by the manager;
	/// </summary>
	/// <param name="value"> New music volume (between 0 and 1); </param>
	public void SetSFXVolume(float value) {
		sfxVolume = value;
		sfxSource.volume = value;
	}

	public float GetSFXVolume() {
		return sfxVolume;
	}
}