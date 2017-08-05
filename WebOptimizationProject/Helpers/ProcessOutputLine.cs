namespace WebOptimizationProject.Helpers
{
    public class ProcessOutputLine
    {
        public ProcessOutputLineType Type { get; }
        public string Txt { get; }

        public ProcessOutputLine(ProcessOutputLineType type, string txt)
        {
            Type = type;
            Txt = txt;
        }

        public override string ToString()
        {
            return $"{Type}: {Txt}";
        }
    }
}
