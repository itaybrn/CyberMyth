using UnityEngine;
using UnityEngine.UI;

namespace recordingIcon
{
    public class recordingIconScript : MonoBehaviour
    {
        private Image uiImage;
        public Sprite notRecTex;
        public Sprite recTex;
        public Sprite analyzingTex;
        public enum Icon
        {
            notRecording,
            recording,
            analyzing
        }

        void Start()
        {
            uiImage = gameObject.GetComponent<Image>();
            uiImage.sprite = notRecTex;
        }

        public void changeTexture(Icon ico)
        {
            switch (ico)
            {
                case Icon.notRecording:
                    uiImage.sprite = notRecTex;
                    break;
                case Icon.recording:
                    uiImage.sprite = recTex;
                    break;
                case Icon.analyzing:
                    uiImage.sprite = analyzingTex;
                    break;
                default:
                    break;
            }
        }
    }
}
