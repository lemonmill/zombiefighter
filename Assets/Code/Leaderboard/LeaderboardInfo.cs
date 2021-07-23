// using System;
// using System.Text;
// using GleyPlugins.GameServices.Scripts;
// using UniRx;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Leaderboard
// {
//     public class LeaderboardInfo : MonoBehaviour
//     {
//         public GameObject[] relatedObjects;
//         public Text textC;
//         
//         private CompositeDisposable disposables;
//         
//         private StringBuilder str = new StringBuilder();
//
//         private void OnEnable()
//         {
//            LeaderboardManager.leaderboardUpdate
//                .Subscribe(OnLeaderboardUpdate)
//                .AddTo(disposables);
//
//            LeaderboardManager.isLoggedIn
//                .Subscribe(OnLoggedUpdate)
//                .AddTo(disposables);
//         }
//
//         private void OnDisable()
//         {
//             disposables?.Clear();
//         }
//
//         private void OnLoggedUpdate(bool isLogged)
//         {
//             foreach (var relatedObject in relatedObjects)
//             {
//                 relatedObject.SetActive(isLogged);
//             }
//         }
//         
//         private void OnLeaderboardUpdate(bool enable)
//         {
//             textC.gameObject.SetActive(enable);
//
//             if (!enable) return;
//
//             str.Clear();
//             str.Append("Your position in leaderboard: <b>");
//             str.Append(LeaderboardManager.currentRank);
//             str.Append("</b>");
//
//             var deltaRank = LeaderboardManager.currentRank - LeaderboardManager.previousRank;
//             if (deltaRank > 0)
//             {
//                 str.Append("\n\n At that game you overtooked <b>");
//                 str.Append(deltaRank);
//                 str.Append("</b> players.\nGreat work, survivor!");
//             }
//
//             textC.text = str.ToString();
//         }
//         
//         
//         public void ShowCoinsLeaderboardUI()
//         {
// #if UNITY_ANDROID
//             GameServices.Instance.ShowSpecificLeaderboard(LeaderboardNames.leaderboard_zombie_fighter_leaderboard);
// #elif UNITY_IOS
//             GameServices.Instance.ShowLeaderboadsUI();
// #else
//             Debug.LogWarning("Unsupported platform for showing leaderboard")
// #endif
//         }
//
//         public void ShowLeaderboardsUI()
//         {
//             GameServices.Instance.ShowLeaderboadsUI();
//         }
//     }
// }