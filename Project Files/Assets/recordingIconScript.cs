using System.Collections;
using System.Collections.Generic;
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
        // Start is called before the first frame update
        void Start()
        {
            this.uiImage = gameObject.GetComponent<Image>();
            this.uiImage.sprite = notRecTex;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void changeTexture(Icon ico)
        {
            switch (ico)
            {
                case Icon.notRecording:
                    this.uiImage.sprite = notRecTex;
                    break;
                case Icon.recording:
                    this.uiImage.sprite = recTex;
                    break;
                case Icon.analyzing:
                    this.uiImage.sprite = analyzingTex;
                    break;
                default:
                    break;
            }
        }
    }
}
