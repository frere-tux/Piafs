using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class LevelManager : MonoBehaviour
    {
        public LevelDescriptor[] levelPrefabs;
        public int currentLevel = 0;

        [SerializeField]
        private AnimBird birdAnim;
        public Mixer birdMixer, playerMixer;

        void Start()
        {
            StartLevel(currentLevel);
        }

        private void StartLevel(int _currentLevel)
        {
            LevelDescriptor lvl = Instantiate(levelPrefabs[_currentLevel].gameObject).GetComponent<LevelDescriptor>();
            lvl.onLevelSolve += delegate () { SolveLevel(lvl); };
            lvl.Init(birdAnim, birdMixer,playerMixer);
        }

        private void SolveLevel(LevelDescriptor _level)
        {
            _level.Destroy();
            currentLevel++;
            if(levelPrefabs.Length > currentLevel)
            {
                StartLevel(currentLevel);
            }
            else
            {
                FinishAllLevels();
            }
        }

        private void FinishAllLevels()
        {
            Debug.Log("VICTORY OF ALL LEVELS WOW");
            currentLevel--;
            StartLevel(currentLevel);
        }
    }
}


