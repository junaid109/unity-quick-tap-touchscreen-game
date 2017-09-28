using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.GoAhead.Data;
using Timer = System.Timers.Timer;
using System.Timers;

namespace Assets.Scripts.GoAhead
{
    /// <summary>
    /// Author: Junaid Malik
    /// Description: control interval at which prize appears as we want 
    /// 30 min delay between prize
    /// </summary>
    ///
    [Serializable]
    public class GATimeController : MonoBehaviour
    {
        //prize occurance
        public static int prizeOccurance;

        //prizes won
        public static int prizesWon;
        
        // reference to prize data object
        public PrizeData prizeData;

        // static instance reference to Time Controller
        public static GATimeController _instance;

        // custom event for a new day
        public event EventHandler NewDay;

        // static game controller reference
        private static GameObject gameController;

        private static Timer timer; // 1 sec = 1000, 30 mins = 1800000, 35 mins = 2100000 

        // duration of time until method to be called
		private double duration = 2100000;

        // static reference for todays date
        private static DateTime currentDate = DateTime.Today;
       
        // live dates for the activity
        private DateTime liveDate1 = new DateTime(2017, 07, 12);
        private DateTime liveDate2 = new DateTime(2017, 07, 13);
        private DateTime liveDate3 = new DateTime(2017, 07, 14);
        private DateTime liveDate4 = new DateTime(2017, 07, 15);
        private DateTime liveDate5 = new DateTime(2017, 07, 16);
        private DateTime liveDate6 = new DateTime(2017, 07, 17);
        private DateTime liveDate7 = new DateTime(2017, 07, 18);
        private DateTime liveDate8 = new DateTime(2017, 07, 19);
        private DateTime liveDate9 = new DateTime(2017, 07, 20);
        private DateTime liveDate10 = new DateTime(2017, 07, 21);
        private DateTime liveDate11 = new DateTime(2017, 07, 22);
        private DateTime liveDate12 = new DateTime(2017, 07, 23);
        private DateTime liveDate13 = new DateTime(2017, 07, 24);
        private DateTime liveDate14 = new DateTime(2017, 07, 25);
        private DateTime liveDate15 = new DateTime(2017, 07, 26);
        private DateTime liveDate16 = new DateTime(2017, 07, 27);
        private DateTime liveDate17 = new DateTime(2017, 07, 28);
        private DateTime liveDate18 = new DateTime(2017, 07, 29);
        private DateTime liveDate19 = new DateTime(2017, 07, 20);
        private DateTime liveDate20 = new DateTime(2017, 07, 21);

        public static GATimeController Instance
        {
            get
            {
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }
        }

        [STAThread]
        void Start()
        {
            prizeOccurance = 0;
            timer = new Timer(duration);
            // Hold the gamcontroller object in a variable for quicker access
            if (gameController == null) gameController = GameObject.FindGameObjectWithTag("GameController");

            timer.AutoReset = true;
            timer.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);
            timer.Start();

            CheckForNewDate();

            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Elapsed time for reset trigger
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ResetData();
            Debug.Log("Timer has reset prize occurence to 0");
        }

        /// <summary>
        /// Static function to reset data of prize occurance
        /// </summary>
        private static void ResetData()
        {
            prizeOccurance = 0;
            prizesWon = 0;
        }

        /// <summary>
        /// Method checks for new date against activity dates 
        /// and resets or sets the data
        /// </summary>
        void CheckForNewDate()
        {
            if (currentDate == DateTime.Today && PlayerPrefs.GetInt("FirstDateCheck0", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck0", 0);
            }
            if (currentDate == liveDate1 && PlayerPrefs.GetInt("FirstDateCheck1", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck1", 0);
            }
            if (currentDate == liveDate2 && PlayerPrefs.GetInt("FirstDateCheck2", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck2", 0);
            }
            if (currentDate == liveDate3 && PlayerPrefs.GetInt("FirstDateCheck3", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck3", 0);
            }
            if (currentDate == liveDate4 && PlayerPrefs.GetInt("FirstDateCheck4", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck4", 0);
            }
            if (currentDate == liveDate5 && PlayerPrefs.GetInt("FirstDateCheck5", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck5", 0);
            }
            if (currentDate == liveDate6 && PlayerPrefs.GetInt("FirstDateCheck6", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck6", 0);
            }
            if (currentDate == liveDate7 && PlayerPrefs.GetInt("FirstDateCheck7", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck7", 0);
            }
            if (currentDate == liveDate8 && PlayerPrefs.GetInt("FirstDateCheck8", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck8", 0);
            }
            if (currentDate == liveDate9 && PlayerPrefs.GetInt("FirstDateCheck9", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck9", 0);
            }
            if (currentDate == liveDate10 && PlayerPrefs.GetInt("FirstDateCheck10", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck10", 0);
            }
            if (currentDate == liveDate11 && PlayerPrefs.GetInt("FirstDateCheck11", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck11", 0);
            }
            if (currentDate == liveDate12 && PlayerPrefs.GetInt("FirstDateCheck12", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck12", 0);
            }
            if (currentDate == liveDate13 && PlayerPrefs.GetInt("FirstDateCheck13", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck13", 0);
            }
            if (currentDate == liveDate14 && PlayerPrefs.GetInt("FirstDateCheck14", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck14", 0);
            }
            if (currentDate == liveDate15 && PlayerPrefs.GetInt("FirstDateCheck15", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck15", 0);
            }
            if (currentDate == liveDate16 && PlayerPrefs.GetInt("FirstDateCheck16", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck16", 0);
            }
            if (currentDate == liveDate17 && PlayerPrefs.GetInt("FirstDateCheck17", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck17", 0);
            }
            if (currentDate == liveDate18 && PlayerPrefs.GetInt("FirstDateCheck18", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck18", 0);
            }
            if (currentDate == liveDate19 && PlayerPrefs.GetInt("FirstDateCheck19", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck19", 0);
            }
            if (currentDate == liveDate20 && PlayerPrefs.GetInt("FirstDateCheck20", 1) == 1)
            {
                gameController.SendMessage("ResetPrizeData");
                PlayerPrefs.SetInt("FirstDateCheck20", 0);
            }
        }
    }
}