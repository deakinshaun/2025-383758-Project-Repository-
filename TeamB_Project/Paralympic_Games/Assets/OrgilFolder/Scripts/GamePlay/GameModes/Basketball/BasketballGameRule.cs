using System;
using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay.GameModes.Basketball
{
    public class BasketballGameRule : NetworkBehaviour
    {
        [SerializeField] private Transform ballSpawnNeutralTf;
        [SerializeField] private Transform team1BallSpawnTf;
        [SerializeField] private Transform team2BallSpawnTf;
        
        public static BasketballGameRule Instance { get; private set; }
        [SerializeField] private NetworkPrefabRef ballPrefab;
        [Networked] public int TeamAScore { get; private set; }
        [Networked] public int TeamBScore { get; private set; }

        [Header("3-point line distance ")] [SerializeField]
        private float threePointRadius = 7.0f;

        public event System.Action<int, int> OnScored;

        private void Awake()
        {
            Instance = this;
        }

        public override void Spawned()
        {
            TeamAScore = 0;
            TeamBScore = 0;
        }

        public void SpawnBallNeutral()
        {
            Runner.Spawn(ballPrefab, ballSpawnNeutralTf.position, ballSpawnNeutralTf.rotation, onBeforeSpawned: (runner, obj) =>
            {
                var ball = obj.GetComponent<Ball>();

                ball.ResetNeutral();
            });
        }

        private void SpawnBallForTeam(int team)
        {

            Transform spawnTf = team switch
            {
                1 => team1BallSpawnTf,
                2 => team2BallSpawnTf,
            };
            Runner.Spawn(ballPrefab, spawnTf.position, spawnTf.rotation, onBeforeSpawned: (runner, obj) =>
            {
                var ball = obj.GetComponent<Ball>();
                ball.InitializePossession(team);
            });
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void RPCRegisterScore(int shootingTeam, Vector3 shotOrigin, PlayerRef scorer, RpcInfo info = default)
        {
            //TODO: Make judgement on the 3 point line
            int points = 2;
            if (shootingTeam == 0) TeamAScore += points;
            else TeamBScore += points;
            
            OnScored?.Invoke(shootingTeam,points);
        }
    }
}