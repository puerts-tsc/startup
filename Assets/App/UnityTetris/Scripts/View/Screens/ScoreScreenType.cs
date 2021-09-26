using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityTetris
{
    public abstract class ScoreScreenType<T> : BaseScreen<T> where T : Component
    {
        [SerializeField]
        protected TMP_Text scoreText;

        protected string mScorePrefix = "SCORE\n";

        protected void SetScoreText(int value)
        {
            scoreText.text = mScorePrefix + value;
        }
    }
}
