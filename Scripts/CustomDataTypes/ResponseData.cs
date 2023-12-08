namespace CustomDataTypes
{
    public struct ResponseData<T>
    {
        public T Response { get; private set; }
        public float ResponseTime { get; private set; }

        public ResponseData(T response, float responseTime) : this()
        {
            Response = response;
            ResponseTime = responseTime;
        }

        public new string ToString() => $"{Response}, {ResponseTime}";
    }
}