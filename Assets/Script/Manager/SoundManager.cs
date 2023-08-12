using System;
using System.Collections.Generic;
using Script.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Script.Manager
{
	public enum SoundType
	{
		BGM,
		Effect,
	
		MaxValue
	}

	public class SoundManager : UIUtil
	{
		public GameObject Sound_UI;
		AudioSource[] audioSources = new AudioSource[(int)SoundType.MaxValue];
		Dictionary<string, AudioClip> clipDictionary = new Dictionary<string, AudioClip>();

		string defualt_Path = "Sound/";

		public SoundManager()
		{
			Managers.Instance.StartCall += Init;
			Managers.Instance.UpdateCall += Update;

			UIInit();
		}

		public void Init()
		{
			GameObject root = GameObject.Find("Sound");
			if(root == null)
			{
				root = new GameObject { name = "Sound" };
				root.transform.parent = Managers.Instance.transform;
			
				for(int i = 0; i < (int)SoundType.MaxValue; i++)
				{
					GameObject obj = new GameObject { name = ((SoundType)i).ToString() };
					audioSources[i] = obj.AddComponent<AudioSource>();
					obj.transform.parent = root.transform;
				}
			}
		}
		void UIInit()
		{
			//Sound_UI = GameObject.FindGameObjectWithTag("Sound");
			Sound_UI = Util.FindChild(Util.FindChild(Managers.Instance.gameObject, "Setting Canvas"), "Sound UI");

			foreach (var soundTypeName in Enum.GetNames(typeof(SoundType)))
				Bind<Slider>(Sound_UI, new string[] { $"{soundTypeName} Slider" });
		}

		void Update()
		{
			for (int i = 0; i < (int)SoundType.MaxValue; i++)
				audioSources[i].volume = Get<Slider>(i).value;
		}

		public void Play(string path, SoundType type = SoundType.Effect, float pitch = 1.0f)
		{
			AudioClip clip = GetORAddAudioClip(path);
			if (clip == null)
				return;
			Play(clip, type, pitch);
		}

		public void Play(AudioClip clip, SoundType type = SoundType.Effect, float pitch = 1.0f)
		{
			if(clip == null)
			{
				Debug.LogWarning("사운드 Clip이 존재하지 않습니다.");
				return;
			}

			AudioSource source;
			if (type == SoundType.BGM)
			{
				source = audioSources[(int)SoundType.BGM];
				if (source.isPlaying)
					source.Stop();

				source.clip = clip;
				source.pitch = pitch;
				source.loop = true;

				source.Play();
			}
			else
			{
				source = audioSources[(int)SoundType.Effect];
				source.pitch = pitch;
				source.PlayOneShot(clip);
			}
		}

		public void Clear()
		{
			foreach(AudioSource source in audioSources)
			{
				if (source == null) continue;
				source.clip = null;
				source.Stop();
			}
			clipDictionary.Clear();
		}

		public AudioClip GetORAddAudioClip(string path)
		{
			if(!path.Contains("/"))
				path = defualt_Path + path;
			else if (!path.Contains("Sound"))
				path = defualt_Path + path;
			
			AudioClip clip = null;
			if(clipDictionary.TryGetValue(path, out clip) == false)
			{
				clip = Resources.Load<AudioClip>(path);
				if (clip == null)
				{
					Debug.LogWarning($"AudioClip Missing : {path}");
					return null;
				}
				
				clipDictionary.Add(path, clip);
			}
			return clip;
		}

		public AudioSource GetAudioSource(SoundType type) => audioSources[(int)type];
	}
}