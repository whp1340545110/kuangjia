namespace PFramework.Data
{
    public class DataSettings : SettingsBase
    {
        public override string SettingsName => "存档设置";

        public override int PriotyInWindow => 1;

        public string dataRootPath = "GameSaveData";

        public bool isAesCrypted = true;

        public string aesCryptKey = "DecWrtPYcwLM2QjP";
    }
}
