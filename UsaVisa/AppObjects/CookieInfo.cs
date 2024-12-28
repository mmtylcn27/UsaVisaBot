namespace UsaVisa.AppObjects
{
    public readonly struct CookieInfo
    {
        public readonly string Name;
        public readonly string Value;

        public CookieInfo(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
