using System;
using UnityEngine;
using UnityEngine.UI;

namespace VRUIP
{
    public class TabController : A_ColorController
    {
        [Header("Tab Properties")]
        [SerializeField] private Tab[] tabs;
        
        [Header("Components")]
        [SerializeField] private Image tabsBackground;

        private void Awake()
        {
            InitializeTabButtons();
        }

        protected override void Start()
        {
            base.Start();
            // Select the first tab
            if (tabs.Length > 0) SelectTab(tabs[0].button);
        }

        private void InitializeTabButtons()
        {
            foreach (var tab in tabs)
            {
                tab.button.Initialize(this);
            }
        }

        // Selects the tab at the given index
        public void SelectTab(int index)
        {
            if (index < 0 || index >= tabs.Length) return;
            SelectTab(tabs[index].button);
        }

        // Selects the given tab button and deselects all others
        public void SelectTab(TabButtonController tabButton)
        {
            foreach (var tab in tabs)
            {
                if (tab.button == tabButton)
                {
                    tab.button.SetSelected(true);
                    tab.panel.SetActive(true);
                }
                else
                {
                    tab.button.SetSelected(false);
                    tab.panel.SetActive(false);
                }
            }
        }
        
        protected override void SetColors(ColorTheme theme)
        {
            tabsBackground.color = theme.thirdColor;
        }

        [Serializable]
        private class Tab
        {
            public TabButtonController button;
            public TabPanelController panel;
        }
    }
}
