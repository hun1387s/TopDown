using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheStack : MonoBehaviour
{
    private const float BoundSize = 3.5f;
    private const float MovingBoundSize = 3f;
    private const float StackMovingSpeed = 5f;
    private const float BlockMovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f;

    public GameObject originBlock = null;

    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize);

    Transform lastBlock = null;
    float blockTransition = 0f;
    float secondaryPosition = 0f;

    int stackCount = -1;
    public int Score { get { return stackCount; } }
    int comboCount = 0;
    public int Combo { get { return comboCount; } }

    int maxCombo = 0;
    public int MaxCombo { get => maxCombo; }

    public Color prevColor;
    public Color currentColor;

    bool isMovingX = true;

    // 저장 불러오기
    int bestScore = 0;
    public int BestScore { get => bestScore; }
    int bestCombo = 0;
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    // 게임 오버 관련
    private bool isGameOver = true;


    // Start is called before the first frame update
    void Start()
    {
        int i =comboCount;
        if (originBlock == null)
        {
            Debug.Log("OriginBlock is NULL");
            return;
        }

        // 불러오기
        // 지정한 Key에 값이 없다면 , default값인 0 반환
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);

        prevColor = GetRandomColor();
        currentColor = GetRandomColor();


        prevBlockPosition = Vector3.down;

        Spawn_Block();
        Spawn_Block();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                // 게임 오버
                Debug.Log("GameOver!");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                StackUIManager.Instance.SetScoreUI();
            }
        }

        MoveBlock();
        
        // 선형 보간을 통한 부드러운 이동
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed*Time.deltaTime); ;
    }

    bool Spawn_Block()
    {
        if (lastBlock != null)
            prevBlockPosition = lastBlock.localPosition;

        GameObject newBlock = null;
        Transform newTrans = null;

        // 복제해서 새로운 블록 생성
        newBlock = Instantiate(originBlock);

        if (null == newBlock)
        {
            Debug.Log("NewBlock Instantiate NULL");
            return false;
        }

        ColorChange(newBlock);

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount;
        blockTransition = 0f;

        lastBlock = newTrans;

        isMovingX = !isMovingX;

        StackUIManager.Instance.UpdateScore();

        return true;
    }

    Color GetRandomColor()
    {
        float r = Random.Range(100f, 250f) / 255f;
        float g = Random.Range(100f, 250f) / 255f;
        float b = Random.Range(100f, 250f) / 255f;

        return new Color(r, g, b);
    }

    void ColorChange(GameObject go)
    {
        Color applyColor = Color.Lerp(prevColor, currentColor, (stackCount % 11) / 10f);

        Renderer rdr = go.GetComponent<Renderer>();

        if (null == rdr)
        {
            Debug.Log("Renderer is NULL");
            return;
        }

        rdr.material.color = applyColor;
        Camera.main.backgroundColor = applyColor - new Color(0.1f, 0.1f, 0.1f);

        if (applyColor.Equals(currentColor) == true)
        {
            prevColor = currentColor;
            currentColor = GetRandomColor();
        }
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        // sin 을 이용해도 되지만, 직선움직임을 위해
        float movePostion = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if (isMovingX)
        {
            lastBlock.localPosition = 
                new Vector3(movePostion * MovingBoundSize, stackCount, secondaryPosition);
        }
        else
        {
            lastBlock.localPosition = 
                new Vector3(secondaryPosition, stackCount, -movePostion * MovingBoundSize);
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.localPosition;

        if (isMovingX)
        {
            float deltaX = prevBlockPosition.x - lastPosition.x;

            // 떨어질 조각 방향 설정
            bool isNegativeNum = (deltaX < 0) ? true : false;

            deltaX = Mathf.Abs(deltaX);            
            if (deltaX > ErrorMargin)
            {
                stackBounds.x -= deltaX;
                if (stackBounds.x <= 0)
                {
                    // 게임 오버
                    return false;
                }

                // 두 블럭의 중심지점 찾기
                float middle = (prevBlockPosition.x + lastPosition.x) / 2f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(isNegativeNum
                            ? lastPosition.x + stackBounds.x / 2 + rubbleHalfScale
                            : lastPosition.x - stackBounds.x / 2 - rubbleHalfScale
                        , lastPosition.y
                        , lastPosition.z),
                    new Vector3(deltaX, 1, stackBounds.y)
                );
                comboCount = 0;
            }
            else
            {
                ComboCheck();
                // errorMargin 보다 작다면 성공으로 보고 보정해준다.
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        else
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;

            bool isNegativeNum = (deltaZ < 0) ? true : false;
            deltaZ = Mathf.Abs(deltaZ);


            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if (stackBounds.y <= 0)
                {
                    // 게임 오버
                    return false;
                }

                // 두 블럭의 중심지점 찾기
                float middle = (prevBlockPosition.z + lastPosition.z) / 2f;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPosition.x
                        , lastPosition.y
                        , isNegativeNum
                            ? lastPosition.z + stackBounds.y / 2 + rubbleHalfScale
                            : lastPosition.z - stackBounds.y / 2 - rubbleHalfScale),
                    new Vector3(stackBounds.x, 1, deltaZ)
                );
                comboCount = 0;
            }
            

            else
            {
                ComboCheck();
                // errorMargin 보다 작다면 성공으로 보고 보정해준다.
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;

        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        // 지금의 블록을 복제
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = scale;

        go.AddComponent<Rigidbody>();
        go.name = "Rubble";

    }

    void ComboCheck()
    {
        comboCount++;
        if (maxCombo < comboCount)
            maxCombo = comboCount;

        if (comboCount % 5 == 0)
        {
            Debug.Log("5 Combo Success!");
            stackBounds += new Vector3(0.5f, 0.5f);
            stackBounds.x = (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y = (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;

        }
    }

    void UpdateScore()
    {
        if (bestScore < stackCount)
        {
            Debug.Log("최고 점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;

            // 저장하기
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
            PlayerPrefs.SetInt(BestScoreKey, bestScore);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount;

        for (int i=1; i < 20; i++)
        {
            if (childCount < i) break;

            // 하위 오브젝트를 인덱스로 찾는 기능
            GameObject go = transform.GetChild(childCount - i).gameObject;

            if (go.name.Equals("Rubble")) continue;

            Rigidbody rigid = go.AddComponent<Rigidbody>();

            rigid.AddForce(
                (Vector3.up * Random.Range(0, 10f) + Vector3.right * (Random.Range(0, 10f) - 5f)) * 100f
                );
        }
    }

    public void Restart()
    {
        int childCount = transform.childCount;

        for (int i =0; i<childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;
        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        prevColor = GetRandomColor();
        currentColor = GetRandomColor();

        Spawn_Block();
        Spawn_Block();

    }

}
