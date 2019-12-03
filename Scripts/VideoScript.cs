using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoScript : MonoBehaviour {
    VideoPlayer vp;

    private void Start() {
        vp = GetComponent<VideoPlayer>();
        vp.loopPointReached += endReached;
    }

    void Update() {
        if (Input.anyKey == true) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }

    void endReached(VideoPlayer vp) {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
