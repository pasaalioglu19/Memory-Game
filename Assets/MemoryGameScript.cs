using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MemoryGame : MonoBehaviour
{
    private int gridSize = 2;
    private Color[] colors = { Color.blue, Color.red, Color.green, Color.yellow, Color.cyan, Color.magenta, Color.grey, new Color(1.0f, 0.5f, 0.0f, 1.0f) };
    private Color[,] gridColors;
    private Color defaultColor = Color.white;
    private GameObject[,] grid, grid2;
    private int currentLevel = 1;
    private float time = 0;
    private int visualize_time = 1;
    private bool musicOn = true;
    private bool isChangable = true;

    public AudioSource music;
    public Text timerText;
    public GameObject correctMapTextPrefab;
    public GameObject winText;
    public GameObject musicButton;
    public GameObject submitButton;
    public GameObject playAgainButton;
    public Canvas canvas;

    private Sprite musicOnIc;
    public Sprite musicOffIc;

    public GameObject cellPrefab;
    private GameObject correctMapText;

    void Start()
    {
        musicOnIc = musicButton.GetComponent<Image>().sprite;
        SetupGrid();
    }

    void SetupGrid()
    {
        submitButton.SetActive(false);
        gridSize = currentLevel + 1;
        gridColors = new Color[gridSize, gridSize];
        grid = new GameObject[gridSize, gridSize];
        int[] colorCount = new int[gridSize];
        int index;

        float cellSize = 1.05f; 

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject cell = Instantiate(cellPrefab); 
                cell.transform.SetParent(transform); 
                cell.transform.localPosition = new Vector3(i * cellSize - currentLevel * 0.5f, j * cellSize - currentLevel * 0.5f, 0);

                grid[i, j] = cell;

                do
                {
                    index = UnityEngine.Random.Range(0, gridSize);
                    gridColors[i, j] = colors[index];
                    colorCount[index]++;
                } while (colorCount[index] > gridSize);

                cell.GetComponent<SpriteRenderer>().color = gridColors[i, j]; 
            }
        }
    }

    void Update()
    {
        if(time >= visualize_time)
        {
            startChoice();
        }
        else
        {
            UpdateTimer(visualize_time - time);
        }

        time += Time.deltaTime;

        // Check for touch input on mobile
        if (Input.touchCount > 0 && time < 0 && isChangable)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
                if (hit.collider != null)
                {
                    GameObject selectedCell = hit.collider.gameObject;
                    Vector3 pos = selectedCell.transform.localPosition;
                    int x = Mathf.FloorToInt((pos.x + currentLevel * 0.5f) / 1.05f);
                    int y = Mathf.FloorToInt((pos.y + currentLevel * 0.5f) / 1.05f);
                    CycleColor(x, y);
                }
            }
        }

        // Check for touch input on PC
        if (Input.GetMouseButtonDown(0) && time < 0 && isChangable) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null)
            {
                GameObject selectedCell = hit.collider.gameObject;
                Vector3 pos = selectedCell.transform.localPosition;
                int x = Mathf.FloorToInt((pos.x + currentLevel * 0.5f) / 1.05f);
                int y = Mathf.FloorToInt((pos.y + currentLevel * 0.5f) / 1.05f);
                CycleColor(x, y);
            }
        }
    }

    private void UpdateTimer(float currentTime)
    {
        currentTime += 1; 

        float minutes = Mathf.FloorToInt(currentTime / 60);
        float seconds = Mathf.FloorToInt(currentTime % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void CycleColor(int x, int y)
    {
        Color currentColor = grid[x, y].GetComponent<SpriteRenderer>().color;
        int nextColorIndex = (Array.IndexOf(colors, currentColor) + 1) % gridSize;
        grid[x, y].GetComponent<SpriteRenderer>().color = colors[nextColorIndex];
    }

    private void HideCell()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                grid[i, j].GetComponent<SpriteRenderer>().color = defaultColor;
            }
        }
        submitButton.SetActive(true);
    }

    public void startChoice()
    {
        timerText.enabled = false;
        time = int.MinValue;
        HideCell();
    }

    public void checkWin()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (grid[i, j].GetComponent<SpriteRenderer>().color != gridColors[i, j])
                {
                    miniMap();
                    return;
                }
            }
        }
        nextLevel();
    }

    private void miniMap()
    {
        correctMapText = Instantiate(correctMapTextPrefab, canvas.transform); 
        RectTransform rectTransform = correctMapText.GetComponent<RectTransform>();
        
        switch (currentLevel)
        {
            case 1:
                rectTransform.anchoredPosition = new Vector2(580, 75);
                break;
            case 2:
                rectTransform.anchoredPosition = new Vector2(600, 85);
                break;
            case 3:
                rectTransform.anchoredPosition = new Vector2(610, 105);
                break;
            case 4:
                rectTransform.anchoredPosition = new Vector2(635, 125);
                break;
            case 5:
                rectTransform.anchoredPosition = new Vector2(650, 145);
                break;
            case 6:
                rectTransform.anchoredPosition = new Vector2(663, 165);
                break;
            case 7:
                rectTransform.anchoredPosition = new Vector2(680, 185);
                break;
        }

        float cellSize = 0.35f;
        isChangable = false;
        grid2 = new GameObject[gridSize, gridSize];

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                GameObject cell2 = Instantiate(cellPrefab);
                cell2.transform.localScale /= 3;
                cell2.transform.SetParent(transform);
                cell2.transform.localPosition = new Vector3(i * cellSize - currentLevel * (1/6F - 0.15F) + 5.2F, j * cellSize - currentLevel * 1/6F, 0);
                cell2.GetComponent<SpriteRenderer>().color = gridColors[i, j];
                grid2[i, j] = cell2;
            }
        }
        playAgainButton.SetActive(true);
    }

    public void restartGame()
    {
        playAgainButton.SetActive(false);
        timerText.enabled = true;
        time = 0;
        cleanBoard();
        isChangable = true;
        currentLevel = 1;
        visualize_time = 1;
        SetupGrid();
    }

    private void nextLevel()
    {
        if (currentLevel == 7)
        {
            winText.SetActive(true);
        }
        else
        {
            timerText.enabled = true;
            time = 0;
            cleanBoard();
            currentLevel++;
            visualize_time *= 4;
            SetupGrid();
        }
    }

    private void cleanBoard()
    {
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                Destroy(grid[i, j]);
                if (!isChangable) 
                    Destroy(grid2[i, j]);
            }
        }
        Destroy(correctMapText);
    }

    public void musicToggle()
    {
        Image musicRend = musicButton.GetComponent<Image>();
        if (musicOn)
        {
            musicRend.sprite = musicOffIc;
            music.Pause();
            musicOn = false;
        }
        else
        {
            musicRend.sprite = musicOnIc;
            music.Play();
            musicOn = true;
        }
    }
}
