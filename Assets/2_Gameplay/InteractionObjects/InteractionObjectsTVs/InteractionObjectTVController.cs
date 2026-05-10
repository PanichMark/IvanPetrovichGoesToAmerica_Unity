using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionObjectTVController : MonoBehaviour
{
	public delegate void ChannelChangedHandler(int channelIndex);

	private VideoPlayer _videoPlayer;
	[SerializeField] private List<VideoClip> _videoClips = new List<VideoClip>();
	private RawImage _tvScreen;

	private int _currentChannelIndex = 0;

	void Start()
	{
		_videoPlayer = GetComponent<VideoPlayer>();
		_tvScreen = transform.parent.Find("CanvasTV").Find("ImageTV").GetComponent<RawImage>();
		PlayChannel(0);
	}

	public void SwitchChannel(bool isNext)
	{
		_videoPlayer.Stop();

		if (isNext)
		{
			_currentChannelIndex++;
			if (_currentChannelIndex >= _videoClips.Count)
				_currentChannelIndex = 0;
		}
		else
		{
			_currentChannelIndex--;
			if (_currentChannelIndex < 0)
				_currentChannelIndex = _videoClips.Count - 1;
		}

		PlayChannel(_currentChannelIndex);
	}

	private void PlayChannel(int index)
	{
		if (index < 0 || index >= _videoClips.Count)
		{
			Debug.LogError("Invalid channel index: " + index);
			return;
		}

		_videoPlayer.clip = _videoClips[index];
		_videoPlayer.Play();

		_tvScreen.texture = _videoPlayer.texture;
	}
}