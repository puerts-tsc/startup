using TMPro;
using UnityEngine;

namespace Tetris.Views
{
    public class GameView : Tetris<GameView>
    {
        public TMP_Text score;
        public TMP_Text level;
        public TMP_Text goal;
        public TMP_Text time;
        public GameObject gameOverPanel;

        private void OnEnable()
        {
            Gameplay.OnScoreChanged += UpdateScore;
            Gameplay.OnLevelChanged += UpdateLevel;
            Gameplay.OnGoalChanged += UpdateGoal;
            Gameplay.OnTimeChanged += UpdateTime;
            Gameplay.OnGameOver += OnGameOver;

            gameOverPanel.SetActive(false);

        }

        private void OnDisable()
        {
            Gameplay.OnScoreChanged -= UpdateScore;
            Gameplay.OnLevelChanged -= UpdateLevel;
            Gameplay.OnGoalChanged -= UpdateGoal;
            Gameplay.OnTimeChanged -= UpdateTime;
            Gameplay.OnGameOver -= OnGameOver;
        }

        private int m, s, ms;

        private void UpdateTime(float val)
        {
            m = (int)val / 60;
            s = (int)val % 60;
            ms = ((int)(val * 1000) % 1000) / 10;

            time.text = string.Format("{0:00}:{1:00}.{2:00}", m, s, ms);
        }

        private void UpdateGoal(int val)
        {
            goal.text = val.ToString();
        }

        private void OnGameOver()
        {
            gameOverPanel.SetActive(true);
        }


        private void UpdateLevel(int val)
        {
            level.text = val.ToString();
        }

        private void UpdateScore(int val)
        {
            score.text = val.ToString();
        }
    }
}
