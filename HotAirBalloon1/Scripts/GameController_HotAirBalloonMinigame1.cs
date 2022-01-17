using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController_HotAirBalloonMinigame1 : MonoBehaviour
{
    public static GameController_HotAirBalloonMinigame1 instance;
    public Camera mainCamera;
    public float f2;
    public bool isLose, isWin, isBegin, isTutorial;
    public HotAirBalloon_HotAirBalloonMinigame1 hotAirBalloon;
    public Text txtTime;
    public int time = 60;
    public Vector2 tmpPos_HotAirBalloon;
    public Vector2 mouseCurrentPos;
    public bool isHoldCar;
    public List<PathCreator> listPath = new List<PathCreator>();
    public List<Transform> listPosEasy = new List<Transform>();
    public List<int> listCheckSameEasy = new List<int>();
    public List<int> listCheckSameHard = new List<int>();
    public Canvas canvas;
    public Coroutine spawnCoroutine;
    public Enemy_HotAirBalloonMinigame1 enemyPrefab;
    public GameObject tutorial;
    public Enemy_HotAirBalloonMinigame1 enemyTutorial;
    public Transform[] arrayWaypointTutorial;
    public Vector3[] arrayVector3Tutorial;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(instance);

        isLose = false;
        isWin = false;
        isTutorial = true;
        isBegin = false;
        arrayVector3Tutorial = new Vector3[8];

    }
    private void Start()
    {
        SetSizeCamera();
        isHoldCar = false;
        mainCamera.orthographicSize *= 0.7f;
        tutorial.SetActive(false);
        SetUpPathTutorial();
        Intro();
        txtTime.text = time.ToString();
        ResetListCheckSameEasy();
        ResetListCheckSameHard();
    }

    void SetSizeCamera()
    {
        float f1;
        f1 = 16f / 9;
        f2 = Screen.width * 1.0f / Screen.height;

        mainCamera.orthographicSize *= f1 / f2;
    }

    void SetUpPathTutorial()
    {
        for (int i = 0; i < arrayWaypointTutorial.Length; i++)
        {
            arrayVector3Tutorial[i] = arrayWaypointTutorial[i].position;
        }
    }

    void Intro()
    {
        canvas.gameObject.SetActive(false);
        mainCamera.DOOrthoSize(mainCamera.orthographicSize * 1f / 0.7f, 2).OnComplete(() =>
        {
            canvas.gameObject.SetActive(true);
            enemyTutorial = Instantiate(enemyPrefab, listPosEasy[9].transform.position, Quaternion.identity);
            Invoke(nameof(ShowTutorial), 1);
        });
    }

    void ShowTutorial()
    {
        isBegin = true;
        if (enemyTutorial != null)
        {
            tutorial.transform.position = arrayVector3Tutorial[0];
            tutorial.SetActive(true);
            float distance = 0;
            for (int i = 1; i < arrayVector3Tutorial.Length; i++)
            {
                distance += (arrayVector3Tutorial[i - 1] - arrayVector3Tutorial[i]).magnitude;
            }
            tutorial.transform.DOPath(arrayVector3Tutorial, distance / 4, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1);
            enemyTutorial.transform.DOPause();
        }
    }

    void SpawnEnemyEasy()
    {
        int ran = Random.Range(0, listCheckSameEasy.Count);
        Enemy_HotAirBalloonMinigame1 tmpEnemy = Instantiate(enemyPrefab, listPosEasy[listCheckSameEasy[ran]].transform.position, Quaternion.identity);
        listCheckSameEasy.RemoveAt(ran);
    }

    void SpawnEnemyHard()
    {
        int ran = Random.Range(0, listCheckSameHard.Count);
        Enemy_HotAirBalloonMinigame1 tmpEnemy = Instantiate(enemyPrefab, listPosEasy[0].transform.position, Quaternion.identity);
        tmpEnemy.pathCreator = listPath[listCheckSameHard[ran]];
        listCheckSameHard.RemoveAt(ran);
    }

    void ResetListCheckSameEasy()
    {
        listCheckSameEasy.Clear();
        for (int j = 0; j < listPosEasy.Count; j++)
        {
            listCheckSameEasy.Add(j);
        }
    }
    void ResetListCheckSameHard()
    {
        listCheckSameHard.Clear();
        for (int j = 0; j < listPath.Count; j++)
        {
            listCheckSameHard.Add(j);
        }
    }


    public IEnumerator LevelEasy()
    {
        while (time > 30)
        {
            for (int i = 0; i < Random.Range(1, 3); i++)
            {
                SpawnEnemyEasy();
            }
            ResetListCheckSameEasy();
            yield return new WaitForSeconds(3);
        }

    }
    void LevelHard()
    {
        int enemyHardCount = 0;
        for (int i = 0; i < listPath.Count; i++)
        {
            if (enemyHardCount < 7)
            {
                int ran1 = Random.Range(0, 2);
                if (ran1 == 0)
                {
                    SpawnEnemyHard();
                    enemyHardCount++;
                }
            }
        }
    }

    IEnumerator CountingTime()
    {
        while (time > 0)
        {
            yield return new WaitForSeconds(1);
            time--;
            txtTime.text = time.ToString();
            if (time == 30)
            {
                StopCoroutine(spawnCoroutine);
                LevelHard();
            }
            if (time == 0 && !isLose)
            {
                isWin = true;
                Debug.Log("Win");
                StopAllCoroutines();
            }
        }
    }

    public void EndGame()
    {
        StopAllCoroutines();
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && isBegin && !isLose && !isWin)
        {
            mouseCurrentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isHoldCar = true;
            tmpPos_HotAirBalloon = mouseCurrentPos - (Vector2)hotAirBalloon.transform.position;
            if (tutorial.activeSelf)
            {
                tutorial.SetActive(false);
                tutorial.transform.DOKill();
                enemyTutorial.transform.DOPlay();
                StartCoroutine(CountingTime());
                spawnCoroutine = StartCoroutine(LevelEasy());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isHoldCar = false;
        }

        if (isHoldCar)
        {
            mouseCurrentPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseCurrentPos = new Vector2(Mathf.Clamp(mouseCurrentPos.x, -mainCamera.orthographicSize * f2 + 1.3f + tmpPos_HotAirBalloon.x, mainCamera.orthographicSize * f2 - 1.3f + tmpPos_HotAirBalloon.x), Mathf.Clamp(mouseCurrentPos.y, -mainCamera.orthographicSize + tmpPos_HotAirBalloon.y, mainCamera.orthographicSize - 2.3f + tmpPos_HotAirBalloon.y));
            hotAirBalloon.transform.position = new Vector2(mouseCurrentPos.x - tmpPos_HotAirBalloon.x, mouseCurrentPos.y - tmpPos_HotAirBalloon.y);
            hotAirBalloon.transform.DOMove(new Vector2(mouseCurrentPos.x - tmpPos_HotAirBalloon.x, mouseCurrentPos.y - tmpPos_HotAirBalloon.y), 0.1f);
        }
    }
}
