// 1. НОВАЯ СТРУКТУРА ДЛЯ ХРАНЕНИЯ ДАННЫХ ОБ NPC
using UnityEngine;

/// <summary>
/// Структура для хранения ссылки на NPC и состояния, в которое его нужно перевести.
/// </summary>
[System.Serializable] // Этот атрибут позволяет структуре отображаться в Инспекторе
public struct CutsceneDataNPC
{
	public GameObject npcObject; // Ссылка на объект NPC
	public NPCStateTypes stateToSet; // Состояние, которое нужно установить
}