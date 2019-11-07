using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardManager : MonoBehaviour
{
    GameObject contentItem;
    List<GameObject> contentItems = new List<GameObject>();
    static public ScoreBoardManager S;
    //User testUser = new User();
    //public List<User> displayedUsers = new List<User>();
    public Transform contentPanel;

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
    }

    public void Init()
    {
        contentPanel = transform.GetChild(6).GetChild(5).GetChild(0).GetChild(0).GetChild(0);
    }

    public IEnumerator AddPlayersToScoreBoard(Challenge c)
    {
        yield return null;
        AddUsersToDisplayList(c);
    }


    public void AddUsersToDisplayList(Challenge c)
    {
        foreach (GameObject contentItem in contentItems)
        {
            Destroy(contentItem);
        }
        for (int i = 0; i < c.userContainer.users.Count; i++)
        {
            //Debug.Log(c.userContainer.users[i].name);
            AddNextPlayer(c.userContainer.users[i]);
        }
    }

    public void AddNextPlayer(User user)
    {
        if (!contentPanel)
            Init();
        contentItem = Instantiate((GameObject)Resources.Load("Prefabs/ScoreboardItem"), contentPanel.transform);
        contentItem.transform.GetChild(0).GetComponent<Text>().text = user.name;
        contentItem.transform.GetChild(1).GetComponent<Text>().text = user.score.ToString();
        contentItems.Add(contentItem);
    }

    public void ResetScoreBoard()
    {
        if(contentItems.Count > 0)
        {
            foreach (GameObject ci in contentItems)
            {
                Destroy(ci);

            }
            contentItems.Clear();
        }
        
    }
}

