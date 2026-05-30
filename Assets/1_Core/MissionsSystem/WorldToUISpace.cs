using UnityEngine;
using UnityEngine.UI; // Для RectTransformUtility

public class WorldToUISpace : MonoBehaviour
{
	private RectTransform _canvasRectTransform;
	private Camera _camera;

	[Range(0f, 5f)]
	public float edgeOffsetPercent = 2f;

	private RectTransform _rectTransform;
	private Vector2 _viewportCenter = new Vector2(0.5f, 0.5f);

	// Метод Awake убран по вашему запросу.
	// Получение компонента перенесено в Initialize.

	public void Initialize(RectTransform canvasRectTransform, Camera camera)
	{
		_canvasRectTransform = canvasRectTransform;
		_camera = camera;

		// Кэшируем RectTransform этого объекта (Image) здесь,
		// чтобы не вызывать GetComponent при каждом обновлении позиции.
		_rectTransform = GetComponent<RectTransform>();
	}

	public void UpdatePosition(Vector3 targetWorldPosition)
	{
		if (_canvasRectTransform == null || _camera == null || _rectTransform == null) return;

		Vector3 screenPoint = _camera.WorldToScreenPoint(targetWorldPosition);

		if (screenPoint.z < 0)
		{
			HandleOffscreenTarget(targetWorldPosition);
			return;
		}

		Vector2 normalizedPoint = new Vector2(screenPoint.x / Screen.width, screenPoint.y / Screen.height);
		bool isOnScreen = IsPointOnScreen(normalizedPoint);

		if (isOnScreen)
		{
			SetAnchoredPosition(screenPoint);
		}
		else
		{
			HandleOffscreenTarget(targetWorldPosition);
		}
	}

	private bool IsPointOnScreen(Vector2 normalizedPoint)
	{
		float offsetX = edgeOffsetPercent / 100f;
		float offsetY = edgeOffsetPercent / 100f;

		return normalizedPoint.x >= offsetX && normalizedPoint.x <= 1 - offsetX &&
			   normalizedPoint.y >= offsetY && normalizedPoint.y <= 1 - offsetY;
	}

	private void HandleOffscreenTarget(Vector3 worldPosition)
	{
		Vector3 directionFromCenter = (worldPosition - _camera.transform.position).normalized;
		Ray ray = new Ray(_camera.transform.position, directionFromCenter);
		Plane plane = new Plane(Vector3.forward, Vector3.zero);

		float distance;
		if (plane.Raycast(ray, out distance))
		{
			Vector3 intersectionPoint = ray.GetPoint(distance);
			Vector2 viewportPoint = _camera.WorldToViewportPoint(intersectionPoint);

			viewportPoint.x = Mathf.Clamp01(viewportPoint.x);
			viewportPoint.y = Mathf.Clamp01(viewportPoint.y);

			Vector2 screenPoint = new Vector2(
				viewportPoint.x * Screen.width,
				viewportPoint.y * Screen.height
			);

			SetAnchoredPosition(screenPoint);
		}
	}

	private void SetAnchoredPosition(Vector2 screenPoint)
	{
		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPoint, _camera, out localPoint);
		_rectTransform.anchoredPosition = localPoint;
	}
}