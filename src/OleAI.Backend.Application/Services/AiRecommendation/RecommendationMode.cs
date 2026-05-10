namespace OleAI.Backend.Application.Services.AiRecommendation;

public enum RecommendationMode
{
    /// <summary>
    /// Standart Mod: Malzeme uyumuna odaklanan dengeli öneriler sunar.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Gurme Modu (Ultimate): Malzeme uyumunu ve geçmiş beğenilerinizi en üst seviyede harmanlar.
    /// </summary>
    Ultimate = 1,

    /// <summary>
    /// Katı Mod (Strict): Sadece elinizdeki malzemelerle yapılabilecek tariflere odaklanır.
    /// </summary>
    Strict = 2,

    /// <summary>
    /// Sana Özel (For You): Senin damak tadına ve benzer kullanıcıların sevdiklerine odaklanan keşif modu.
    /// </summary>
    ForYou = 3,

    /// <summary>
    /// Hızlı Pişir (Quick): En az malzeme ve en kısa hazırlık süresi gerektiren pratik tarifler.
    /// </summary>
    Quick = 4,

    /// <summary>
    /// Keşfet (Explore): Daha önce denemediğin, farklı kategorilerden yeni lezzetler önerir.
    /// </summary>
    Explore = 5,

    /// <summary>
    /// Minimum Alışveriş (Minimal): Dolabındaki malzemelerle yapabileceğin, en az ekstra malzeme gerektirmeyen tarifler.
    /// </summary>
    MinimalShopping = 6
}
