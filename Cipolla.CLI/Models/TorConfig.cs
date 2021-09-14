namespace Cipolla.CLI.Models
{
    public static class TorConfigTemplate
    {
        public static string GetTorConfig(ushort SocksPort, ushort ControlPort, string DataPath)
        {
            return $@"
EntryNodes {{de}},{{nl}},{{at}} StrictNodes 1
ExitNodes {{nl}},{{it}},{{es}},{{fr}} StrictNodes 1

SocksPort {SocksPort}
ControlPort {ControlPort}
DataDirectory {DataPath}
            ".Trim();
        }
    }
}
