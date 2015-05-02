using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PhotoReel : MonoBehaviour
{
    public GameObject photoPrefab;

    void Start()
    {
        float photoHeight = ((RectTransform) photoPrefab.transform).rect.height;

        for (int i = 0; i < LevelController.instance.photos.Count; i++)
        {
            Texture photo = LevelController.instance.photos[i];
            GameObject photoInstance = Instantiate(photoPrefab) as GameObject;
            photoInstance.transform.SetParent(this.transform);

            RawImage photoImage = photoInstance.GetComponent<RawImage>();
            photoImage.texture = photo;

            photoInstance.transform.position = photoInstance.transform.position.SetY(i * (photoHeight + 15));
        }
    }
}
