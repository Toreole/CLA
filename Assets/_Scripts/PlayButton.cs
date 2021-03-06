﻿using UnityEngine;

namespace LATwo
{
    public class PlayButton : MonoBehaviour
    {
        public void StartGame() => GameManager.StartGame();
        public void QuitGame() => Application.Quit();
    }
}