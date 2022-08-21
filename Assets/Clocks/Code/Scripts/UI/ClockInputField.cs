#region USING
using System;
using System.Collections;
using System.Text.RegularExpressions;
using NRTW.Code.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#endregion


namespace NRTW.Code.Scripts.UI {
    public delegate void InputFieldEvent();
    public class ClockInputField : MonoBehaviour, IPointerEnterHandler {
        #region FIELDS
        public event InputFieldEvent inputFieldPointerEnter;
        
        private TMP_InputField inputField;
        private static readonly Regex regex = new (@"^(?:(?:[01]?[0-9]|2[0-3])(?<min>:(?:[0-5][0-9]?)?)?(?<sec>:(?:[0-5][0-9]?)?)?)?$");
        #endregion
        

        
        #region BUILTIN METHODS
        private void Awake() {
            inputField = GetComponent<TMP_InputField>();
            inputField.characterLimit = 8;
        }


        private void OnEnable() {
            inputField.onValidateInput += OnValidateInput;
            inputField.onSubmit.AddListener(OnSubmit);
        }


        private void OnDisable() {
            inputField.onValidateInput -= OnValidateInput;
        }

        #endregion
        


        #region CUSTOM METHODS
        private char OnValidateInput(string text, int index, char charToValidate) {
            if ((!char.IsNumber(charToValidate) || !char.IsSymbol(':')) && !regex.IsMatch(text + charToValidate)) return '\0';
            return charToValidate;
        }


        private void OnSubmit(string text) {
            if (!regex.IsMatch(text)) inputField.text = "00:00:00";
            
            var sec = regex.Match(inputField.text).Groups["sec"];
            var secRegex = new Regex(@"^:(\d)?$");

            var finalComplete = string.Empty;
            if (!sec.Success) finalComplete += ":00";
            else if (secRegex.IsMatch(sec.Captures[0].Value)) {
                if (secRegex.Match(sec.Captures[0].Value).Groups[0].Success) {
                    if (secRegex.Match(sec.Captures[0].Value).Groups[0].Value.Equals(":")) {
                        finalComplete += "00";
                    }
                    else {
                        finalComplete += "0";
                    }
                }
            }
            inputField.text += finalComplete;
        }
        
        
        public void OnPointerEnter(PointerEventData eventData) {
            inputFieldPointerEnter?.Invoke();
        }
        #endregion
    }
}