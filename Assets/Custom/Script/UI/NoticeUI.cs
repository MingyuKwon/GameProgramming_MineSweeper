using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;


public class NoticeUI : MonoBehaviour, IPointerClickHandler
{
    public static bool currentShowingNotice {
        get{
            if(noticePanelStatic == null) return false;
            return noticePanelStatic.activeInHierarchy;
        }
    }
    public static GameObject noticePanelStatic;

    public TextMeshProUGUI str;
    public GameObject noticePanel;
    public RectTransform rectTransform;

    int callCount;
    int strCount;
    bool isNowTalking;

    string[] currentShowingText;

    private void Awake() {
        EventManager.instance.showNoticeUIEvent += showNotice;
        noticePanelStatic = noticePanel;
    }

    private void OnDestroy() {
        EventManager.instance.showNoticeUIEvent -= showNotice;
    }

    public void OnEnable() {
        EventManager.instance.NoticeCountIncreaseEvent += IncreaseCount;
        callCount = 0;
    }

    public void OnDisable() {
        callCount = 0;
        EventManager.instance.NoticeCountIncreaseEvent -= IncreaseCount;
    }

    public void showNotice(string[] texts, bool isTyping, int panelWidth, int panelHeight)
    {

        if(texts == null) // 이건 Notice를 닫기를 원할 떄
        {
            noticePanel.gameObject.SetActive(false);
            callCount = 0;
        }else
        {
            currentShowingText = texts;
            noticePanel.gameObject.SetActive(true);
            callCount = 0;
            rectTransform.sizeDelta = new Vector2(panelWidth, panelHeight);
            Debug.Log(LanguageManager.currentLanguage);

            if(isTyping)
            {
                GameAudioManager.instance.PlayUIMusic(UIAudioType.Click);
                str.text = "";
                StartCoroutine(Typing(texts));
            }else
            {
                str.text = texts[0];   
            }
            
        }
    }

    IEnumerator Typing(string[] texts)
    {
        strCount = texts.Length;
        int previousCallcount;

        while(strCount > callCount)
        {
            previousCallcount = callCount;
            isNowTalking = true;
            yield return StartCoroutine(TypeTextAnimation(texts[callCount], 1f));
            isNowTalking = false;
            while(previousCallcount == callCount)
            {
                yield return new WaitForEndOfFrame();
            }
        }
    }

    private void changeText(string language){
        if(currentShowingText == null) return;
        if(language == "E")
        {
            str.text = currentShowingText[0];
        }else if(language == "K")
        {
            str.text = currentShowingText[1];
        }
    }

    private void IncreaseCount()
    {
        if(isNowTalking) return; // 아직 텍스트 치는거 안끝났으면 이 이후는 받지 않음

        callCount++;
        GameAudioManager.instance.PlayUIMusic(UIAudioType.Click);

        if(callCount >= strCount)
        {
            noticePanel.gameObject.SetActive(false);
            callCount = 0;
            StopAllCoroutines();
        }
    }

    IEnumerator TypeTextAnimation(string message, float time)
    {
        //각 프레임이 1/프레임 의 시간동안 지속이 된다
        // 그럼 원하는 시간동안 되도록 프레임을 이용한 방법으로 시간을 맞추기 위해서는

        float timePerChar = time / 100;

        Stack<char> stack = new Stack<char>();

        str.text = "";

        int index = -1;
        bool continueLock = false;

        foreach (char letter in message.ToCharArray())
        {
            index++;
            if(letter == '<')
            {
               if(message.Length -1 < index+1)
               {
                    Debug.Log("Incorrect <> type.");
                    yield return null;
               }

               if(message[index+1] == 'b')
               {    
                    stack.Push('b');
                    str.text +="<b></b>";
               }else if(message[index+1] == 'i')
               {
                    stack.Push('i');
                    str.text +="<i></i>";
               }else if(message[index+1] == '/')
               {
                    stack.Pop();
               }

               continueLock = true;
            }else if(letter == '>')
            {
                continueLock = false;
                continue;
            }

            if(continueLock)
            {
                continue;
            }

            if(stack.Count !=0){
                if(stack.Peek() == 'b' || stack.Peek() == 'i')
                {
                    str.text = str.text.Insert(str.text.Length-4, letter.ToString());
                }

            }else
            {
                str.text += letter;
            }
            yield return new WaitForSecondsRealtime(timePerChar);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(currentShowingNotice) EventManager.instance.Invoke_NoticeCountIncreaseEvent();
    }
}


