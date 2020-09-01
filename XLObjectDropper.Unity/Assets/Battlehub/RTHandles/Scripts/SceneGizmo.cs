using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Events;
using UnityEngine.UI;
using Battlehub.Utils;
using Battlehub.RTCommon;
using TMPro;

namespace Battlehub.RTHandles
{
    [RequireComponent(typeof(Camera))]
    public class SceneGizmo : RTEComponent
    {
        public Button BtnProjection;
        public Transform Pivot;
        public Vector2 Size = new Vector2(96, 96);
        public Vector2 Offset = new Vector2(0, 0);
        public Vector3 Up = Vector3.up;
        public RuntimeHandlesComponent Appearance;
        public RectTransform m_output;
        private CanvasScaler m_canvasScaler;
        private Canvas m_canvas;

        public UnityEvent OrientationChanging;
        public UnityEvent OrientationChanged;
        public UnityEvent ProjectionChanged;

        private float m_scale;
        private Rect m_cameraPixelRect;
        private float m_aspect;
        private Camera m_camera;
        private RenderTextureCamera m_renderTextureCamera;

        private float m_xAlpha = 1.0f;
        private float m_yAlpha = 1.0f;
        private float m_zAlpha = 1.0f;
        private float m_animationDuration = 0.2f;

        private GUIStyle m_buttonStyle;
        private Rect m_buttonRect;

        private bool m_mouseOver;
        private Vector3 m_selectedAxis;
        private GameObject m_collidersGO;
        private BoxCollider m_colliderProj;
        private BoxCollider m_colliderUp;
        private BoxCollider m_colliderDown;
        private BoxCollider m_colliderForward;
        private BoxCollider m_colliderBackward;
        private BoxCollider m_colliderLeft;
        private BoxCollider m_colliderRight;
        private Collider[] m_colliders;

        private Vector3 m_position;
        private Quaternion m_rotation;
        private Vector3 m_gizmoPosition;
        private IAnimationInfo m_rotateAnimation;
        
        private float m_screenHeight;
        private float m_screenWidth;

        public bool IsOrthographic
        {
            get { return m_camera.orthographic; }
            set
            {
                m_camera.orthographic = value;
                Window.Camera.orthographic = value;

                if(BtnProjection != null)
                {
                    Text txt = BtnProjection.GetComponentInChildren<Text>();
                    if (txt != null)
                    {
                        if (value)
                        {
                            txt.text = "Ortho";
                        }
                        else
                        {
                            txt.text = "Persp";
                        }
                    }
                    else
                    {
                        TextMeshProUGUI txtPro = BtnProjection.GetComponentInChildren<TextMeshProUGUI>();
                        if(txtPro != null)
                        {
                            if(value)
                            {
                                txtPro.text = "Ortho";
                            }
                            else
                            {
                                txtPro.text = "Persp";
                            }
                        }
                    }
                }

         
                if (ProjectionChanged != null)
                {
                    ProjectionChanged.Invoke();
                    InitColliders();
                }
            }
        }

        [SerializeField]
        private Color m_textColor = Color.white;

        public Color TextColor
        {
            get { return m_textColor; }
            set
            {
                m_textColor = value;
                SetTextColor();
            }
        }

        private void SetTextColor()
        {
            if (BtnProjection != null)
            {
                Text txt = BtnProjection.GetComponentInChildren<Text>();
                if (txt != null)
                {
                    txt.color = m_textColor;
                }

                TextMeshProUGUI txtPro = BtnProjection.GetComponentInChildren<TextMeshProUGUI>();
                if(txtPro != null)
                {
                    txtPro.color = m_textColor;
                }
            }
        }

        protected override void AwakeOverride()
        {
            base.AwakeOverride();
        
            RuntimeHandlesComponent.InitializeIfRequired(ref Appearance);

            if (Pivot == null)
            {
                Pivot = transform;
            }

            m_collidersGO = new GameObject();
            m_collidersGO.transform.SetParent(transform, false);
            m_collidersGO.transform.position = GetGizmoPosition();
            m_collidersGO.transform.rotation = Quaternion.identity;
            m_collidersGO.name = "Colliders";

            m_colliderProj = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderUp = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderDown = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderLeft = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderRight = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderForward = m_collidersGO.AddComponent<BoxCollider>();
            m_colliderBackward = m_collidersGO.AddComponent<BoxCollider>();

            m_colliders = new[] { m_colliderProj, m_colliderUp, m_colliderDown, m_colliderRight, m_colliderLeft, m_colliderForward, m_colliderBackward };
            DisableColliders();

            m_camera = GetComponent<Camera>();
            m_renderTextureCamera = GetComponent<RenderTextureCamera>();
            if (m_renderTextureCamera == null)
            {
                m_camera.clearFlags = CameraClearFlags.Depth;
                m_camera.renderingPath = RenderingPath.Forward;
                m_camera.allowMSAA = false;
                m_camera.allowHDR = false;  
                
                if(BtnProjection != null)
                {
                    m_canvas = BtnProjection.GetComponentInParent<Canvas>();
                }
            }
            else
            {
                if (RenderPipelineInfo.Type != RPType.Standard)
                {
                    m_camera.allowMSAA = false;
                    m_camera.allowHDR = false;
                }

                m_camera.clearFlags = CameraClearFlags.SolidColor;
                m_camera.backgroundColor = new Color(0, 0, 0, 0);
                if (m_output != null)
                {
                    Vector2 size = Size;
                    m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                    m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

                    m_canvasScaler = m_output.GetComponentInParent<CanvasScaler>();
                    m_canvas = m_output.GetComponentInParent<Canvas>();
                }
            }

            m_camera.cullingMask = 0;
            m_camera.orthographic = Window.Camera.orthographic;

            m_screenHeight = Screen.height;
            m_screenWidth = Screen.width;

            UpdateLayout();
            InitColliders();
            UpdateAlpha(ref m_xAlpha, Vector3.right, 1);
            UpdateAlpha(ref m_yAlpha, Vector3.up, 1);
            UpdateAlpha(ref m_zAlpha, Vector3.forward, 1);
            if (Run.Instance == null)
            {
                GameObject runGO = new GameObject();
                runGO.name = "Run";
                runGO.AddComponent<Run>();
            }

            if (BtnProjection != null)
            {
                BtnProjection.onClick.AddListener(OnBtnModeClick);
                SetTextColor();
            }

            if(!GetComponent<SceneGizmoInput>())
            {
                gameObject.AddComponent<SceneGizmoInput>();
            }
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
            #endif
        }

        protected virtual void Start()
        {
            if(IsOrthographic != Window.Camera.orthographic)
            {
                IsOrthographic = Window.Camera.orthographic;
            }

        }

        protected override void OnDestroyOverride()
        {
            base.OnDestroyOverride();
        
            if (BtnProjection != null)
            {
                BtnProjection.onClick.RemoveListener(OnBtnModeClick);
            }
            if (Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                if(Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }
            #if UNITY_2019_1_OR_NEWER
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            #endif
        }

        protected virtual void OnEnable()
        {
            if(m_output != null)
            {
                m_output.gameObject.SetActive(true);
            }

            if(BtnProjection != null)
            {
                BtnProjection.gameObject.SetActive(true);
            }
        }

        protected virtual void OnDisable()
        {
            if (m_output != null)
            {
                m_output.gameObject.SetActive(false);
            }

            if (BtnProjection != null)
            {
                BtnProjection.gameObject.SetActive(false);
            }
        }

        private void OnBtnModeClick()
        {
            IsOrthographic = !Window.Camera.orthographic;
        }

#if UNITY_2019_1_OR_NEWER
        private void OnEndCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            if(m_camera == camera)
            {
                Appearance.DoSceneGizmo(camera, GetGizmoPosition(), Quaternion.identity, m_selectedAxis, Size.y * Appearance.SceneGizmoScale / 96, m_textColor, m_xAlpha, m_yAlpha, m_zAlpha);
            }
        }
#endif

        private void OnPostRender()
        {
            Appearance.DoSceneGizmo(Camera.current, GetGizmoPosition(), Quaternion.identity, m_selectedAxis, Size.y * Appearance.SceneGizmoScale / 96, m_textColor,  m_xAlpha, m_yAlpha, m_zAlpha);
        }

        private void OnGUI()
        {
            if(BtnProjection != null)
            {
                return;
            }
            if (Window.Camera.orthographic)
            {
                if (GUI.Button(m_buttonRect, "Ortho", m_buttonStyle))
                {
                    IsOrthographic = false;
                }
            }
            else
            {
                if (GUI.Button(m_buttonRect, "Persp", m_buttonStyle))
                {
                    IsOrthographic = true;
                }
            }
        }

        protected override void OnWindowDeactivated()
        {
            base.OnWindowDeactivated();
            if(Editor != null && Editor.Tools != null && Editor.Tools.ActiveTool == this)
            {
                Editor.Tools.ActiveTool = null;
            }
        }

        private void Update()
        {
            Sync();

            float delta = Time.deltaTime / m_animationDuration;
            bool updateAlpha = UpdateAlpha(ref m_xAlpha, Vector3.right, delta);
            updateAlpha |= UpdateAlpha(ref m_yAlpha, Vector3.up, delta);
            updateAlpha |= UpdateAlpha(ref m_zAlpha, Vector3.forward, delta);


            if (Editor.Tools.IsViewing)
            {
                m_selectedAxis = Vector3.zero;
                return;
            }

            if(Editor.Tools.ActiveTool != null && Editor.Tools.ActiveTool != this)
            {
                m_selectedAxis = Vector3.zero;
                return;
            }

            Vector2 guiMousePositon = Window.Pointer.ScreenPoint;
            guiMousePositon.y = Screen.height - guiMousePositon.y;
            bool isMouseOverButton = BtnProjection != null ?
                RectTransformUtility.RectangleContainsScreenPoint((RectTransform)BtnProjection.transform, m_editor.Input.GetPointerXY(0), m_canvas.worldCamera)  : 
                m_buttonRect.Contains(guiMousePositon, true);
            if(isMouseOverButton)
            {
                Editor.Tools.ActiveTool = this;
            }
            else
            {
                if(Editor.Tools.ActiveTool == this)
                {
                    Editor.Tools.ActiveTool = null;
                }
            }

            bool pointerOverSceneGizmo;
            if(m_renderTextureCamera == null)
            {
                pointerOverSceneGizmo = m_camera.pixelRect.Contains(Window.Pointer.ScreenPoint);
            }
            else
            {
                pointerOverSceneGizmo = RectTransformUtility.RectangleContainsScreenPoint(m_renderTextureCamera.RectTransform, m_editor.Input.GetPointerXY(0), m_canvas.worldCamera);
            }

            if (pointerOverSceneGizmo && Editor.ActiveWindow == Window && Window.IsPointerOver)
            {
                if (!m_mouseOver || updateAlpha)
                {
                    InitColliders();
                    EnableColliders();
                }

                Collider collider = HitTest();
                if (collider == null || m_rotateAnimation != null && m_rotateAnimation.InProgress)
                {
                    m_selectedAxis = Vector3.zero;
                }
                else if (collider == m_colliderProj)
                {
                    m_selectedAxis = Vector3.one;
                }
                else if (collider == m_colliderUp)
                {
                    m_selectedAxis = Vector3.up;
                }
                else if (collider == m_colliderDown)
                {
                    m_selectedAxis = Vector3.down;
                }
                else if (collider == m_colliderForward)
                {
                    m_selectedAxis = Vector3.forward;
                }
                else if (collider == m_colliderBackward)
                {
                    m_selectedAxis = Vector3.back;
                }
                else if (collider == m_colliderRight)
                {
                    m_selectedAxis = Vector3.right;
                }
                else if (collider == m_colliderLeft)
                {
                    m_selectedAxis = Vector3.left;
                }


                if (m_selectedAxis != Vector3.zero || isMouseOverButton)
                {
                    Editor.Tools.ActiveTool = this;
                }
                else
                {
                    if(Editor.Tools.ActiveTool == this)
                    {
                        Editor.Tools.ActiveTool = null;
                    }
                }

                m_mouseOver = true;
            }
            else
            {
                if (m_mouseOver)
                {
                    DisableColliders();

                    if(Editor.Tools.ActiveTool == this)
                    {
                        Editor.Tools.ActiveTool = null;
                    }   
                }
                m_selectedAxis = Vector3.zero;
                m_mouseOver = false;
            }
        }

        public void Click()
        {
            if (m_selectedAxis != Vector3.zero)
            {
                if (m_selectedAxis == Vector3.one)
                {
                    IsOrthographic = !IsOrthographic;
                }
                else
                {
                    ChangeOrientation(-m_selectedAxis);
                }
            }
        }

        public void ChangeOrientation(Vector3 axis)
        {
            if (m_rotateAnimation == null || !m_rotateAnimation.InProgress)
            {
                if (OrientationChanging != null)
                {
                    OrientationChanging.Invoke();
                }
            }

            if (m_rotateAnimation != null)
            {
                m_rotateAnimation.Abort();
            }

            Vector3 pivot = Pivot.transform.position;
            Vector3 radiusVector = Vector3.back * (Window.Camera.transform.position - pivot).magnitude;
            Quaternion targetRotation = Quaternion.LookRotation(axis, Up);
            m_rotateAnimation = new QuaternionAnimationInfo(Window.Camera.transform.rotation, targetRotation, 0.4f, QuaternionAnimationInfo.EaseOutCubic,
                (target, value, t, completed) =>
                {
                    Window.Camera.transform.position = pivot + value * radiusVector;
                    Window.Camera.transform.rotation = value;

                    if (completed)
                    {
                        DisableColliders();
                        EnableColliders();

                        if (OrientationChanged != null)
                        {
                            OrientationChanged.Invoke();
                        }
                    }
                });

            Run.Instance.Animation(m_rotateAnimation);
        }

        private void Sync()
        {
            if (m_position != transform.position || m_rotation != transform.rotation)
            {
                InitColliders();
                m_position = transform.position;
                m_rotation = transform.rotation;
            }

            
            if (m_screenHeight != Screen.height || m_screenWidth != Screen.width || m_cameraPixelRect != Window.Camera.pixelRect || m_scale != Appearance.SceneGizmoScale)
            {
                UpdateLayout();
            }

            if (m_aspect != m_camera.aspect)
            {
                if(m_renderTextureCamera == null)
                {
                    Vector2 size = Size * Appearance.SceneGizmoScale;
                    Vector2 offset = Offset * Appearance.SceneGizmoScale;
                    m_camera.pixelRect = new Rect(Window.Camera.pixelRect.min.x + Window.Camera.pixelWidth - size.x + offset.x, Window.Camera.pixelRect.min.y + Window.Camera.pixelHeight - size.y + offset.y, size.x, size.y);
                }
                else
                {
                    if(m_output != null)
                    {
                        m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x);
                        m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y);
                    }
                }

                m_aspect = m_camera.aspect;
            }

            if(m_camera.depth != Window.Camera.depth + 1)
            {
                m_camera.depth = Window.Camera.depth + 1;
            }
            m_camera.transform.rotation = Window.Camera.transform.rotation;
        }

        private void EnableColliders()
        {
            m_colliderProj.enabled = true;
            if (m_zAlpha == 1)
            {
                m_colliderForward.enabled = true;
                m_colliderBackward.enabled = true;
            }
            if (m_yAlpha == 1)
            {
                m_colliderUp.enabled = true;
                m_colliderDown.enabled = true;
            }
            if (m_xAlpha == 1)
            {
                m_colliderRight.enabled = true;
                m_colliderLeft.enabled = true;
            }
        }


        private void DisableColliders()
        {
            for (int i = 0; i < m_colliders.Length; ++i)
            {
                m_colliders[i].enabled = false;
            }
        }

        private Vector2 ScreenPointToViewPoint(Vector2 screenPoint)
        {
            if (m_renderTextureCamera == null)
            {
                return screenPoint;
            }

            Vector2 viewPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(m_renderTextureCamera.RectTransform, screenPoint, m_renderTextureCamera.Canvas.worldCamera, out viewPoint);

            if (m_canvasScaler != null)
            {
                viewPoint *= m_canvasScaler.scaleFactor;
            }

            return viewPoint;
        }

        private Collider HitTest()
        {
            Ray ray = m_camera.ScreenPointToRay(ScreenPointToViewPoint(m_window.Editor.Input.GetPointerXY(0)));

            float minDistance = float.MaxValue;
            Collider result = null;
            for(int i = 0; i < m_colliders.Length; ++i)
            {
                Collider collider = m_colliders[i];
                RaycastHit hitInfo;
                if (collider.Raycast(ray, out hitInfo, m_gizmoPosition.magnitude * 5))
                {
                    if(hitInfo.distance < minDistance)
                    {
                        minDistance = hitInfo.distance;
                        result = hitInfo.collider;
                    }
                }
            }

            return result;
        }

        private Vector3 GetGizmoPosition()
        {
            return transform.TransformPoint(Vector3.forward * 5);
        }
        
        private void InitColliders()
        {
            m_gizmoPosition = GetGizmoPosition();
            float sScale = RuntimeHandlesComponent.GetScreenScale(m_gizmoPosition, m_camera) * Size.y * Appearance.SceneGizmoScale / 96;

            m_collidersGO.transform.rotation = Quaternion.identity;
            m_collidersGO.transform.position = GetGizmoPosition();

            const float size = 0.15f;
            m_colliderProj.size = new Vector3(size, size, size) * sScale;

            m_colliderUp.size = new Vector3(size, size * 2, size) * sScale;
            m_colliderUp.center = new Vector3(0.0f, size + size / 2, 0.0f) * sScale;

            m_colliderDown.size = new Vector3(size, size * 2, size) * sScale;
            m_colliderDown.center = new Vector3(0.0f, -(size + size / 2), 0.0f) * sScale;

            m_colliderForward.size = new Vector3(size, size, size * 2) * sScale;
            m_colliderForward.center = new Vector3(0.0f,  0.0f, size + size / 2) * sScale;

            m_colliderBackward.size = new Vector3(size, size, size * 2) * sScale;
            m_colliderBackward.center = new Vector3(0.0f, 0.0f, -(size + size / 2)) * sScale;

            m_colliderRight.size = new Vector3(size * 2, size, size) * sScale;
            m_colliderRight.center = new Vector3(size + size / 2, 0.0f, 0.0f) * sScale;

            m_colliderLeft.size = new Vector3(size * 2, size, size) * sScale;
            m_colliderLeft.center = new Vector3(-(size + size / 2), 0.0f, 0.0f) * sScale;
        }

        private bool UpdateAlpha(ref float alpha, Vector3 axis, float delta)
        {
            bool hide = Math.Abs(Vector3.Dot(Window.Camera.transform.forward, axis)) > 0.9;
            if (hide)
            {
                if (alpha > 0.0f)
                {
                    
                    alpha -= delta;
                    if (alpha < 0.0f)
                    {
                        alpha = 0.0f;
                    }
                    return true;
                }
            }
            else
            {
                if (alpha < 1.0f)
                {
                    alpha += delta;
                    if (alpha > 1.0f)
                    {
                        alpha = 1.0f;
                    }
                    return true;
                }
            }

            return false;
        }

        public void UpdateLayout()
        {
            m_screenHeight = Screen.height;
            m_screenWidth = Screen.width;
            m_cameraPixelRect = Window.Camera.pixelRect;
            m_scale = Appearance.SceneGizmoScale;

            if (m_camera == null)
            {
                return;
            }

            m_aspect = m_camera.aspect;

            if (Window.Camera != null)
            {
                bool initColliders = false;

                if(m_renderTextureCamera == null)
                {
                    Vector2 size = Size * Appearance.SceneGizmoScale;
                    Vector2 offset = Offset * Appearance.SceneGizmoScale;
                    m_camera.pixelRect = new Rect(Window.Camera.pixelRect.min.x + Window.Camera.pixelWidth - size.x + offset.x, Window.Camera.pixelRect.min.y + Window.Camera.pixelHeight - size.y + offset.y, size.x, size.y);
                }
                else
                {
                    if (m_output != null)
                    {
                        m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size.x);
                        m_output.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size.y);
                    }
                }

                if (m_camera.pixelRect.height == 0 || m_camera.pixelRect.width == 0)
                {
                   // enabled = false;
                    return;
                }
                else
                {
                    if (!enabled)
                    {
                        initColliders = true;
                    }
                   // enabled = true;
                }
                m_camera.depth = Window.Camera.depth + 1;
                m_aspect = m_camera.aspect;

                if(m_renderTextureCamera == null)
                {
                    Vector2 size = Size * Appearance.SceneGizmoScale;
                    Vector2 offset = Offset * Appearance.SceneGizmoScale;
                    m_buttonRect = new Rect(Window.Camera.pixelRect.min.x + Window.Camera.pixelWidth - size.x / 2 - 17 + offset.x, (Screen.height - Window.Camera.pixelRect.yMax) + size.y - offset.y - 3, 35, 14);
                }
                
                m_buttonStyle = new GUIStyle();
                m_buttonStyle.alignment = TextAnchor.MiddleCenter;
                m_buttonStyle.normal.textColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
                m_buttonStyle.fontSize = 12;

                if (initColliders)
                {
                    InitColliders();
                }

                if(m_renderTextureCamera != null)
                {
                    m_renderTextureCamera.ResizeRenderTexture();
                }
            }
        }  
    }
}

