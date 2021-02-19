namespace HelloWorld.Dtos
{
    public class StateDto<T>
    {
        public StateDto(string key, T value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; set; }

        public T Value { get; set; }
    }
}
