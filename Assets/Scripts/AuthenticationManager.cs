using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class User
{
    public string name;
    public int score = 0;
}

public class AuthenticationManager : MonoBehaviour {

    static public AuthenticationManager S;
    public string urlLogin = "gamify_action_login.php";
    public string urlRegister = "gamify_action_register.php";
    public string urlUpdateScore = "gamify_update_score.php";
    public string urlUpdateBoard = "gamify_update_board.php";
    public string urlGetCurrentChallenge = "gamify_get_challenge.php";
    public string urlUpdateChallenge = "gamify_update_challenge.php";
    WWWForm form;

    
    private void OnEnable()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        DontDestroyOnLoad(this);
    }
    private void OnDestroy()
    {
        Debug.Log("AuthenticationManager destroyed");
        StopAllCoroutines();
    }

    public void JoinButtonTapped()
    {
        //textFeedback.text = "Logging in...";
        //StartCoroutine(JoinChallenge());
    }

    public IEnumerator LoginToChallenge()
    {
        
        yield return StartCoroutine(GetCurrentChallengeUsers());
    }

    public IEnumerator GetCurrentChallengeUsers()
    {
        AppManager.S = FindObjectOfType<AppManager>();
        Challenge responseChallenge = new Challenge();
        form = new WWWForm();
        AppManager.S.ReAssignPIN();
        form.AddField("PIN", AppManager.S.challenge.PIN);
        WWW w = new WWW(urlGetCurrentChallenge, form);
        yield return w;

        if (string.IsNullOrEmpty(w.error))
        {
            ServerResponse sr = JsonUtility.FromJson<ServerResponse>(w.text);
            if (sr.success == true)
            {
                responseChallenge.PIN = sr.PIN;
                responseChallenge.gameIndex = sr.gameIndex;
                responseChallenge.userContainer = JsonUtility.FromJson<UserContainer>(sr.userContainerJSON);
                bool userExistsInDB = false;
                for (int i = 0; i < responseChallenge.userContainer.users.Count; i++)
                {
                    if (responseChallenge.userContainer.users[i].name == AppManager.S.user.name)
                    {
                        userExistsInDB = true;
                        responseChallenge.userContainer.users[i] = AppManager.S.user;
                        
                    }
                    if (responseChallenge.userContainer.users[i].name == "")
                    {
                        if (AppManager.S.challenge.userContainer.users.Contains(responseChallenge.userContainer.users[i]))
                            AppManager.S.challenge.userContainer.users.Remove(responseChallenge.userContainer.users[i]);
                        responseChallenge.userContainer.users.Remove(responseChallenge.userContainer.users[i]);
                        
                    }

                }
                if (!userExistsInDB)
                {
                    if (!string.IsNullOrEmpty(AppManager.S.user.name))
                    {
                        responseChallenge.userContainer.users.Add(AppManager.S.user);
                        yield return StartCoroutine(UpdateChallengeInDB(responseChallenge));
                    }
                }
                //***Updating the current Challenge

                AppManager.S.challenge = responseChallenge;
                
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 && (!string.IsNullOrEmpty(AppManager.S.user.name) || AppManager.S.gameMode == GameMode.View))
                {
                    ScoreBoardManager.S.AddUsersToDisplayList(AppManager.S.challenge);
                }
                    
            }
            else
            {
                Debug.Log(sr.error);
                AppManager.S.DisplayMessage("PIN Error");
                AppManager.S.AssignDismissMessageButton(0);
                AppManager.S.StopCoroutines();
            }
                
        }
        else
        {
            Debug.Log(w.error);
        }
    }

    public IEnumerator UploadScore()
    {
        yield return null;
        yield return StartCoroutine(GetCurrentChallengeUsers());
        yield return StartCoroutine(UpdateChallengeInDB(AppManager.S.challenge));
    }

    public IEnumerator CreateChallenge(Challenge c)
    {
        form = new WWWForm();
        form.AddField("gameIndex", c.gameIndex);
        form.AddField("PIN", c.PIN);
        string userContainer = JsonUtility.ToJson(c.userContainer);
        form.AddField("userContainer", userContainer);

        WWW w = new WWW(urlRegister, form);
        yield return w;
        if (string.IsNullOrEmpty(w.error))
        {
            ServerResponse sr = JsonUtility.FromJson<ServerResponse>(w.text);
            if (sr.success == true)
            {
                Debug.Log("Challenge Created Successfully");
            }
            else
                AppManager.S.DisplayMessage("Registration Error2", sr.error, true);
        }
        else
            AppManager.S.DisplayMessage("Registration Error3", w.error, true);
    }

    //TODO: Implement 
    public IEnumerator GetLastChallenge()
    {
        form = new WWWForm();
        form.AddField("PIN", AppManager.S.challenge.PIN);
        WWW w = new WWW(urlUpdateScore, form);
        yield return w;

        if (string.IsNullOrEmpty(w.error))
        {
            ServerResponse sr = JsonUtility.FromJson<ServerResponse>(w.text);
            if (sr.success == true)
            {
                AppManager.S.challenge = JsonUtility.FromJson<Challenge>(w.text);
                AppManager.S.challenge.userContainer = JsonUtility.FromJson<UserContainer>(sr.userContainerJSON);
            }
            else
                Debug.Log(sr.error);
        }
        else
        {
            Debug.Log(w.error);
        }
    }

    public IEnumerator UpdateChallengeInDB(Challenge updatedChallenge)
    {
        WWWForm newForm = new WWWForm();
        newForm.AddField("PIN", updatedChallenge.PIN);
        string userContainerString = JsonUtility.ToJson(updatedChallenge.userContainer);
        newForm.AddField("userContainerString", userContainerString);
        WWW w = new WWW(urlUpdateChallenge, newForm);
        yield return w;

        if (string.IsNullOrEmpty(w.error))
        {
            ServerResponse sr = JsonUtility.FromJson<ServerResponse>(w.text);
            if (sr.success == true)
            {
                Debug.Log("Challenge updated successfully");
            }
            else
                Debug.Log(sr.error);
        }
        else
        {
            Debug.Log(w.error);
        }
    }

}

