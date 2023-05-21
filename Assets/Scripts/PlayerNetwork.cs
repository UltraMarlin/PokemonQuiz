using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Rigidbody playerRigidbody;
    [SerializeField] private MeshRenderer playerMesh;
    private Material newPlayerMaterial;

    private NetworkVariable<float> playerHue = new NetworkVariable<float>(
        -1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private NetworkVariable<int> score = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [SerializeField] private GameObject spawnBallPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            transform.position = new Vector3(0f, 4f, 0f);
            ChangeColorRandom();
        }

        //playerHue.OnValueChanged += SetColor;
        score.OnValueChanged += UpdateScore;


        if (IsOwner && IsServer)
        {
            SpawnObjectServerRpc();
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);

        if (Input.GetButtonDown("ActionP"))
        {
            ChangeColorRandom();
        }
    }

    private void ChangeColorRandom()
    {
        playerHue.Value = Random.value;
    }

    private void UpdateScore(int preValue, int newValue)
    {
        //PlayerScoreList.scoreTextList[(int)OwnerClientId].text = PlayerScoreString(newValue);
    }

    private string PlayerScoreString(int scoreValue)
    {
        return $"Player {OwnerClientId + 1}: {scoreValue}";
    }

    [ServerRpc]
    private void SpawnObjectServerRpc()
    {
        GameObject spawnedObject = Instantiate(spawnBallPrefab);
        spawnedObject.transform.position = new Vector3(Random.Range(-18f, 18f), Random.Range(1f, 4f), Random.Range(-18f, 18f));
        spawnedObject.GetComponent<NetworkObject>().Spawn(true);
    }

    [ServerRpc]
    private void DestroyObjectServerRpc()
    {
        Destroy(GameObject.FindGameObjectWithTag("Ball"));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner) return;
        score.Value++;
        DestroyObjectServerRpc();
        SpawnObjectServerRpc();
    }
}
