using System.Collections.Generic; 
using UnityEngine; 

[System.Serializable] 
public class CrosswordQuestion
{
    public string Question;
    
    public string Answer;
}

public class CrosswordData : MonoBehaviour
{
    public List<CrosswordQuestion> questions = new List<CrosswordQuestion>
    {
        new CrosswordQuestion { Question = "Кого ловил Яков Михайлович волшебным сачком?", Answer = "МУХ" },
        new CrosswordQuestion { Question = "Какой предмет нужен, чтобы зайти в компьютерный класс?", Answer = "БАХИЛЫ" },
        new CrosswordQuestion { Question = "Второе слово в аббревиатуре ФИИТ?", Answer = "ИНФОРМАТИКА" },
        new CrosswordQuestion { Question = "Продолжите фразу: «ОСЛУ всегда…»", Answer = "СОВМЕСТНА" },
        new CrosswordQuestion { Question = "Чему равна производная от константы?", Answer = "НУЛЮ" },
        new CrosswordQuestion { Question = "Каким методом можно решить систему линейных уравнений?", Answer = "ГАУСА" },
        new CrosswordQuestion { Question = "Как по другому называют 'водопадную' модель процесса создания ПО?", Answer = "КАСКАДНАЯ" },
        new CrosswordQuestion { Question = "Сколько мест в 120 аудитории?", Answer = "150" },
        new CrosswordQuestion { Question = "Кто стоит напротив турникетов внутри Мехмата?", Answer = "СПОТТИ" },
        new CrosswordQuestion { Question = "Какой напиток пользуется у студентов наибольшим спросом?", Answer = "КОФЕ" },
        new CrosswordQuestion { Question = "Назовите отчество того, чьё имя носит МехМат.", Answer = "ИЗРАИЛЕВИЧ" },
        new CrosswordQuestion { Question = "Какое правило применяют для нахождения пределов функций при раскрытии неопределённостей вида 0/0 или бесконечность/бесконечность?", Answer = "ЛОПИТАЛЯ" },
        new CrosswordQuestion { Question = "Сколько пианино есть на мехмате?", Answer = "ТРИ" }
    };
}
