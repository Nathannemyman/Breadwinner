using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneHelper {
	public static void LoadScene(string str, bool additive = false, bool setActive = false) {
		str ??= UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;

		UnityEngine.SceneManagement.SceneManager.LoadScene (
			str, additive ? UnityEngine.SceneManagement.LoadSceneMode.Additive : 0);

		if (setActive) {
            // to mark it active we have to wait a frame for it to load.
			CallAfterDelay.Create(1.0f, () => {
				UnityEngine.SceneManagement.SceneManager.SetActiveScene(
					UnityEngine.SceneManagement.SceneManager.GetSceneByName(str));
			});
		}
	}

	public static void UnloadScene(string str) {
		UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(str);
	}
}