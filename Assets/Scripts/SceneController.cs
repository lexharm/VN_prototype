using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public const int gridRows = 2;
    public const int gridCols = 4;
    public const float offsetX = 90f;
    public const float offsetY = 120f;

    [SerializeField] private MemoryCard originalCard;
    [SerializeField] private Sprite[] images;
    [SerializeField] private Text scoreLabel;
    [SerializeField] private VNCreator.VNCreator_DisplayUI mainScript;

    [Header("Visuals")]
    public Image characterImg;

    [Header("Text")]
    public Text dialogueTxt;

    [Header("Buttons")]
    public Button choiceBtn1;
    public Button choiceBtn2;
    public Button saveBtn;
    public Button menuButton;

    private MemoryCard _firstRevealed;
    private MemoryCard _secondRevealed;

    private Vector3 characterPosition;

    public bool canReveal
    {
        get { return _secondRevealed == null; }
    }

    private int _triesCount = 0;
    private int _score = 0;

    public void CardRevealed(MemoryCard card)
    {
        if (_firstRevealed == null)
        {
            _firstRevealed = card;
        } else
        {
            _secondRevealed = card;
            StartCoroutine(CheckMatch());
        }
    }

    private IEnumerator CheckMatch()
    {
        _triesCount++;
        scoreLabel.text = "Number of tries: " + _triesCount;
        
        if (_firstRevealed.id == _secondRevealed.id)
        {
            _score++;
        } else
        {
            yield return new WaitForSeconds(.5f);
            _firstRevealed.Unreveal();
            _secondRevealed.Unreveal();
        }

        _firstRevealed = null;
        _secondRevealed = null;

        if (_score == 4)
        {
            if (_triesCount < 10)
            {
                Vector3[] path = new Vector3[] { characterPosition + new Vector3(700, 0, 0),
                                                                                           characterPosition + new Vector3(700, 300, 0),
                                                                                           characterPosition + new Vector3(500, 300, 0),
                                                                                           characterPosition + new Vector3(500, -300, 0)};
                iTween.MoveTo(characterImg.gameObject, iTween.Hash("path", path,
                                                                   "time", 3f,
                                                                   "easetype", iTween.EaseType.linear));
                choiceBtn1.gameObject.SetActive(true);
            }
            else
                choiceBtn2.gameObject.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        characterPosition = characterImg.gameObject.transform.position;

        dialogueTxt.text = mainScript.GetText();

        Vector3 startPos = originalCard.transform.position;

        int[] numbers = { 0, 0, 1, 1, 2, 2, 3, 3 };
        numbers = ShuffleArray(numbers);

        for (int i = 0; i < gridCols; i++)
        {
            for (int j = 0; j < gridRows; j++)
            {
                MemoryCard card;
                if (i == 0 && j == 0)
                {
                    card = originalCard;
                } else
                {
                    card = Instantiate(originalCard, transform) as MemoryCard;
                }

                int index = j * gridCols + i;
                int id = numbers[index];
                card.SetCard(id, images[id]);

                float posX = (offsetX * i) + startPos.x;
                float posY = -(offsetY * j) + startPos.y;

                card.transform.position = new Vector3(posX, posY, startPos.z);
            }
        }

        if (choiceBtn1 != null)
        {
            choiceBtn1.onClick.AddListener(delegate {
                characterImg.transform.position = characterPosition;
                mainScript.NextNodeAdd(0); 
            });
            choiceBtn1.gameObject.SetActive(false);
        }
        if (choiceBtn2 != null) { 
            choiceBtn2.onClick.AddListener(delegate {
                characterImg.transform.position = characterPosition;
                mainScript.NextNodeAdd(1); 
            });
            choiceBtn2.gameObject.SetActive(false);
        }
        if (saveBtn != null)
            saveBtn.onClick.AddListener(mainScript.SaveAdd);
        if (menuButton != null)
            menuButton.onClick.AddListener(mainScript.ExitGameAdd);
    }

    private int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(0, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    public void Restart()
    {
        //Application.LoadLevel("SampleScene");
        //SceneManager.LoadScene("SampleScene");
    }
}
