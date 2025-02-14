﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using System;

public class Window_QuestPointer : MonoBehaviour
{

    [SerializeField] private Camera uiCamera;
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite crossSprite;

    private List<QuestPointer> questPointerList;

    private void Awake()
    {
        questPointerList = new List<QuestPointer>();
    }

    private void Update()
    {
        Updater();
    }

    private void Updater()
    {
        foreach (QuestPointer questPointer in questPointerList)
        {
            questPointer.Update();
        }
    }

    public QuestPointer CreatePointer(Vector3 targetPosition)
    {
        GameObject pointerGameObject = Instantiate(transform.Find("pointerTemplate").gameObject);
        pointerGameObject.SetActive(true);
        pointerGameObject.transform.SetParent(transform, false);
        QuestPointer questPointer = new QuestPointer(targetPosition, pointerGameObject, uiCamera, arrowSprite, crossSprite);
        questPointerList.Add(questPointer);
        return questPointer;
    }

    public void DestroyPointer(QuestPointer questPointer)
    {
        questPointerList.Remove(questPointer);
        questPointer.DestroySelf();
    }

    public class QuestPointer
    {

        private Vector3 targetPosition;
        private GameObject pointerGameObject;
        private Sprite arrowSprite;
        private Sprite crossSprite;
        private Camera uiCamera;
        private RectTransform pointerRectTransform;
        private SpriteRenderer pointerImage;

        public QuestPointer(Vector3 targetPosition, GameObject pointerGameObject, Camera uiCamera, Sprite arrowSprite, Sprite crossSprite)
        {
            this.targetPosition = targetPosition;
            this.pointerGameObject = pointerGameObject;
            this.uiCamera = uiCamera;
            this.arrowSprite = arrowSprite;
            this.crossSprite = crossSprite;

            pointerRectTransform = pointerGameObject.GetComponent<RectTransform>();
            pointerImage = pointerGameObject.GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            float borderSizeHeight = 50f;
            float borderSizeWidth = 150f;
            Vector3 targetPositionScreenPoint = Camera.main.WorldToScreenPoint(targetPosition);
            bool isOffScreen = targetPositionScreenPoint.x <= borderSizeWidth || targetPositionScreenPoint.x >= Screen.width - borderSizeWidth || targetPositionScreenPoint.y <= borderSizeHeight || targetPositionScreenPoint.y >= Screen.height - borderSizeHeight;

            if (isOffScreen)
            {
                RotatePointerTowardsTargetPosition();

                pointerImage.sprite = arrowSprite;
                Vector3 cappedTargetScreenPosition = targetPositionScreenPoint;
                cappedTargetScreenPosition.x = Mathf.Clamp(cappedTargetScreenPosition.x, borderSizeWidth, Screen.width - borderSizeWidth);
                cappedTargetScreenPosition.y = Mathf.Clamp(cappedTargetScreenPosition.y, borderSizeWidth, Screen.height - borderSizeHeight);

                Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(cappedTargetScreenPosition);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);
            }
            else
            {
                pointerImage.sprite = crossSprite;
                Vector3 pointerWorldPosition = uiCamera.ScreenToWorldPoint(targetPositionScreenPoint);
                pointerRectTransform.position = pointerWorldPosition;
                pointerRectTransform.localPosition = new Vector3(pointerRectTransform.localPosition.x, pointerRectTransform.localPosition.y, 0f);

                pointerRectTransform.localEulerAngles = Vector3.zero;
            }
        }

        private void RotatePointerTowardsTargetPosition()
        {
            Vector3 toPosition = targetPosition;
            Vector3 fromPosition = Camera.main.transform.position;
            fromPosition.z = 0f;
            Vector3 dir = (toPosition - fromPosition).normalized;
            float angle = UtilsClass.GetAngleFromVectorFloat(dir);
            pointerRectTransform.localEulerAngles = new Vector3(0, 0, angle);
        }

        public void DestroySelf()
        {
            Destroy(pointerGameObject);
        }

    }
}
