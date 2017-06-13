using System;

namespace JR.DevFw.Framework.TaskBox
{
    public class TaskMessageParser
    {
        public static string ConvertSyncMessageToString(TaskMessage message)
        {
            return JsonSerializer.SerializerObject<TaskMessage>(message);
        }

        /// <summary>
        /// ´ÓJson×Ö·û×ªÎªSyncMessage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TaskMessage ConvertToSyncMessage(string message)
        {
            if (!String.IsNullOrEmpty(message))
            {
                return JsonSerializer.DeserializerObject<TaskMessage>(message);
            }
            return default(TaskMessage);
        }
    }
}