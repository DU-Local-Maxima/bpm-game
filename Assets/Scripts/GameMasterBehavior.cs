using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMasterBehavior : MonoBehaviour
{
    public float _respawnWaitTimeStart = 2.0f;    
    public GameObject _blockToGenerate;
    public int _maximumBlocks = 150;
    public AudioClip _gameOverAudioClip;

    private AudioSource _audioSource;    
    private List<AudioClip> _allClips = new List<AudioClip>();
    private List<string> _clipColors = new List<string>();
    private float _timeSinceLastSpawn = 0.0f;
    private int _blockCount = 0;
    private float _respawnWaitTime;
    private MessageBehavior messageBehavior;
    private FlashBehavior flash;
    private string gameState = "started"; // started, inprogress, ended
    private Dictionary<string, Color> colors = new Dictionary<string, Color>();

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _respawnWaitTime = _respawnWaitTimeStart;
        messageBehavior = GameObject.Find("MessageText").GetComponent<MessageBehavior>();
        flash = GameObject.Find("FlashCanvas").GetComponent<FlashBehavior>();

        messageBehavior.ShowMessage("Music-averse Squares\n\nPress numbers 1-5 to change song to match blocks\n\nBlocks hitting ground will cause game to SPEED UP\n\nIncorrect song selection will cause game to SPEED UP\n\nPress SPACE or TOUCH to play");

        // load all clips
        foreach (Object r in Resources.LoadAll("Audio Assets"))
        {
            _allClips.Add((AudioClip)r);
        }
        _allClips = _allClips.OrderBy(c => c.name).ToList(); // ensure in filename order! (as in the folder)

        // gather all bpms and colors
        foreach (AudioClip clip in _allClips)
        {
            List<string> nameParts = clip.name.Split(' ').ToList();
            nameParts.RemoveAt(0); // track number not used
            nameParts.RemoveAt(0); // bpm not used
            _clipColors.Add(nameParts[0]);
            nameParts.RemoveAt(0);
        }

        // generate dictionary of all valid colors
        colors["black"] = Color.black;
        colors["blue"] = Color.blue;
        colors["clear"] = Color.clear;
        colors["cyan"] = Color.cyan;
        colors["gray"] = Color.gray;
        colors["green"] = Color.green;
        colors["grey"] = Color.grey;
        colors["magenta"] = Color.magenta;
        colors["red"] = Color.red;
        colors["white"] = Color.white;
        colors["yellow"] = Color.yellow;

        // add button click events
        foreach (var button in GameObject.Find("ButtonCanvas").GetComponentsInChildren<Button>())
        {
            button.onClick.AddListener(() => HandleChangingSong(button.name));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
        bool clickDetected = Input.GetMouseButtonDown(0);
        switch (gameState)
        {   
            case "started":
                if (Input.GetKeyUp(KeyCode.Space) || clickDetected)
                {
                    gameState = "inprogress";
                    messageBehavior.HideMessage();
                    ToggleButtons(true);
                }
                break;
            case "inprogress":
                var input = Input.inputString;
                if (!string.IsNullOrEmpty(input))
                {
                    HandleChangingSong(input);   
                }
                HandleBlockGeneration();
                if (IsGameOver())
                {
                    messageBehavior.ShowMessage("GAME OVER\n\nPress SPACE or TOUCH to play again");
                    gameState = "ended";
                    ToggleButtons(false);
                }
                break;
            case "ended":
                if (Input.GetKeyUp(KeyCode.Space) || clickDetected)
                {
                    GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
                    foreach(GameObject b in blocks)
                        Destroy(b);
                    _blockCount = 0;
                    _respawnWaitTime = 2.00f;
                    messageBehavior.HideMessage();
                    ToggleButtons(true);
                    gameState = "inprogress";
                }
                break;
        }
    }

    public void SpeedUp()
    {
        if (_respawnWaitTime > 0.00f)
        {
            _respawnWaitTime -= 0.05f;

            float colorComponent = 1.0f - ((_respawnWaitTimeStart - _respawnWaitTime) / _respawnWaitTimeStart);
            flash.DoFlash(new Color(1.0f, colorComponent, colorComponent));
        }
    }

    public void BlockDestroyed()
    {
        _blockCount -= 1;
    }

    public bool IsGameOver()
    {
        return _blockCount > _maximumBlocks || _respawnWaitTime <= 0.0f;
    }

    private void CheckForBlocks(string color)
    {
        bool foundOne = false;
        foreach (var g in GameObject.FindGameObjectsWithTag("Block"))
        {
            var bb = g.GetComponent<BlockBehavior>();
            if (bb.NameOfColor() == color)
            {
                foundOne = true;
                break;
            }            
        }
        if (!foundOne)
        {
            SpeedUp();
        }
    }

    public void HandleChangingSong(string input)
    {
        int number;
        bool success = int.TryParse(input, out number);
        if (success && number > 0 && number <= _allClips.Count)
        {
            _audioSource.clip = _allClips[number - 1];
            _audioSource.Play();
            CheckForBlocks(_clipColors[number - 1]);    
        }
    }

    private void HandleBlockGeneration()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn > _respawnWaitTime)
        {            
            GameObject newBlock = Instantiate(_blockToGenerate);

            var sr = newBlock.GetComponent<SpriteRenderer>();
            string randomColor = _clipColors[Random.Range(0, _clipColors.Count)];
            sr.material.color = colors[randomColor];            

            newBlock.transform.position = new Vector2(Random.Range(-7.5f, 6f), 5.0f);
            _blockCount += 1;
            _timeSinceLastSpawn = _timeSinceLastSpawn - _respawnWaitTime;
        }
    }

    private void ToggleButtons(bool toggle)
    {
        foreach (var button in GameObject.FindGameObjectsWithTag("Button"))
        {
            button.GetComponent<Button>().interactable = toggle;
        }
    }
}
