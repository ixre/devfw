using System;

namespace J6.DevFw.Data.Orm
{
    public class FieldValueException : ApplicationException
    {
        public FieldValueException()
        {
        }

        public FieldValueException(string message) : base(message)
        {
        }

        public override string Message
        {
            get { return base.Message ?? "数据库字段与映射发生错误!"; }
        }
    }
}