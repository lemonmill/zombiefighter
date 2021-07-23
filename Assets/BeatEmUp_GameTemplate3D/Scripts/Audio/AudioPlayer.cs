using UnityEngine;

namespace BeatEmUpTemplate {
	
	[RequireComponent(typeof(AudioSource))]
	public class AudioPlayer : MonoBehaviour {

		public AudioItem[] AudioList;
		private AudioSource source;
		private float sfxVolume = 1f;

		private void Awake(){
			GlobalAudioPlayer.audioPlayer = this;
			source = GetComponent<AudioSource>();

			//set settings
			GameSettings settings = Resources.Load("GameSettings", typeof(GameSettings)) as GameSettings;
			if(settings != null){
				sfxVolume = settings.SFXVolume;
			}
		}

		//play a sfx
		public void playSFX(string name){
			bool SFXFound = false;
			foreach(AudioItem audioItem in AudioList){
				if(audioItem.name == name){

					//pick a random number (not same twice)
					if (audioItem.clip.Length == 0 || (audioItem.clip.Length==1 && audioItem.clip[0] == null))
					{
						Debug.LogError("Null audioclip for sfx: " + name);
						return;
					}
					int rand = Random.Range (0, audioItem.clip.Length);
					source.PlayOneShot(audioItem.clip[rand]);
					source.volume = audioItem.volume * sfxVolume;
					source.loop = audioItem.loop;
					SFXFound = true;
					break;
				}
			}
			if (!SFXFound) Debug.Log ("no sfx found with name: " + name);
		}

		//plays a sfx at a certain world position
		public void playSFXAtPosition(string name, Vector3 worldPosition, Transform parent){
			bool SFXFound = false;
			foreach(AudioItem audioItem in AudioList){
				if(audioItem.name == name){

					//check the time threshold
					if (Time.time - audioItem.lastTimePlayed < audioItem.MinTimeBetweenCall) {
						return;
					} else {
						audioItem.lastTimePlayed = Time.time;
					}

					//pick a random number
					int rand = Random.Range (0, audioItem.clip.Length);

					//create gameobject for the audioSource
					GameObject audioObj = new GameObject ();
					audioObj.transform.parent = parent;
					audioObj.name = name;
					audioObj.transform.position = worldPosition;
					AudioSource audiosource = audioObj.AddComponent<AudioSource>();

					//audio source settings
					audiosource.clip = audioItem.clip[rand];
					audiosource.spatialBlend = 1.0f;
					audiosource.minDistance = 4f;
					audiosource.volume = audioItem.volume * sfxVolume;
					audiosource.outputAudioMixerGroup = source.outputAudioMixerGroup;
					audiosource.loop = audioItem.loop;
					audiosource.Play ();

					//Destroy on finish
					if (!audioItem.loop && audiosource.clip != null) { 
						TimeToLive TTL = audioObj.AddComponent<TimeToLive> ();
						TTL.LifeTime = audiosource.clip.length;
					}
					SFXFound = true;
				}
			}
			if (!SFXFound) Debug.Log ("no sfx found with name: " + name);
		}

		public void playSFXAtPosition(string name, Vector3 worldPosition){
			playSFXAtPosition (name, worldPosition, transform.root);
		}
	}
}
