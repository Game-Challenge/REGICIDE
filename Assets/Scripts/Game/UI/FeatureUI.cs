using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

class FeatureUI : UIWindow
{
    #region 脚本工具生成的代码
    private GameObject m_goFeature;
    protected override void ScriptGenerator()
    {
        m_goFeature = FindChild("m_goFeature").gameObject;
        var move = CreateWidget<FeatureMove>("m_goFeature");
    }
    #endregion

    #region 事件
    #endregion

}

class FeatureMove : UIEventItem<FeatureMove>
{
    public enum UIDragType
    {
        Draging,
        Drop
    }
    private UIDragType m_dragState = UIDragType.Drop;
    private Vector3 m_itemOldPos;
    private Vector3 m_itemCachePos;
    private bool m_CanDrag = true;
    protected override void OnCreate()
    {
        base.OnCreate();

        BindBeginDragEvent(delegate (FeatureMove item, PointerEventData data)
        {
            if (!m_CanDrag)
            {
                return;
            }
            StartDragItem(UIDragType.Draging);
        });

        BindEndDragEvent(delegate (FeatureMove item, PointerEventData data)
        {
            if (!m_CanDrag)
            {
                return;
            }
            EndDrag();
        });
    }

    protected override void OnUpdate()
    {
        if (!m_CanDrag)
        {
            return;
        }
        UpdateDragPos();
    }

    private void StartDragItem(UIDragType type)
    {
        Debug.Log("Start Drag" + type);
        if (type != UIDragType.Drop)
        {
            m_itemOldPos = transform.position;
            Vector3 pos;
            UISys.Mgr.GetMouseDownUiPos(out pos);
            m_itemOldPos = pos;
            UpdateDragPos();
            m_dragState = type;
        }
    }

    private void EndDrag()
    {
        m_dragState = UIDragType.Drop;
        transform.position = m_itemOldPos;
    }

    private void UpdateDragPos()
    {
        if (m_dragState == UIDragType.Drop)
        {
            return;
        }

        Vector3 pos;
        UISys.Mgr.GetMouseDownUiPos(out pos);
        transform.position += (pos - m_itemOldPos);
        m_itemOldPos = pos;
    }
}