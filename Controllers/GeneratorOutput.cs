namespace CodeFirstGenerator.Controllers
{
    public class GeneratorOutput
    {
        public GeneratorOutput(string template, string code, string outputPath, bool overwrite)
        {
            Template = template;
            Code = code;
            OutputPath = outputPath;
            Overwrite = overwrite;
        }
        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool Overwrite { get; set; }

        /// <summary>
        /// 模板路径
        /// </summary>
        public string Template { get; set; }

        /// <summary>
        /// 生成的代码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; set; }
    }
}