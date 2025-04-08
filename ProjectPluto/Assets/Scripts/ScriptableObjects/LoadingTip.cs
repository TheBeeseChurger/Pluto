using UnityEngine;

[CreateAssetMenu(fileName = "LoadingTip", menuName = "Scriptable Objects/LoadingTip")]
public class LoadingTip : ScriptableObject
{
    [TextAreaAttribute]
    public string text;
    public Sprite sprite;
}
