using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Zenject;

public class SelectionDisplayUI : MonoBehaviour
{
    [SerializeField] private SelectionDisplayEntryUI entryPrefab;
    [SerializeField] private Transform parent;
    
    [Inject] private ISelectionManager _selectionManager;

    private void Start()
    {
        _selectionManager.OnSelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        ClearEntries();
        foreach (var creature in _selectionManager.SelectedCreatures)
        {
            var entry = Instantiate(entryPrefab, parent);
            entry.SetCreature(creature);
        }
    }

    private void ClearEntries()
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}