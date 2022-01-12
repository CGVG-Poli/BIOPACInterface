using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CardInfoCode : VisualElement
{
    public string Title
    {
        get
        {
            return m_Label_Title.text;
        }
        set
        {
            m_Label_Title.text = value;
        }
    }
    
    public Label m_Label_Title = default;

    public string Info
    {
        get
        {
            return m_Label_Info.text;
        }
        set
        {
            m_Label_Info.text = value;
        }
    }

    public Label m_Label_Info = default;

    //TODO Find a way to display and change the image from 
    public StyleBackground IconBackground
    {
        get
        {
            return m_ve_Icon.style.backgroundImage;
        }
        set
        {
            m_ve_Icon.style.backgroundImage = new StyleBackground();
        }
    }

    public Image m_ve_Icon = default;

    public new class UxmlFactory : UxmlFactory<CardInfoCode, UxmlTraits> { }
    
    public new class UxmlTraits : VisualElement.UxmlTraits
    {
        UxmlStringAttributeDescription m_Title = new UxmlStringAttributeDescription { name = "label-title", defaultValue = "Label" };
        UxmlStringAttributeDescription m_Info = new UxmlStringAttributeDescription { name = "label-info", defaultValue = "Label" };
        
        //Attempt to display image in the inspector for this custom element
        UxmlTypeAttributeDescription<Image> m_Image = new UxmlTypeAttributeDescription<Image>() { name = "label-info" };

        public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        {
            base.Init(ve, bag, cc);
            var ate = ve as CardInfoCode;
            
            ate.Clear();
            
            VisualTreeAsset template = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/BIOPACInterface/Samples/BIOPACInterface_Client_Server/UI/Templates/CardInfo.uxml"); 
            template.CloneTree(ate);
            ate.Init();
            
            ate.Title = m_Title.GetValueFromBag(bag, cc);
            ate.Info = m_Info.GetValueFromBag(bag, cc);
            //ate.IconBackground = m_Image.GetValueFromBag(bag,cc);
        }
    }

    public CardInfoCode()
    {
        
    }

    public void Init()
    {
        m_Label_Title = this.Q<Label>("label-title");
        m_Label_Info = this.Q<Label>("label-info");
    }
    
    
}
