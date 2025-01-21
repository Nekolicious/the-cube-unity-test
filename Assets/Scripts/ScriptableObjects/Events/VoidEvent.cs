using UnityEngine;

[CreateAssetMenu(menuName = "Events/Void Event")]
public class VoidEvent : GameEventSO<Void>
{
}

[System.Serializable]
public struct Void { }
