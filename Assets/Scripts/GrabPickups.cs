using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GrabPickups : MonoBehaviour {

	private AudioSource pickupSoundSource;

    public Text levelText;

    void Awake() {
		pickupSoundSource = DontDestroy.instance.GetComponents<AudioSource>()[1];
        levelText.text = "Level: " + LevelGenerator.level.ToString();
    }

	void OnControllerColliderHit(ControllerColliderHit hit) {
		if (hit.gameObject.tag == "Pickup") {
			pickupSoundSource.Play();
			SceneManager.LoadScene("Play");
            //incrementing the level
            LevelGenerator.level++;
            levelText.text = "Level: " + LevelGenerator.level.ToString();
        }
	}
}
