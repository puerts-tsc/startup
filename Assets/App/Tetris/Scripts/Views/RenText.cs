using TMPro;
using UnityEngine;

namespace Tetris.Views
{
    public class RenText : Tetris<RenText>
    {
        private Saro.Tween tween;
        private TMP_Text text;

        public void RestartTween(int count)
        {
            if (!tween) tween = GetComponent<Saro.Tween>();
            if (!text) text = GetComponent<TMP_Text>();

            text.text = string.Format("Ren\n{0}", count.ToString());
            tween.ResetNormalizedTime();
            tween.enabled = true;
        }
    }
}
