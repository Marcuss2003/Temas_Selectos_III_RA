using UnityEngine;
using Vuforia;

public class TargetStatus : MonoBehaviour {
    private ObserverBehaviour mObserver;
    private NarrativaPersistente manager;

    void Start() {
        mObserver = GetComponent<ObserverBehaviour>();
        manager = FindObjectOfType<NarrativaPersistente>();
    }

    void Update() {
        if (mObserver.TargetStatus.Status == Status.TRACKED) {
            manager.OnTargetFound(gameObject.name);
        } else {
            manager.OnTargetLost(gameObject.name);
        }
    }
}