using CCL.Importer.Types;
using CCL.Types;
using DV;
using DV.InventorySystem;
using DV.Localization;
using DV.ServicePenalty.UI;
using System.Collections.Generic;
using UnityEditor.Localization.Editor;
using UnityEngine;

namespace CCL.Importer.WorkTrains
{
    internal class CommsRadioCCLWorkTrain : MonoBehaviour, ICommsRadioMode
    {
        private enum State
        {
            EnterMode,
            PickVehicle,
            ConfirmPurchase,
            ReturnToEnter
        }

        private class Entry
        {
            public CCL_CarVariant? Livery;

            public bool IsDefaultEntry => Livery == null;
            public float Price => Livery != null ? Livery.UnlockPrice : 0;
        }

        private const float ReturnToMainScreenTime = 4.0f;

        public CommsRadioController Controller = null!;
        public CommsRadioDisplay Display = null!;

        private State _currentState;
        private int _index;
        private bool _confirm = true;
        private Coroutine? _resetDisplayCoro;
        private List<Entry> _entries = new();

        public ButtonBehaviourType ButtonBehaviour { get; private set; }
        private int EntryCount => _entries.Count;
        private int AvailableCount => EntryCount - 1;
        private Entry Selected => _entries[_index];
        private AudioClip? ConfirmSound => Controller.crewVehicleControl.confirmSound;
        private AudioClip? CancelSound => Controller.crewVehicleControl.cancelSound;
        private AudioClip? MoneySound => Controller.crewVehicleControl.moneyRemovedSound;
        private static double PlayerMoney => Inventory.Instance.PlayerMoney;

        private void Awake()
        {
            Display = Controller.crewVehicleControl.display;
        }

        public void Enable()
        {
            _currentState = State.EnterMode;
            ButtonBehaviour = ButtonBehaviourType.Regular;
            SetStartingDisplay();

            WorkTrainPurchaseHandler.LiveryUnlocked += LiveryUnlocked;
            UpdateEntries();
        }

        public void Disable()
        {
            StopDisplayCoro();
            WorkTrainPurchaseHandler.LiveryUnlocked -= LiveryUnlocked;
        }

        public void OnUpdate()
        {
            //Controller.laserBeam.SetBeamColor(GetLaserBeamColor());
        }

        public void OnUse()
        {
            switch (_currentState)
            {
                case State.EnterMode:
                    // Nothing available to buy, don't go further.
                    if (AvailableCount <= 0)
                    {
                        SetDisplayToNoEntries();
                        SetState(State.ReturnToEnter);
                        PlayRadioSound(CancelSound);
                        return;
                    }
                    // Go to vehicle list.
                    SetState(State.PickVehicle);
                    PlayRadioSound(ConfirmSound);
                    return;

                case State.PickVehicle:
                    // Default entry exists to allow going back.
                    // Picking it... goes back.
                    if (Selected.IsDefaultEntry)
                    {
                        SetState(State.EnterMode);
                        PlayRadioSound(CancelSound);
                        return;
                    }
                    // If the player doesn't have enough money to purchase, go back.
                    if (PlayerMoney < Selected.Price)
                    {
                        SetDisplayToNoFunds();
                        SetState(State.ReturnToEnter);
                        PlayRadioSound(CancelSound);
                        break;
                    }
                    // Go to purchase confirmation.
                    SetState(State.ConfirmPurchase);
                    PlayRadioSound(ConfirmSound);
                    break;

                case State.ConfirmPurchase:
                    // This should not happen but safety.
                    if (Selected.Livery == null)
                    {
                        Debug.LogError("Selected livery is null, this should not happen at this point!", this);
                        SetState(State.EnterMode);
                        PlayRadioSound(CancelSound);
                        return;
                    }
                    // If the purchase was cancelled.
                    if (!_confirm)
                    {
                        SetState(State.PickVehicle);
                        PlayRadioSound(CancelSound);
                        return;
                    }
                    // This should also not be reached but safety x2.
                    if (PlayerMoney < Selected.Price)
                    {
                        SetDisplayToNoFunds();
                        SetState(State.ReturnToEnter);
                        PlayRadioSound(CancelSound);
                        return;
                    }
                    // Buy and unlock the livery. Keep a reference because the list is changed.
                    var selected = Selected;
                    if (WorkTrainPurchaseHandler.Unlock(selected.Livery))
                    {
                        Inventory.Instance.RemoveMoney(selected.Price);
                        SetDisplayToCompletePurchase();
                        Play2DSound(MoneySound);
                        Play2DSound(QuickAccess.Audio.WinSound);
                    }
                    SetState(State.ReturnToEnter);
                    return;

                case State.ReturnToEnter:
                    // This is just a temporary state while returning to start,
                    // so using the remote just skips it.
                    StopDisplayCoro();
                    SetStartingDisplay();
                    SetState(State.EnterMode);
                    return;

                default:
                    return;
            }
        }

        public bool ButtonACustomAction()
        {
            switch (_currentState)
            {
                case State.PickVehicle:
                    // Nothing to scroll.
                    if (AvailableCount <= 0) return false;
                    // Scroll backwards.
                    _index = MathHelper.Loop(_index - 1, EntryCount);
                    SetDisplayToSelectedVehicle();
                    return true;
                case State.ConfirmPurchase:
                    // Toggle confirmation state.
                    _confirm = !_confirm;
                    SetDisplayToConfirmation();
                    return true;
                default:
                    Debug.LogError($"Unexpected state '{_currentState}', ignoring request.", this);
                    return false;
            }
        }

        public bool ButtonBCustomAction()
        {
            switch (_currentState)
            {
                case State.PickVehicle:
                    // Nothing to scroll.
                    if (AvailableCount <= 0) return false;
                    // Scroll forwards.
                    _index = MathHelper.Loop(_index + 1, EntryCount);
                    SetDisplayToSelectedVehicle();
                    return true;
                case State.ConfirmPurchase:
                    // Toggle confirmation state.
                    _confirm = !_confirm;
                    SetDisplayToConfirmation();
                    return true;
                default:
                    Debug.LogError($"Unexpected state '{_currentState}', ignoring request.", this);
                    return false;
            }
        }

        private void SetState(State state)
        {
            if (_currentState == state) return;

            switch (state)
            {
                case State.EnterMode:
                    ButtonBehaviour = ButtonBehaviourType.Regular;
                    SetStartingDisplay();
                    break;
                case State.PickVehicle:
                    ButtonBehaviour = ButtonBehaviourType.Override;
                    // Ensure index is valid.
                    _index = Mathf.Clamp(_index, 0, EntryCount - 1);
                    SetDisplayToSelectedVehicle();
                    break;
                case State.ConfirmPurchase:
                    ButtonBehaviour = ButtonBehaviourType.Override;
                    SetDisplayToConfirmation();
                    break;
                case State.ReturnToEnter:
                    ButtonBehaviour = ButtonBehaviourType.Regular;
                    StartDisplayCoro();
                    break;
                default:
                    Debug.LogError($"Unexpected state '{state}', ignoring request.", this);
                    return;
            }

            _currentState = state;
        }

        public void OverrideSignalOrigin(Transform signalOrigin) { }

        public Color GetLaserBeamColor()
        {
            return Color.clear;
        }

        public void SetStartingDisplay()
        {
            StopDisplayCoro();
            Display.SetDisplay(Localization.WorkTrains.PurchaseModeTitle,
                Localization.WorkTrains.EnterSelection);
        }

        private void SetDisplayToSelectedVehicle()
        {
            StopDisplayCoro();

            if (Selected.IsDefaultEntry)
            {
                Display.SetContentAndAction(Localization.WorkTrains.ExitSelection, CommsRadioLocalization.CONFIRM);
                return;
            }

            Display.SetContentAndAction(Localization.WorkTrains.SelectedCarDisplay(
                LocalizationAPI.L(Selected.Livery!.localizationKey), Selected.Price),
                CommsRadioLocalization.CONFIRM);
        }

        private void SetDisplayToConfirmation()
        {
            StopDisplayCoro();

            Display.SetContentAndAction(Localization.WorkTrains.PurchaseConfirm(Selected.Price),
                _confirm ? CommsRadioLocalization.CONFIRM : CommsRadioLocalization.CANCEL);
        }

        private void SetDisplayToNoFunds()
        {
            StopDisplayCoro();
            Display.SetContent(CommsRadioLocalization.INSUFFICIENT_FUNDS);
        }

        private void SetDisplayToNoEntries()
        {
            StopDisplayCoro();
            Display.SetContent(Localization.WorkTrains.NoneForPurchase);
        }

        private void SetDisplayToCompletePurchase()
        {
            StopDisplayCoro();
            Display.SetContent(Localization.WorkTrains.PurchaseComplete);
        }

        private void StartDisplayCoro()
        {
            _resetDisplayCoro = StartCoroutine(ReturnToStartRoutine());
        }

        private void StopDisplayCoro()
        {
            if (_resetDisplayCoro != null)
            {
                StopCoroutine(_resetDisplayCoro);
            }
        }

        private System.Collections.IEnumerator ReturnToStartRoutine()
        {
            yield return WaitFor.Seconds(ReturnToMainScreenTime);
            SetStartingDisplay();
            SetState(State.EnterMode);
        }

        private void PlayRadioSound(AudioClip? clip)
        {
            if (clip != null)
            {
                CommsRadioController.PlayAudioFromRadio(clip, transform);
            }
        }

        private void Play2DSound(AudioClip? clip)
        {
            if (clip != null)
            {
                clip.Play2D();
            }
        }

        private void LiveryUnlocked(CCL_CarVariant variant)
        {
            UpdateEntries();
            Controller.crewVehicleControl.UpdateAvailableVehicles();
        }

        private void UpdateEntries()
        {
            _entries.Clear();
            _entries.Add(new Entry());

            foreach (var item in WorkTrainPurchaseHandler.LockedLiveries)
            {
                _entries.Add(new Entry { Livery = item });
            }
        }
    }
}
