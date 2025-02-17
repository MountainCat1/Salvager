using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Managers
{
    public class CursorData
    {
        public Texture2D Cursor;
        public int Priority;
        public object Source;
    }
    
    public enum CursorPriority
    {
        Default = 0,
        Targeting = 1,
        Dragging = 2
    }

    public interface ICursorManager
    {
        void SetCursor(object source, Texture2D cursor, CursorPriority priority);
        void RemoveCursor(object source);
    }

    public class CursorManager : MonoBehaviour, ICursorManager
    {
        private readonly List<CursorData> _cursors = new List<CursorData>();

        public void SetCursor(object source, Texture2D cursor, CursorPriority priority)
        {
            _cursors.Add(new CursorData
            {
                Cursor = cursor,
                Priority = (int)priority,
                Source = source
            });
            _cursors.Sort((a, b) => a.Priority.CompareTo(b.Priority));
            UpdateCursor();
        }

        public void RemoveCursor(object source)
        {
            _cursors.RemoveAll(x => x.Source == source);
            UpdateCursor();
        }

        private void UpdateCursor()
        {
            if (_cursors.Count == 0)
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                return;
            }

            Cursor.SetCursor(
                _cursors.OrderByDescending(x => x.Priority).First().Cursor,
                Vector2.zero,
                CursorMode.Auto
            );
        }

        private void OnDestroy()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }
}