// using System;
// using GleyPlugins.GameServices.Scripts;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UniRx;
// using UnityEngine;
//
// public class LeaderboardManager : MonoBehaviour
// {
//     public static int currentRank = 0;
//     public static int previousRank = 0;
//     public static ReactiveProperty<bool> isLoggedIn = new ReactiveProperty<bool>(false);
//
//     public static ReactiveProperty<bool> leaderboardUpdate = new ReactiveProperty<bool>(false);
//
//     private static IDisposable disposable;
//
//     private void Awake()
//     {
//         Initialize();
//     }
//
//     private void OnDestroy()
//     {
//         disposable?.Dispose();
//     }
//
//     static void Initialize()
//     {
//         GameServices.Instance.LogIn(x =>
//         {
//             if (x)
//                 Debug.Log("GAME SERVICES | Succesful Login");
//             else
//                 Debug.LogError("GAME SERVICES | Unuccesful Login!");
//             
//             
//             Debug.Log("GAME SERVICES | Social authenticated: " + Social.localUser.authenticated);
//             if (!Social.localUser.authenticated)
//                 Social.localUser.Authenticate(y =>
//                 {
//                     Debug.Log("GAME SERVICES | Authentication through social: " + y);
//                 });
//             isLoggedIn.Value = x;
//         });
//         
//         var save = SavableSettings.instance;
//         disposable?.Dispose();
//         disposable = save.coinsPerLevelHighscore.Subscribe(HighscoreUpdated);
//         
//         LoadScores();
//     }
//
//     private static void HighscoreUpdated(int newHighscore)
//     {
//         leaderboardUpdate.Value = false;
//         if (newHighscore==0) return;
//         if (GlobalGameSettings.currentLevelId == 0)
//             GameServices.Instance.SubmitScore(newHighscore, LeaderboardNames.leaderboard_zombie_fighter_leaderboard, HighscoreUpdateCompleted);
//     }
//
//     private static void HighscoreUpdateCompleted(bool success, GameServicesError errorMessage)
//     {
//         if (!success)
//             Debug.LogError($"Can't update leaderboard | {errorMessage.ToString()}");
//     }
//
//     public static void LoadScores()
//     {
//         leaderboardUpdate.Value = false;
//         
//         var leaderboardID =
//             GameServices.Instance.leaderboardManager.GetLeaderboardID(LeaderboardNames.leaderboard_zombie_fighter_leaderboard.ToString());
//      
//         PlayGamesPlatform.Instance.LoadScores(leaderboardID, 
//             LeaderboardStart.PlayerCentered,
//             1,
//             LeaderboardCollection.Public, 
//             LeaderboardTimeSpan.AllTime, 
//             LoadedPlayerLeaderboard);
//     }
//
//     private static void LoadedPlayerLeaderboard(LeaderboardScoreData scoreData)
//     {
//         switch (scoreData.Status)
//         {
//             case ResponseStatus.Success:
//             case ResponseStatus.SuccessWithStale:
//                 if (scoreData.PlayerScore == null)
//                 {
//                     leaderboardUpdate.Value = false;
//                     break;
//                 }
//                 var playerScore = scoreData.PlayerScore;
//                 previousRank = currentRank != 0 ? currentRank : playerScore.rank;
//                 currentRank = playerScore.rank;
//                 leaderboardUpdate.Value = true;
//                 break;
//             default:
//                 Debug.LogError("Cant get leaderboardData: " + scoreData.Status);
//                 leaderboardUpdate.Value = false;
//                 break;
//         }
//     }
// }
