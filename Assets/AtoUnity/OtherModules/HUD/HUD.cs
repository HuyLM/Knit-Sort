using AtoGame.Base.UnityInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AtoGame.OtherModules.HUD
{
    [DisallowMultipleComponent]
    public class HUD : MonoBehaviour
    {
        [SerializeField] private int order;
        [Header("[Frames]")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.AssetsOnly]
#endif
        [SerializeField] private Frame[] prefabFrames;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.SceneObjectsOnly]
#endif
        [SerializeField] private Frame[] onSceneFrames;

        [SerializeField] private Frame defaulFrame;
        [SerializeField] private Transform container;
        [SerializeField] private bool useBackToPrevious;
        [SerializeField] private bool mustHaveAFrame;

        [ReadOnly]
        [SerializeField] private List<Frame> loadedFrames = new List<Frame>();
        [ReadOnly]
        [SerializeField] private readonly List<Frame> activeFrames = new List<Frame>();
        [ReadOnly]
        [SerializeField] private Stack<Frame> previousFrames = new Stack<Frame>();
        private bool active;
		
		public bool IsActive => active;

        protected virtual void Awake()
        {
            if (container == null)
            {
                container = this.transform;
            }
            LoadOnSceneFrames();
        }

        private void LoadOnSceneFrames()
        {
            if (onSceneFrames == null || onSceneFrames.Length == 0)
            {
                return;
            }
            for (int i = 0; i < onSceneFrames.Length; ++i)
            {
                Frame frame = onSceneFrames[i];
                if (frame == null)
                {
                    continue;
                }
                if (frame.Initialized == false)
                {
                    InitializeFrame(frame);
                }
                loadedFrames.Add(frame);
            }
        }

        protected virtual void Start()
        {
            active = true;
            if (defaulFrame != null)
            {
                if (loadedFrames.Contains(defaulFrame) == false)
                {
                    Frame newFrame = null;
                    if (onSceneFrames.Contains(defaulFrame))
                    {
                        newFrame = defaulFrame;
                    }
                    else
                    {
						defaulFrame.gameObject.SetActive(false);
                        newFrame = Instantiate(defaulFrame, container);
						defaulFrame.gameObject.SetActive(true);
                    }
                    if (newFrame != null)
                    {
                        loadedFrames.Add(newFrame);
                        if (!newFrame.Initialized)
                        {
                            InitializeFrame(newFrame);
                        }
                    }
                    Show(newFrame);
                }
                else
                {
                    Show(defaulFrame);
                }
            }
        }

        protected virtual void OnDestroy()
        {
            foreach (Frame frame in loadedFrames)
            {
                if (frame == null)
                    continue;
            }
        }

        protected virtual void OnEnable()
        {
            HUDManager.Instance.Add(this);
        }

        protected virtual void OnDisable()
        {
            if (HUDManager.Initialized)
            {
                HUDManager.Instance.Remove(this);
            }
        }

        protected virtual void InitializeFrame(Frame frame)
        {
            if (frame == null && frame.Initialized)
                return;

            frame.gameObject.SetActive(false);
            frame.Initialize(this);
        }

        public bool BackToPrevious(Action onCompleted)
        {
            if(useBackToPrevious)
            {
                if (mustHaveAFrame && activeFrames.Count <= 1)
                {
                    return false;
                }
                // Hide Current;
                Hide(()=> {
                    // Show Previous frame
                    if (previousFrames.Count == 0)
                    {
                        return;
                    }
                    Frame previousFrame = previousFrames.Pop();
                    if (previousFrame.IsHidding)
                    {
                        Show(previousFrame, onCompleted, false, false, false);
                    }
                    else if (previousFrame.IsPausing)
                    {
                        Resume(previousFrame, onCompleted, false);
                    }
                }, false, false);
                return true;
            }
            return false;
        }

        public virtual bool Back()
        {
            if(active == false)
            {
                return false;
            }
            Frame frameOnTop = GetFrameOnTop();
            if (frameOnTop == null)
            {
                return false;
            }
            return frameOnTop.Back();
        }

    

        public void SetActive(bool active)
        {
            this.active = active;
        }

        #region Get States
        public int Order { get => order; }

        public int GetActiveFrameCount()
        {
            return activeFrames.Count;
        }

        public Frame GetFrameOnTop()
        {
            if(activeFrames.Count < 1)
                return null;
            for(int i = activeFrames.Count - 1; i >= 0; --i)
            {
                Frame frame = activeFrames[i];
                if(frame != null && frame.CanOnTop())
                {
                    return frame;
                }
            }
            return null;
        }

        public F GetFrameOnTop<F>() where F : Frame
        {
            return GetFrameOnTop() as F;
        }

        public bool IsFrameOnTop(Frame target)
        {
            return GetFrameOnTop() == target;
        }

        public IEnumerable<Frame> GetFrames()
        {
            foreach (Frame frame in prefabFrames)
            {
                yield return frame;
            }
        }

        public IEnumerable<Frame> GetLoadedFrames()
        {
            foreach (Frame frame in loadedFrames)
            {
                yield return frame;
            }
        }

        public IEnumerable<Frame> GetActiveFrames()
        {
            foreach (Frame frame in activeFrames)
            {
                yield return frame;
            }
        }

        public F GetFrame<F>() where F : Frame
        {
            foreach (Frame frame in GetLoadedFrames())
            {
                if (frame is F)
                {
                    if (!frame.GetType().IsSubclassOf(typeof(F)))
                    {
                        return frame as F;
                    }
                }
            }

            foreach (Frame frame in GetFrames())
            {
                if (frame is F)
                {
                    if (!frame.GetType().IsSubclassOf(typeof(F)))
                    {
						frame.gameObject.SetActive(false);
                        Frame loadFrame = Instantiate(frame, container);
						frame.gameObject.SetActive(true);
                        if (loadFrame != null)
                        {
                            loadedFrames.Add(loadFrame);
                            if (!loadFrame.Initialized)
                            {
                                InitializeFrame(loadFrame);
                            }
                            return loadFrame as F;
                        }
                    }
                }
            }

            Debug.LogError($"[Frame] {typeof(F).Name} can't load");
            return null;
        }

        public F GetActiveFrame<F>() where F : Frame
        {
            foreach (Frame frame in GetActiveFrames())
            {
                if (frame is F)
                {
                    return frame as F;
                }
            }
            return null;
        }

        public bool ContainsFrame<F>() where F : Frame
        {
            foreach (Frame frame in GetLoadedFrames())
            {
                if (frame is F)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsFrame(Frame target)
        {
            foreach (Frame frame in GetLoadedFrames())
            {
                if (frame == target)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsActiveFrame<F>() where F : Frame
        {
            foreach (Frame frame in GetActiveFrames())
            {
                if (frame is F)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ContainsActiveFrame(Frame target)
        {
            foreach (Frame frame in GetActiveFrames())
            {
                if (frame == target)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        #region Show & Hide & Pause & Resume

        public F Show<F>(Action onCompleted = null, bool instant = false, bool hideCurrent = false, bool pauseCurrent = false) where F : Frame
        {
            return Show(GetFrame<F>(), onCompleted, instant, hideCurrent, pauseCurrent) as F;
        }

        public virtual Frame Show(Frame frame, Action onCompleted = null, bool instant = false, bool hideCurrent = false, bool pauseCurrent = false)
        {
            if (active == false)
            {
                return null;
            }
            if (frame == null)
                return null;
            if (IsFrameOnTop(frame))
                return frame;

            if (!frame.Initialized)
            {
                InitializeFrame(frame);
            }

            if (frame.Hud != this)
            {
                Debug.LogWarningFormat("[HUD] The trying to open a frame {0} is not within the scope of the current hud {1}.", frame.name, GetType().Name);
                return null;
            }

            if (ContainsActiveFrame(frame))
            {
                Debug.LogWarningFormat("[HUD] The trying to open a frame {0} has been opened before.", frame.name);
                activeFrames.Remove(frame);
            }

            if (hideCurrent)
            {
                Hide();
            }
            else if (pauseCurrent)
            {
                Pause();
            }

            frame.transform.SetAsLastSibling();
            activeFrames.Add(frame);
            return frame.ShowByHUD(onCompleted, instant);
        }

        public F Hide<F>(Action onCompleted = null, bool instant = false, bool addToPrevious = true) where F : Frame
        {
            return Hide(GetFrame<F>(), onCompleted, instant, addToPrevious) as F;
        }

        public Frame Hide(Action onCompleted = null, bool instant = false, bool addToPrevious = true)
        {
            return Hide(GetFrameOnTop(), onCompleted, instant, addToPrevious);
        }

        public virtual Frame Hide(Frame frame, Action onCompleted = null, bool instant = false, bool addToPrevious = true)
        {
            if (active == false)
            {
                return null;
            }
            if (frame == null)
                return null;
            if (!ContainsActiveFrame(frame))
            {
                Debug.LogWarningFormat("[HUD] The frame {0} has not been opened before.", frame.name);
                return null;
            }

            if (useBackToPrevious && addToPrevious)
            {
                if (previousFrames.Count > 0)
                {
                    Frame previousFrame = previousFrames.Peek();
                    if (previousFrame != frame)
                    {
                        previousFrames.Push(frame);
                    }
                }
                else
                {
                    previousFrames.Push(frame);
                }
            }

            activeFrames.Remove(frame);
            return frame.HideByHUD(onCompleted, instant);
        }

        public virtual Frame Pause(Frame frame, Action onCompleted = null, bool instant = false, bool addToPrevious = true)
        {
            if (active == false)
            {
                return null;
            }
            if (frame == null)
                return null;
            if (!ContainsActiveFrame(frame))
            {
                Debug.LogWarningFormat("[HUD] The frame {0} has not been opened before.", frame.name);
                return null;
            }

            if (useBackToPrevious && addToPrevious)
            {
                if (previousFrames.Count > 0)
                {
                    Frame previousFrame = previousFrames.Peek();
                    if (previousFrame != frame)
                    {
                        previousFrames.Push(frame);
                    }
                }
                else
                {
                    previousFrames.Push(frame);
                }
            }

            return frame.PauseByHUD(onCompleted, instant);
        }

        public virtual Frame Resume(Frame frame, Action onCompleted = null, bool instant = false)
        {
            if (active == false)
            {
                return null;
            }
            if (frame == null)
                return null;
            if (!ContainsActiveFrame(frame))
            {
                Debug.LogWarningFormat("[HUD] The frame {0} has not been opened before.", frame.name);
                return null;
            }

            return frame.ResumeByHUD(onCompleted, instant);
        }

        public int PauseAll()
        {
            if (active == false)
            {
                return 0;
            }
            int hideCount = activeFrames.Count;
            for (int i = activeFrames.Count - 1; i >= 0; i--)
            {
                activeFrames[i].PauseByHUD(null, true);
            }
            return hideCount;
        }

        public int HideAll()
        {
            if (active == false)
            {
                return 0;
            }
            int hideCount = activeFrames.Count;
            for (int i = activeFrames.Count - 1; i >= 0; i--)
            {
                activeFrames[i].HideByHUD(null, true);
            }
            activeFrames.Clear();
            if (useBackToPrevious)
            {
                previousFrames.Clear();
            }
            return hideCount;
        }

        #endregion

        #region Extensions

        public void Show(Frame frame)
        {
            Show(frame, null);
        }

        public void HideAndShow(Frame frame)
        {
            Show(frame, null, false, true);
        }

        public void PauseAndShow(Frame frame)
        {
            Show(frame, null, false, false, true);
        }

        public void HideAndShowInstant(Frame frame)
        {
            Show(frame, null, true, true);
        }

        public void PauseAndShowInstant(Frame frame)
        {
            Show(frame, null, true, false, true);
        }

        public void Hide(Frame frame)
        {
            Hide(frame, null, false, true);
        }

        public void HideInstant(Frame frame)
        {
            Hide(frame, null, true, true);
        }

        public void Hide()
        {
            Hide(null, false);
        }

        public void Pause(Frame frame)
        {
            Pause(frame, null, false);
        }

        public void Pause()
        {
            Pause(GetFrameOnTop(), null, false);
        }

        public void Resume(Frame frame)
        {
            Resume(frame, null, false);
        }

        public void Resume()
        {
            Resume(GetFrameOnTop(), null, false);
        }

        #endregion
    }

    public abstract class HUD<T> : HUD where T : HUD<T>
    {
        private static T instance;

        public static bool HasInstance => instance != null;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if (instance == null)
                    {
                        Debug.LogErrorFormat("[SINGLETON] Class {0} must be added to scene before run!", typeof(T));
                    }
                }
                return instance;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (instance == null)
            {
                instance = this as T;
            }
            else if (instance != this)
            {
                Debug.LogWarningFormat("[SINGLETON] Class {0} is initialized multiple times!", typeof(T));
                Destroy(this.gameObject);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            instance = null;
        }
    }
}
