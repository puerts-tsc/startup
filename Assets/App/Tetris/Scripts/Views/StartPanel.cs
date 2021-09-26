using UnityEngine;
using UnityEngine.UI;

namespace Tetris.Views
{
    public class StartPanel : Tetris<StartPanel>
    {
        private Button goBtn;

        private void OnEnable()
        {
            goBtn = GetComponent<Button>();

            SquaresTransition transition = new SquaresTransition();

            if (goBtn) goBtn.onClick.AddListener(() =>
            {
                SceneTransitionMgr.Instance.StartTransition(transition, 1);
            });

        }

        private void OnDisable()
        {
            if (goBtn) goBtn.onClick.RemoveAllListeners();
        }

    }
}
