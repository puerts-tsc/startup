using System.Collections;
using System.Runtime.CompilerServices;
using Tetris.Blocks;
using UnityEngine;
using UnityEngine.Events;

namespace Tetris
{
    public class Game : Tetris<Game>
    {
        public float StartTime {
            get { return startTime; }
            set { startTime = value; }
        }

        static Game m_Instance;
        public static Game instance => m_Instance ??= FindObjectOfType<Game>();
        public static  Vector3 basePos => instance.transform.position;

        [SerializeField]
        private GameState state = GameState.Gamming;

        [SerializeField]
        private Block[] blocks;

        [SerializeField]
        LineRenderer linePrefab;

        public UnityEvent OnPauseCallback;
        public UnityEvent OnGammingCallback;
        private Gameplay m_Gameplay;
        private TetrisVFX m_TetrisVFX;
        private const float offset = .5f;
        private bool left_pressed;
        private bool right_pressed;
        private bool up_pressed;
        private float startTime = .15f;
        private float lastStartTime;
        private float inputDelta = .03f;
        private float lastInputTime;

        private enum GameState
        {
            Pause,
            Gamming
        }

        void Start()
        {
            m_TetrisVFX = GameObject.Find( "VFX & SFX" ).GetComponent<TetrisVFX>();
            // Tetirs Constructor
            if ( m_TetrisVFX ) m_Gameplay = Gameplay.Instance.New(  BlockSpawner.Instance.New( blocks ), m_TetrisVFX );
            else m_Gameplay =  Gameplay.Instance.New(  BlockSpawner.Instance.New( blocks ) );

            // draw row line
            for ( int i = 0; i <= Gameplay.Height + Gameplay.ExtraHeight - 1; i++ ) {
                var row = Instantiate( linePrefab, this.transform );
                row.positionCount = 2;
                row.SetPosition( 0, this.transform.position + new Vector3( -offset, i - offset ) );
                row.SetPosition( 1, this.transform.position + new Vector3( Gameplay.Width - offset, i - offset ) );
            }

            // draw col line
            for ( int i = 0; i <= Gameplay.Width; i++ ) {
                var col = Instantiate( linePrefab, this.transform );
                col.positionCount = 2;
                col.SetPosition( 0, this.transform.position + new Vector3( i - offset, -offset ) );
                col.SetPosition( 1,
                    this.transform.position +
                    new Vector3( i - offset, Gameplay.Height + Gameplay.ExtraHeight - 1 - offset ) );
            }

            state = GameState.Pause;
            StartGame();
        }

        void Update()
        {
            if ( state == GameState.Gamming ) {
                ProcessBlockInput();

                // auto drop
                m_Gameplay.Fall( Time.deltaTime );
            }
        }

        private void ProcessBlockInput()
        {
            // hard & soft drop
            if ( Input.GetKeyDown( KeyCode.Space ) ) {
                m_Gameplay.HardDrop(); // execute in one frame
            }
            else if ( !up_pressed && Input.GetKeyDown( KeyCode.DownArrow ) ) {
                up_pressed = true;
                m_Gameplay.SoftDrop();
            }
            else if ( up_pressed && Input.GetKeyUp( KeyCode.DownArrow ) ) {
                up_pressed = false;
                m_Gameplay.NormalDrop();
            }

            // move left
            if ( Input.GetKeyDown( KeyCode.LeftArrow ) ) {
                m_Gameplay.MoveLeft();
                left_pressed = true;
            }

            if ( !right_pressed && left_pressed && Input.GetKey( KeyCode.LeftArrow ) ) {
                if ( lastStartTime >= startTime ) {
                    if ( lastInputTime >= inputDelta ) {
                        m_Gameplay.MoveLeft();
                        lastInputTime = 0;
                    }
                    else {
                        lastInputTime += Time.deltaTime;
                    }
                }
                else {
                    lastStartTime += Time.deltaTime;
                }
            }

            if ( left_pressed && Input.GetKeyUp( KeyCode.LeftArrow ) ) {
                left_pressed = false;
                lastStartTime = 0;
                lastInputTime = 0;
            }

            // move right
            if ( Input.GetKeyDown( KeyCode.RightArrow ) ) {
                m_Gameplay.MoveRight();
                right_pressed = true;
            }

            if ( !left_pressed && right_pressed && Input.GetKey( KeyCode.RightArrow ) ) {
                if ( lastStartTime >= startTime ) {
                    if ( lastInputTime >= inputDelta ) {
                        m_Gameplay.MoveRight();
                        lastInputTime = 0;
                    }
                    else {
                        lastInputTime += Time.deltaTime;
                    }
                }
                else {
                    lastStartTime += Time.deltaTime;
                }
            }

            if ( right_pressed && Input.GetKeyUp( KeyCode.RightArrow ) ) {
                right_pressed = false;
                lastStartTime = 0;
                lastInputTime = 0;
            }

            // rotate
            if ( Input.GetKeyDown( KeyCode.Z ) ) {
                m_Gameplay.AntiClockwiseRotation();
            }

            if ( Input.GetKeyDown( KeyCode.X ) ) {
                m_Gameplay.ClockwiseRotation();
            }

            // hold
            if ( Input.GetKeyDown( KeyCode.C ) ) {
                m_Gameplay.HoldBlock();
            }

            // logger
            //if (Input.GetKeyDown(KeyCode.Return))
            //{
            //    Tetris.logger.Print();
            //}
        }

        public void PauseGame()
        {
            state = GameState.Pause;
            OnPauseCallback?.Invoke();
        }

        public void StartGame()
        {
            StartCoroutine( Gamming() );
        }

        private IEnumerator Gamming()
        {
            yield return new WaitForSeconds( 1f );
            OnGammingCallback?.Invoke();
            m_TetrisVFX.TextVFX_Start();
            yield return new WaitForSeconds( 5f );
            state = GameState.Gamming;
            m_Gameplay.NextBlock();
            m_TetrisVFX.PlayBG( Utils.BGMainTheme );
        }

        public void QuitGame()
        {
            Application.Quit();
        }

    #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if ( m_Gameplay != null ) m_Gameplay.DrawGizmos();
        }
    #endif
    }
}