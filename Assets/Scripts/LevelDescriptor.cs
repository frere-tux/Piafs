using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Piafs
{
    public class LevelDescriptor : MonoBehaviour
    {
        public Modulator outputModulator;
        public float songIntervalMin, songIntervalMax;
        public float answerIntervalMin, answerIntervalMax;
        public SequenceElement[] birdSong;
        [Header("Level design helpers")]
        public GameObject librarySlotPrefab;
		public GameObject plugSlotPrefab;
		public GameObject sliderPrefab;
		public GameObject interfaceHolder;

        [HideInInspector]
        public LevelManager levelManager;
        [HideInInspector]
        public Mixer birdMixer, playerMixer;
        [HideInInspector]
        public AnimBird birdAnim;
        [HideInInspector]
        public AnimBirdCarton cartonAnim;

        [Header("Debug")]
        public bool debugLevel;
        public bool doNotGenerateBird;
        

        public System.Action onLevelSolve;

        private List<Trigger> lastPlayerTriggers = new List<Trigger>();
        [HideInInspector]
        public List<LevelControls> levelControls;
        [HideInInspector]
        public List<Trigger> levelTriggers;
        [HideInInspector]
        public List<Slot> levelSlots;
        private bool originalVersion = true;
        private float nextSongTime = -1f;
        private bool playingSong = false;
        private LevelDescriptor sibling;
        private Coroutine playSong;


        [System.Serializable]
        public struct SequenceElement
        {
            public float duration;
            public float randomization;
            public float pause;
            public float pauseRandomization;
            public Trigger pattern;
        }

        void OnValidate()
        {
            
            if (transform.parent == null)
            {
                levelManager = null;
            }
            else
            {
                levelManager = transform.parent.gameObject.GetComponent<LevelManager>();
            }
            if (levelManager == null) throw new System.Exception(string.Format("You have to put level {0} in a level manager !", name));
            if (outputModulator == null) throw new System.Exception("No output is assigned for this level !");
            birdAnim = levelManager.birdAnim;
            cartonAnim = levelManager.cartonAnim;
            birdMixer = levelManager.birdMixer;
            playerMixer = levelManager.playerMixer;
        }

        void Start()
        {
            if(debugLevel)
            {
                Init(birdAnim, cartonAnim, birdMixer, playerMixer);
            }
        }

        public void Init(AnimBird _birdAnim, AnimBirdCarton _cartonAnim, Mixer _birdMixer, Mixer _playerMixer)
        {
            birdAnim = _birdAnim;
            cartonAnim = _cartonAnim;
            birdMixer = _birdMixer;
            playerMixer = _playerMixer;

            if(originalVersion)
            {
                playerMixer.modulators.Clear();
                OutputFlag flag = GetComponentInChildren<OutputFlag>();

                cartonAnim.modulator = outputModulator;
                playerMixer.modulators.Add(outputModulator);

                levelControls = GetComponentsInChildren<LevelControls>().ToList();
                if(!doNotGenerateBird) InstantiateBirdVersion();
                foreach(SequenceElement seq in birdSong)
                {
                    seq.pattern.onEndTrigger += delegate (Trigger trigger, bool longEnough) { PlayerFinishesPattern(trigger, longEnough); };
                    seq.pattern.onStartTrigger += delegate { PlayerPlaysPattern(); };
                }
            }
            else
            {
                playSong = StartCoroutine(PlaySong(1f));
            }
        }

        public void RegisterLevelElements()
        {
            levelControls = GetComponentsInChildren<LevelControls>().ToList();
            levelTriggers = GetComponentsInChildren<Trigger>().ToList();
            levelSlots.Clear();
            GetComponentsInChildren<Slot>().ToList().ForEach(a => { if (a.valid) levelSlots.Add(a); });
        }

        public void Destroy()
        {
			Debug.Log("Destroy level");
            Destroy(sibling.gameObject);
            Destroy(gameObject);
        }

        IEnumerator PlaySong(float delay)
        {
            nextSongTime = -1f;
            yield return new WaitForSeconds(delay);
            foreach(SequenceElement seq in birdSong)
            {
                seq.pattern.envelopes.ForEach(e=>e.Trigger());
                yield return new WaitForSeconds(seq.duration + Random.Range(-seq.randomization, seq.randomization));
                seq.pattern.envelopes.ForEach(e => e.Untrigger());
                yield return new WaitForSeconds(seq.pause + Random.Range(-seq.pauseRandomization, seq.pauseRandomization));
            }
            RescheduleSong(songIntervalMin,songIntervalMax);
        }

        void PlayerPlaysPattern()
        {
            if(!doNotGenerateBird)sibling.StopPattern();
            cartonAnim.move = true;
        }

        void StopPattern()
        {
            nextSongTime = -1f;
            foreach (SequenceElement seq in birdSong)
            {
                seq.pattern.envelopes.ForEach(e=>e.Untrigger());
            }
            if(playSong != null) StopCoroutine(playSong);
            
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Space) && debugLevel)
            {
                Debug.Log("Victory : " + CheckVictory());
            }

            if(Time.time > nextSongTime && nextSongTime > 0f)
            {
                playSong = StartCoroutine(PlaySong(0f));
            }
        }

        private void PlayerFinishesPattern(Trigger trigger, bool longEnough)
        {
            cartonAnim.move = false;
            lastPlayerTriggers.Add(trigger);
            while(lastPlayerTriggers.Count > birdSong.Length)
            {
                lastPlayerTriggers.RemoveAt(0);
            }
            if(!doNotGenerateBird)sibling.RescheduleSong(answerIntervalMin,answerIntervalMax);
            bool result = CheckVictory();
            Debug.Log("Victory ? " + result);
            if (result && onLevelSolve != null) onLevelSolve();
            return;
        }

        private bool CheckVictory()
        {
            bool levelControlsOk = levelControls.TrueForAll(control => 
            {
                bool b = control.IsSolved();
				if (!b)
				{
					Debug.Log(control);
					b = control.IsSolved();
				}
                return b;
            });
            bool triggersOk = true;
            if(lastPlayerTriggers.Count == birdSong.Length)
            {
                for (int i = 0; i < lastPlayerTriggers.Count; i++)
                {
                    if (lastPlayerTriggers[i] != birdSong[i].pattern)
                    {
                        triggersOk = false;
                        break;
                    }
                }
            }
            else
            {
                triggersOk = false;
            }
            return levelControlsOk && triggersOk;
        }

        private void RescheduleSong(float min, float max)
        {
            nextSongTime = Time.time + Random.Range(min, max);
        }

        private void InstantiateBirdVersion()
        {
            GameObject birdCopy = Instantiate(gameObject,gameObject.transform.position + Vector3.right * 50f, Quaternion.identity);
            birdCopy.transform.SetParent(transform.parent, true);
            birdCopy.name = "Bird Synth";
            sibling = birdCopy.GetComponent<LevelDescriptor>();
            sibling.originalVersion = false;
            sibling.sibling = this;
            Brick[] allBricks = birdCopy.GetComponentsInChildren<Brick>();
            foreach(Brick b in allBricks)
            {
                b.slot.GrabOscillator();
            }
            foreach(Brick b in allBricks)
            {
                if (b.rightSlot != null)
                {
                    b.rightSlot.GrabOscillator();
                    b.Drop(b.rightSlot);
                }
            }
            Slider[] allSliders = birdCopy.GetComponentsInChildren<Slider>();
            foreach (Slider s in allSliders)
            {
                s.SolveValue();
            }

            birdMixer.modulators.Add(sibling.outputModulator);
            birdAnim.modulator = sibling.outputModulator;

            sibling.Init(birdAnim, cartonAnim, birdMixer, playerMixer);
        }

        private List<Modulator> GetUsedModulators()
        {
            List<Modulator> result = new List<Modulator>();
            outputModulator.GetDependenciesRecursive(result);
            Slot[] slots = GetComponentsInChildren<Slot>();
            Brick[] bricks = GetComponentsInChildren<Brick>();
            foreach(Slot s in slots)
            {
                s.GetDependencies(result);
            }
            foreach (Brick b in bricks)
            {
                b.GetDependencies(result);
            }
            return result;
        }

        [ContextMenu("Tag all unused")]
        private void DeleteUnusedModulators()
        {
            List<Modulator> used = GetUsedModulators();
            List<Modulator> all = GetComponentsInChildren<Modulator>().ToList();
            List<Modulator> unused = all.Except(used).ToList();
            unused.ForEach(a => {
                Debug.Log("Unused : " + a.name);
                if (!a.name.StartsWith("UNUSED_")) a.name = "UNUSED_" + a.name;
            });
            if (unused.Count == 0) Debug.Log("No unused modulators !");
        }

        
    }
}

