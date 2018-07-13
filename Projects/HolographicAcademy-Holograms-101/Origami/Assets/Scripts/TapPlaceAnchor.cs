using UnityEngine;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;

public class TapPlaceAnchor : MonoBehaviour
{

    public string ObjectAnchorStoreName;

    WorldAnchorStore anchorStore;

    bool Placing = false;
    // Use this for initialization
    void Start()
    {
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        anchorStore = store;
        Placing = true;

        string[] ids = anchorStore.GetAllIds();
        for (int index = 0; index < ids.Length; index++)
        {
            if (ids[index] == ObjectAnchorStoreName)
            {
                WorldAnchor wa = anchorStore.Load(ids[index], gameObject);
                Placing = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Placing)
        {   
            // Do a raycast into the world that will only hit the Spatial Mapping mesh.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo,
                30.0f, SpatialMapping.PhysicsRaycastMask))
            {
                // Move this object's parent object to
                // where the raycast hit the Spatial Mapping mesh.
                this.transform.parent.position = hitInfo.point;

                // Rotate this object's parent object to face the user.
                Quaternion toQuat = Camera.main.transform.localRotation;
                toQuat.x = 0;
                toQuat.z = 0;
                this.transform.parent.rotation = toQuat;
            }
        }
    }

    void OnSelect()
        {
            if (anchorStore == null)
            {
                return;
            }

            if (Placing)
            {
                SpatialMapping.Instance.DrawVisualMeshes = true;
                WorldAnchor attachingAnchor = gameObject.AddComponent<WorldAnchor>();
                if (attachingAnchor.isLocated)
                {
                    bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
                }
                else
                {
                    attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
                }
            }
            else
            {
                SpatialMapping.Instance.DrawVisualMeshes = false;
                WorldAnchor anchor = gameObject.GetComponent<WorldAnchor>();
                if (anchor != null)
                {
                    DestroyImmediate(anchor);
                }

                string[] ids = anchorStore.GetAllIds();
                for (int index = 0; index < ids.Length; index++)
                {
                    if (ids[index] == ObjectAnchorStoreName)
                    {
                        bool deleted = anchorStore.Delete(ids[index]);
                        break;
                    }
                }
            }

            Placing = !Placing;
        }

    private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                bool saved = anchorStore.Save(ObjectAnchorStoreName, self);
                self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
            }
        }
    
}