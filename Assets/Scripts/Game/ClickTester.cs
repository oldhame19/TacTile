
using UnityEngine;

public class ClickTester : MonoBehaviour
{
    private void Update()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        Camera cam = Camera.main;

        if (cam == null)
        {
            Debug.LogError("ClickTester: No MainCamera found.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            Debug.Log(
                "RAY HIT: " + hit.collider.name +
                " | Object: " + hit.collider.gameObject.name +
                " | Has TileView: " +
                (hit.collider.GetComponent<TileView>() != null) +
                " | Parent has TileView: " +
                (hit.collider.GetComponentInParent<TileView>() != null)
            );
        }
        else
        {
            Debug.Log("RAY HIT NOTHING.");
        }
    }
}
