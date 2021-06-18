﻿using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEngine.UI.Extensions.Examples
{
    public class Example04Scene : MonoBehaviour
    {
        [SerializeField]
        Example04ScrollView scrollView = null;
        [SerializeField]
        Button prevCellButton = null;
        [SerializeField]
        Button nextCellButton = null;
        [SerializeField]
        Text selectedItemInfo = null;

        void Start()
        {
            prevCellButton.onClick.AddListener(scrollView.SelectPrevCell);
            nextCellButton.onClick.AddListener(scrollView.SelectNextCell);
            scrollView.OnSelectedIndexChanged(HandleSelectedIndexChanged);

            var cellData = Enumerable.Range(0, 20)
                .Select(i => new Example04CellDto { Message = "Cell " + i })
                .ToList();

            scrollView.UpdateData(cellData);
            scrollView.UpdateSelection(0);
        }

        void HandleSelectedIndexChanged(int index)
        {
            selectedItemInfo.text = String.Format("Selected item info: index {0}", index);
        }
    }
}
