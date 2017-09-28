using UnityEngine;

namespace Assets.Scripts.GoAhead
{
    public class GATarget : MonoBehaviour
    {
        //referemce to the Game Controller, which is taken by the first time this script runs, and is remembered across all other scripts of this type
        public GameObject gameController;

        [Tooltip("The helmet object of this target, assigned from inside the target itself")]
        public GameObject helmet;

        [Tooltip("The broken helmet that appears when the helmet breaks. This is assigned from the project pane")]
        public Transform brokenHelmet;

        [Tooltip("The health of the target when it's wearing a helmet")]
        public int helmetHealth = 2;

        // The health of the target when it's not wearing a helmet
        internal int health = 1;

        [Tooltip("The tag of the object that can hit this target")]
        public string targetTag = "Player";

        // Is the target dead?
        internal bool isDead = false;

        // How long to wait before showing the target
        internal float showTime = 0;

        // How long to wait before hiding the target, after it has been revealed
        internal float hideDelay = 0;

        // The animated part of the target. By default this is taken from this object
        internal Animator targetAnimator;

        [Tooltip("The animation name when showing a target")]
        public string animationShow = "TargetShow";

        [Tooltip("The animation name when hiding a target")]
        public string animationHide = "TargetHide";

        [Tooltip("A list of animations when the target dies. The animation is chosen randomly from the list")]
        public string[] animationDie = {"TargetSmack", "TargetWhack", "TargetThud"};

        public AudioClip[] audioClips;

        //Tags for items that can be hit by player
        private string poundTag = "PoundPrize";

        private string bagTag = "Bag";

        private string wrongItemTag = "WrongItem";

        // Use this for initialization
        void Start()
        {
            // Hold the gamcontroller object in a variable for quicker access
            if (gameController == null) gameController = GameObject.FindGameObjectWithTag("GameController");

            // The animator of the target. This holds all the animations and the transitions between them
            targetAnimator = GetComponent<Animator>();
        }

        /// <summary>
        /// Update this instance.
        /// </summary>
        void Update()
        {
            // Count down the time until the target is hidden
            if (isDead == false && hideDelay > 0)
            {
                hideDelay -= Time.deltaTime;

                // If enough time passes, hide the target
                if (hideDelay <= 0) HideTarget();
            }
        }

        /// <summary>
        /// Raises the trigger enter2d event. Works only with 2D physics.
        /// </summary>
        /// <param name="other"> The other collider that this object touches</param>
        void OnTriggerEnter2D(Collider2D other)
        {
            // Check if we hit the correct target
            if (isDead == false && other.tag == targetTag)
            {
                // Give hit bonus for this target
                //gameController.SendMessage("HitBonus", other.transform);

                // Change the health of the target
              //  ChangeHealth(-1);
            }

            if (this.gameObject.tag == poundTag)
            {
                //Debug.Log(gameObject.name.ToString() + "100 prize won!");
                gameController.SendMessage("PoundHit", other.transform);
                HideTarget();
                this.gameObject.SetActive(false);
            }

            if (this.gameObject.tag == bagTag)
            {
                //Debug.Log(gameObject.name.ToString() + "bag collected!");
                GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(audioClips[0]);
                gameController.SendMessage("BagHit", +1);
                gameController.SendMessage("HitBonus", other.transform);
                HideTarget();
                this.gameObject.SetActive(false);
            }

            if (this.gameObject.tag == wrongItemTag)
            {
                //Debug.Log(gameObject.name.ToString() + "wrong item clicked");
                GameObject.FindGameObjectWithTag("Sound").GetComponent<AudioSource>().PlayOneShot(audioClips[0]);
                gameController.SendMessage("WrongHit", other.transform);
                gameController.SendMessage("WrongItemHit");
            }
        }

        /// <summary>
        /// Changes the health of the target, and checks if it should die
        /// </summary>
        /// <param name="changeValue"></param>
        public void ChangeHealth(int changeValue)
        {
            // Chnage health value
            health += changeValue;

            if (health > 0)
            {
                // Animated the hit effect
                targetAnimator.Play("TargetHit",0, 0f);
            }
            else
            {
                // Health reached 0, so the target is dead
                Die();
            }
        }

        /// <summary>
        /// Kills the object and gives it a random animation from a list of death animations
        /// </summary>
        public void Die()
        {
            // The target is now dead. It can't move.
            isDead = true;

            // If there is a helment object, deactivate it and create a helmet break effect
            if (helmet && helmet.activeSelf == true)
            {
                // Create the helmet break effect
                if (brokenHelmet) Instantiate(brokenHelmet, helmet.transform.position, helmet.transform.rotation);

                // Deactivate the helmet object
                helmet.SetActive(false);
            }

            // Choose one of the death animations randomly
            if (animationDie.Length > 0)
                targetAnimator.Play(animationDie[Mathf.FloorToInt(Random.Range(0, animationDie.Length))]);
        }

        /// <summary>
        /// Hides the target, animating it and then sets it to hidden
        /// </summary>
        void HideTarget()
        {
            // Play the hiding animation
            GetComponent<Animator>().Play(animationHide);
        }

        /// <summary>
        /// Shows the regular target
        /// </summary>
        /// <returns>The target.</returns>
        public void ShowTarget(float showDuration)
        {
            // The target is not dead anymore, so it can appear from the hole
            isDead = false;
            // If the target has a helmet, deactivate it
            if (helmet) helmet.SetActive(false);
            // Set the health of the target to 1 hit
            health = 1;
            // Play the show animation
            GetComponent<Animator>().Play(animationShow);
            // Set how long to wait before hiding the target again
            hideDelay = showDuration;
        }

        /// <summary>
        /// Shows the target with a helmet
        /// </summary>
        /// <returns>The target.</returns>
        public void ShowHelmet(float showDuration)
        {
            // The target is not dead anymore, so it can appear from the hole
            isDead = false;

            // If the target has a helmet, deactivate it
            if (helmet) helmet.SetActive(true);

            // Set the health of the target to the helmet health
            health = helmetHealth;

            // Play the show animation
            GetComponent<Animator>().Play(animationShow);

            // Set how long to wait before hiding the target again
            hideDelay = showDuration;
        }

        /// <summary>
        /// Shows the quick target
        /// </summary>
        /// <returns>The target.</returns>
        public void ShowQuick(float showDuration)
        {
            // The target is not dead anymore, so it can appear from the hole
            isDead = false;

            // If the target has a helmet, deactivate it
            if (helmet) helmet.SetActive(false);

            // Set the health of the target to 1 hit
            health = 1;

            // Play the show animation
            GetComponent<Animator>().Play("TargetQuick");

            // Set how long to wait before hiding the target again
            hideDelay = 0;
        }
    }
}