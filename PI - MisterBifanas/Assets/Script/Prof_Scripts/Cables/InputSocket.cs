using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TVStore
{
    /// <summary>A socket that accepts one plug from any OutputSocket.</summary>
    public class InputSocket : CableSocket
    {
        static readonly List<InputSocket> all = new List<InputSocket>();
        public static IReadOnlyList<InputSocket> All => all;

        [SerializeField, ColorUsage(false, true)] Color highlightColor = new Color(2.5f, 1.8f, 0.2f);

        public UnityEvent<OutputSocket> onConnected = new UnityEvent<OutputSocket>();
        public UnityEvent<OutputSocket> onDisconnected = new UnityEvent<OutputSocket>();

        /// <summary>Raised whenever a plug is attached or removed.</summary>
        public event Action<InputSocket> StateChanged;

        public OutputSocket Source { get; private set; }
        public bool IsOccupied => Source != null;

        bool highlighted;

        void OnEnable()
        {
            all.Add(this);
            RefreshIndicator();
        }

        void OnDisable() => all.Remove(this);

        public bool CanAccept(OutputSocket source) => Source == null || Source == source;

        /// <summary>World pose for a plug seated in this socket (plug +Z points into the socket).</summary>
        public Pose PlugPose(CablePlug plug)
        {
            return new Pose(transform.position + transform.forward * plug.bodyLength,
                Quaternion.LookRotation(-transform.forward, transform.up));
        }

        public void SetHighlighted(bool value)
        {
            highlighted = value;
            RefreshIndicator();
        }

        internal void Attach(OutputSocket source)
        {
            Source = source;
            RefreshIndicator();
            onConnected.Invoke(source);
            StateChanged?.Invoke(this);
        }

        internal void Detach(OutputSocket source)
        {
            if (Source != source) return;
            Source = null;
            RefreshIndicator();
            onDisconnected.Invoke(source);
            StateChanged?.Invoke(this);
        }

        void RefreshIndicator()
        {
            SetIndicatorColor(highlighted ? highlightColor : IsOccupied ? activeColor : idleColor);
        }
    }
}
