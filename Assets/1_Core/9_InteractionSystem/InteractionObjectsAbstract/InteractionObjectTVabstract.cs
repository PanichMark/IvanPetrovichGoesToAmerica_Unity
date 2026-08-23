using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections;

public abstract class InteractionObjectTVabstract : MonoBehaviour, ISaveLoad
{
	public bool IsTVturnedOn { get; protected set; }
	public int TVindex { get; protected set; }
	[SerializeField] protected List<VideoClip> _videoClips = new List<VideoClip>();
	[SerializeField] protected RawImage _tvScreen;

	protected VideoPlayer _videoPlayer;
	protected int _currentChannelIndex = 0;

	protected virtual void Start()
	{
		InitializeComponents();
	}

	private void InitializeComponents()
	{
		_videoPlayer = GetComponent<VideoPlayer>();
		if (_videoPlayer != null && _tvScreen != null)
		{
			_videoPlayer.targetTexture = _tvScreen.texture as RenderTexture;
		}
	}

	protected virtual void TurnOn()
	{
	}

	protected virtual void TurnOff()
	{
	}

	protected void PlayChannel(int index)
	{
		if (index < 0 || index >= _videoClips.Count)
		{
			Debug.LogError("Invalid channel index: " + index);
			return;
		}

		_videoPlayer.clip = _videoClips[index];
		_videoPlayer.Play();
	}
	public void AssignTVsIndexes(int index)
	{
		TVindex = index;
	}
	protected void SwitchChannelInternal(bool isNext)
	{
		_videoPlayer.Stop();

		if (isNext)
		{
			_currentChannelIndex++;
			if (_currentChannelIndex >= _videoClips.Count) _currentChannelIndex = 0;
		}
		else
		{
			_currentChannelIndex--;
			if (_currentChannelIndex < 0) _currentChannelIndex = _videoClips.Count - 1;
		}

		PlayChannel(_currentChannelIndex);
	}

	protected void SetScreenActive(bool state)
	{
		if (_tvScreen != null)
		{
			_tvScreen.gameObject.SetActive(state);
		}
	}

	public IEnumerator SaveData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.TVsData == null || !data.TVsData.ContainsKey(currentScene))
			yield break;

		var targetList = data.TVsData[currentScene];

		int indexInList = targetList.FindIndex(item => item.TVindex == TVindex);

		var updatedItem = new TVdata
		{
			TVindex = TVindex,
			TVnameSystem = name,
			IsTVturnedOn = IsTVturnedOn,
			TVchannel = _currentChannelIndex
		};

		if (indexInList != -1)
		{
			targetList[indexInList] = updatedItem;
		}
		else
		{
			targetList.Add(updatedItem);
		}

		yield return null;
	}

	public IEnumerator LoadData(GameData data)
	{
		if (!System.Enum.TryParse(SceneManager.GetSceneAt(1).name, out GameScenesGameplayDataEnum currentScene)) yield break;

		if (data.TVsData == null || !data.TVsData.TryGetValue(currentScene, out var sourceList)) yield break;

		var savedState = sourceList.Find(item => item.TVindex == TVindex);

		if (savedState.Equals(default(TVdata))) yield break;

		IsTVturnedOn = savedState.IsTVturnedOn;

		if (IsTVturnedOn)
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}

		if (IsTVturnedOn && _videoClips.Count > 0)
		{
			_currentChannelIndex = Mathf.Clamp(savedState.TVchannel, 0, _videoClips.Count - 1);
			PlayChannel(_currentChannelIndex);
		}

		yield return null;
	}
}