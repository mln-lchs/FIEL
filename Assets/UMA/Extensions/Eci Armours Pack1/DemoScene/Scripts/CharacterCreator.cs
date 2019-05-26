using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UMA;
using UMA.CharacterSystem;
using UnityEngine.UI;

public class CharacterCreator : MonoBehaviour
{
    public DynamicCharacterAvatar avatar;
    public Slider headSizeSlider;
    public Slider bellySlider;
    public Slider breastSizeSlider;
    public Slider armWidthSlider;
    public Slider forearmWidthSlider;
    public Slider gluteusSizeSlider;
    public Slider headWidthSlider;
    public Slider legSeparationSlider;
    public Slider lowerMuscleSlider;
    public Slider lowerWeightSlider;
    public Slider upperMuscleSlider;
    public Slider upperWeightSlider;
    public Slider waistSlider;
    private Dictionary<string, DnaSetter> dna;
    void OnEnable()
     {
        avatar.CharacterUpdated.AddListener(Updated);
        headSizeSlider.onValueChanged.AddListener(HeadSizeChange);
        bellySlider.onValueChanged.AddListener(BellyChange);
        breastSizeSlider.onValueChanged.AddListener(BreastSizeChange);
        armWidthSlider.onValueChanged.AddListener(ArmWidthChange);
        forearmWidthSlider.onValueChanged.AddListener(ForearmWidthChange);
        gluteusSizeSlider.onValueChanged.AddListener(GluteusSizeChange);
        headWidthSlider.onValueChanged.AddListener(HeadWidthChange);
        legSeparationSlider.onValueChanged.AddListener(LegSeparationChange);
        lowerMuscleSlider.onValueChanged.AddListener(LowerMuscleChange);
        lowerWeightSlider.onValueChanged.AddListener(LowerWeightChange);
        upperMuscleSlider.onValueChanged.AddListener(UpperMuscleChange);
        upperWeightSlider.onValueChanged.AddListener(UpperWeightChange);
        waistSlider.onValueChanged.AddListener(WaistChange);
     }
    private void OnDisable()
    {
        avatar.CharacterUpdated.RemoveListener(Updated);
        headSizeSlider.onValueChanged.RemoveListener(HeadSizeChange);
        bellySlider.onValueChanged.RemoveListener(BellyChange);
        breastSizeSlider.onValueChanged.RemoveListener(BreastSizeChange);
        armWidthSlider.onValueChanged.RemoveListener(ArmWidthChange);
        forearmWidthSlider.onValueChanged.RemoveListener(ForearmWidthChange);
        gluteusSizeSlider.onValueChanged.RemoveListener(GluteusSizeChange);
        headWidthSlider.onValueChanged.RemoveListener(HeadWidthChange);
        legSeparationSlider.onValueChanged.RemoveListener(LegSeparationChange);
        lowerMuscleSlider.onValueChanged.RemoveListener(LowerMuscleChange);
        lowerWeightSlider.onValueChanged.RemoveListener(LowerWeightChange);
        upperMuscleSlider.onValueChanged.RemoveListener(UpperMuscleChange);
        upperWeightSlider.onValueChanged.RemoveListener(UpperWeightChange);
        waistSlider.onValueChanged.RemoveListener(WaistChange);
    }
    public void SwitchGender(bool male)
    {
        if (male && avatar.activeRace.name != "HumanMaleDCS")
            avatar.ChangeRace("HumanMaleDCS");
        if (!male && avatar.activeRace.name != "HumanFemaleDCS")
            avatar.ChangeRace("HumanFemaleDCS");
    }
    void Updated(UMAData data)
    {
        dna = avatar.GetDNA();
        headSizeSlider.value = dna["headSize"].Get();
        bellySlider.value = dna["belly"].Get();
        breastSizeSlider.value = dna["breastSize"].Get();
        armWidthSlider.value = dna["armWidth"].Get();
        forearmWidthSlider.value = dna["forearmWidth"].Get();
        gluteusSizeSlider.value = dna["gluteusSize"].Get();
        headWidthSlider.value = dna["headWidth"].Get();
        legSeparationSlider.value = dna["legSeparation"].Get();
        lowerMuscleSlider.value = dna["lowerMuscle"].Get();
        lowerWeightSlider.value = dna["lowerWeight"].Get();
        upperMuscleSlider.value = dna["upperMuscle"].Get();
        upperWeightSlider.value = dna["upperWeight"].Get();
        waistSlider.value = dna["waist"].Get();
    }
    public void HeadSizeChange(float val)
    {
        dna["headSize"].Set(val);
        avatar.BuildCharacter();
    }
    public void BellyChange(float val)
    {
        dna["belly"].Set(val);
        avatar.BuildCharacter();
    }
    public void BreastSizeChange(float val)
    {
        dna["breastSize"].Set(val);
        avatar.BuildCharacter();
    }
    public void ArmWidthChange(float val)
    {
        dna["armWidth"].Set(val);
        avatar.BuildCharacter();
    }
    public void ForearmWidthChange(float val)
    {
        dna["forearmWidth"].Set(val);
        avatar.BuildCharacter();
    }
    public void GluteusSizeChange(float val)
    {
        dna["gluteusSize"].Set(val);
        avatar.BuildCharacter();
    }
    public void HeadWidthChange(float val)
    {
        dna["headWidth"].Set(val);
        avatar.BuildCharacter();
    }
    public void LegSeparationChange(float val)
    {
        dna["legSeparation"].Set(val);
        avatar.BuildCharacter();
    }
    public void LowerMuscleChange(float val)
    {
        dna["lowerMuscle"].Set(val);
        avatar.BuildCharacter();
    }
    public void LowerWeightChange(float val)
    {
        dna["lowerWeight"].Set(val);
        avatar.BuildCharacter();
    }
    public void UpperMuscleChange(float val)
    {
        dna["upperMuscle"].Set(val);
        avatar.BuildCharacter();
    }
    public void UpperWeightChange(float val)
    {
        dna["upperWeight"].Set(val);
        avatar.BuildCharacter();
    }
    public void WaistChange(float val)
    {
        dna["waist"].Set(val);
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType1a(SlotLibrary slot)
    {
            avatar.SetSlot("Helmet", "HelmMalePack1Type1a_Recipe");
        avatar.BuildCharacter();
            avatar.SetSlot("Helmet", "HelmPack1Type1a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType1b(SlotLibrary slot)
    {
        avatar.SetSlot("Helmet", "HelmMalePack1Type1b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Helmet", "HelmPack1Type1b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType2a(SlotLibrary slot)
    {
        avatar.SetSlot("Helmet", "HelmMalePack1Type2a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Helmet", "HelmPack1Type2a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType2b(SlotLibrary slot)
    {
        avatar.SetSlot("Helmet", "HelmMalePack1Type2b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Helmet", "HelmPack1Type2b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType3a(SlotLibrary slot)
    {
        avatar.SetSlot("Helmet", "HelmMalePack1Type3a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Helmet", "HelmPack1Type3a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotHelmType3b(SlotLibrary slot)
    {
        avatar.SetSlot("Helmet", "HelmMalePack1Type3b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Helmet", "HelmPack1Type3b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType1a(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type1a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type1a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType1b(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type1b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type1b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType2a(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type2a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type2a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType2b(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type2b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type2b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType3a(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type3a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type3a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotTorsoType3b(SlotLibrary slot)
    {
        avatar.SetSlot("Chest", "TorsoMalePack1Type3b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Chest", "TorsoPack1Type3b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType1a(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type1a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type1a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType1b(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type1b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type1b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType2a(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type2a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type2a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType2b(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type2b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type2b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType3a(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type3a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type3a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotLegsType3b(SlotLibrary slot)
    {
        avatar.SetSlot("Legs", "LegsMalePack1Type3b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Legs", "LegsPack1Type3b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType1a(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type1a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type1a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType1b(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type1b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type1b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType2a(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type2a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type2a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType2b(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type2b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type2b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType3a(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type3a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type3a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotGlovesType3b(SlotLibrary slot)
    {
        avatar.SetSlot("Hands", "GlovesMalePack1Type3b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Hands", "GlovesPack1Type3b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType1a(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type1a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type1a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType1b(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type1b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type1b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType2a(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type2a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type2a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType2b(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type2b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type2b_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType3a(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type3a_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type3a_Recipe");
        avatar.BuildCharacter();
    }
    public void ChangeSlotBootsType3b(SlotLibrary slot)
    {
        avatar.SetSlot("Feet", "BootsMalePack1Type3b_Recipe");
        avatar.BuildCharacter();
        avatar.SetSlot("Feet", "BootsPack1Type3b_Recipe");
        avatar.BuildCharacter();
    }
}
