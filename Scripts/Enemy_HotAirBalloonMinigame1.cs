using DG.Tweening;
using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_HotAirBalloonMinigame1 : MonoBehaviour
{
    public PathCreator pathCreator;
    public float speed;
    float distanceTravelled;
    public GameObject VFXPrefab;
    public bool isDestroy;


    private void Start()
    {
        speed = 1;
        isDestroy = false;
        if (pathCreator == null)
        {
            if (transform.position.x > 0 && transform.position.y > 0)
            {
                transform.DOMove(new Vector2(-(transform.position.x + Random.Range(0, 5)), -(transform.position.y + Random.Range(0, 5))), 10).OnComplete(() => { Destroy(gameObject); }); 
            }
            if (transform.position.x > 0 && transform.position.y < 0)
            {
                transform.DOMove(new Vector2(-(transform.position.x + Random.Range(0, 5)), -(transform.position.y - Random.Range(0, 5))), 10).OnComplete(() => { Destroy(gameObject); });
            }
            if (transform.position.x < 0 && transform.position.y > 0)
            {
                transform.DOMove(new Vector2(-(transform.position.x - Random.Range(0, 5)), -(transform.position.y + Random.Range(0, 5))), 10).OnComplete(() => { Destroy(gameObject); });
            }
            if (transform.position.x < 0 && transform.position.y < 0)
            {
                transform.DOMove(new Vector2(-(transform.position.x - Random.Range(0, 5)), -(transform.position.y - Random.Range(0, 5))), 10).OnComplete(() => { Destroy(gameObject); });
            }
        }
    }


    private void Update()
    {
        if (pathCreator != null)
        {
            distanceTravelled += speed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled);
        }

        if (GameController_HotAirBalloonMinigame1.instance.isWin && !isDestroy)
        {
            isDestroy = true;
            GameObject tmpVFX = Instantiate(VFXPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            tmpVFX.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() =>
            {
                Destroy(tmpVFX);
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameObject tmpVFX = Instantiate(VFXPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
            GameController_HotAirBalloonMinigame1.instance.isHoldCar = false;
            GameController_HotAirBalloonMinigame1.instance.isLose = true;
            GameController_HotAirBalloonMinigame1.instance.hotAirBalloon.GetComponent<PolygonCollider2D>().enabled = false;
            GameController_HotAirBalloonMinigame1.instance.EndGame();
            Debug.Log("Thua");
            GameController_HotAirBalloonMinigame1.instance.hotAirBalloon.transform.DOMoveY(-20, 8).OnComplete(() => { Destroy(GameController_HotAirBalloonMinigame1.instance.hotAirBalloon); });
            tmpVFX.GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() =>
            {
                Destroy(tmpVFX);             
            });
        }
    }
}
