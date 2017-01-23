using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Piafs
{
    public class LevelManager : MonoBehaviour
    {
        public LevelDescriptor[] levels;
        public int currentLevel = 0;

        [SerializeField]
        private AnimBird birdAnim;
        [SerializeField]
        private AnimBirdCarton cartonAnim;
        public Mixer birdMixer, playerMixer;
        public GameObject interfaceParent;
        public Camera camera;
        public float moveCameraDistance;
        public float moveCameraSmothing;
        public bool move = false;

        private Vector3 baseCameraPosition;
        private Vector3 moveCameraPosition;

        void Start()
        {
            baseCameraPosition = camera.transform.position;
            moveCameraPosition = baseCameraPosition;
            moveCameraPosition.y -= moveCameraDistance;
            int childCount = transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            StartLevel(currentLevel);
        }

        void Update()
        {
            Vector3 position = camera.transform.position;
            if (move)
            {
                position = Toolkit.Damp(position, moveCameraPosition, moveCameraSmothing, Time.deltaTime);
                camera.transform.position = position;
            }
            else
            {
                position = Toolkit.Damp(position, baseCameraPosition, moveCameraSmothing, Time.deltaTime);
                camera.transform.position = position;
            }
        }

        

        private void StartLevel(int _currentLevel)
        {
            LevelDescriptor lvl = Instantiate(levels[_currentLevel].gameObject,interfaceParent.transform,false).GetComponent<LevelDescriptor>();
            lvl.gameObject.name = levels[_currentLevel].name + " - Playing Now";
            lvl.gameObject.SetActive(true);
            lvl.debugLevel = false;
            lvl.doNotGenerateBird = false;
            lvl.onLevelSolve += delegate () { SolveLevel(lvl); };
            lvl.Init(birdAnim, cartonAnim, birdMixer,playerMixer);
            move = true;
        }

        private void SolveLevel(LevelDescriptor _level)
        {
            _level.Destroy();
            currentLevel++;
            if(levels.Length > currentLevel)
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


