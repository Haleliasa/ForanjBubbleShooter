#nullable enable

using Field;
using Shooting;
using UnityEngine;

namespace Bubbles {
    [RequireComponent(typeof(Bubble))]
    [RequireComponent(typeof(Projectile))]
    public class ProjectileBubble : MonoBehaviour, IFieldObject {
        [Tooltip("sec")]
        [Min(0f)]
        [SerializeField]
        private float fieldAttachDuration = 0.1f;

        private (Vector2, Vector2)? attachPath;
        private float attachTime;

        public Bubble Bubble { get; private set; } = null!;

        public Projectile Projectile { get; private set; } = null!;

        public void Init(Color color) {
            if (Bubble == null) {
                Bubble = GetComponent<Bubble>();
            }
            Bubble.Init(color);

            if (Projectile == null) {
                Projectile = GetComponent<Projectile>();
            }
        }

        void IFieldObject.Init(Transform position) {
            Projectile.Stop();
            transform.SetParent(position, worldPositionStays: true);
            this.attachPath = (Bubble.Position, position.position);
            this.attachTime = 0f;
        }

        void IFieldObject.Detach() {
            Bubble.transform.SetParent(null, worldPositionStays: true);
        }

        void IFieldObject.Destroy(FieldObjectDestroyType type) {
            this.attachPath = null;
            Bubble.Destroy(type);
        }

        private void FixedUpdate() {
            if (this.attachPath.HasValue) {
                (Vector2 from, Vector2 to) = this.attachPath.Value;
                Attach(from, to, ref this.attachTime, Time.fixedDeltaTime, out bool finished);
                if (finished) {
                    this.attachPath = null;
                }
            }
        }

        private void OnCollisionEnter(Collision collision) {
            HitFieldCell(collision.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            HitFieldCell(collision.gameObject);
        }

        private void HitFieldCell(GameObject obj) {
            if (!Projectile.IsMoving) {
                return;
            }
            IFieldCell? cell = obj.GetComponentInChildren<IFieldCell>();
            cell?.Hit(
                this,
                Bubble.Color,
                Bubble.Position,
                destroy: Mathf.Approximately(Projectile.Power, 1f));
        }

        private void Attach(
            Vector2 from,
            Vector2 to,
            ref float time,
            float deltaTime,
            out bool finished) {
            time += deltaTime;
            if (time < this.fieldAttachDuration) {
                Bubble.Move(Vector2.Lerp(from, to, time / this.fieldAttachDuration));
                finished = false;
            } else {
                Bubble.Pin(to);
                finished = true;
            }
        }
    }
}
