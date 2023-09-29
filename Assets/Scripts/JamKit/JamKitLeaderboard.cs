using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace JamKit
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string name;
        public int score;
        public DateTime date;
    }

    public partial class JamKit
    {
        //private const string serverUrl = "http://localhost:3000";
        private const string serverUrl = "https://torrengserver.onrender.com";

        [System.Serializable]
        private class Response
        {
            public ScoreEntry[] scores;
        }

        public void GetScores(Action<bool, ScoreEntry[]> callback)
        {
            Run(GetScoresCoroutine(callback));
        }

        private IEnumerator GetScoresCoroutine(Action<bool, ScoreEntry[]> callback)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(serverUrl + "/scores"))
            {
                request.SetRequestHeader("Authorization", "Basic osman1234");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Leaderboard get fail");
                    callback(false, new ScoreEntry[] { });
                    yield break;
                }

                Response res = JsonUtility.FromJson<Response>(request.downloadHandler.text);

                callback(true, res.scores);
            }
        }

        public void PostScore(string name, int score)
        {
            Run(PostCoroutine(name, score));
        }

        private static IEnumerator PostCoroutine(string name, int score)
        {
            Dictionary<string, string> postData = new Dictionary<string, string>()
            {
                { "name", name },
                { "score", score.ToString() },
            };

            using (UnityWebRequest request = UnityWebRequest.Post(serverUrl + "/new-score", postData))
            {
                request.SetRequestHeader("Authorization", "Basic osman1234");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Leaderboard post fail");
                }
            }
        }
    }
}