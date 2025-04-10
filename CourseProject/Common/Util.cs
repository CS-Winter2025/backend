using Newtonsoft.Json;

namespace CourseProject.Common
{
    public static class Util
    {
        public static Dictionary<string, string> ParseJson(string jsonString)
        {
            if (jsonString == null) return null;
            try
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString);
                return parsed;

            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static string SerializeJson(Dictionary<string, string> pairs)
        {
            var jsonString = JsonConvert.SerializeObject(pairs);
            return jsonString;
        }
    }
}
