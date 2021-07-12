namespace GameBerry.Event
{
    public class GetAllGameChartResponseMsg : Message
    {
        public bool IsSuccess = false;
    }

    public class GetOneChartAndSaveResponseMsg : Message
    {
        public bool IsSuccess = false;
    }

    public class CompleteCharacterInfoTableLoadMsg : Message
    {
        public bool IsSuccess = false;
    }

    public class CompleteCharacterUpGradeStatTableLoadMsg : Message
    {
        public bool IsSuccess = false;
    }

    public class CompleteCharacterEquipmentInfoLoadMsg : Message
    {
        public bool IsSuccess = false;
    }

    public class CompleteCharacterSkillInfoLoadMsg : Message
    {
        public bool IsSuccess = false;
    }
}