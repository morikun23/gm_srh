using UnityEngine;

/// <summary>
/// シーンファイルをシリアライズ化するための機能
/// 版元：http://baba-s.hatenablog.com/entry/2017/11/14/110000
/// </summary>
[System.Serializable]
public class SceneObject {
	[SerializeField]
	private string m_SceneName;

	public static implicit operator string(SceneObject sceneObject)
	{
		return sceneObject.m_SceneName;
	}

	public static implicit operator SceneObject(string sceneName)
	{
		return new SceneObject() { m_SceneName = sceneName };
	}
}