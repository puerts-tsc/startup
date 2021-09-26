using UnityEngine;

//Single block from pieces in view
namespace UnityTetris
{
    public class TetriminoBlock : PoolingObject
    {
        public override string objectName => "TetriminoBlock";
        void OnBecameVisible() { }
        public Vector2Int position { get; private set; }

        RectTransform rect;

        private MeshRenderer msRenderer;

        //Gets references to the components
        public void Awake()
        {
            rect = GetComponent<RectTransform>();
            msRenderer = GetComponent<MeshRenderer>();
        }

        //Sets the color of the block
        public void SetColor(Color c)
        {
            msRenderer.material.color = c;
        }

        //Positioning the block
        public void MoveTo(int x, int y)
        {
            position = new Vector2Int(x, y);
            rect.anchoredPosition3D = new Vector3(x, 0, -y);
        }
    }
}