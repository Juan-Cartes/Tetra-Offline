using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TSpinResult
{
    public bool isSpin;
    public bool isMini;
    public TSpinResult(bool isSpin, bool isMini)
    {
        this.isSpin = isSpin;
        this.isMini = isMini;
    }
}

public class Board : MonoBehaviour
{

    public bool remote = false;
    public bool replay = false;

    public GameObject playField;

    public GameObject grid;

    public Sprite transparent;
    public Sprite gridSprite; //No longer used - Remove soon.

    public Skin skin;

    [HideInInspector]
    public Color matrixColor;

    public bool doGravity = true;


    public GameObject lineClearParticles;
    public GameObject lockDownParticles;
    public GameObject hardDropParticles;


    public TextMeshProUGUI timeText;
    public TextMeshProUGUI timeLeftText;
    public TextMeshProUGUI linesText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI winner;

    public GameObject BackToBackAnimation;
    public GameObject TetraAnimation;
    public GameObject TSpinAnimation;
    public GameObject PerfectClearAnimation;

    public GameObject PauseMenu;

    public Canvas GameOver;

    [HideInInspector]
    public int linesTotal = 0;
    [HideInInspector]
    public int backToBackCount = -1;
    [HideInInspector]
    public bool lastLineClearWasB2B = false;
    [HideInInspector]
    public int combos = -1;

    [HideInInspector]
    public int score;

    private int linesForNextLevel = 10;

    [HideInInspector]
    public float width_mino;
    [HideInInspector]
    public float height_mino;

    [HideInInspector]
    public float elapsedTime;

    public int level = 1;

    [HideInInspector]
    public bool isTSpin;
    [HideInInspector]
    public bool isMini;

    [HideInInspector]
    public bool initialRotation;
    [HideInInspector]
    public int initialRotationValue;
    [HideInInspector]
    public bool PerfectClear;
    [HideInInspector]
    public bool paused = false;

    public Animator introAnimator;
    public Animator dangerAnimator;

    public bool force20G = false;

    public float lineClearDelay = .4f;
    public float entryDelay = .016f;
    public static int lockDelay = 500;

    public int softdropMultiplier = 20;
    public bool allowQuickRestart = false;
    public bool shouldLevelUp = true;
    public bool useMasterLevels = false;
    public int lineClearGoal = -1;
    public int timeLimit = -1; //Time limit - In Seconds - -1 = Disabled

    public SceneTransitions sceneTransitions;

    public static readonly int MATRIX_HEIGHT = 40;
    public static readonly int MATRIX_WIDTH = 10;

    public int[,] matrix = new int[MATRIX_HEIGHT, MATRIX_WIDTH];
    public Image[,] grids = new Image[MATRIX_HEIGHT, MATRIX_WIDTH];
    private Image[,] flashes = new Image[MATRIX_HEIGHT, MATRIX_WIDTH];

    private RectTransform rectangle;

    [HideInInspector]
    public IList<int> lineClears = new List<int>();

    [HideInInspector]
    public IList<int> flashX = new List<int>();
    [HideInInspector]
    public IList<int> flashY = new List<int>();

    [HideInInspector]
    public IList<float> flashTimer = new List<float>();

    [HideInInspector]
    public float flashTimerLineClear;

    [HideInInspector]
    public Vector3 originalPositionParent;
    [HideInInspector]
    public ParticlePool ParticlePool;

    [HideInInspector]
    public bool dead;
    [HideInInspector]
    public bool started = false;

    [HideInInspector]
    public int softDropInARow;

    [SerializeField]
    private GameObject projectileParent = null; //Can be null if is not online board.

    [SerializeField]
    private Image auraImg = null;

    private Vector3 originalScale; //Required for aspect ratio fix for 21:9 screens. Original scale of the parent

    public ReplayRecorder recorder;
    public int player = 0;
    public ReplayViewer viewer;

    public HighScoreScript highScoreScript;

    private Animation boardAnimations;

    private new Camera camera;

    private Mino mino;

    private int perfectClearRow = 0;

    private float lastCheckedAspectRatio = -1f;

    public bool Fourwiding;
    public bool Versus;

    private bool controllerDisconnectionFlag;
    private bool steamOverlayPause;

    void Awake()
    {

        skin = SkinManager.Instance.currentSkin;
        originalScale = transform.parent.localScale;
        camera = FindObjectOfType<Camera>();
        UpdateAspectRatioFix();
        if (recorder != null)
        {
            recorder.StartRecord();
        }

        if(lineClearGoal == 150 && ConfigFile.Instance.GetBool("endless_marathon", false))
        {
            lineClearGoal = -1;
        }

        originalPositionParent = transform.parent.localPosition;

        boardAnimations = GetComponent<Animation>();

        matrixColor = new Color(235f / 255f, 235f / 255f, 235f / 255f);


        gameObject.AddComponent<ParticlePool>();

        ParticlePool = GetComponent<ParticlePool>();

        ParticleData[] datas = new ParticleData[3];

        ParticleData data1 = new ParticleData
        {
            id = "lockdown",
            particle = lockDownParticles
        };

        ParticleData data2 = new ParticleData
        {
            id = "harddrop",
            particle = hardDropParticles
        };

        ParticleData data3 = new ParticleData
        {
            id = "lineclear",
            particle = lineClearParticles
        };

        datas[0] = data1;
        datas[1] = data2;
        datas[2] = data3;

        ParticlePool.particles = datas;
        rectangle = (RectTransform)playField.transform;

        width_mino = rectangle.rect.width / MATRIX_WIDTH;
        height_mino = rectangle.rect.height / skin.VisibleMinos;


        ((RectTransform)grid.transform).sizeDelta = new Vector2(width_mino, height_mino);

        for (int y = MATRIX_HEIGHT - skin.VisibleMinos - skin.GetRenderOverAmount(); y < MATRIX_HEIGHT; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                for (int i = 0; i < (ConfigFile.Instance.GetBool("display_flash", true) ? 2 : 1); i++)
                {
                    GameObject instance = Instantiate(grid);
                    instance.transform.SetParent(playField.transform);
                    instance.transform.localPosition = new Vector3(ToUnitsX(x), ToUnitsY(y), 0f);
                    instance.transform.localScale = Vector3.one;
                    instance.SetActive(true);

                    instance.GetComponent<Image>().sprite = transparent;


                    if (i == 1)
                    {
                        instance.GetComponent<Image>().sprite = null;
                        instance.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
                    }
                    if (i == 0)
                    {
                        grids[y, x] = instance.GetComponent<Image>();
                    }
                    else
                    {
                        flashes[y, x] = instance.GetComponent<Image>();
                    }
                }


            }
        }
        gameObject.AddComponent<Mino>();
        this.mino = GetComponent<Mino>();
        GetComponent<Mino>().enabled = false;
        
        if (!Versus && (!remote || replay))
        {
            introAnimator.SetTrigger("Start");
        }


    }

    public void AddPiece(Mino mino)
    {
        int[][] shape = mino.GetShape(mino.rotation);

        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                {
                    SetBlock(shape[y][x], mino.x + x, mino.y + y);
                    if (ConfigFile.Instance.GetBool("display_flash", true))
                    {
                        flashX.Add(mino.x + x);
                        flashY.Add(Mathf.FloorToInt(mino.y) + y);
                        flashTimer.Add(0);
                    }
                }
            }
        }

        //Pattern phase

        //Update t-spin vars

        TSpinResult tSpinResult = IsTSpin(mino);
        isTSpin = tSpinResult.isSpin;
        isMini = tSpinResult.isMini;


        //Line clear detection
        bool lineCleared;
        for (int y = 0; y < MATRIX_HEIGHT; y++)
        {
            for (int x = 0; x < MATRIX_WIDTH; x++)
            {
                if (matrix[y, x] == 0)
                {
                    break;
                }
                if (x == MATRIX_WIDTH - 1)
                {
                    for (int x2 = 0; x2 < MATRIX_WIDTH; x2++)
                    {
                        matrix[y, x2] = 0;
                    }

                    lineClears.Add(y);

                    if (ConfigFile.Instance.GetBool("display_particles", true))
                    {
                        GameObject particles = ParticlePool.Pool("lineclear");
                        particles.transform.SetParent(playField.transform);
                        particles.transform.localPosition = new Vector3(ToUnitsX(5), ToUnitsY(y > MATRIX_HEIGHT ? y + 1 : y));
                        particles.transform.SetParent(null);

                        Vector3 pos = particles.transform.position;
                        particles.transform.position = new Vector3(pos.x, pos.y, 0);
                        particles.transform.localScale = Vector3.one;

                        particles.GetComponent<ParticleSystem>().Play();
                    }

                    for (int i = 0; i < flashY.Count; i++)
                    {

                        if (flashY[i] == y)
                        {
                            flashX.RemoveAt(i);
                            flashY.RemoveAt(i);
                            flashTimer.RemoveAt(i);
                            i--;
                        }

                    }

                }
            }
        }
        lineCleared = lineClears.Count > 0;
        if (lineCleared)
        {
            if (!Fourwiding)
            {
                combos++;
            }
            linesTotal += lineClears.Count;
            

            if (lineClears.Count == 4 || (isTSpin && lineClears.Count > 0))
            {
                backToBackCount++;
            }
            else
            {
                backToBackCount = -1;
            }
        }
        else
        {
            if (combos > 0)
            {
                comboText.GetComponent<Animation>().Play("ComboDie", PlayMode.StopAll);
                SoundManager.Instance.PlayAudio("combo-end");
            }
            combos = -1;
        }

        PlaySoundEffects();
        lastLineClearWasB2B = IsBackToBack();
        score += GetBaseScore() + (IsBackToBack() ? Mathf.RoundToInt(GetBaseScore() * 1.5f) : 0) + GetComboScore();

        if (combos > 0)
        {
            comboText.transform.parent.gameObject.SetActive(true);
            comboText.text = combos + "\nCombo";
            comboText.GetComponent<Animation>().Rewind("Combo");
            comboText.GetComponent<Animation>().Rewind("ComboStrong");
            if (combos > 9)
            {
                comboText.GetComponent<Animation>().Play("ComboStrong", PlayMode.StopAll);
            }
            else
            {
                comboText.GetComponent<Animation>().Play("Combo", PlayMode.StopAll);
            }
        }
        else
        {
        }

        bool canPc = true;
        // We use 18 because there's no reason to check so high for a perfect clear
        for (int y = 18; y < MATRIX_HEIGHT; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (matrix[y, x] != 0) canPc = false;
            }
        }
        if (canPc)
        {
            SoundManager.Instance.PlayAudio("perfect-clear");
        }

        //Update Perfect Clear in a Row count
        if (lineClears.Count > 2)
        {
            if (!canPc)
            {
                perfectClearRow = 0;
            }
        }

        if (canPc)
        {
            perfectClearRow++;
        }

        PerfectClear = canPc;

        if (lineClears.Count > 0)
        {
           // ShowAura();
           /**
            * Cannot show aura because it used a Unity asset which cannot be added for licensing reasons
            * */
        }

        /*Garbage handling*/
        GarbageQueue queue = GetComponent<GarbageQueue>();
        if (queue != null)
        {
            int linesSent = LineCalculation.GetLinesSent(lineClears.Count, combos, PerfectClear, IsBackToBack(), (isTSpin && !isMini));
            GarbageCounterResult result = null;
            if (lineCleared)
            {
                if (!Fourwiding)
                {
                    result = queue.Counter(linesSent);
                    linesSent = result.linesLeft;
                }
                else
                {
                    result = new GarbageCounterResult
                    {
                        counter = false
                    };
                    queue.AddToQueue(2, this);
                }

            }
            
        }

        /*Animation*/
        Animate();

        if (lineCleared)
        {
            Invoke("Collapse", lineClearDelay);
        }
        else
        {
            Invoke("Collapse", entryDelay);
        }
        CalculateDanger();
        GetComponent<Hold>().SetLocked(false);
    }

    public IEnumerator LineSendingEffect(int lines, bool counter, Vector2 target)
    {

        if (counter)
        {
            SoundManager.Instance.PlayAudio("garbage-counter");
        }

        if (lines <= 3 && lines >= 1)
        {
            SoundManager.Instance.PlayAudio("garbage-send-small");
        }
        else if (lines <= 5 && lines >= 4)
        {
            SoundManager.Instance.PlayAudio("garbage-send-medium");
        }
        else if (lines >= 6)
        {
            SoundManager.Instance.PlayAudio("garbage-send-large");
        }

        if (!Fourwiding)
        {
            GameObject template = projectileParent.transform.Find(Mathf.Min(lines, 6) + " Lines").gameObject; //6 being the max effect.

            GameObject instance = Instantiate(template);
            instance.transform.position = transform.position;
            instance.SetActive(true);

            float travelTime = .4f;

            LeanTween.cancel(instance);
            LeanTween.move(instance, target, travelTime).setEaseInCirc();

            yield return new WaitForSeconds(travelTime);


            if (lines <= 3 && lines >= 1)
            {
                SoundManager.Instance.PlayAudio("garbage-hit-small");
            }
            else if (lines <= 5 && lines >= 4)
            {
                SoundManager.Instance.PlayAudio("garbage-hit-medium");
            }
            else if (lines >= 6)
            {
                SoundManager.Instance.PlayAudio("garbage-hit-large");
            }

            float fadeOutTime = .3f;

            ParticleSystem[] systems = instance.GetComponentsInChildren<ParticleSystem>();
            if (instance.GetComponent<ParticleSystem>() != null)
            {
                var mainI = instance.GetComponent<ParticleSystem>().main;

                float alpha = 1f;
                while (alpha > 0)
                {
                    yield return null;
                    alpha -= Time.deltaTime * (1 / fadeOutTime);
                    Color color = mainI.startColor.color;
                    color.a = alpha;
                    mainI.startColor = color;
                    foreach (ParticleSystem system in systems)
                    {
                        var main = system.main;
                        color = main.startColor.color;
                        color.a = alpha;
                        main.startColor = color;

                    }
                }

            }
            else
            {
                float alpha = 1f;
                while (alpha > 0)
                {
                    yield return null;
                    alpha -= Time.deltaTime * (1 / fadeOutTime);
                    foreach (ParticleSystem system in systems)
                    {
                        var main = system.main;
                        Color color = main.startColor.color;
                        color.a = alpha;
                        main.startColor = color;
                    }
                }
            
            }

            Destroy(instance);
        }


    }

    public void Toggle4wMode()
    {
        Fourwiding = !Fourwiding;
        if (Fourwiding)
        {
            //TODO - Four widing animation
        }
    }

    public void ShowAura()
    {
        auraImg.sprite = skin.NextAura();

        boardAnimations.Stop();
        boardAnimations.Play("Aura");
    }

    private void CalculateDanger()
    {
        if (GetHigestPoint() < 24)
        {
            if (!dangerAnimator.GetBool("InDanger")) //The player just entered the danger zone
            {
                if (!remote)
                {
                    if (ShouldRecord())
                    {
                        recorder.AddInputToFrameData(player, "danger");
                    }
                    MusicManager.StartDanger();
                }
            }
            dangerAnimator.SetBool("InDanger", true);
        }
        else if (GetHigestPoint() > 28)
        {
            if (dangerAnimator.GetBool("InDanger")) //The player just left the danger zone
            {
                if (!remote)
                {
                    if (ShouldRecord())
                    {
                        recorder.AddInputToFrameData(player, "stopdanger");
                    }
                    MusicManager.StopDanger();
                }
            }
            dangerAnimator.SetBool("InDanger", false);
        }

    }

    public void Die()
    {
        if (dead) return; //Do nothing if the player is already dead.
        dead = true;
        SoundManager.Instance.PlayAudio("board-fall");

        if (ShouldRecord())
        {
            recorder.AddInputToFrameData(player, "die");
        }


        if (SceneManager.GetActiveScene().name.Equals("Marathon"))
        {
            if (ConfigFile.Instance.GetInt("highscore_marathon", 0) < score && !ConfigFile.Instance.GetBool("endless_marathon", false))
            {
                highScoreScript.UpdateMarathonScore(score, recorder);
            }
        }


        GetComponent<Mino>().enabled = false;
        MusicManager.StopDanger();
        SoundManager.Instance.StopTopoutWarning();
        if (remote && !replay)
        {
            return;
        }

        boardAnimations.Play("Death");
        Animation animation = gameOverText.GetComponent<Animation>();
        animation["Text-Up"].layer = 0;
        animation.Play("Text-Up");
        animation["Game-Over"].layer = 1;
        animation.Play("Game-Over");
        Invoke("ShowGameOver", 1.0f);
        
    }


    public void ShowGameOver()
    {
        GameOver.gameObject.SetActive(true);
    }


    private void Animate()
    {
        if (lineClears.Count < 0)
        {
            TSpinAnimation.SetActive(false);
            TSpinAnimation.GetComponent<Animation>()["TSpin"].time = 0;
            TetraAnimation.SetActive(false);
            TetraAnimation.GetComponent<Animation>()["Tetra"].time = 0;
        }
        if (IsBackToBack() && lineClears.Count > 0)
        {
            BackToBackAnimation.SetActive(true);
            BackToBackAnimation.GetComponent<Animation>()["Back-To-Back"].time = 0;
            BackToBackAnimation.GetComponent<Animation>().Play(PlayMode.StopAll);
        }

        if (isTSpin)
        {
            TSpinAnimation.SetActive(true);
            TSpinAnimation.GetComponent<Animation>()["TSpin"].time = 0;
            TSpinAnimation.GetComponent<Animation>().Play(PlayMode.StopAll);

            TSpinAnimation.transform.Find("mini").gameObject.SetActive(false);
            TSpinAnimation.transform.Find("zero").gameObject.SetActive(false);
            TSpinAnimation.transform.Find("single").gameObject.SetActive(false);
            TSpinAnimation.transform.Find("double").gameObject.SetActive(false);
            TSpinAnimation.transform.Find("triple").gameObject.SetActive(false);
            if (isMini)
            {
                TSpinAnimation.transform.Find("mini").gameObject.SetActive(true);
            }

            if (lineClears.Count == 0)
            {
                TSpinAnimation.transform.Find("zero").gameObject.SetActive(true);
            }
            else if (lineClears.Count == 1)
            {
                TSpinAnimation.transform.Find("single").gameObject.SetActive(true);
            }
            else if (lineClears.Count == 2)
            {
                TSpinAnimation.transform.Find("double").gameObject.SetActive(true);
            }
            else if (lineClears.Count == 3)
            {
                TSpinAnimation.transform.Find("triple").gameObject.SetActive(true);
            }

            ParticleSystem stars = TSpinAnimation.transform.Find("Stars").GetComponent<ParticleSystem>();
            stars.emission.SetBurst(0, new ParticleSystem.Burst(0f, lineClears.Count));
            stars.Play();


        }

        if (lineClears.Count == 4)
        {
            TetraAnimation.SetActive(true);
            TetraAnimation.GetComponent<Animation>()["Tetra"].time = 0;
            TetraAnimation.GetComponent<Animation>().Play(PlayMode.StopAll);
        }

        if (PerfectClear)
        {
            PerfectClearAnimation.SetActive(true);
            PerfectClearAnimation.GetComponent<Animation>()["PerfectClear"].time = 0;
            PerfectClearAnimation.GetComponent<Animation>().Play(PlayMode.StopAll);
        }


    }

    public void Collapse()
    {

        if (ShouldRecord())
        {
            recorder.AddInputToFrameData(player, "collapse");
        }

        foreach (int y in lineClears)
        {
            for (int x = 0; x < 10; x++)
            {
                if (y > 0)
                {
                    if (grids[y, x] != null)
                    {
                        Color c = grids[y, x].color;
                        c.a = 1;
                        grids[y, x].color = c;
                    }
                }
                for (int shiftY = y; shiftY > 0; shiftY--)
                {
                    this.matrix[shiftY, x] = this.matrix[shiftY - 1, x];
                }
            }
        }
        if (lineClears.Count > 0)
        {
            SoundManager.Instance.PlayAudio("collapse" + lineClears.Count);

            if (lineClearGoal != -1 && linesTotal >= lineClearGoal)
            {
                StartCoroutine(Win(false));
                Invoke("ShowGameOver", 2f);
            }
        }
        lineClears.Clear();
        for (int x = 0; x < 10; x++)
        {
            matrix[0, x] = 0;
        }

        isTSpin = false;
        isMini = false;
        if (!remote)
        {
            NextPiece();
        }

    }

    private void PlaySoundEffects()
    {

        int lines = lineClears.Count;


        if (lines == 0)
        {
            if (isTSpin)
            {
                SoundManager.Instance.PlayAudio("tspin0" + (isMini ? "-mini" : ""));
            }
        }
        else
        {
            if (backToBackCount == 1)
            {
                SoundManager.Instance.PlayAudio("b2b-start");
            }
            else if (backToBackCount > 1)
            {
                SoundManager.Instance.PlayAudio("b2b-continue");
                SoundManager.Instance.PlayAudio("b2b-continue-" + backToBackCount);
            }
            else if (lastLineClearWasB2B && backToBackCount == -1)
            {
                SoundManager.Instance.PlayAudio("b2b-end");
            }

            if (combos > 0)
            {
                SoundManager.Instance.PlayAudio("combo" + (combos < SoundManager.Instance.renAudioLength ? "" + combos : SoundManager.Instance.renAudioLength.ToString()));

            }

            SoundManager.Instance.PlayAudio((isTSpin ? "tspin" : "erase") + lines + (isMini ? "-mini" : "") + (IsBackToBack() ? "-b2b" : ""));
        }


    }


    public void NextPiece()
    {
        Randomizer randomizer = GetComponent<Randomizer>();

        if (!GetComponent<Mino>().enabled)
        {
            GetComponent<Mino>().enabled = true;
        }
        Mino mino = GetComponent<Mino>();
        mino.id = randomizer.Next();
        mino.remote = remote;
        mino.board = this;
        mino.Init();

        this.mino = mino;

        SoundManager.Instance.PlayAudio("piece-" + Pieces.PIECES_ID_NAME[randomizer.queue[0]]);

        if (ShouldRecord())
        {
            recorder.AddInputToFrameData(player, "nextpiece");
            string queue = "";
            for (int i = 0; i < 5; i++)
            {
                queue += randomizer.queue[i] + (i != 4 ? " " : "");
            }
            recorder.AddInputToFrameData(player, "queue " + queue);
        }
        CalculateDanger();

    }

    public bool WouldNextPieceCauseDeath()
    {

        Randomizer randomizer = GetComponent<Randomizer>();
        GarbageQueue queue = GetComponent<GarbageQueue>();

        int linesOffset = queue != null ? queue.incomingGarbage : 0;

        int[,] tmp = new int[MATRIX_HEIGHT, MATRIX_WIDTH];

        for (int y = 0; y < MATRIX_HEIGHT; y++)
        {
            for (int x = 1; x < 9; x++)
            {
                tmp[y, x] = matrix[y, x];
            }
        }

        if (linesOffset > 39 - 17) // fix this
        {
            return true;
        }

        int[][] activeShape = mino.GetShape();
        int groundDistance = mino.GetGroundDistance();

        for (int y = 0; y < activeShape.Length; y++)
        {
            for (int x = 0; x < activeShape[y].Length; x++)
            {
                if (activeShape[y][x] != 0)
                {
                    tmp[Mathf.FloorToInt(mino.y) + groundDistance + y, mino.x + x] = activeShape[y][x];
                }
            }
        }


        int piece = randomizer.queue[0];
        int[][] shape = Pieces.GetDefaultShapeFromId(piece);

        int checkX = (piece == Mino.ID_O ? 4 : 3);
        int checkY = 18 + linesOffset;

        if (IsValid(tmp, shape, 0, 1, checkX, checkY))
        {
            checkY++;
        }
        if (!IsValid(tmp, shape, 0, 0, checkX, checkY))
        {
            return true;
        }

        return false;
    }

    public void SetBlock(int block, int x, float y)
    {

        int realY = Mathf.FloorToInt(y);
        if (!IsOutOfBounds(x, realY, false))
        {
            matrix[realY, x] = block;
        }
        if (!IsOutOfBounds(x, realY, true))
        {
            grids[realY, x].sprite = GetMino(block);
        }
    }

    public bool IsOutOfBounds(int x, int y, bool mustBeVisible)
    {
        return x < 0 || x >= MATRIX_WIDTH || y < (mustBeVisible ? MATRIX_HEIGHT - skin.VisibleMinos - skin.GetRenderOverAmount() : 0) || y >= MATRIX_HEIGHT;
    }

    public void Hold()
    {
        if (ShouldRecord())
        {
            recorder.AddInputToFrameData(player, "hold");
        }
        if (!paused)
        {
            GetComponent<Hold>().HoldPiece(GetComponent<Mino>().id, false);
        }
    }

    public IEnumerator Win(bool reset)
    {
        dead = true;
        if (!remote && ShouldRecord())
        {
            recorder.StopRecord();
        }
        else
        {
            //TODO
        }

        winner.transform.parent.gameObject.SetActive(true);
        Animation animation = winner.GetComponent<Animation>();
        animation["Text-Up"].layer = 0;
        animation.Play("Text-Up");
        animation["Win"].layer = 1;
        animation.Play("Win");

        if (SceneManager.GetActiveScene().name.Equals("Marathon"))
        {
            if (ConfigFile.Instance.GetInt("highscore_marathon", 0) < score && !ConfigFile.Instance.GetBool("endless_marathon", false))
            {
                highScoreScript.UpdateMarathonScore(score, recorder);
            }
        }

        if (SceneManager.GetActiveScene().name.Equals("Sprint"))
        {
            if (ConfigFile.Instance.GetFloat("besttime_sprint", 0) > elapsedTime || ConfigFile.Instance.GetFloat("besttime_sprint", 0) == 0)
            {
                highScoreScript.UpdateSprintScore(elapsedTime, recorder);
            }
        }

        if (SceneManager.GetActiveScene().name.Equals("Ultra"))
        {
            if (ConfigFile.Instance.GetInt("highscore_ultra", 0) < score)
            {
                highScoreScript.UpdateUltraScore(score, recorder);
            }
        }



        dangerAnimator.SetBool("InDanger", false);
        dangerAnimator.SetBool("NextPieceDead", false);
        dangerAnimator.SetTrigger("ForceIdle");
        yield return new WaitForSeconds(1);

        if (reset)
        {
            boardAnimations.Stop("Death");
            sceneTransitions.PlayFadeout();
            yield return new WaitForSeconds(.6f);
            sceneTransitions.Restart();
            ResetBoard();
            yield return new WaitForSeconds(.5f);
            introAnimator.Rebind();
            introAnimator.SetTrigger("Start");
        }

    }
    public IEnumerator Lose(bool reset)
    {
        dead = true;
        if (!remote && ShouldRecord())
        {
            recorder.StopRecord();
        }
        else
        {
            //TODO
        }
        gameOverText.transform.parent.gameObject.SetActive(true);
        Animation animation = gameOverText.GetComponent<Animation>();
        boardAnimations["Death"].layer = 1;
        boardAnimations.Play("Death");
        animation["Text-Up"].layer = 0;
        animation.Play("Text-Up");
        animation["Game-Over"].layer = 1;
        animation.Play("Game-Over");
        SoundManager.Instance.PlayAudio("boardfall");
        dangerAnimator.SetBool("InDanger", false);
        dangerAnimator.SetBool("NextPieceDead", false);
        dangerAnimator.SetTrigger("ForceIdle");
        yield return new WaitForSeconds(1.6f);
        if (reset)
        {
            boardAnimations.Stop("Death");
            ResetBoard();
        }

    }


    public void ResetBoard()
    {

        dangerAnimator.ResetTrigger("ForceIdle");
        matrix = new int[MATRIX_HEIGHT, MATRIX_WIDTH];
        GetComponent<Hold>().ResetHold();
        if (!remote)
        {
            GetComponent<Autoshift>().ResetShift();
        }
        GetComponent<Randomizer>().seed = -1;
        GetComponent<Randomizer>().OnEnable();
        GetComponent<Mino>().enabled = false;

        initialRotation = false;
        initialRotationValue = 0;

        GarbageQueue queue = GetComponent<GarbageQueue>();
        if (queue != null)
        {
            queue.ClearQueue();
        }

        if (Fourwiding)
        {
            Fourwiding = false;
            //TODO - Stop animation
        }

        transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        transform.rotation = Quaternion.Euler(Vector3.zero);
        started = false;
        elapsedTime = 0;
        score = 0;
        linesTotal = 0;
        dead = false;
        linesForNextLevel = 10;
        level = 1;

        combos = -1;
        comboText.transform.parent.gameObject.SetActive(false);
        backToBackCount = -1;
        gameOverText.transform.localPosition = new Vector3(0, -1000);
        gameOverText.transform.parent.gameObject.SetActive(false);
        winner.transform.localPosition = new Vector3(0, -1000);
        winner.transform.parent.gameObject.SetActive(false);
        winner.GetComponent<Animation>().Stop();
        dangerAnimator.SetBool("InDanger", false);
        dangerAnimator.SetBool("NextPieceDead", false);
        MusicManager.StopDanger();
        SoundManager.Instance.StopTopoutWarning();
    }

    public void PushUp(int lines)
    {

        lines = Math.Min(lines, MATRIX_HEIGHT);

        for (int k = 0; k < lines; k++)
        {
            for (int i = 1; i < 39; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    int blk = matrix[i + 1, j];
                    matrix[i, j] = blk;
                }
            }

            // Blank top row
            for (int j = 0; j < 10; j++)
            {
                matrix[39, j] = 0;
            }
        }
    }

    private void UpdateAspectRatioFix()
    {
        if (camera.aspect != lastCheckedAspectRatio)
        {
            lastCheckedAspectRatio = camera.aspect;
            if (camera.aspect >= 3.5)
            {
                //Ultrawide (32:9)
                if (originalScale.x == 70)
                {
                    transform.parent.localScale = new Vector3(38f, 38f, 0f);
                }
                else
                {
                    transform.parent.localScale = new Vector3(35f, 35f, 35f);
                }
            }
            else if (camera.aspect >= 2.3)
            {
                //Ultrawide (21:9)
                if (originalScale.x == 70)
                {
                    transform.parent.localScale = new Vector3(54f, 54f, 0f);
                }
                else
                {
                    transform.parent.localScale = new Vector3(50f, 50f, 0f);
                }
            }
        }
    }

    void Update()
    {

        UpdateAspectRatioFix();

        if ((Input.GetKeyDown(Controls.Instance.controls["pause"]) && PauseMenu != null))
        {
            if (!paused)
            {
                Pause();
            }
            else
            {
                UnPause();
            }
        }

        if (
            (Input.GetKeyDown(Controls.Instance.controls["harddrop"]))
            && mino != null && mino.enabled)
        {
            if (!mino.lockedDown && !remote && !replay && !dead && !paused)
            {
                mino.HardDrop(true);
            }
        }


        if (Input.GetKeyDown(Controls.Instance.controls["hold"]))
        {
            if (!remote && !replay)
            {
                Hold();
            }
        }

        if (Input.GetKeyDown(Controls.Instance.controls["retry"]))
        {
            if (allowQuickRestart)
            {
                MusicManager.StopDanger();
                sceneTransitions.ChangeSceneWithFadeout(SceneManager.GetActiveScene().name);
            }
        }

        for (int y = MATRIX_HEIGHT - skin.VisibleMinos - skin.GetRenderOverAmount(); y < MATRIX_HEIGHT; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                if (!paused)
                {
                    Sprite s = GetMino(matrix[y, x]);
                    if (grids[y, x].sprite != s)
                    {
                        grids[y, x].sprite = s;
                        if (matrix[y, x] != 0)
                        {
                            grids[y, x].color = matrixColor;
                        }
                    }
                    else
                    {
                        if (matrix[y, x] != 0)
                        {
                            grids[y, x].color = matrixColor;
                        }
                    }
                }
                else
                {
                    grids[y, x].sprite = transparent;
                    grids[y, x].color = Color.white;
                }
            }
        }



        //Line clear flash

        if (ConfigFile.Instance.GetBool("display_flash", true))
        {
            if (lineClears.Count != 0)
            {
                flashTimerLineClear += Time.deltaTime * 1000 * (replay ? viewer.multiplier : 1);
            }
            else
            {
                flashTimerLineClear = 0;
            }

            foreach (int y in lineClears)
            {
                for (int x = 0; x < 10; x++)
                {
                    if (!IsOutOfBounds(x, y, true))
                    {
                        if (flashes[y, x] == null) break;
                        Color c = flashes[y, x].color;
                        c.a = 1 - (flashTimerLineClear / (lineClearDelay * 1000));
                        if (c.a < 0) c.a = 0;
                        flashes[y, x].color = c;
                    }
                }
            }

            if (lineClears.Count == 0)
            {
                for (int y = MATRIX_HEIGHT - skin.VisibleMinos; y < MATRIX_HEIGHT; y++)
                {
                    for (int x = 0; x < MATRIX_WIDTH; x++)
                    {
                        Color c = flashes[y, x].color;
                        c.a = 0;
                        flashes[y, x].color = c;
                    }
                }
            }


        }

        //Lockdown flash

        IList<int> toRemove = new List<int>();

        for (int i = 0; i < flashX.Count; i++)
        {

            int x = flashX[i];
            int y = flashY[i];
            if (!IsOutOfBounds(x, y, true))
            {
                Color c = flashes[y, x].color;
                c.a = 1 - (flashTimer[i] / 35);
                flashes[y, x].color = c;
            }
            if (flashTimer[i] > 35)
            {
                toRemove.Add(i);
            }
            else
            {
                flashTimer[i] += Time.deltaTime * 1000;
            }
        }

        foreach (int i in toRemove)
        {
            flashX.RemoveAt(0);
            flashY.RemoveAt(0);
            flashTimer.RemoveAt(0);

        }


        //IRS
        if (ConfigFile.Instance.GetBool("irs", true))
        {
            if (mino != null && (!mino.enabled || mino.lockedDown) && !remote && !replay && !dead && !paused)
            {
                if (Input.GetKeyDown(Controls.Instance.controls["rotate_right"]))
                {
                    Rotate(1);
                }
                else if (Input.GetKeyDown(Controls.Instance.controls["rotate_left"]))
                {
                    Rotate(-1);
                }
            }
        }
        if (mino != null && mino.lockedDown)
        {
            mino.SetVisible(false);
        }

        if (linesTotal >= linesForNextLevel && shouldLevelUp)
        {
            if (level < 20)
            {
                SoundManager.Instance.PlayAudio("level-up");
                linesForNextLevel += 10;
                level++;
            }
        }

        if (!paused && !dead && started)
        {
            elapsedTime += Time.deltaTime * (replay ? viewer.multiplier : 1);
            if (timeLimit != -1)
            {
                if (timeLimit - elapsedTime <= 0)
                {
                    StartCoroutine(Win(false));
                    Invoke("ShowGameOver", 1.5f);
                }
            }
        }
        UpdateUI();

    }

    public void Rotate(int direction)
    {

        SoundManager.Instance.PlayAudio("buffer-set");
        if (direction == Mino.ROTATE_DIRECTION_RIGHT)
        {
            if (ShouldRecord())
            {
                recorder.AddInputToFrameData(player, "rotate 1");
            }
            initialRotation = true;
            initialRotationValue++;
            if (initialRotationValue > 3)
            {
                initialRotationValue = 0;
            }
        }
        else if (direction == Mino.ROTATE_DIRECTION_LEFT)
        {
            if (ShouldRecord())
            {
                recorder.AddInputToFrameData(player, "rotate -1");
            }
            initialRotation = true;
            initialRotationValue--;
            if (initialRotationValue < 0)
            {
                initialRotationValue = 3;
            }

        }
    }

    public int GetHigestPoint()
    {
        int highestPoint = MATRIX_HEIGHT;
        for (int y = 39; y >= 19; y--)
        {
            for (int x = 0; x < 10; x++)
            {
                if (matrix[y, x] != 0)
                {
                    highestPoint = y;
                    break;
                }
            }

        }
        GarbageQueue queue = GetComponent<GarbageQueue>();
        return highestPoint - (queue != null ? queue.incomingGarbage : 0);
    }


    private void UpdateUI()
    {
        if (timeText != null)
        {

            TimeSpan t = TimeSpan.FromSeconds(elapsedTime);

            string time = string.Format("{0:D2}:{1:D2}.{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds
            );

            timeText.text = time;

        }

        if (timeLeftText != null && timeLimit != -1)
        {
            float timeLeft = timeLimit - elapsedTime;
            TimeSpan t = TimeSpan.FromSeconds(timeLeft < 0 ? 0 : timeLeft);

            string time = string.Format("{0:D2}:{1:D2}.{2:D3}",
                t.Minutes,
                t.Seconds,
                t.Milliseconds
            );

            timeLeftText.text = time;
        }

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (linesText != null)
        {
            if (lineClearGoal == -1)
            {
                linesText.text = linesTotal.ToString();
            }
            else
            {
                linesText.text = linesTotal + "/" + lineClearGoal;
            }
        }

        if (levelText != null)
        {
            levelText.text = level.ToString();
        }


    }
    public void DrawBlockAt(int x, int y, int block, float lockTimer)
    {
        if (IsOutOfBounds(x, y, true))
        {
            return;
        }
        if (!paused && mino != null)
        {
            float dyingColorValue = (mino.manipulations >= Mino.manipulationLimit) ?
                .5f : (mino.lockdownLimit - lockTimer) / mino.lockdownLimit * .5f + .5f;
            Color dyingTint = new Color(dyingColorValue, dyingColorValue, dyingColorValue, 1);
            grids[y, x].sprite = GetMino(block);
            grids[y, x].color = dyingTint;
        }
    }

    public void DrawGhostBlockAt(int x, int y, int block)
    {
        if (IsOutOfBounds(x, y, true))
        {
            return;
        }
        if (!paused)
        {
            grids[y, x].sprite = GetGhostMino(block);
            grids[y, x].color = Color.white;
        }
    }

    public void Pause()
    {
        if ((!remote || (replay && !viewer.finished)) && !dead)
        {
            if (replay)
            {
                viewer.playback = false;
            }

            if (ShouldRecord())
            {
                recorder.PauseRecord();
            }

            introAnimator.speed = 0f;

            SoundManager.Instance.PlayAudio("pause");
            paused = true;
            PauseMenu.SetActive(true);
            GetComponent<Randomizer>().Hide();
            GetComponent<Hold>().Hide();
        }
    }

    public void UnPause()
    {
        if ((!remote || (replay && !viewer.finished)) && !dead)
        {
            if (replay)
            {
                viewer.playback = true;
            }

            if (recorder != null)
            {
                recorder.UnpauseRecord();
            }

            SoundManager.Instance.PlayAudio("unpause");

            GameObject eventsystem = GameObject.Find("EventSystem");
            eventsystem.GetComponent<EventSystem>().SetSelectedGameObject(null);
            paused = false;
            introAnimator.speed = 1f;
            PauseMenu.SetActive(false);
            GetComponent<Randomizer>().Show();
            GetComponent<Hold>().Show();
        }
    }

    public bool IsValid(int[,] matrixToCheck, int[][] shape, int offsetx, int offsety, int originX, int originY)
    {
        for (int y = 0; y < shape.Length; y++)
        {
            for (int x = 0; x < shape[y].Length; x++)
            {
                if (shape[y][x] != 0)
                { //Solid
                    int checkX = x + originX + offsetx;
                    int checkY = (int)Mathf.FloorToInt(y + originY) + offsety;
                    if (checkY < 0)
                    {
                        return false;
                    }
                    if (checkY >= MATRIX_HEIGHT)
                    {
                        return false;
                    }
                    if (checkX < 0 || checkX >= 10)
                    {
                        return false;
                    }
                    if (matrixToCheck[checkY, checkX] != 0)
                    {
                        return false;
                    }

                }
            }
        }
        return true;

    }

    public bool IsValid(Mino tetromino, int offsetx, int offsety, int rotation)
    {
        int[][] shape = tetromino.GetShape(rotation);
        return IsValid(matrix, shape, offsetx, offsety, tetromino.x, Mathf.FloorToInt(tetromino.y));
    }

    public int GetBaseScore()
    {
        switch (lineClears.Count)
        {
            case 1:
                if (isTSpin)
                {
                    if (isMini)
                    {
                        return 200 * level;
                    }
                    return 800 * level;
                }
                else
                {
                    return 100 * level;
                }
            case 2:
                if (isTSpin)
                {
                    if (isMini)
                    {
                        return 400 * level;
                    }
                    return 1200 * level;
                }
                else
                {
                    return 300 * level;
                }
            case 3:

                if (isTSpin)
                {
                    return 1600 * level;
                }
                else
                {
                    return 500 * level;
                }
            case 4:
                return 800 * level;
            default:
                return 0;
        }
    }

    private int GetComboScore()
    {
        return combos >= 1 ? (50 * combos * level) : 0;
    }

    public TSpinResult IsTSpin(Mino mino)
    {
        bool isSpin = false;
        bool isMini = false;
        if (mino.id == Mino.ID_T &&
            mino.lastRotationX == mino.x && mino.lastRotationY == Mathf.FloorToInt(mino.y))
        {
            int validTests = 0;
            for (int i = 0; i < SRS.SPIN_POINTS[0].Length; i++)
            {
                int offsetX = SRS.SPIN_POINTS[mino.rotation][i][0];
                int offsetY = SRS.SPIN_POINTS[mino.rotation][i][1];
                if (IsCoordinateSolid(offsetX + mino.x, offsetY + Mathf.FloorToInt(mino.y)))
                {
                    validTests++;
                }
                if (i == 1 && validTests < 2 && !mino.lastKick)
                {
                    isMini = true;
                }
            }
            if (validTests >= 3)
            {
                isSpin = true;
            }
            else
            {
                isMini = false;
            }



        }
        return new TSpinResult(isSpin, isMini);
    }

    public bool IsCoordinateSolid(int x, int y)
    {
        return y >= MATRIX_HEIGHT || x < 0 || x >= MATRIX_WIDTH || matrix[y, x] != 0;
    }

    public float ToUnitsX(float x)
    {
        return x * width_mino;
    }

    public Sprite GetMino(int id)
    {

        switch (id)
        {
            case -1:
                return null;
            case 0:
                return transparent;
            case 1:
                return skin.GetMino("i");
            case 2:
                return skin.GetMino("o");
            case 3:
                return skin.GetMino("j");
            case 4:
                return skin.GetMino("l");
            case 5:
                return skin.GetMino("z");
            case 6:
                return skin.GetMino("s");
            case 7:
                return skin.GetMino("t");
            case 8:
                return skin.GetMino("garbage");
            default:
                break;
        }
        return null;

    }

    public Sprite GetGhostMino(int id)
    {

        switch (id)
        {
            case 1:
                return skin.GetGhost("i");
            case 2:
                return skin.GetGhost("o");
            case 3:
                return skin.GetGhost("j");
            case 4:
                return skin.GetGhost("l");
            case 5:
                return skin.GetGhost("z");
            case 6:
                return skin.GetGhost("s");
            case 7:
                return skin.GetGhost("t");
            default:
                break;
        }
        return null;

    }

    private bool SecretGradeDetect()
    {

        int visibleMatrix = MATRIX_HEIGHT - 20;
        int checkOffset = 1;
        int startY = visibleMatrix + checkOffset;

        int checkHole;
        int sum;

        if (!IsCoordinateSolid(0, startY))
        {
            checkHole = 0;
            sum = 1;
        }
        else if (!IsCoordinateSolid(9, startY))
        {
            checkHole = 9;
            sum = -1;
        }
        else
        {
            return false;
        }


        for (int y = startY; y < MATRIX_HEIGHT; y++)
        {
            bool solid = false;
            bool emptyHole = false;
            for (int x = 0; x < 10; x++)
            {
                if (x == checkHole)
                {
                    if (!IsCoordinateSolid(x, y))
                    {
                        emptyHole = true;
                    }
                }
                else
                {
                    if (IsCoordinateSolid(x, y))
                    {
                        solid = true;
                    }
                    else
                    {
                        solid = false;
                        break;
                    }
                }


            }

            if (!solid || !emptyHole)
            {
                return false;
            }

            checkHole += sum;
            if (checkHole < 0)
            {
                checkHole = 1;
                sum = 1;
            }
            if (checkHole > 9)
            {
                checkHole = 8;
                sum = -1;
            }

        }


        return true;
    }

    public float ToUnitsY(float y)
    {
        return 0 - (y * height_mino - (20 - (skin.VisibleMinos - 20)) * height_mino);
    }

    public bool IsBackToBack()
    {
        return backToBackCount > 0;
    }

    public bool ShouldRecord()
    {
        return recorder != null && recorder.record;
    }

}
