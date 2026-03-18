using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI; // <-- Обязательно добавьте эту строку для доступа к UI
using System.Collections.Generic;

public class TVController : MonoBehaviour
{
	[SerializeField] private VideoPlayer videoPlayer;
	[SerializeField] private List<VideoClip> videoClips = new List<VideoClip>();

	// <-- НОВАЯ СТРОКА -->
	[SerializeField] private RawImage tvScreen; // Ссылка на ваш RawImage на экране ТВ

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
			if (currentChannelIndex >= videoClips.Count) currentChannelIndex = 0;
		}
		else
		{
			currentChannelIndex--;
			if (currentChannelIndex < 0) currentChannelIndex = videoClips.Count - 1;
		}

		PlayChannel(currentChannelIndex);
	}

	private void PlayChannel(int index)
	{
		videoPlayer.clip = videoClips[index];
		videoPlayer.Play();

		// <-- НОВАЯ СТРОКА: Явно говорим RawImage, какую текстуру показывать -->
		//tvScreen.texture = videoPlayer.texture;
	}
}