using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using TSG.Models.App_LocalResources;
using WinstantPay.Common.Extension;

namespace TSG.Models.Enums
{
    public enum DelegatedAuthorirySourceLimitationTypeEnum
    {
        [Display(Name = "DelegatedAuthorirySourceLimitationType_QrCode", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "Qr")]
        Qr = 1,
        [Display(Name = "DelegatedAuthorirySourceLimitationType_SecretCode", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "SecretCode")]
        SecretCode = 2,
        [Display(Name = "DelegatedAuthorirySourceLimitationType_Nfc", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "Nfc")]
        Nfc = 3,
        [Display(Name = "DelegatedAuthorirySourceLimitationType_BarCode", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "BarCode")]
        BarCode = 4,
        [Display(Name = "DelegatedAuthorirySourceLimitationType_Vo2TechCard", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "VoTechCard")]
        Vo2TechCard = 5,

        // For red envelope
        [Display(Name = "DelegatedAuthorirySourceLimitationType_RedEnvelope", ResourceType = typeof(GlobalModel))]
        [EnumMember(Value = "RedEnvelope")]
        RedEnvelope = 100
    }
}