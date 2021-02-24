using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int lightBeamsActivated;
    [SerializeField] private float distance = 1000;
       
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.instance.onLightPillarActivated += addActiveLightBeam;
        GameEvents.instance.onGamePause += pauseTime;
        GameEvents.instance.onGameResume += resumeTime;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        increaseTerrainDetail();
    }

    private void addActiveLightBeam() //Increases current light beams activated.
    {
        lightBeamsActivated++;
    }

    private void increaseTerrainDetail()
    {
        Terrain.activeTerrain.detailObjectDistance = distance;
    }

    private void pauseTime()
    {
        Time.timeScale = 0;
    }

    private void resumeTime()
    {
        Time.timeScale = 1;
    }
}
