using CodeFirstGenerator.Entities;

namespace ELF.Shared
{
    public class AmisResult<TData>
    {
        /*
• status: 返回 0，表示当前接口正确返回，否则按错误请求处理；
• msg: 返回接口处理信息，主要用于表单提交或请求失败时的 toast 显示；
• data: 必须返回一个具有 key-value 结构的对象。
         */
        public int Status { get; set; }
        public string Msg { get; set; } = "";
        public TData Data { get; set; } = default!;
    }

    public class AmisResult : AmisResult<object>
    {
        public static AmisResult Ok()
        {
            return new AmisResult();
        }
        public static AmisResult<TData> Ok<TData>(TData entityInfo)
        {
            return new AmisResult<TData>() { Status = 0, Data = entityInfo };
        }

        internal static AmisResult Fail(string msg = "")
        {
            return new AmisResult() { Status = -1, Msg = msg };
        }
    }
}
