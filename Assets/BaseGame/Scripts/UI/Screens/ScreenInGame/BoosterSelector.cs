using _Game.Scripts.Manager;
using _Game.Scripts.UI.Core;
using Core.UI;
using Core.UI.Modals;
using CoreData;
using Cysharp.Threading.Tasks;
using LitMotion;
using Manager;
using R3;
using TMPro;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Views;
using UnityEngine;
using UnityEngine.UI;

public class BoosterSelector : MonoBehaviour
{
    [field: SerializeField] public BoosterType BoosterType {get; private set;} = new();
    [field: SerializeField] public CanvasGroup BoosterCG {get; private set;}
    [field: SerializeField] public Image BoosterIcon {get; private set;}
    [field: SerializeField] public SerializableReactiveProperty<float> BoosterAmount {get; private set;}
    [field: SerializeField] public TextMeshProUGUI BoosterAmountTxt {get; private set;}
    [field: SerializeField] public TextMeshProUGUI BoosterUnlockLvTxt {get; private set;}
    [field: SerializeField] public TextMeshProUGUI BoosterPriceTxt {get; private set;}
    [field: SerializeField] public GameObject BoosterAmountObj {get; private set;}
    [field: SerializeField] public GameObject AddMoreBoosterObj {get; private set;}
    [field: SerializeField] public GameObject UnlockedObj {get; private set;}
    [field: SerializeField] public GameObject LockedObj {get; private set;}
    [field: SerializeField] public CustomButton Button {get; private set;}
    [field: SerializeField] public GameObject TutorialHand {get; private set;}
    private int level;
    private BoosterData boosterData;
    private bool isShowOnTutorial;
    private void Start()
    {
        BoosterAmount = InGameDataManager.Instance.InGameData.ResourceData.GetResource(ResourceType.Booster, (int)BoosterType).Value;
        BoosterAmount.Subscribe(UpdateBoosterAmount).AddTo(this);
        Button.AddListener(SelectBooster);
        boosterData = ItemGlobalConfig.Instance.GetBoosterData(BoosterType);
        Debug.LogError("Need booster icon");
//        BoosterIcon.sprite = boosterData.Sprite;
        level = GameManager.Instance.currentLevel.Value;
        BoosterUnlockLvTxt.text = $"Lv.{boosterData.UnlockLevel}";
        BoosterPriceTxt.text = $"{boosterData.Price}<sprite=0>";
        SetState();
        isShowOnTutorial = false;
        GameManager.Instance.currentLevelManager.isPause.ReactiveProperty.Subscribe(CheckPauseGame).AddTo(this);
        GameManager.Instance.currentLevel.ReactiveProperty.Subscribe(OnChangeLevel).AddTo(this);
    }

    void OnChangeLevel(int level)
    {
        if (level == boosterData.UnlockLevel)
        {
            if (InGameDataManager.Instance.InGameData.ResourceData.IsBoosterUsedOnTut(BoosterType)
                || boosterData.skeletonDataAsset == null)
            {
                LockedObj.SetActive(false);
                UnlockedObj.SetActive(true);
                BoosterAmountObj.SetActive(BoosterAmount.Value > 0);
                AddMoreBoosterObj.SetActive(BoosterAmount.Value <= 0);
                BoosterIcon.gameObject.SetActive(true);
            }
        }
    }

    void CheckPauseGame(bool pause)
    {
        if (!pause && BoosterCG.alpha < 1)
        {
            BoosterCG.alpha = 1;
        }

        if (pause)
        {
            BoosterCG.alpha = 0.5f;
        }
    }

    void SetState()
    {
        BoosterCG.alpha = 0.5f;
        if(level > boosterData.UnlockLevel)
        {
            BoosterIcon.gameObject.SetActive(true);
            LockedObj.SetActive(false);
            UnlockedObj.SetActive(true);
            BoosterAmountObj.SetActive(BoosterAmount.Value > 0);
            AddMoreBoosterObj.SetActive(BoosterAmount.Value <= 0);
        }
        else if (level < boosterData.UnlockLevel)
        {
            BoosterIcon.gameObject.SetActive(false);
            LockedObj.SetActive(true);
            UnlockedObj.SetActive(false);
            BoosterAmountObj.SetActive(false);
            AddMoreBoosterObj.SetActive(false);
        }
        else
        {
            if (InGameDataManager.Instance.InGameData.ResourceData.IsBoosterUsedOnTut(BoosterType)
                || boosterData.skeletonDataAsset == null)
            {
                LockedObj.SetActive(false);
                UnlockedObj.SetActive(true);
                BoosterAmountObj.SetActive(BoosterAmount.Value > 0);
                AddMoreBoosterObj.SetActive(BoosterAmount.Value <= 0);
                BoosterIcon.gameObject.SetActive(true);
            }
            else
            {
                LockedObj.SetActive(true);
                UnlockedObj.SetActive(false);
                BoosterAmountObj.SetActive(false);
                AddMoreBoosterObj.SetActive(false);
                Button.SetInteractable(false);
                BoosterIcon.gameObject.SetActive(true);
                BoosterIcon.transform.localScale = Vector3.zero;
            }
        }
    }
    
    public async void GetBoosterOnTutorial(Vector3 root)
    {
        Button.SetInteractable(false);
        BoosterIcon.gameObject.SetActive(true);
        BoosterCG.alpha = 1f;
        InGameDataManager.Instance.InGameData.ResourceData.AddBoosterUsedOnTut(BoosterType);
        LockedObj.SetActive(false);
        UnlockedObj.SetActive(true);
        BoosterIcon.transform.position = root;
        BoosterIcon.transform.localScale = Vector3.one * 3.5f;
        await LSequence.Create()
            .AppendInterval(1.5f)
            .Append(LMotion.Create(BoosterIcon.transform.localPosition, Vector3.zero, 0.75f)
                .WithEase(Ease.InBack)
                .Bind(x => BoosterIcon.transform.localPosition = x))
            .Join(LMotion.Create(BoosterIcon.transform.localScale, Vector3.one, 0.75f)
                .WithEase(Ease.InBack)
                .Bind(x => BoosterIcon.transform.localScale = x)).Run().AddTo(this);
        
        await LMotion.Create(transform.localScale, Vector3.one * 1.1f, 0.1f)
            .WithEase(Ease.InQuad)
            .Bind(x => transform.localScale = x).AddTo(this);
        
        await LMotion.Create(transform.localScale, Vector3.one, 0.1f)
            .WithOnComplete(ActiveBooster)
            .WithEase(Ease.OutQuad)
            .Bind(x => transform.localScale = x).AddTo(this);
        //GamePlayManager.Instance.SetBlockInput(false);
        Button.SetInteractable(true);
    }

    void ActiveBooster()
    {
        BoosterAmountObj.SetActive(true);
        AddMoreBoosterObj.SetActive(BoosterAmount.Value <= 0);
        Button.SetInteractable(true);
        //TutorialHand.SetActive(true);
        isShowOnTutorial = true;
        //DelayHideTut().Forget();
    }

    async UniTask DelayHideTut()
    {
        await UniTask.Delay(15000);
        InGameDataManager.Instance.InGameData.ResourceData.AddBoosterUsedOnTut(BoosterType);
        TutorialHand.SetActive(false);
    }
    
    public void ShowTutorialHand(Transform hint)
    {
        hint.gameObject.SetActive(!LockedObj.activeSelf);
        hint.position = TutorialHand.transform.position;
    }
    public void HideTutorialHand()
    {
        TutorialHand.SetActive(false);
    }
    
    void UpdateBoosterAmount(float amount)
    {
        BoosterAmountTxt.text = $"{amount}";
        AddMoreBoosterObj.SetActive(amount <= 0 && !LockedObj.activeSelf);
        BoosterAmountObj.SetActive(amount > 0 && !LockedObj.activeSelf);
    }
    
    public void SelectBooster()
    {
        //TutorialHand.SetActive(false);
        if((GameManager.Instance.currentLevelManager.isPause.Value && !isShowOnTutorial) 
           || LockedObj.activeSelf)
            return;
        if (BoosterAmount.Value > 0)
        {
            if (CheckUseCondition())
            {
            }
            //BoosterManager.Instance.UseBooster(BoosterType);
        }
        else
        {
            if (DefaultGlobalConfig.Instance.ShowModalOfferBooster)
            {
                ViewOptions options = new ViewOptions(nameof(ModalBooster));
                ModalContainer.Find(ContainerKey.Modals).PushAsync(options, this);
                GameManager.Instance.SetPause(true);
            }
            else
            {
                if(InGameDataManager.Instance.InGameData.ResourceData.IsEnoughResourceValue(ResourceType.Currency, (int)CurrencyType.Money, boosterData.Price))
                {
                    InGameDataManager.Instance.InGameData.ResourceData.SubResourceValue(ResourceType.Currency, (int)CurrencyType.Money, boosterData.Price);
                    InGameDataManager.Instance.InGameData.ResourceData.AddResourceValue(ResourceType.Booster, (int)boosterData.BoosterType, DefaultGlobalConfig.Instance.BoosterAmountOnPurchase);
                    //InGameAnalyticController.EventTrackResourceSpend?.Invoke(ResourceType.Currency, (CurrencyType.Money).ToString(), "ingame", "buy_booster", (int)boosterData.Price);
                }
                else
                {
                    GameManager.Instance.SetPause(true);
                    ShopPackageDataConfig packData = ShopGlobalConfig.Instance.GetShopPackageDataConfig(PackageName.bigbundle);
                    ViewOptions options = new ViewOptions(nameof(ModalShop));
                    ModalContainer.Find(ContainerKey.Modals).PushAsync(options, packData);
                }
            }
            
        }
    }
    
    public async UniTask DelayUseBooster()
    {
        await UniTask.Delay(100);
        SelectBooster();
    }
    
    private async UniTask ShowModalBooster(BoosterType boosterType)
    {
        GameResource resource = new GameResource(ResourceType.Booster, (int)boosterType, 1);
        ViewOptions options = new ViewOptions(nameof(ModalBooster));
        await ModalContainer.Find(ContainerKey.Modals).PushAsync(options, resource);
    }

    bool CheckUseCondition()
    {
        // if (BoosterType == BoosterType.Wand)
        // {
        //     return BoosterManager.Instance.CanUseBoosterBreakIce();
        // }
        return true;
    }
}
