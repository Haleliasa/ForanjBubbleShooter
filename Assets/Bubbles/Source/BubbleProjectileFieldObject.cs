using Field;
using Shooting;
using System.Collections;
using UnityEngine;

namespace Bubbles {
    [RequireComponent(typeof(Bubble))]
    [RequireComponent(typeof(Projectile))]
    public class BubbleProjectileFieldObject : MonoBehaviour, IFieldObject {
        private Projectile projectile;

        public Bubble Bubble { get; private set; }

        void IFieldObject.Init(Transform position) {
            this.projectile.enabled = false;
            transform.SetParent(position, worldPositionStays: true);
        }

        IEnumerator IFieldObject.Destroy(FieldObjectDestroyType type) {
            return Bubble.Destroy(type);
        }

        private void Awake() {
            Bubble = GetComponent<Bubble>();
            this.projectile = GetComponent<Projectile>();
        }
    }
}
