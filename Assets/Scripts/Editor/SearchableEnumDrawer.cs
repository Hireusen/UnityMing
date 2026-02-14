using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

[CustomPropertyDrawer(typeof(SearchableEnumAttribute))]
public class SearchableEnumDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.Enum) {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        // 라벨 그리기
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // 현재 선택된 Enum 이름 가져오기
        string currentName = property.enumNames[property.enumValueIndex];

        // 버튼 그리기 (클릭 시 팝업 오픈)
        if (GUI.Button(position, currentName, EditorStyles.popup)) {
            var dropdown = new EnumDropdown(property);
            dropdown.Show(position);
        }
    }

    // Unity의 고급 드롭다운 API 사용
    private class EnumDropdown : AdvancedDropdown
    {
        private SerializedProperty _property;
        private string[] _enumNames;

        public EnumDropdown(SerializedProperty property) : base(new AdvancedDropdownState())
        {
            _property = property;
            _enumNames = property.enumNames;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Select Enum");

            for (int i = 0; i < _enumNames.Length; i++) {
                // Enum 이름에 슬래시(/)가 있다면 하위 메뉴로 자동 분류 (선택 사항)
                // 예: "Enemy/Normal", "Enemy/Boss"
                var item = new AdvancedDropdownItem(_enumNames[i])
                {
                    id = i // 인덱스를 ID로 저장
                };
                root.AddChild(item);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            _property.serializedObject.Update();
            _property.enumValueIndex = item.id; // 저장해둔 인덱스로 값 변경
            _property.serializedObject.ApplyModifiedProperties();
        }
    }
}
