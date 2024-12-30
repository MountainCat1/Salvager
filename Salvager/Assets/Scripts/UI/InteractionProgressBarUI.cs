using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{

    public class InteractionProgressBarUI : MonoBehaviour
    {
        [SerializeField] private Creature creature;
        [SerializeField] private Slider progressBar;

        private void Start()
        {
            creature.Interacted += OnInteracted;
            
            progressBar.gameObject.SetActive(false);
        }

        private void OnInteracted(Interaction interaction)
        {
            UpdateProgressBar(interaction);
        }

        private void UpdateProgressBar(Interaction interaction)
        {
            progressBar.value = (float)interaction.CurrentProgress;
            progressBar.gameObject.SetActive(interaction.Status == InteractionStatus.InProgress);
            progressBar.maxValue = (float)interaction.InteractionTime;
        }
    }

}
