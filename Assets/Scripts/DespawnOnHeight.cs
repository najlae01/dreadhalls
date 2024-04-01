using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DespawnOnHeight : MonoBehaviour
{
    public float despawnHeight = -4f;

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= despawnHeight)
        {
            LevelGenerator.level = 1;
            SceneManager.LoadScene("GameOver");
            Destroy(GameObject.Find("WhisperSource"));
        }
    }
}
