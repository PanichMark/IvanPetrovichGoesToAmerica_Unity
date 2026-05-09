using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionObjectTVController : MonoBehaviour
{
	public delegate void ChannelChangedHandler(int channelIndex);

	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private List<VideoClip> videoClips = new List<VideoClip>();
	[SerializeField] private RawImage tvScreen;

	private int currentChannelIndex = 0;

	void Start()
	{
		PlayChannel(0);
	}

	public void SwitchChannel(bool isNext)
	{
		videoPlayer.Stop();

		if (isNext)
		{
			currentChannelIndex++;
			if (currentChannelIndex >= videoClips.Count)
				currentChannelIndex = 0;
		}
		else
		{
			currentChannelIndex--;
			if (currentChannelIndex < 0)
				currentChannelIndex = videoClips.Count - 1;
		}

		PlayChannel(currentChannelIndex);
	}

	private void PlayChannel(int index)
	{
		if (index < 0 || index >= videoClips.Count)
		{
			Debug.LogError("Invalid channel index: " + index);
			return;
		}

		videoPlayer.clip = videoClips[index];
		videoPlayer.Play();

		tvScreen.texture = videoPlayer.texture;
	}
}