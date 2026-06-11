using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections.Generic;

public class InteractionObjectTVController : MonoBehaviour, IElectroShockable
{
	public delegate void TVStateChangedHandler(bool isOn);
	public event TVStateChangedHandler OnTVStateChanged;

	public bool IsTVturnedOn;
	private VideoPlayer _videoPlayer;
	[SerializeField] private List<VideoClip> _videoClips = new List<VideoClip>();
	private RawImage _tvScreen;

	private int _currentChannelIndex = 0;

	void Start()
	{
		_videoPlayer = GetComponent<VideoPlayer>();
		_tvScreen = transform.Find("TVcanvas").Find("TVscreen").GetComponent<RawImage>();
		_videoPlayer.targetTexture = _tvScreen.texture as RenderTexture;

		// Изначально телевизор выключен
		TurnOff();
	}

	// Этот метод вызывается кнопкой питания
	public void TogglePower()
	{
		if (IsTVturnedOn)
		{
			TurnOff();
		}
		else
		{
			TurnOn();
		}
	}

	private void TurnOn()
	{
		IsTVturnedOn = true;
		_tvScreen.gameObject.SetActive(true);
		PlayChannel(_currentChannelIndex);

		// Оповещаем подписчиков, что ТВ включен
		OnTVStateChanged?.Invoke(IsTVturnedOn);
	}

	private void TurnOff()
	{
		IsTVturnedOn = false;
		_videoPlayer.Stop();
		_tvScreen.gameObject.SetActive(false);

		// Оповещаем подписчиков, что ТВ выключен
		OnTVStateChanged?.Invoke(IsTVturnedOn);
	}

	public void SwitchChannel(bool isNext)
	{
		if (!IsTVturnedOn) return; // Не переключаем, если ТВ выключен

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
	}

	public void Electrify()
	{
		if (!IsTVturnedOn)
		{
			TurnOn();
		}
		else
		{
			SwitchChannel(true);
		}
	}
}