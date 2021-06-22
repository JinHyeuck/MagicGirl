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

    public class CompletePlayerTableLoadMsg : Message
    {
        public bool IsSuccess = false;
    }
}