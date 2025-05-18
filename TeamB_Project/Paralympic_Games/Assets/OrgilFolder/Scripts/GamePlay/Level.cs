using Fusion;
using UnityEngine;

namespace OrgilFolder.Scripts.GamePlay
{
    public class Level : NetworkBehaviour
    {
        public static Level Current { get; private set; }

        [SerializeField] private Transform[] team1SpawnPositions;
        [SerializeField] private Transform[] team2SpawnPositions;
        [SerializeField] private Transform[] spectatorPositions;

        public static void Load(Level level)
        {
            Unload();
            if (GameManager.Instance.Runner.CanSpawn)
            {
            }
        }

        public static void Unload()
        {
            if (Current)
            {
                GameManager.Instance.Runner.Despawn(Current.Object);
                Current = null;
            }
        }

        public override void Spawned()
        {
            Current = this;
            GameManager.Instance.Rpc_LoadDone();
        }

        public (Vector3, Quaternion) GetSpawnPositionAndRotation(int index, int team)
        {
            Transform spawnTf = team switch
            {
                1 => team1SpawnPositions[index % team1SpawnPositions.Length],
                2 => team2SpawnPositions[index % team2SpawnPositions.Length],
                -1 => spectatorPositions[index % spectatorPositions.Length],
            };

            if (spawnTf)
            {
                return (spawnTf.position, spawnTf.rotation);
            }

            return (Vector3.zero, Quaternion.identity);
        }
    }
}