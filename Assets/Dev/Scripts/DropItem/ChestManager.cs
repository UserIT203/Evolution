using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ChestManager : MonoBehaviour
{
    [Inject] private DiContainer _diContainer;

    [SerializeField] private List<Chest> _chests;

    [SerializeField] private Transform _chestPosition;

    private Dictionary<Rarity, Chest> _chestsDictianory;

    private void Awake()
    {
        _chestsDictianory = new Dictionary<Rarity, Chest>();

        foreach (var chest in _chests)
        {
            Chest chestTemplate = Instantiate(chest);
            _diContainer.Inject(chestTemplate);

            chestTemplate.transform.SetParent(_chestPosition, false);
            chestTemplate.transform.localPosition = Vector3.zero;
            chestTemplate.transform.localRotation = Quaternion.identity;

            chestTemplate.gameObject.SetActive(false);

            _chestsDictianory.Add(chest.ChestConfig.ChestRarity, chestTemplate);
        }
    }

    public void OpenChest(Rarity rarityChest)
    {
        _chestsDictianory[rarityChest].gameObject.SetActive(true);
        _chestsDictianory[rarityChest].OpenChest();
    }
}
