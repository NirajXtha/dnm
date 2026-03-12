public enum FigureFilter
{
    Actual=1,
    Hundred = 100,
    Thousand = 1000,
    Lakh = 100000,
    Crore = 10000000
}

public enum FrequencyDayFilter
{
    SevenDay = 7,
    FifteenDay = 15,
    ThrityDay = 30,
    SixtyDay = 60
}

public enum FixedDayFilter
{
    ThirtyDay = 30,
    SixtyDay = 60,
    NinetyDay = 90,
    HundreTwentyDay = 120
}

public enum PeriodFilter
{
    Year = 'Y',
    Quarter = 'Q',
    Month = 'M',
    Week = 'W',
    Day = 'D',
    None = 'X'
}

public enum CalenderType
{
    Bs='B',
    Ad='A',
}

public enum FileExportType
{
    pdf,
    xlsx
}

public enum MessageProcessStatus
{
    Draft = 'D',
    InProgress = 'P',
    Failed ='F',
    Send = 'S',
    NoCheck='N',

}
public enum FequencyTypeEnum
{
    daily = 'D',
    onetime = 'O',
    fequencyrange = 'F',

}

public enum YesNo
{
    Yes = 'Y',
    NO = 'N'
}