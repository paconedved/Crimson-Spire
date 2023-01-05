using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public RawImage compass;

    public Transform player;

    public GameObject iconPrefab;

    public List<QuestMarker> qMark = new List<QuestMarker>();

    float compassUnit;

    private void Start()
    {
        compassUnit = compass.rectTransform.rect.width / 360f;
    }

    // Update is called once per frame
    void Update()
    {
        compass.uvRect = new Rect(player.localEulerAngles.y / 360f, 0f, 1, 1);

        foreach(QuestMarker marker in qMark)
        {
            marker.image.rectTransform.anchoredPosition = GetPosOnCompass(marker);
        }
    }

    public void AddQuestMarkers(QuestMarker marker)
    {
        GameObject newMark = Instantiate(iconPrefab, compass.transform);
        marker.image = newMark.GetComponent<Image>();
        marker.image.sprite = marker.icon;

        qMark.Add(marker);
    }

    Vector2 GetPosOnCompass(QuestMarker marker)
    {
        Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 playerFwd = new Vector2(player.transform.forward.x, player.transform.forward.z);

        float angle = Vector2.SignedAngle(marker.position - playerPos, playerFwd);
        return new Vector2(compassUnit * angle, 0f);
    }
}
