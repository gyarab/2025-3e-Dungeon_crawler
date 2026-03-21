using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class PlayerDetector : MonoBehaviour
{
    public bool playerDetected = false;
    public Tilemap room;
    public DetectorManager manager;
    public UnityEvent onPlayerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&manager!=null)
        {
            playerDetected = true;
            manager.detected.Add(this);
            onPlayerEnter?.Invoke();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && manager != null)
        {
            playerDetected = false;
            manager.detected.Remove(this);
        }
    }
}