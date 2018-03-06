namespace DacDeployer.Helpers
{
    public enum Mode
    {
        Compile,
        Deploy,
        Help
    }

    public static class ModeDefiner
    {
        public static Mode  DefineMode()
        {
            if (ConsoleAppArgsParser.ParamExists("compile"))
                return Mode.Compile;

            if (ConsoleAppArgsParser.ParamExists("deploy"))
                return Mode.Deploy;
            
            return Mode.Help;
        }

    }
}
