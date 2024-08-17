﻿using System.Collections.Generic;
using System.Linq;
using _Project.Script.UI.Interface;
using _Project.Script.UI.Services;
using UI;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace _Project.Script.UI
{
    public class CarousselUI : UIContainer, IUIContainerLayout
    {

        [SerializeField] private CarrouselLayoutPreferences _layoutPreferences;
        [SerializeField] private float _centralPanelAlpha = 1f;
        [SerializeField] private float _neighbouringPanelAlpha = .4f;
        [SerializeField] private float _otherPanelAlpha = 0f;
        [SerializeField] private int _adjustNeighbouringPanelAlphaIndex = 1;
        [SerializeField] private int _adjustNeighbouringPanelInteractableIndex = 0;
       
        public void Layout(LayoutPreference preference) => LayoutService.Layout(preference);
        public IUILayoutService LayoutService { get; set; } = new UICarrouselLayoutService();

        public IUIPanel SelectedUIPanel => GetUIPanelByIndex(SelectedIndex);
        public int SelectedIndex => LayoutService.CurrentIndex;
        public int NextIndex => GetNextIndex();

        private int GetNextIndex()
        {
            var index = SelectedIndex + 1;
            int wrappedIndex = (index + NumOfPanels) % NumOfPanels;
            
            return wrappedIndex;
        }

        public int PreviousIndex => GetPreviousIndex();

        private int GetPreviousIndex()
        {
            var index = SelectedIndex - 1;
            int wrappedIndex = (index + NumOfPanels) % NumOfPanels;
            return wrappedIndex;
        }

        protected override void OnInitialize()
        {
            LayoutService.OnFinishLayout += OnChangeCurrentIndex;
            LayoutService.OnLayoutUpdate += LayoutUpdated; 
            LayoutService.Initialize(_layoutPreferences, ChildPanels.ConvertAll<IUIPanel>(panel => panel));
            LayoutUpdated(0f, SelectedIndex);
            OnChildSelectExit += SelectPanel;
        }

        private void SelectPanel(IUIPanelInteractable<XRSimpleInteractable> obj)
        {
           Debug.Log($"{Name} - Selected {obj.Panel.Name}");
           if (obj is not IUIPanel panel)
           {
               Debug.Log($"Selected {obj.Panel.Name} is not a IUIPanel ");
               return;
           }
           
           ToPanel(panel);
        }

        private void LayoutUpdated(float angle, int index)
        {
            SetPanelsVisibility(index, _adjustNeighbouringPanelAlphaIndex, _centralPanelAlpha, _neighbouringPanelAlpha, _otherPanelAlpha);
            SetPanelsInteractability(index, _adjustNeighbouringPanelInteractableIndex);
        }

        private void SetPanelsInteractability(int centralIndex, int indexOnSideOfCentral)
        {
            List<int> interactableIndex = new List<int>();
            if (indexOnSideOfCentral < 0)
            {
               interactableIndex = Enumerable.Range(0, NumOfPanels).ToList();
            }


            // Calculate the range of indices that should be interactable
           else 
                for (int offset = -indexOnSideOfCentral; offset <= indexOnSideOfCentral; offset++)
                {
                    int index = (centralIndex + offset + NumOfPanels) % NumOfPanels;
                    interactableIndex.Add(index);
                }
            
      
            for (int i = 0; i < NumOfPanels; i++)
            {
                var panel = GetUIPanelByIndex(i);
                if(panel == null) continue;
                bool interactable = interactableIndex.Contains(i) && panel.Visible;
                
                if(panel is UIPanel uiPanel) uiPanel.SetInteractable(interactable);
            }
            
        }
        private void SetPanelsVisibility(int centralIndex, int indexOnSideOfCentral , float centralAlpha, float neighbouringAlpha, float otherAlpha)
        {
            List<int> indexToShow = new List<int>();
            
            if (indexOnSideOfCentral < 0)
            {
                indexToShow = Enumerable.Range(0, NumOfPanels).ToList();
            }
            // Calculate the range of indices that should have partial visibility
            else for (int offset = -indexOnSideOfCentral; offset <= indexOnSideOfCentral; offset++)
                {
                    int index = (centralIndex + offset + NumOfPanels) % NumOfPanels;
                    indexToShow.Add(index);
                }
            
      
            for (int i = 0; i < NumOfPanels; i++)
            {
                var panel = GetUIPanelByIndex(i);
                if(panel == null) continue;
                float alpha = otherAlpha;

                if (indexToShow.Contains(i)) alpha = i == centralIndex ? centralAlpha : neighbouringAlpha;
                
                if(panel is UIPanel uiPanel) uiPanel.SetAlpha(alpha,true, .2f);
            
            }
            
        }


        private void OnValidate()
        {
            UpdateLayout();
        }

        public LayoutPreference LayoutPreferences { get => _layoutPreferences; set => _layoutPreferences = value as CarrouselLayoutPreferences; }
        public void UpdateLayout() => Layout(_layoutPreferences);
        
        public void ToNextPanel() => LayoutService.Layout(NextIndex);
        public void ToPreviousPanel() => LayoutService.Layout(PreviousIndex);
        public void ToPanel(int index) => LayoutService.Layout(index);
        public void ToPanel(IUIPanel panel) => ToPanel(GetUIPanelIndex(panel));

        
        public void OnChangeCurrentIndex(int index){
            Debug.Log($"Current Index : {index}");
        }


        public void CloseCurrentPanel() => ClosePanel(GetUIPanelByIndex(SelectedIndex) as UIPanel);

        protected override void OnClosePanel()
        {
            LayoutService.Initialize(_layoutPreferences, ChildPanels.ConvertAll<IUIPanel>(panel => panel));
        }
        }

}




    // public GameObject FallBackGui
    // {
    //     get
    //     {
    //         if (!_fallBackGui)
    //         {
    //             if (FallBackPrefab)
    //             {
    //                 Debug.Log("[WristMenu] instantiating FallBack GUI ");
    //                 _fallBackGui = Instantiate(FallBackPrefab, this.transform);
    //                 
    //                 ToggleComponent(typeof(XRBaseInteractable),false,_fallBackGui);
    //                 var gui = _fallBackGui.GetComponent<UIPanel>();
    //                 if(gui) gui.RootPrefab = FallBackPrefab;
    //                 Guis.Add(_fallBackGui);
    //
    //                 return _fallBackGui;
    //             }
    //
    //             
    //             return null;
    //         }
    //
    //         return _fallBackGui;
    //
    //     }
    //
    //     set => _fallBackGui = value;
    // }
    //
    // [Header("Shadow Gui")]
    // [SerializeField] private GameObject _guiShadowPrefab;
    // [SerializeField] private GameObject _guiShadow;
    //
    // public GameObject GuiShadow
    // {
    //     get
    //     {
    //         if (!_guiShadow)
    //         {
    //             if (_guiShadowPrefab)
    //             {
    //                 Debug.Log("[WristMenu] instantiating FallBack GUI ");
    //                 return _guiShadow = Instantiate(_guiShadowPrefab, this.transform);
    //             }
    //
    //             
    //             return null;
    //         }
    //
    //         return _guiShadow;
    //
    //     }
    //
    //     set => _guiShadow = value;
    // }
    //
    //
    // private void OnValidate()
    // {
    //     
    // }
    //
    // [Header("Debug")]
    // public bool testMode = false;
    // public bool useIndex;
    // public int indexToFront;
    //
    // private GameObject _nextGui;
    // private GameObject _previousGui;
    // private int _indexCurrentGui;
    // private float _angle;
    // private float _cachedAngleOffset;
    //
    //
    // [SerializeField] private List<GameObject> _guis;
    // private int numberOfTurn = 0;
    // public UIPanel guiExiting;
    // public UIPanel guiEntering;
    //
    //
    //
    //
    // public List<GameObject> Guis
    // {
    //     get => _guis != null ? _guis : new List<GameObject>(); 
    //     set => _guis = value;
    // }
    //
    // public override GameObject CurrentGui {
    //
    //     get
    //     {
    //         if(_currentGui == null) return _previousGui ? _previousGui : FallBackGui;
    //         
    //         return _currentGui;
    //     }
    //     set
    //     {
    //         if (CurrentGui == value && !value) return;
    //         
    //         if (value != null)
    //         {
    //             UpdateGuisPosition(_angleOffset);
    //             MoveGuiToCentralPosition(value);
    //             UpdateGuisPosition(_angleOffset);
    //         }
    //
    //         else
    //         {
    //           
    //             _currentGui =  Guis.Count > 0 ? _previousGui : FallBackGui;
    //             UpdateGuisPosition(_angleOffset);
    //             MoveGuiToCentralPosition(_currentGui);
    //             UpdateGuisPosition(_angleOffset);
    //             
    //         }
    //     }
    //     
    // }
    //
    //
    //
    // public int GetIndexOfGui(GameObject gui, List<GameObject> guis)
    // {
    //     int index = -1;
    //     if (guis.Count == 0) return index;
    //     if (!guis.Contains(gui)) return index;
    //
    //
    //     index = guis.IndexOf(gui);
    //     return index;
    // }
    //
    //
    // public int GetIndexOfNeighbouringGui(GameObject currentCui, List<GameObject> guis, bool preceding)
    // {
    //
    //     if (guis.Count == 1) return -1; //only one element , no neighbour
    //
    //     int indexOfCurrentGui = GetIndexOfGui(currentCui, guis);
    //     if (indexOfCurrentGui < 0) return -1; // count find current gui in list
    //
    //     if (preceding) return indexOfCurrentGui == 0 ? guis.Count - 1 : indexOfCurrentGui - 1; //are we first element , if yes, preceding guis is last index
    //     return indexOfCurrentGui == guis.Count - 1 ? 0 : indexOfCurrentGui + 1;  //are we last element , if yes, next guis is first
    // }
    //
    // public int GetIndexOfNeighbouringGui(int currentIndex, List<GameObject> guis, bool preceding)
    // {
    //
    //     if (guis.Count == 1) return -1; //only one element , no neighbour
    //
    //
    //     if (preceding) return currentIndex == 0 ? guis.Count - 1 : currentIndex - 1; //are we first element , if yes, preceding guis is last index
    //     return currentIndex == guis.Count - 1 ? 0 : currentIndex + 1;  //are we last element , if yes, next guis is first
    // }
    //
    // public GameObject GetGuiByIndex(List<GameObject> guis, int index)
    // {
    //     if (guis.Count == 0) return null;
    //     if (index < 0) return null;
    //
    //     if (index >= guis.Count) return null;
    //     return guis[index];
    // }
    //
    // private void MakeCurrentGui(GameObject newGui)
    // {
    //
    //     if (!Guis.Contains(newGui))
    //     {
    //         AddGuiToWristMenu(newGui, true);
    //         return;
    //     }
    //
    //     _currentGui = newGui;
    //     _indexCurrentGui = GetIndexOfGui(_currentGui, Guis);
    //    
    //     if (_currentGui) TogglePanel(_currentGui, true);
    // }
    //
    // private GameObject GetCurrentGuiByPosition(float radius, float angleTolerance)
    // {
    //     if (Guis.Count == 0) return null;
    //
    //     
    //     foreach (var gui in Guis) 
    //     {
    //         if (!gui) continue;
    //     
    //         float z = gui.transform.localPosition.z;
    //
    //         var angle = angleTolerance *Mathf.Deg2Rad;
    //
    //         if (z >=  radius*Mathf.Cos(angle)) return gui;
    //     }
    //
    //     return null;
    // }
    //
    // private void UpdateNeighbouringGuis(bool useIndex = false)
    // {   
    //     if(CurrentGui == null) 
    //     {
    //         TogglePanel(_previousGui, false, true);
    //         TogglePanel(_nextGui,false, true);
    //
    //         _previousGui = null;
    //         _nextGui = null;
    //         return;
    //     }
    //
    //     UpdateNextGuis(useIndex);
    //     UpdatePreviousGuis(useIndex);
    //
    //     if(_previousGui.gameObject.activeSelf)  SetMaxGuiAlpha(_previousGui, neighbourGuiAlpha);
    //     if (_nextGui.gameObject.activeSelf) SetMaxGuiAlpha(_nextGui, neighbourGuiAlpha);
    //
    //     TogglePanel(_previousGui, true, true);
    //     TogglePanel(_nextGui, true, true);
    //     
    //     
    // }
    //
    // private void UpdatePreviousGuis(bool useIndex = false)
    // {
    //     int indexPrevGui = -1;
    //     if (useIndex) indexPrevGui = GetIndexOfNeighbouringGui(_indexCurrentGui, Guis, true);
    //
    //     else indexPrevGui = GetIndexOfNeighbouringGui(CurrentGui, Guis, true);
    //     //Debug.Log("index prev :" + indexPrevGui);
    //     if (indexPrevGui < 0) return;
    //     _previousGui = GetGuiByIndex(Guis, indexPrevGui);
    //     //_previousGuiIndex = indexPrevGui;
    // }
    //
    // private void UpdateNextGuis(bool useIndex = false)
    // {
    //     int indexNextGui = -1;
    //     if (useIndex) indexNextGui = GetIndexOfNeighbouringGui(_indexCurrentGui, Guis, false);
    //
    //     else indexNextGui = GetIndexOfNeighbouringGui(CurrentGui, Guis, false);
    //
    //    // Debug.Log("index next :" + indexNextGui);
    //     if (indexNextGui < 0) return;
    //     _nextGui = GetGuiByIndex(Guis, indexNextGui);
    //    // _nextGuiIndex = indexNextGui;
    //
    // }
    //
    // public void AddGuiToWristMenu(GameObject newGui, bool makeCurrent)
    // {
    //     if (!newGui) return;
    //     /*check if already there ? */
    //     
    //     if(!makeCurrent)Guis.Add(newGui);
    //     else Guis.Insert(_indexCurrentGui == 0 ? _indexCurrentGui : _indexCurrentGui-1, newGui);
    //     
    //     var UIPanel = newGui.GetComponent<UIPanel>();
    //     if (UIPanel) UIPanel.Container = this;
    //     
    //     newGui.transform.parent = this.transform;
    //     newGui.transform.localPosition = Vector3.zero;
    //     newGui.transform.localScale = new Vector3(0.2f,0.2f,0.2f);
    //    
    //     newGui.transform.localEulerAngles = new Vector3(0, 180, 0);
    //    var turnToFace = newGui.GetComponent<TurnToFace>();
    //    if (turnToFace) turnToFace.enabled = false;
    //     
    //    _angle = CalculateAngle(Guis);
    //    
    //    _startAngleOffset = _angle / 2;
    //    
    //    if (!makeCurrent) return;
    //
    //     MakeCurrentGui(newGui);
    //     UpdateNeighbouringGuis(true);
    // }
    //
    // public void HideAllExcept(List<GameObject> allGuis, List<GameObject> guisToShow, bool show = false)
    // {
    //     if (allGuis.Count == 0) return;
    //
    //     if (!show)
    //     {
    //         foreach (var gui in allGuis)
    //         {
    //
    //             if (guisToShow.Count == 0)
    //             {
    //                TogglePanel(gui, false);
    //             }
    //
    //             else
    //             {
    //                 if (!guisToShow.Contains(gui)) TogglePanel(gui, false);
    //             }
    //         }
    //     }
    //
    //     else 
    //     {
    //
    //         foreach (var gui in allGuis)
    //         {
    //            TogglePanel(gui, show);
    //         }
    //
    //     }
    // }
    //
    // private void HideAll()
    // {
    //     if(Guis == null) return;
    //     if (Guis.Count == 0) return;
    //
    //     foreach(var gui in Guis) 
    //     {
    //         if (!gui) continue;
    //         TogglePanel(gui, false);
    //     }
    //     
    // }
    //
    // private void OrganizeGuis(List<GameObject> allGuis, float radius, float offset = 0, float spread = 0f)
    // {
    //     if (allGuis.Count == 0) return;
    //   
    //
    //     int numberOfElements = allGuis.Count;
    //     float angleStep = 360f / numberOfElements;  //how much angle per element 
    //
    //     if (spread != 0)
    //     {
    //         angleStep += spread;
    //         numberOfElements = (int)(360 / angleStep); //why ? 
    //     }
    //
    //     
    //     if (!Mathf.Approximately(_angle, angleStep))
    //     {
    //         _angle = angleStep;
    //         _startAngleOffset = angleStep / 2; 
    //     }
    //    
    //     offset += _startAngleOffset;
    //
    //     if (allGuis.Count == 1)
    //     {
    //         _angleOffset = 0;
    //         if(allGuis[0].transform.localPosition != Vector3.zero) allGuis[0].transform.localPosition = Vector3.zero;
    //         return;
    //     }
    //
    //     for (int i = 0; i < numberOfElements; i++)
    //     {
    //         SetGuiPositionInCircle(allGuis[i], radius ,i * angleStep , offset);
    //     }
    // }
    //
    //
    // public void SetGuiPositionInCircle(GameObject gui, float radius, float angleStep, float angleOffset) 
    // {
    //     
    //     //todo add support for different axis 
    //     
    //     gui.transform.localPosition = Vector3.zero;
    //
    //     float angle = angleStep * Mathf.Deg2Rad; // Convert angle to radians.
    //     angle += angleOffset * Mathf.Deg2Rad;
    //
    //
    //     //Calculate the position of the GUI element in the circle.
    //     
    //     Vector3 offset =  new Vector3(0f, 0f, 0f);
    //     
    //     switch (axis)
    //     {
    //         case Axis.Up:
    //             float yUp = radius * Mathf.Cos(angle);
    //             float zUp = radius * Mathf.Sin(angle);
    //             offset = new Vector3(0f, yUp, zUp);
    //             break;
    //         case Axis.Side:
    //             float xSide = radius * Mathf.Cos(angle);
    //             float zSide = radius * Mathf.Sin(angle);
    //             offset = new Vector3(xSide, 0f, zSide);
    //             break;
    //         case Axis.Front:
    //             float xFront = radius * Mathf.Cos(angle);
    //             float yFront = radius * Mathf.Sin(angle);
    //             offset = new Vector3(xFront, yFront, 0f);
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException();
    //     }
    //     
    //     // Calculate the position of the GUI element in the circle.
    //     // float x = radius * Mathf.Cos(angle);
    //     // float z = radius * Mathf.Sin(angle);
    //
    //     Vector3 newPosition = gui.transform.localPosition + offset;
    //     
    //     gui.transform.localPosition = newPosition;
    // }
    //
    // private List<GameObject> GetGuisInTransform()
    // {
    //     var guis = panels; // GetComponentsInChildren<UIPanel>();
    //     List<GameObject> guisList = new List<GameObject>();
    //
    //     if(guis.Count== 0 ) return guisList;
    //     foreach (var gui in guis) 
    //     {
    //         if (!gui) continue;
    //         gui.Container = this;
    //         guisList.Add(gui.gameObject);
    //         
    //     }
    //
    //     return guisList;    
    // }
    //
    // private List<GameObject> GetGuisToShow() => GetGuisToShow(Guis, _indexCurrentGui, _indexToShow);
    //
    // private List<GameObject> GetGuisToShow(List<GameObject> objs ,int middleIndex , int index = 1)
    // {
    //     List<GameObject> guistoShow = new List<GameObject>();
    //     
    //     if (index < 0)
    //     {
    //         return objs;
    //     }
    //
    //     if (index == 0)
    //     {
    //         var middle =GetGuiByIndex(objs,middleIndex);
    //        guistoShow.Add(middle);
    //        return guistoShow;
    //     }
    //
    //     var indexList = new List<int>();
    //     int listSize = guistoShow.Count;
    //
    //     // If index is greater than or equal to half the list size, toggle the whole list
    //     if (index >= listSize / 2)
    //     {
    //         return objs;
    //     }
    //    
    //     // Get all indexes between minIndex and maxIndex to be toggled
    //     var minIndex = middleIndex - index;
    //     var maxIndex = middleIndex + index;
    //
    //     // Ensure indexes are within the list range, wrapping around if necessary
    //     for (int i = minIndex; i <= maxIndex; i++)
    //     {
    //         int wrappedIndex = (i + listSize) % listSize; // This wraps around the list
    //         indexList.Add(wrappedIndex);
    //     }
    //     
    //     foreach (var i in indexList)
    //     {
    //         var gui = GetGuiByIndex(objs, i);
    //         
    //         if(!gui) continue;
    //         guistoShow.Add(gui);
    //     }
    //    
    //     
    //     // guistoShow.Add(_previousGui);
    //     // guistoShow.Add(_nextGui);
    //     // guistoShow.Add(_currentGui);
    //
    //    
    //     return guistoShow;
    // }
    //
    //
    // public void UpdateGuisPosition(float value) 
    // {
    //     if (Guis.Count == 0) return ;
    //     
    //     if (Guis.Count == 1)
    //     {
    //         
    //         if (_currentGui != Guis[0]) _currentGui = Guis[0];
    //         OrganizeGuis(Guis, _radius,  _angleOffset = 0, _spread);
    //         SetMaxGuiAlpha(CurrentGui, 1f);
    //         ToggleComponents();
    //         return;
    //     
    //     }
    //     OrganizeGuis(Guis, _radius,  _angleOffset = value, _spread);
    //    
    //     MakeCurrentGui(GetCurrentGuiByPosition(_radius, _angleTolerance = _startAngleOffset));
    //     
    //     SetMaxGuiAlpha(CurrentGui, 1f);
    //
    //     UpdateNeighbouringGuis(true);
    //     HideAllExcept(Guis, GetGuisToShow());
    //
    //     
    //     ToggleComponents();
    //     
    //
    // }
    //
    // public void MoveGuiToCentralPosition(int index)
    // {
    //     //Debug.Log(_guis.Count);
    //     if (Guis.Count == 0|| Guis.Count == 1) return;
    //     //if (_guis.Count == 1) index = 0;
    //     
    //
    //     /* Make sure index is in range*/
    //     if (index > Guis.Count) index %= Guis.Count;
    //     else if( index < 0) index %= -Guis.Count;
    //
    //
    //     int destinationIndex = index;
    //     float offset = (Guis.Count -2) *_startAngleOffset/2; // to have the gui centered - else its a bit offset 
    //    
    //     
    //     //Debug.Log("Destination Index : " + destinationIndex);
    //     float destinationAngle = -(destinationIndex * _angle) + offset;
    //     
    //     /*Count the number of turn  - numberOfTurn > 0 : turn left : numberOfTurn < 0 turn right*/
    //     if (index - _indexCurrentGui == Guis.Count -1 )
    //     {
    //         numberOfTurn++;
    //     }
    //     
    //     else if (index - _indexCurrentGui == -(Guis.Count -1) )
    //     {
    //         numberOfTurn--;
    //     }
    //     
    //    
    //     //Debug.Log("[ScrollGUI] Addoffset to destination angle : " +angleOffset + "* numberofturn : " + numberOfTurn );
    //    
    //     /*Add 360 degres to number when we increment number of turn*/ 
    //     destinationAngle += numberOfTurn*360f;
    //     
    //     
    //     StartCoroutine(LerpGuisToAngle(_angleOffset, destinationAngle, 1/_scrollSpeed));
    // }
    //
    // public void MoveGuiToCentralPosition(GameObject gui) 
    // {
    //     if (Guis.Count != 0)
    //     {
    //         if (!Guis.Contains(gui))
    //         {
    //             AddGuiToWristMenu(gui, true);
    //         }
    //     }
    //
    //
    //     int destinationIndex = Guis.IndexOf(gui);
    //     
    //     MoveGuiToCentralPosition(destinationIndex);
    // }
    //
    // private float CalculateAngle(List<GameObject> allGuis)
    // {
    //     int numberOfElements = allGuis.Count;
    //     float angleStep = 360f / numberOfElements;
    //
    //      return angleStep;
    //    
    // }
    //
    // public IEnumerator LerpGuisToAngle(float currentAngle, float angleDestination,float duration, float delay = 0)
    // {
    //     yield return new WaitForSeconds(delay);
    //
    //     float angle;
    //
    //     float currentTime = 0;
    //   
    //     while (currentTime < duration)
    //     {
    //         
    //        _angleOffset = angle = Mathf.Lerp( currentAngle, angleDestination, currentTime / duration); 
    //         currentTime+=Time.deltaTime;
    //         yield return null;
    //     }
    //     _angleOffset = angleDestination;
    //     _cachedAngleOffset = _angleOffset;
    //    
    // }
    //
    //
    // public void NextGuiToCenter() 
    // {
    //     if (!_nextGui) return;
    //    // MoveGuiToCentralPosition(_indexCurrentGui + 1);
    //     MoveGuiToCentralPosition(_nextGui);
    // }
    //
    // public void PrevGuiToCenter()
    // {
    //     if (!_previousGui) return;
    //     //MoveGuiToCentralPosition(_indexCurrentGui -1);
    //     MoveGuiToCentralPosition(_previousGui);
    // }
    //
    // public void MoveClickedGuiToCenter(GameObject gui) 
    // {
    //     if(!gui) return;
    //    
    //     if(!Guis.Contains(gui)) return;
    //     
    //     if(gui == CurrentGui) return;
    //
    //     MoveGuiToCentralPosition(gui);
    // }
    //
    // public void DestroyWristGui(GameObject gui)
    // {
    //     if(!Guis.Contains(gui)) ;
    //     Guis.Remove(gui);
    //     Destroy(gui);
    //     
    //     CurrentGui =null;
    //     
    // }
    //
    //
    // public void DestroyCurrentGUI() 
    // {
    //     if (!CurrentGui || Guis.Count == 0)
    //     {
    //         Debug.Log("[WristScrollGUI] No gui to close");
    //         return;
    //     }
    //     
    //     if(CurrentGui == _fallBackGui) 
    //     {
    //         Debug.Log("[WristScrollGUI] wont close : is fall back gui ");
    //         return;
    //     }
    //     
    //     if (!GetUIPanel(CurrentGui).IsClosable)
    //     {
    //         Debug.Log("[WristScrollGUI] Gui not Closable");
    //         return;
    //     }
    //
    //    
    //     
    //     DestroyWristGui(CurrentGui);
    //     //CurrentGui = null;
    //     //Debug.Log("Guis count: " + _guis.Count);
    //     
    //     if (Guis.Count == 1)
    //     {
    //        
    //         Debug.Log("[WristScrollGUI] One Gui left");
    //        
    //         UpdateGuisPosition(_angleOffset = 0);
    //         SetMaxGuiAlpha(CurrentGui, 1, false);
    //         numberOfTurn = 0;
    //         //Debug.Log("Last Guis : " + CurrentGui);
    //     }
    //
    //  
    //
    // }
    //
    //  private void ResetGuiExiting(SelectExitEventArgs obj)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // private void AddToWristMenu(SelectExitEventArgs obj)
    // {
    //     var gui = obj.interactableObject.transform.gameObject.GetComponent<UIPanel>();
    //     if(!gui) return;
    //     if(!gui.IsDocked) return;
    //     
    //     
    //     ToggleAddedGuiShadow(false, gui);
    //
    //     if (GUIManager.Instance.guisInWord.Contains(gui.gameObject))
    //         GUIManager.Instance.guisInWord.Remove(gui.gameObject);
    //     
    //     
    //     MoveGuiToCentralPosition(gui.gameObject);
    //     UpdateGuisPosition(_angleOffset);
    //     var grabHelper = obj.interactableObject.transform.gameObject.GetComponent<GrabHelper>();
    //    
    //     if(grabHelper) grabHelper.OnThisObjectRelease -= AddToWristMenu;
    //
    // }
    //
    // private IEnumerator ResetGuiExiting(float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //     guiExiting.Container = null;
    //     guiExiting = null;
    //     CurrentGui = null;
    //
    // }
    //
    // private void ToggleAddedGuiShadow(bool state, UIPanel guiInTrigger)
    // {
    //     Debug.Log("Shadow ui");
    //     /*Make shadow gui current one */
    //     if (!_guiShadow && state)
    //     {
    //         CurrentGui = GuiShadow;
    //     }
    //
    //     /*Enable or disable Gui visual*/
    //     guiInTrigger.AlphaControlComponent.GetComponent<Canvas>().enabled = !state;
    //     var renderers = guiInTrigger.GetComponents<Renderer>();
    //     if (renderers.Length > 0)
    //     {
    //         foreach (var renderer in renderers)
    //         {
    //             renderer.enabled = !state;
    //         }
    //     }
    //
    //     /*If we have a line visual, move the attach point - using XRItoolkit*/
    //     var lineAttach =
    //         FindChildObjectByName(state ? guiInTrigger.transform : _guiShadow ? _guiShadow.transform : null,
    //             "[Ray Interactor] Dynamic Attach");
    //
    //     if (lineAttach)
    //     {
    //         lineAttach.transform.parent = state ? _guiShadow.transform : guiInTrigger.transform;
    //         lineAttach.transform.localPosition = state ? Vector3.zero :
    //             guiInTrigger.SnapVolume ? guiInTrigger.SnapVolume.transform.localPosition : Vector3.zero;
    //     }
    //
    //
    //     /*Destroy the shadow gui if not needed*/
    //     if (CurrentGui == _guiShadow && !state)
    //     {
    //         DestroyCurrentGUI();
    //     }
    // }
    //
    // public GameObject FindChildObjectByName(Transform parentTransform, string objectName)
    // {
    //     if (!parentTransform) return null;
    //     Transform childTransform = parentTransform.Find(objectName);
    //
    //     if (childTransform != null)
    //     {
    //         return childTransform.gameObject;
    //     }
    //
    //     return null;
    // }
    //
    //
    //
    // private void ToggleComponents()
    // {
    //     if(Guis.Count == 0) return;
    //     List<Type> comps = new List<Type>() { typeof(XRBaseInteractable), typeof(TrackedDeviceGraphicRaycaster) };
    //
    //      // disable grabbing and interaction on guis that are not current
    //     foreach (var gui in Guis)
    //     {
    //        ToggleComponents(comps, CurrentGui == gui, gui);
    //     }
    //    
    //    
    //
    //     
    // }
    //
    // private void ToggleComponents(List<Type> comps, bool state, List<GameObject> gos)
    // {
    //     if(comps.Count ==0) return;
    //
    //     foreach (var go in gos)
    //     {
    //         ToggleComponents(comps, state, go);
    //     }
    //     
    // }
    //
    // private void ToggleComponents(List<Type> comps, bool state, GameObject go)
    // {
    //     if(comps.Count == 0) return;
    //
    //     if(!go) return;
    //     
    //     foreach (var comp in comps)
    //     {
    //         ToggleComponent(comp, state, go);
    //     }
    //     
    // }
    //
    // private void ToggleComponents(Type comp, bool state, List<GameObject> gos)
    // {
    //     ToggleComponents(new List<Type>(){comp}, state, gos);
    // }
    //
    // private  void ToggleComponent(Type comp,  bool state, GameObject go)
    // {
    //     if (go == null) return;
    //     
    //     if (!typeof(Component).IsAssignableFrom(comp)) return;
    //
    //     var script = go.GetComponent(comp);
    //     script = go.GetComponentInChildren(comp);
    //     if(!script) return;
    //     
    //     var monoScript = script as MonoBehaviour;
    //     if(!monoScript) return;
    //
    //     monoScript.enabled = state;
    // }
    //
    //
    // protected override void OnInitialize()
    // {
    //    //
    // }
    //
    // protected override void OnAwake()
    // {
    //     Guis = GetGuisInTransform();
    //    
    //     ToggleComponents(typeof(TurnToFace), false, Guis);
    // }
    //
    // protected override void OnStart()
    // {
    //     _indexCurrentGui = _startIndex;
    //
    //     if (Guis.Count > 0) CurrentGui = GetGuiByIndex(_guis, _indexCurrentGui);
    //     else CurrentGui = FallBackGui;
    //     
    //     //UpdateGuisPosition(_angleOffset);
    //     //HideAllExcept(Guis, GetGuisToShow());
    // }
    //
    //
    //
    // private void OnEnable()
    // {
    //     UpdateGuisPosition(_angleOffset);
    //     HideAllExcept(Guis, GetGuisToShow());
    // }
    //
    // private void OnDisable()
    // {
    //     HideAll();
    // }
    //
    //
    //
    // void Update()
    // {  
    //     if (useIndex && indexToFront != _indexCurrentGui) MoveGuiToCentralPosition(indexToFront);
    //       
    //     if ( Mathf.Approximately(_angleOffset, _cachedAngleOffset)) UpdateGuisPosition(_angleOffset); //TODO: move to a async
    // }
    //
    //
    // private void OnTriggerEnter(Collider other)
    // {
    //     Debug.Log("Trigger Enter");
    //     
    //    var guiEnteringTrigger = other.GetComponent<UIPanel>();
    //    if(!guiEnteringTrigger) return;
    //    
    //    if(Guis.Contains(guiEnteringTrigger.gameObject) ) return;
    //    if(guiEnteringTrigger.IsDocked) return;
    //    if(!guiEnteringTrigger.IsDockable) return; 
    //    
    //    
    //    var grabHelper = guiEnteringTrigger.GetComponent<GrabHelper>();
    //    if (grabHelper)
    //    {
    //        if(!grabHelper.IsGrabbed) return;
    //        grabHelper.OnThisObjectRelease += AddToWristMenu;
    //    }
    //
    //    
    //    guiEntering = guiEnteringTrigger;
    //    guiEntering.Container = this;
    //
    //    ToggleAddedGuiShadow(true, guiEnteringTrigger);
    //    //need to add a listener if object is released to add it to wrist menu
    // }
    //
    //
    // private void OnTriggerExit(Collider other)
    // {
    //     var guiExitingTrigger = other.GetComponent<UIPanel>();
    //     if(!guiExitingTrigger) return;
    //     if(!guiExitingTrigger.IsDockable) return;
    //     
    //     //Debug.Log("Exit");
    //     var grabHelper = guiExitingTrigger.GetComponent<GrabHelper>();
    //     
    //     if(grabHelper)
    //         if(!grabHelper.IsGrabbed) return;
    //     
    //     if (Guis.Contains(guiExitingTrigger.gameObject))
    //     {
    //         guiExiting = guiExitingTrigger;
    //         
    //       //if (grabHelper) grabHelper.OnThisObjectRelease += ResetGuiExiting;
    //         
    //         var guiTransform = guiExiting.transform;
    //         guiTransform.parent = GUIManager.Instance.UIsContainer.transform;
    //         guiTransform.localRotation = Quaternion.identity;
    //         guiTransform.localEulerAngles += new Vector3(0, 180, 0); 
    //         
    //         //guiTransform.localScale = Vector3.one;
    //         var turnToFace = guiExitingTrigger.GetComponent<TurnToFace>();
    //         if (turnToFace) turnToFace.enabled = true;
    //         
    //         GUIManager.Instance.guisInWord.Add(guiExitingTrigger.gameObject);
    //         
    //         
    //         Guis.Remove(guiExitingTrigger.gameObject);
    //         PrevGuiToCenter();
    //
    //         StartCoroutine(ResetGuiExiting(.1f)); 
    //         
    //         return;
    //     }
    //
    //     guiExitingTrigger.Container = null;
    //     ToggleAddedGuiShadow(false, guiExitingTrigger);
    //     //if(guiEnteringTrigger)
    // }