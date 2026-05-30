using UnityEngine;

public class WorldToUISpace : MonoBehaviour
{
	private RectTransform _canvasRectTransform;
	private Camera _camera;
	public float edgeOffsetPercent = 2f;

	public void Initialize(RectTransform canvasRectTransform, Camera camera)
	{
		_canvasRectTransform = canvasRectTransform;
		_camera = camera;
	}

	public void UpdatePosition(Vector3 worldPos)
	{
		if (_canvasRectTransform == null || _camera == null) return;

		Vector3 screenPoint = _camera.WorldToScreenPoint(worldPos);

		// Если объект за камерой, считаем позицию на границе экрана
		if (screenPoint.z < 0)
		{
			HandleOffscreenTarget(worldPos);
			return;
		}

		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screenPoint, _camera, out localPoint);
		GetComponent<RectTransform>().anchoredPosition = localPoint;
	}

	private void HandleOffscreenTarget(Vector3 worldPos)
	{
		var dir = (worldPos - _camera.transform.position).normalized;
		Ray ray = new Ray(_camera.transform.position, dir);
		Plane plane = new Plane(Vector3.forward, Vector3.zero);

		if (plane.Raycast(ray, out float dist))
		{
			Vector3 point = ray.GetPoint(dist);
			Vector2 viewport = _camera.WorldToViewportPoint(point);
			viewport.x = Mathf.Clamp01(viewport.x);
			viewport.y = Mathf.Clamp01(viewport.y);

			Vector2 screen = new Vector2(viewport.x * Screen.width, viewport.y * Screen.height);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRectTransform, screen, _camera, out Vector2 local);
			GetComponent<RectTransform>().anchoredPosition = local;
		}
	}
}