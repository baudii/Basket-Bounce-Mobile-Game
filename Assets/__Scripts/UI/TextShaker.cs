using System.Collections;
using UnityEngine;
using TMPro;
using System;
using KK.Common;

namespace BasketBounce.UI
{
	public class TextShakeEffect : MonoBehaviour
	{
		[SerializeField] TMP_Text textMeshPro;
		[SerializeField] WordShaker[] wordShakers;

		Vector3[][] originalVertices;

		private bool isShaking = false;

		private IEnumerator Start()
		{
			// Ensure the textMeshPro reference is assigned
			if (textMeshPro == null)
			{
				textMeshPro = GetComponent<TMP_Text>();
			}
			yield return new WaitForSeconds(0.1f);

			TMP_TextInfo textInfo = textMeshPro.textInfo;

			// Store the original vertices positions
			originalVertices = new Vector3[textInfo.meshInfo.Length][];
			for (int i = 0; i < textInfo.meshInfo.Length; i++)
			{
				originalVertices[i] = textInfo.meshInfo[i].vertices.Clone() as Vector3[];
			}

			StartShaking();
		}

		public void StartShaking()
		{
			if (!isShaking)
			{
				StartCoroutine(ShakeWord());
			}
			else
			{
				this.Warning("Shaking is already in progress or the word index is out of range.");
			}
		}

		private IEnumerator ShakeWord()
		{
			this.Log("Started shaking!");
			isShaking = true;

			// Store the original text info
			textMeshPro.ForceMeshUpdate();
			TMP_TextInfo textInfo = textMeshPro.textInfo;

			while (isShaking)
			{
				// Update the mesh and textInfo every frame
				textMeshPro.ForceMeshUpdate();
				textInfo = textMeshPro.textInfo;
				foreach (WordShaker wordShaker in wordShakers)
				{
					wordShaker.Shake(textInfo, originalVertices);
				}

				// Update the mesh with the new vertex positions
				for (int i = 0; i < textInfo.meshInfo.Length; i++)
				{
					textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
					textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
				}

				yield return null; // Wait for the next frame
			}

			this.Log("Resseting original verticies state");
			for (int i = 0; i < originalVertices.Length; i++)
			{
				Array.Copy(originalVertices[i], textInfo.meshInfo[i].vertices, originalVertices[i].Length);
			}

			// Update the mesh after stopping the shaking
			for (int i = 0; i < textInfo.meshInfo.Length; i++)
			{
				textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
				textMeshPro.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
			}

			this.Log("Stopped shaking!");
			isShaking = false;
		}

		public void StopShaking()
		{
			this.Log("Shaking was interrupted!");
			isShaking = false;
		}

		[Serializable]
		class WordShaker
		{
			public int startWordIndex, endWordIndex;
			public Vector2 shakeMagnitude;
			public float shakeSpeed;

			public float GetX(float t, int charIndex) => Mathf.Sin(t * shakeSpeed + charIndex) * shakeMagnitude.x;
			public float GetY(float t, int charIndex) => Mathf.Cos(t * shakeSpeed + charIndex) * shakeMagnitude.y;

			public void Shake(TMP_TextInfo textInfo, Vector3[][] originalVertices)
			{
				int firstCharIndex = textInfo.wordInfo[startWordIndex].firstCharacterIndex;
				int lastCharIndex = textInfo.wordInfo[endWordIndex].lastCharacterIndex;
				for (int charIndex = firstCharIndex; charIndex <= lastCharIndex; charIndex++)
				{
					TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

					// Only shake visible characters
					if (!charInfo.isVisible)
						continue;

					// Get the vertices of the character
					int materialIndex = charInfo.materialReferenceIndex;
					int vertexIndex = charInfo.vertexIndex;

					Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

					// Apply shaking by modifying vertex positions
					Vector3 offset = new Vector3(GetX(Time.time, charIndex), GetY(Time.time, charIndex), 0);
					vertices[vertexIndex + 0] = originalVertices[materialIndex][vertexIndex + 0] + offset;
					vertices[vertexIndex + 1] = originalVertices[materialIndex][vertexIndex + 1] + offset;
					vertices[vertexIndex + 2] = originalVertices[materialIndex][vertexIndex + 2] + offset;
					vertices[vertexIndex + 3] = originalVertices[materialIndex][vertexIndex + 3] + offset;
				}
			}
		}
	}
}