namespace DryCleaning.Domain;

/// <summary>
/// Виды обработки вещей в химчистке.
/// Возможные значения: химчистка, аквачистка, паровая обработка,
/// ручная стирка и глажение.
/// </summary>
public enum CleaningType
{
    DryCleaning = 0,
    WetCleaning = 1,
    SteamCleaning = 2,
    HandWash = 3,
    Ironing = 4
}
