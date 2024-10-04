using UnityEngine;

[CreateAssetMenu(fileName = "TextData", menuName = "SO/TextData")]
public class TextDataSO : ScriptableObject
{
	[TextArea(3, 10)]
	public string[] texts;
}
