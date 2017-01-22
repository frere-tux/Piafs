using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Piafs
{
    public class LevelDescriptor : MonoBehaviour
    {
        public Mixer birdMixer;
        [SerializeField]
        private AnimBird birdAnim;
        public float songIntervalMin, songIntervalMax;
        public bool debugLevel;
        [SerializeField]
        private SequenceElement[] birdSong;

        private List<Trigger> lastPlayerTriggers = new List<Trigger>();
        private List<LevelControls> levelControls;
        private bool originalVersion = true;
        private float nextSongTime = -1f;
        private bool playingSong = false;
        private LevelDescriptor sibling;
        private Coroutine playSong;

        [System.Serializable]
        private struct SequenceElement
        {
            public float duration;
            public float randomization;
            public float pause;
            public float pauseRandomization;
            public Trigger pattern;
        }

        void Start()
        {
            if(originalVersion)
            {
                levelControls = GetComponentsInChildren<LevelControls>().ToList();
                InstantiateBirdVersion();
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

        IEnumerator PlaySong(float delay)
        {
            nextSongTime = -1f;
            yield return new WaitForSeconds(delay);
            foreach(SequenceElement seq in birdSong)
            {
                seq.pattern.envelope.Trigger();
                yield return new WaitForSeconds(seq.duration + Random.Range(-seq.randomization, seq.randomization));
                seq.pattern.envelope.Untrigger();
                yield return new WaitForSeconds(seq.pause + Random.Range(-seq.pauseRandomization, seq.pauseRandomization));
            }
            RescheduleSong();
        }

        void PlayerPlaysPattern()
        {
            sibling.StopPattern();
        }

        void StopPattern()
        {
            nextSongTime = -1f;
            foreach (SequenceElement seq in birdSong)
            {
                seq.pattern.envelope.Untrigger();
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

        public bool PlayerFinishesPattern(Trigger trigger, bool longEnough)
        {
            lastPlayerTriggers.Add(trigger);
            while(lastPlayerTriggers.Count > birdSong.Length)
            {
                lastPlayerTriggers.RemoveAt(0);
            }
            sibling.RescheduleSong();
            bool result = CheckVictory();
            Debug.Log("Victory ? " + result);
            return result;
        }

        public bool CheckVictory()
        {
            bool levelControlsOk = levelControls.TrueForAll(control => control.IsSolved());
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

        public void RescheduleSong()
        {
            nextSongTime = Time.time + Random.Range(songIntervalMin, songIntervalMax);
        }

        public void InstantiateBirdVersion()
        {
            GameObject birdCopy = Instantiate(gameObject,gameObject.transform.position + Vector3.right * 50f, Quaternion.identity);
            birdCopy.name = "Bird Model";
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
            OutputFlag outputFlag = birdCopy.GetComponentInChildren<OutputFlag>();
            if(outputFlag != null)
            {
                birdMixer.modulators.Add(outputFlag.gameObject.GetComponent<Modulator>());
                birdAnim.modulator = outputFlag.gameObject.GetComponent<Modulator>();
            }
            else
            {
                Debug.LogError("No output flag on level");
            }
        }

        public List<Modulator> GetUsedModulators()
        {
            OutputFlag flag = GetComponentInChildren<OutputFlag>();
            if(!flag)
            {
                Debug.LogError("no output flag");
                return null;
            }

            List<Modulator> result = new List<Modulator>();
            flag.GetComponent<Modulator>().GetDependenciesRecursive(result);
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
        public void DeleteUnusedModulators()
        {
            List<Modulator> used = GetUsedModulators();
            List<Modulator> all = GetComponentsInChildren<Modulator>().ToList();
            List<Modulator> unused = all.Except(used).ToList();
            unused.ForEach(a => {
                Debug.Log("Unused : " + a.name);
                if (!a.name.StartsWith("UNUSED_")) a.name = "UNUSED_" + a.name;
            });
        }
    }
}

