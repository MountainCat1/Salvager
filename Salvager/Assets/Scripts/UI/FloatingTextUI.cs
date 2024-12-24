using TMPro;
using UnityEngine;

namespace Managers
{
    public class FloatingTextUI : MonoBehaviour
    {
        [field: SerializeField] public TextMeshProUGUI Text { get; private set; }
    }
}