using UnityEngine;

public class GameplayCanvases
{
	public GameObject CanvasLockMechanical { get; private set; }
	public GameObject CanvasLockElectronic { get; private set; }
	public GameObject CanvasNote { get; private set; }
	public GameObject CanvasDialogue { get; private set; }

	public GameObject CanvasMainMenuChooseMission { get; private set; }
	public GameObject CanvasMainMenuReadNews { get; private set; }
	public GameObject CanvasCutscene { get; private set; } // Новое поле

	public GameplayCanvases(GameObject canvasLockMechanical,
							GameObject canvasLockElectronic,
							GameObject canvasNote,
							GameObject canvasDialogue,
							GameObject canvasMainMenuChooseMission,
							GameObject canvasMainMenuReadNews,
							GameObject canvasCutscene) // Новый аргумент
	{
		CanvasLockMechanical = canvasLockMechanical;
		CanvasLockElectronic = canvasLockElectronic;
		CanvasNote = canvasNote;
		CanvasDialogue = canvasDialogue;

		CanvasMainMenuChooseMission = canvasMainMenuChooseMission;
		CanvasMainMenuReadNews = canvasMainMenuReadNews;
		CanvasCutscene = canvasCutscene;
	}
}