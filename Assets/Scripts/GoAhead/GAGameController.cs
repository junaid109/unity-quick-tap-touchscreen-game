using System;
using System.Collections;
using Assets.Scripts.GoAhead.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.GoAhead
{
    /// <summary>
    /// This class controls the spawing of the targets, score and game time.
    /// </summary>
    public class GAGameController : MonoBehaviour
    {
        [Tooltip("How fast the player object moves. This is only for keyboard and gamepad controls")]
        [Space(5)]
        public float playerObjectSpeed = 15;

        [Tooltip("The chance for a quick target to appear. This overrides the chance for a target with a helmet to appear")]
        [Space(5)]
        public float quickChance = 0.05f;

        [Tooltip("The chance for a target with a helmet to appear")]
        [Space(5)]
        public float helmetChance = 0.2f;

        [Tooltip("How long to wait before starting the game. Ready?GO! time")]
        [Space(5)]
        public float startDelay = 1;

        [Tooltip("The effect displayed before starting the game")]
        [Space(5)]
        public Transform readyGoEffect;

        [Tooltip("How many seconds are left before game over")]
        [Space(5)]
        public float timeLeft = 30;

        [Tooltip("The text object that displays the time")]
        [Space(5)]
        public Text timeText;

        [Tooltip("The image object that displays the radial")]
        [Space(5)]
        public float radialFillAmount;

        [Tooltip("The image object that displays the radial")]
        [Space(5)]
        public Image radialFillImage;

        [Tooltip("A list of targets ( The targets that appear and hide in the holes )")]
        [Space(5)]
        public Transform[] targets;

        [Tooltip("How many targets to show at once")]
        [Space(5)]
        public int maximumTargets = 5;

        [Tooltip("How long to wait before showing the targets")]
        [Space(5)]
        public float showDelay = 3;

        [Tooltip("How long to wait before hiding the targets again")]
        [Space(5)]
        public float hideDelay = 2;

        [Tooltip("The attack button, click it or tap it to attack with the hammer")]
        [Space(5)]
        public string attackButton = "Fire1";

        [Tooltip("How many points we get when we hit a target. This bonus is increased as we hit more targets")]
        [Space(5)]
        public int hitTargetBonus = 10;

        [Tooltip("Counts the current hit streak, which multiplies your bonus for each hit. The streak is reset after the targets hide")]
        internal int streak = 1;

        [Tooltip("The bonus effect that shows how much bonus we got when we hit a target")]
        [Space(5)]
        public Transform bonusEffect;

        [Tooltip("The wrong hit effect that shows hit an incorrect target")]
        [Space(5)]
        public Transform wrongHitEffect;

        [Tooltip("The score of the player")]
        [Space(5)]
        public int score = 0;

        [Tooltip("The score text object which displays the current score of the player")] [Space(5)]
        public Transform scoreText;

        [Tooltip("The bag objects to fill ui")] [Space(5)]
        public Transform[] bagObjectTransforms;

        [Tooltip("The empty bag objects ui")] [Space(5)]
        public Transform[] emptyBagObjectTransforms;

        [Tooltip("The pound prize that appears after 5 bags are collected")]
        [Space(5)] [Header("This the pound prize to appear after 5 bags are collected")]
        public Transform poundPrizeTransform;

        [Tooltip("A list of levels, each with its own target score, target limit, and time bonus")]
        [Space(5)]
        public Level[] levels;

        [Tooltip("The current level we are on. We must reach the target score in order to go to the next level")]
        [Space(5)]
        public int currentLevel = 0;

        [Tooltip("If you set this to true the game will continue forever after the last level in the list. Otherwise you will get the victory screen after the last level")]
        [Space(5)]
        public bool isEndless = false;

        // Various canvases for the UI
        [Header("Game UI Object")]
        public Transform gameCanvas;

        [Header("Progress UI Object")]
        public Transform progressCanvas;

        [Header("Pause UI Object")]
        public Transform pauseCanvas;

        [Header("Game Over UI Object")]
        public Transform gameOverCanvas;

        [Header("Victory Pound UI Object")]
        public Transform poundVictoryCanvas;

        [Header("Victory Bag UI Object")]
        public Transform bagVictoryCanvas;

        [Header("Animated Fruit Group")]
        public Transform animatedGroup;

        // The level of the main menu that can be loaded after the game ends
        [Space(5)]
        public string mainMenuLevelName = "GA_StartMenu";

        [Tooltip("The animation name when showing a bag target")]
        [Space(5)]
        public string bagAnimationShow = "Bag1Anim";

        // Various sounds and their source        
        [Space(5)]
        public AudioClip soundLevelUp;

        [Space(5)]
        public AudioClip soundGameOver;

        [Space(5)]
        public AudioClip soundVictory;

        [Space(5)]
        public AudioClip soundBagVictory;

        [Space(5)]
        public Animator bagUiAnimator;

        // The animation component that holds the animation clips
        internal Animation animationObject;

        [Space(5)]
        public string soundSourceTag = "GameController";

        // The button that will restart the game after game over
        [Space(5)]
        public string confirmButton = "Submit";

        // The button that pauses the game. Clicking on the pause button in the UI also pauses the game
        [Space(5)]
        public string pauseButton = "Cancel";

        [Space(5)]
        public PrizeData sessionPrizeData;

        // The counts for the collected bags and wrong items clicked
        private int bagsCollected;
        private int wrongItemCollected;
    
        //The player object that aims and attacks. In our case this is the hammer object.
        internal Transform playerObject;
        // The player animator holds all the animations of the player object
        internal Animator playerAnimator;
        // Are we using the mouse now?
        internal bool usingMouse = false;
        // The position we are aiming at now
        internal Vector3 aimPosition;
        internal float showDelayCount = 0;
        internal bool isGameOver = false;
        internal bool isPaused = false;
        internal bool isPoundPrizeWon = false;
        internal GameObject soundSource;
        internal int index = 0;
        internal int highScore = 0;
        internal int scoreMultiplier = 1;
        internal float hideDelayCount = 0;

        private float radialImageFillTime;

        public int BagsCollected
        {
            get { return bagsCollected; }
            set
            {
                if (value >= 5)
                {
                    Debug.Log("You have collected 5 bags!");
                    value = 5;
                }
                bagsCollected = value;
            }
        }

        //public Transform slowMotionEffect;

        void Awake()
        {
            // Activate the pause canvas early on, so it can detect info about sound volume state
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);

            if (playerObject == null)
            {
                playerObject = GameObject.FindGameObjectWithTag("Player").transform;

                playerAnimator = playerObject.GetComponent<Animator>();
            }
        }

        /// <summary>
        /// Start is only called once in the lifetime of the behaviour.
        /// The difference between Awake and Start is that Start is only called if the script instance is enabled.
        /// This allows you to delay any initialization code, until it is really needed.
        /// Awake is always called before any Start functions.
        /// This allows you to order initialization of scripts
        /// </summary>
        void Start()
        {
            StartCoroutine(RandomPosition());

			SetOldData();

            //Deactivate the pound circle so player has to unlock it after 5 bags collected
            poundPrizeTransform.gameObject.SetActive(false);

            // Check if we are running on a mobile device. If so, remove the playerObject as we don't need it for taps
            if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android)
            {
                // If a playerObject is assigned, hide it
                if (playerObject) playerObject.gameObject.SetActive(false);

                playerObject = null;
            }

            //Update the score
            UpdateScore();

            // set the fill amount to the fill time
            radialImageFillTime = radialFillAmount;

            //Hide the bags ui
            foreach (Transform transform in bagObjectTransforms)
            {
                transform.gameObject.SetActive(false);
            }

            //Hide the cavases
            if (gameOverCanvas) gameOverCanvas.gameObject.SetActive(false);
            if (poundVictoryCanvas) poundVictoryCanvas.gameObject.SetActive(false);
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);

            //Get the highscore for the player
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            highScore = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name + "HighScore", 0);
#else
			highScore = PlayerPrefs.GetInt(Application.loadedLevelName + "HighScore", 0);
#endif
            //Assign the sound source for easier access
            if (GameObject.FindGameObjectWithTag(soundSourceTag)) soundSource = GameObject.FindGameObjectWithTag(soundSourceTag);

            // Reset the spawn delay
            showDelayCount = 0;

            // Check what level we are on
            UpdateLevel();

            // Move the targets from one side of the screen to the other, and then reset them
            foreach (Transform movingTarget in targets)
            {
                movingTarget.SendMessage("HideTarget", SendMessageOptions.DontRequireReceiver);
            }
           // if (readyGoEffect) Instantiate(readyGoEffect);
        }

        /// <summary>
        /// Update is called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        void Update()
        {
            // Delay the start of the game
            if (startDelay > 0)
            {
                startDelay -= Time.deltaTime;
            }
            else
            {

                //If the game is over, listen for the Restart and MainMenu buttons
                if (isGameOver == true)
                {
                    //The jump button restarts the game
                    if (Input.GetButtonDown(confirmButton))
                    {
                        Restart();
                    }

                    //The pause button goes to the main menu
                    if (Input.GetButtonDown(pauseButton))
                    {
                        MainMenu();
                    }
                }
                else
                {
                    // Count down the time until game over
                    if (timeLeft > 0)
                    {
                        // Count down the time
                        timeLeft -= Time.deltaTime;

                        //Update the radial image
                        radialImageFillTime -= Time.deltaTime;
                        radialFillImage.fillAmount = radialImageFillTime / radialFillAmount;

                        // Update the timer
                        UpdateTime();
                    }

                    // Keyboard and Gamepad controls
                    if (playerObject && isPaused == false)
                    {
                        // If we move the mouse in any direction, then mouse controls take effect
                        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0 || Input.GetMouseButtonDown(0) || Input.touchCount > 0) usingMouse = true;

                        // We are using the mouse, hide the playerObject
                        if (usingMouse == true)
                        {
                            // Calculate the mouse/tap position
                            aimPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                            // Make sure it's 2D
                            aimPosition.z = 0;
                        }

                        // If we press gamepad or keyboard arrows, then mouse controls are turned off
                        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
                        {
                            usingMouse = false;
                        }

                        // Move the playerObject based on gamepad/keyboard directions
                        aimPosition += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), aimPosition.z) * playerObjectSpeed * Time.deltaTime;

                        // Limit the position of the playerObject to the edges of the screen
                        // Limit to the left screen edge
                        if (aimPosition.x < Camera.main.ScreenToWorldPoint(Vector3.zero).x) aimPosition = new Vector3(Camera.main.ScreenToWorldPoint(Vector3.zero).x, aimPosition.y, aimPosition.z);

                        // Limit to the right screen edge
                        if (aimPosition.x > Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x) aimPosition = new Vector3(Camera.main.ScreenToWorldPoint(Vector3.right * Screen.width).x, aimPosition.y, aimPosition.z);

                        // Limit to the bottom screen edge
                        if (aimPosition.y < Camera.main.ScreenToWorldPoint(Vector3.zero).y) aimPosition = new Vector3(aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.zero).y, aimPosition.z);

                        // Limit to the top screen edge
                        if (aimPosition.y > Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y) aimPosition = new Vector3(aimPosition.x, Camera.main.ScreenToWorldPoint(Vector3.up * Screen.height).y, aimPosition.z);

                        // Place the playerObject at the position of the mouse/tap, with an added offset
                        //playerObject.position = aimPosition;

                        playerObject.eulerAngles = Vector3.forward * (playerObject.position.x - aimPosition.x) * -10;

                        // Move the hammer towards the aim posion
                        //if ( Vector3.Distance(playerObject.position, aimPosition) > Time.deltaTime * playerObjectSpeed )    playerObject.position = Vector3.MoveTowards(playerObject.position, aimPosition, Time.deltaTime * playerObjectSpeed);
                        //else    playerObject.position = aimPosition;
                        playerObject.position = aimPosition;
                        // If we press the shoot button, SHOOT!
                        //if ( usingMouse == false && Input.GetButtonDown(attackButton) )    Shoot();
                        if (playerObject && !EventSystem.current.IsPointerOverGameObject() && Input.GetButtonUp(attackButton))
                        {
                            playerAnimator.Play("HammerDown");
                        }
                    }
                    // Count down to the next target spawn
                    if (showDelayCount > 0) showDelayCount -= Time.deltaTime;
                    else
                    {
                        // Reset the spawn delay count
                        showDelayCount = showDelay;
                        ShowTargets(maximumTargets);
                    }

                    //Toggle pause/unpause in the game
                    if (Input.GetButtonDown(pauseButton))
                    {
                        if (isPaused == true) Unpause();
                        else Pause(true);
                    }
                }
            }
        }

        IEnumerator RandomPosition()
        {
            targets[11].localPosition = new Vector3(-182, -248, 10f);
            yield return new WaitForSecondsRealtime(5f);
            targets[11].localPosition = new Vector3(-190.31f, -248.57f, 10f);
            yield return new WaitForSecondsRealtime(5f);
            targets[11].localPosition = new Vector3(-191.06f, -244.57f, 10f);
            yield return new WaitForSecondsRealtime(5f);
            targets[11].localPosition = new Vector3(-181.07f, -244.57f, 10f);
            yield return new WaitForSecondsRealtime(5f);
            targets[11].localPosition = new Vector3(-181.22f, -251.66f, 10f);
            yield return new WaitForSecondsRealtime(5f);
            StartCoroutine(RandomPosition());
        }

        /// <summary>
        /// Updates the timer text, and checks if time is up
        /// </summary>
        void UpdateTime()
        {
            // Update the timer text
            if (timeText)
            {
                timeText.text = timeLeft.ToString("0");
            }

            // Time's up and prize not won
            if (timeLeft <= 0 && isPoundPrizeWon == false && bagsCollected == 0)
            {
                //hide the bag canvas
                bagVictoryCanvas.gameObject.SetActive(false);
                StartCoroutine(GameOver(0));
            }

            // Time's up and prize just won
            //if (timeLeft <= 0 && isPoundPrizeWon == true)
            //{
            //    //hide the bag canvas
            //    bagVictoryCanvas.gameObject.SetActive(true);
            //    StartCoroutine(PoundVictory());
            //}

            if (wrongItemCollected == 3)
            {
                //hide the bag canvas
                bagVictoryCanvas.gameObject.SetActive(false);
                StartCoroutine(GameOver(0));
            }

            if (timeLeft <= 0 && bagsCollected >= 1 && isPoundPrizeWon == false)
            {
                //hide the bag canvas
                bagVictoryCanvas.gameObject.SetActive(false);
                StartCoroutine(BagVictory());
            }

            if (bagsCollected >= 1 && isPoundPrizeWon == false && GATimeController.prizeOccurance > 2)
            {
                //hide the bag canvas
                bagVictoryCanvas.gameObject.SetActive(false);
                StartCoroutine(BagVictory());
            }

        }

        /// <summary>
        /// Shows a random batch of targets. Due to the random nature of selection, some targets may be selected more than once making the total number of targets to appear smaller.
        /// </summary>
        /// <param name="targetCount">The maximum number of target that will appear</param>
        void ShowTargets(int targetCount)
        {
            // Limit the number of tries when showing targets, so we don't get stuck in an infinite loop
            int maximumTries = targetCount * 5;
            try
            {
                // Show several targets that are within the game area
                while (targetCount > 0 && maximumTries > 0)
                {
                    maximumTries--;

                    // Choose a random target
                    int randomTarget = Mathf.FloorToInt(Random.Range(0, targets.Length));

                    targetCount--;

                    // Show a random targets from the list of moving targets
                    if (Random.value < quickChance) targets[randomTarget].SendMessage("ShowQuick", hideDelay);
                    else if (Random.value < helmetChance) targets[randomTarget].SendMessage("ShowHelmet", hideDelay);
                    else targets[randomTarget].SendMessage("ShowTarget", hideDelay, SendMessageOptions.DontRequireReceiver);
                }
                // Reset the streak when showing a batch of new targets
                streak = 1;
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
            }
        }

        /// <summary>
        /// Bag number is passed in and count is kept of how many
        /// </summary>
        /// <param name="bagNumber"></param>
        void BagHit(int bagNumber)
        {
            ++bagsCollected;
         //   Debug.Log(bagsCollected);
            ShowCollectedBag();
        }

        /// <summary>
        /// Once pound is collected we run the PoundVictory coroutine
        /// </summary>
        void PoundHit()
        {
            StartCoroutine("PoundVictory");
        }

        /// <summary>
        /// Wrong item clicked increment the count
        /// </summary>
        void WrongItemHit()
        {
            ++wrongItemCollected;
            Debug.Log(wrongItemCollected);
        }

        /// <summary>
        /// Play the animtion once a bag is collected and set it to active
        /// </summary>
        public void ShowCollectedBag()
        {
            switch (bagsCollected)
            {
                case 1:
                    bagUiAnimator.SetTrigger("PlayBag1Anim");
                    bagObjectTransforms[0].gameObject.SetActive(true);
                    break;

                case 2:
                    bagUiAnimator.SetTrigger("PlayBag2Anim");
                    bagObjectTransforms[1].gameObject.SetActive(true);
                    break;

                case 3:
                    bagUiAnimator.SetTrigger("PlayBag3Anim");
                    bagObjectTransforms[2].gameObject.SetActive(true);
                    break;

                case 4:
                    bagUiAnimator.SetTrigger("PlayBag4Anim");
                    bagObjectTransforms[3].gameObject.SetActive(true);
                    break;

                case 5:
                    bagUiAnimator.SetTrigger("PlayBag5Anim");
                    bagObjectTransforms[4].gameObject.SetActive(true);
                   // SetOldData();
                    CheckForPoundPrize(GATimeController.prizeOccurance);
                    break;
            }
        }

        /// <summary>
        /// Method to check for prize occurance and if not deactivates prize
        /// </summary>
        void CheckForPoundPrize(int prizeOccurance)
        {
            if (prizeOccurance <= 2 && sessionPrizeData.PoundCount > 0 && GATimeController.prizesWon <= 2 && GATimeController.prizesWon < 1)
            {
                SetOldData();

                ActivatePoundPrize();

                GATimeController.prizeOccurance++;
                sessionPrizeData.PoundCount--;

                SetNewData(sessionPrizeData.PoundCount);

                Debug.Log("Prize occurence is now: " + GATimeController.prizeOccurance);
            }
            else
            {
                DectivatePoundPrize();
            }
        }

        /// <summary>
        /// Activates pound prize transform object in scene
        /// </summary>
        void ActivatePoundPrize()
        {
            poundPrizeTransform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Deactivates pound prize transform object in scene
        /// </summary>
        void DectivatePoundPrize()
        {
            poundPrizeTransform.gameObject.SetActive(false);
            StartCoroutine("BagVictory");
        }

        /// <summary>
        /// Restore old data from dB
        /// </summary>
        void SetOldData()
        {
            var oldCount = PlayerPrefs.GetString("PoundCount");
            var oldTempCount = Convert.ToUInt32(oldCount);
            sessionPrizeData.PoundCount = oldTempCount;
        }

        /// <summary>
        /// Set the data for the prize count
        /// </summary>
        /// <param name="data"></param>
        void SetNewData(uint data)
        {
            var newTempCount = sessionPrizeData.PoundCount;
            sessionPrizeData.PoundCount = newTempCount;
            PlayerPrefs.SetString("PoundCount", sessionPrizeData.PoundCount.ToString());
        }

        /// <summary>
        /// Reset prize data to 10
        /// </summary>
        void ResetPrizeData()
        {
            PlayerPrefs.SetString("PoundCount", "10");
            sessionPrizeData.PoundCount = 10;
        }

        /// <summary>
        /// Give a bonus when the target is hit. The bonus is multiplied by the number of targets on screen
        /// </summary>
        /// <param name="hitSource">The target that was hit</param>
        void HitBonus(Transform hitSource)
        {
            // If we have a bonus effect
            if (bonusEffect)
            {
                // Create a new bonus effect at the hitSource position
                Transform newBonusEffect = Instantiate(bonusEffect, hitSource.position + Vector3.up, Quaternion.identity) as Transform;

                // Display the bonus value multiplied by a streak
                newBonusEffect.Find("Text").GetComponent<Text>().text = "Bag Collected!"; //+ (hitTargetBonus * streak).ToString();

                // Rotate the bonus text slightly
                newBonusEffect.eulerAngles = Vector3.forward * Random.Range(-10, 10);
            }
            // Increase the hit streak
            streak++;
            // Add the bonus to the score
            ChangeScore(hitTargetBonus * streak);
        }

        /// <summary>
        /// Create a Wrong Hit effect when the target is hit. The bonus is multiplied by the number of targets on screen
        /// </summary>
        /// <param name="hitSource">The target that was hit</param>
        void WrongHit(Transform hitSource)
        {
            // If we have a bonus effect
            if (wrongHitEffect)
            {
                // Create a new bonus effect at the hitSource position
                Transform newWrongHitEffect = Instantiate(wrongHitEffect, hitSource.position + Vector3.up, Quaternion.identity) as Transform;

                // Display the bonus value multiplied by a streak
                wrongHitEffect.Find("Text").GetComponent<Text>().text = "Ooops!";

                // Rotate the bonus text slightly
                wrongHitEffect.eulerAngles = Vector3.forward * Random.Range(-10, 10);
            }
        }

        /// <summary>
        /// Change the score
        /// </summary>
        /// <param name="changeValue">Change value</param>
        void ChangeScore(int changeValue)
        {
            score += changeValue;

            //Update the score
            UpdateScore();
        }

        /// <summary>
        /// Updates the score value and checks if we got to the next level
        /// </summary>
        void UpdateScore()
        {
            //Update the score text
            if (scoreText) scoreText.GetComponent<Text>().text = score.ToString();

            // If we reached the required number of points, level up!
            if (score >= levels[currentLevel].scoreToNextLevel)
            {
                if (currentLevel < levels.Length - 1) LevelUp();
                //else if (isEndless == false) StartCoroutine(Victory(0));
            }

            // Update the progress bar to show how far we are from the next level
            if (progressCanvas)
            {
                if (currentLevel == 0) progressCanvas.GetComponent<Image>().fillAmount = score * 1.0f / levels[currentLevel].scoreToNextLevel * 1.0f;
                else progressCanvas.GetComponent<Image>().fillAmount = (score - levels[currentLevel - 1].scoreToNextLevel) * 1.0f / (levels[currentLevel].scoreToNextLevel - levels[currentLevel - 1].scoreToNextLevel) * 1.0f;
            }

            if (isPoundPrizeWon)
            {
                StartCoroutine("PoundVictory");
            }
        }

        /// <summary>
        /// Set the score multiplier ( Get double score for hitting and destroying targets )
        /// </summary>
        void SetScoreMultiplier(int setValue)
        {
            // Set the score multiplier
            scoreMultiplier = setValue;
        }

        /// <summary>
        /// Levels up, and increases the difficulty of the game
        /// </summary>
        void LevelUp()
        {
            currentLevel++;

            // Update the level attributes
            UpdateLevel();

            //Run the level up effect, displaying a sound
            LevelUpEffect();
        }

        /// <summary>
        /// Updates the level and sets some values like maximum targets, throw angle, and level text
        /// </summary>
        void UpdateLevel()
        {
            // Display the current level we are on
            if (progressCanvas) progressCanvas.Find("Text").GetComponent<Text>().text = (currentLevel + 1).ToString();

            // Set the maximum number of targets
            maximumTargets = levels[currentLevel].maximumTargets;

            // Give time bonus for winning the level
            timeLeft += levels[currentLevel].timeBonus;

            // Update the timer
            UpdateTime();
        }

        /// <summary>
        /// Shows the effect associated with leveling up ( a sound and text bubble )
        /// </summary>
        void LevelUpEffect()
        {
            // Show the time bonus effect when we level up
            if (bonusEffect)
            {
                // Create a new bonus effect at the hitSource position
                Transform newBonusEffect = Instantiate(bonusEffect) as Transform;

                newBonusEffect.position = new Vector3(0, Camera.main.ScreenToWorldPoint(timeText.transform.position).y, 0);

                // Display the bonus value multiplied by a streak
                newBonusEffect.Find("Text").GetComponent<Text>().text = "EXTRA TIME!\n+" + levels[currentLevel].timeBonus.ToString();
            }

            //If there is a source and a sound, play it from the source
            if (soundSource && soundLevelUp)
            {
                soundSource.GetComponent<AudioSource>().pitch = 1;

                soundSource.GetComponent<AudioSource>().PlayOneShot(soundLevelUp);
            }
        }

        /// <summary>
        /// Pause the game, and shows the pause menu
        /// </summary>
        /// <param name="showMenu">If set to <c>true</c> show menu.</param>
        public void Pause(bool showMenu)
        {
            isPaused = true;

            //Set timescale to 0, preventing anything from moving
            Time.timeScale = 0;

            //Show the pause screen and hide the game screen
            if (showMenu == true)
            {
                if (pauseCanvas) pauseCanvas.gameObject.SetActive(true);
                if (gameCanvas) gameCanvas.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void Unpause()
        {
            isPaused = false;

            //Set timescale back to the current game speed
            Time.timeScale = 1;

            //Hide the pause screen and show the game screen
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(true);
        }

        /// <summary>
        /// Runs the game over event and shows the game over screen
        /// </summary>
        IEnumerator GameOver(float delay)
        {
            isGameOver = true;

            yield return new WaitForSeconds(delay);

            //Remove the pause and game screens
            if (pauseCanvas) pauseCanvas.gameObject.SetActive(false);
            if (gameCanvas) gameCanvas.gameObject.SetActive(false);

            //Show the game over screen
            if (gameOverCanvas)
            {
                //hide the pound prize
                poundPrizeTransform.gameObject.SetActive(false);

                //hide the bag canvas
                bagUiAnimator.gameObject.SetActive(false);

                //hide the game canvas  
                gameCanvas.gameObject.SetActive(false);

                //Show the game over screen
                gameOverCanvas.gameObject.SetActive(true);

                //activate background animated group
                animatedGroup.gameObject.SetActive(true);

                soundSource.GetComponent<AudioSource>().pitch = 1;

                soundSource.GetComponent<AudioSource>().PlayOneShot(soundGameOver);


                //Check if we got a high score
                if (score > highScore) 
                gameOverCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();
                {
                    highScore = score;

                    //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
                }
                try
                {
                    //Write the high sscore text
                //    gameOverCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

                } catch (MissingReferenceException ex)
                {
                    Debug.Log(ex);
                }
                //If there is a source and a sound, play it from the source
                      
            }
        }

        /// <summary>
        /// Runs the victory event and shows the victory screen
        /// </summary>
        IEnumerator Victory(float delay)
        {
            isGameOver = true;

            yield return new WaitForSeconds(delay);

            // update number of times 100 pnd has been won, as we want to only appear twice
            GATimeController.prizesWon++;

            //Remove the pause and game screens
            if (pauseCanvas) Destroy(pauseCanvas.gameObject);
            if (gameCanvas) Destroy(gameCanvas.gameObject);

            //Show the game over screen
            if (poundVictoryCanvas)
            {
                //hide the pound prize
                poundPrizeTransform.gameObject.SetActive(false);

                //hide the game canvas  
                gameCanvas.gameObject.SetActive(false);

                //Show the game over screen
                poundVictoryCanvas.gameObject.SetActive(true);

                //Write the score text
                poundVictoryCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();

                //Check if we got a high score
                if (score > highScore)
                {
                    highScore = score;

                    //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
                }

                //Write the high sscore text
                poundVictoryCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

                //If there is a source and a sound, play it from the source
                if (soundSource && soundVictory)
                {
                    soundSource.GetComponent<AudioSource>().pitch = 1;

                    soundSource.GetComponent<AudioSource>().PlayOneShot(soundVictory);

                    Debug.Log(soundVictory.ToString());
                }
            }
        }

        /// <summary>
        /// Runs the pound victory event and shows the victory screen
        /// </summary>
        IEnumerator PoundVictory()
        {
            //Remove the pause and game screens
            // if (pauseCanvas) Destroy(pauseCanvas.gameObject);
            //if (gameCanvas) Destroy(gameCanvas.gameObject);
            // if(gameOverCanvas) Destroy(gameOverCanvas.gameObject);
           
            // update number of times 100 pnd has been won, as we want to only appear twice
            GATimeController.prizesWon++;

            isPoundPrizeWon = true;

            //Show the game over screen
            if (poundVictoryCanvas)
            {
                //hide the pound prize
                poundPrizeTransform.gameObject.SetActive(false);

                //hide the game canvas  
                gameCanvas.gameObject.SetActive(false);

                //hide the bag canvas
                bagUiAnimator.gameObject.SetActive(false);

                //Show the game over screen
                poundVictoryCanvas.gameObject.SetActive(true);

                //activate background animated group
                animatedGroup.gameObject.SetActive(true);
                
                soundSource.GetComponent<AudioSource>().pitch = 1;
            
                soundSource.GetComponent<AudioSource>().PlayOneShot(soundVictory);

                Debug.Log(soundVictory.ToString());

                //Write the score text
                poundVictoryCanvas.Find("Base/TextScore").GetComponent<Text>().text = "SCORE " + score.ToString();

                //Check if we got a high score
                if (score > highScore)
                {
                    highScore = score;

                    //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
                }

                //Write the high sscore text
                poundVictoryCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();
            }
            yield return new WaitForSeconds(2f);
        }

        /// <summary>
        /// Runs the pound victory event and shows the victory screen
        /// </summary>
        IEnumerator BagVictory()
        {
            //Remove the pause and game screens
            // if (pauseCanvas) Destroy(pauseCanvas.gameObject);
            //if (gameCanvas) Destroy(gameCanvas.gameObject);
            // if(gameOverCanvas) Destroy(gameOverCanvas.gameObject);

            isPoundPrizeWon = false;

            //Show the game over screen
            if (bagVictoryCanvas)
            {
                //hide the pound prize
                poundPrizeTransform.gameObject.SetActive(false);

                //hide the game canvas  
                gameCanvas.gameObject.SetActive(false);

                //hide the bag canvas
               // bagUiAnimator.gameObject.SetActive(false);

                //Show the game over screen
                bagVictoryCanvas.gameObject.SetActive(true);

                //activate background animated group
                animatedGroup.gameObject.SetActive(true);

                //Write the score text
                if (bagsCollected > 1)
                {
                    bagVictoryCanvas.Find("Base/TextScore").GetComponent<Text>().text = "You collected " + bagsCollected.ToString() + " bags";
                }

                if (bagsCollected  <= 1)
                {
                    bagVictoryCanvas.Find("Base/TextScore").GetComponent<Text>().text = "You collected " + bagsCollected.ToString() + " bag";
                }

                //Check if we got a high score
                if (score > highScore)
                {
                    highScore = score;

                    //Register the new high score
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
                    PlayerPrefs.SetInt(SceneManager.GetActiveScene().name + "HighScore", score);
#else
					PlayerPrefs.SetInt(Application.loadedLevelName + "HighScore", score);
#endif
                }

                //Write the high sscore text
                bagVictoryCanvas.Find("Base/TextHighScore").GetComponent<Text>().text = "HIGH SCORE " + highScore.ToString();

                //If there is a source and a sound, play it from the source
                if (soundSource && soundBagVictory)
                {
                    soundSource.GetComponent<AudioSource>().pitch = 1;

                    soundSource.GetComponent<AudioSource>().PlayOneShot(soundBagVictory);
                }
            }
            yield return new WaitForSeconds(2f);
        }

        /// <summary>
        /// Restart the current level
        /// </summary>
        void Restart()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
#else
			Application.LoadLevel(Application.loadedLevelName);
#endif
        }

        /// <summary>
        /// Restart the current level
        /// </summary>
        void MainMenu()
        {
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
            SceneManager.LoadScene(mainMenuLevelName);
#else
			Application.LoadLevel(mainMenuLevelName);
#endif
        }
    }
}