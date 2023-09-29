using UnityEngine;

namespace JamKit
{
    public partial class JamKit : MonoBehaviour
    {
        [SerializeField] private Globals _globals;
        public Globals Globals => _globals;

        private void Awake() // Should be the only awake
        {
            JamKit existingInstance = FindObjectOfType<JamKit>();
            if (existingInstance != this)
            {
                DestroyImmediate(gameObject);
            }
        }

        private void Start()
        {
            StartSfx();
        }

        private void Update()
        {
            UpdateSfx();
        }
    }
}